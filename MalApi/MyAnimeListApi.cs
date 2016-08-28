using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using HtmlAgilityPack;

namespace MalApi
{
    /// <summary>
    /// Class for accessing myanimelist.net. Methods are thread-safe. Properties are not.
    /// </summary>
    public class MyAnimeListApi : IMyAnimeListApi
    {
        private const string MalAppInfoUri = "https://myanimelist.net/malappinfo.php?status=all&type=anime";
        private const string RecentOnlineUsersUri = "https://myanimelist.net/users.php";

        /// <summary>
        /// What to set the user agent http header to in API requests. Null to use the default .NET user agent.
        /// This is current synonymous with the <c>MalApiKey</c> property because that is how API keys
        /// are passed to MAL.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// MAL API key to use. Some methods require this to be set or else MAL will return an error.
        /// The documentation for each method states whether or not this is required.
        /// This is currently synonymous with the <c>UserAgent</c> property because that is how
        /// API keys are passed to MAL.
        /// </summary>
        public string MalApiKey { get { return UserAgent; } set { UserAgent = value; } }

        private int m_timeoutInMs = 15 * 1000;

        /// <summary>
        /// Timeout in milliseconds for requests to MAL. Defaults to 15000 (15s).
        /// </summary>
        public int TimeoutInMs { get { return m_timeoutInMs; } set { m_timeoutInMs = value; } }

        public MyAnimeListApi()
        {
            ;
        }

        private HttpWebRequest InitNewRequest(string uri, string method)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            if (UserAgent != null)
            {
                request.UserAgent = UserAgent;
            }
            request.Timeout = TimeoutInMs;
            request.ReadWriteTimeout = TimeoutInMs;
            request.Method = method;
            request.KeepAlive = false;

            // Very important optimization! Time to get an anime list of ~150 entries 2.6s -> 0.7s
            request.AutomaticDecompression = DecompressionMethods.GZip;

            return request;
        }

        private TReturn ProcessRequest<TReturn>(HttpWebRequest request, Func<string, TReturn> processingFunc, string baseErrorMessage)
        {
            return ProcessRequest(request, (string html, object dummy) => processingFunc(html), (object)null,
                httpErrorStatusHandler: null, baseErrorMessage: baseErrorMessage);
        }

        /// <summary>
        /// Expected to do one of the following:
        /// 1) Set handled to true and return a valid result
        /// 2) Throw a new exception that better matches the abstraction level, for example a MalAnimeNotFoundException.
        /// 3) Set handled to false and return anything
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        delegate TReturn HttpErrorStatusHandler<TData, TReturn>(WebException ex, TData data, out bool handled);

        private TReturn ProcessRequest<TReturn, TData>(HttpWebRequest request, Func<string, TData, TReturn> processingFunc, TData data, HttpErrorStatusHandler<TData, TReturn> httpErrorStatusHandler, string baseErrorMessage)
        {
            string responseBody = null;
            try
            {
                Logging.Log.DebugFormat("Starting MAL request to {0}", request.RequestUri);
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Logging.Log.DebugFormat("Got response. Status code = {0}.", response.StatusCode);
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            throw new MalApiRequestException(string.Format("{0} Status code was {1}.", baseErrorMessage, response.StatusCode));
                        }

                        using (Stream responseBodyStream = response.GetResponseStream())
                        using (StreamReader responseBodyReader = new StreamReader(responseBodyStream, Encoding.UTF8))
                        {
                            // XXX: Shouldn't be hardcoding UTF-8
                            responseBody = responseBodyReader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    if (httpErrorStatusHandler == null)
                    {
                        throw;
                    }

                    bool handled;
                    TReturn result = httpErrorStatusHandler(ex, data, out handled);
                    
                    // If the handler did not know what to do with the http error, rethrow the exception
                    if (!handled)
                    {
                        throw;
                    }

                    // If it did know what to do and returned a result, return that result
                    return result;

                    // If it did know what to do and threw a better exception, it will get caught below and thrown further.
                }

                Logging.Log.Debug("Read response body.");

                return processingFunc(responseBody, data);
            }
            catch (MalUserNotFoundException)
            {
                throw;
            }
            catch (MalAnimeNotFoundException)
            {
                throw;
            }
            catch (MalApiException)
            {
                // Log the body of the response returned by the API server if there was an error.
                // Don't log it otherwise, logs could get big then.
                if (responseBody != null)
                {
                    Logging.Log.DebugFormat("Response body:{0}{1}", Environment.NewLine, responseBody);
                }
                throw;
            }
            catch (Exception ex)
            {
                if (responseBody != null)
                {
                    // Since we read the response, the error was in processing the response, not with doing the request/response.
                    Logging.Log.DebugFormat("Response body:{0}{1}", Environment.NewLine, responseBody);
                    throw new MalApiException(string.Format("{0} {1}", baseErrorMessage, ex.Message), ex);
                }
                else
                {
                    // If we didn't read a response, then there was an error with the request/response that may be fixable with a retry.
                    throw new MalApiRequestException(string.Format("{0} {1}", baseErrorMessage, ex.Message), ex);
                }
            }
        }

        /// <summary>
        /// Gets a user's anime list. This method requires a MAL API key.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        public MalUserLookupResults GetAnimeListForUser(string user)
        {
            string userInfoUri = MalAppInfoUri + "&u=" + Uri.EscapeDataString(user);

            Logging.Log.InfoFormat("Getting anime list for MAL user {0} using URI {1}", user, userInfoUri);

            HttpWebRequest request = InitNewRequest(userInfoUri, "GET");

            Func<string, MalUserLookupResults> responseProcessingFunc = (xml) =>
            {
                using (TextReader xmlTextReader = new StringReader(xml))
                {
                    try
                    {
                        return MalAppInfoXml.Parse(xmlTextReader);
                    }
                    catch (MalUserNotFoundException ex)
                    {
                        throw new MalUserNotFoundException(string.Format("No MAL list exists for {0}.", user), ex);
                    }
                }
            };
            MalUserLookupResults parsedList = ProcessRequest(request, responseProcessingFunc,
                baseErrorMessage: string.Format("Failed getting anime list for user {0} using url {1}", user, userInfoUri));

            Logging.Log.InfoFormat("Successfully retrieved anime list for user {0}", user);
            return parsedList;
        }

        private static Lazy<Regex> s_recentOnlineUsersRegex =
            new Lazy<Regex>(() => new Regex("/profile/(?<Username>[^\"]+)\">\\k<Username>",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase));
        public static Regex RecentOnlineUsersRegex { get { return s_recentOnlineUsersRegex.Value; } }

        /// <summary>
        /// Gets a list of users that have been on MAL recently. This scrapes the HTML on the recent users page and therefore
        /// can break if MAL changes the HTML on that page. This method does not require a MAL API key.
        /// </summary>
        /// <returns></returns>
        public RecentUsersResults GetRecentOnlineUsers()
        {
            Logging.Log.InfoFormat("Getting list of recent online MAL users using URI {0}", RecentOnlineUsersUri);

            HttpWebRequest request = InitNewRequest(RecentOnlineUsersUri, "GET");

            RecentUsersResults recentUsers = ProcessRequest(request, ScrapeUsersFromHtml,
                baseErrorMessage: "Failed getting list of recent MAL users.");

            Logging.Log.Info("Successfully got list of recent online MAL users.");
            return recentUsers;
        }

        private RecentUsersResults ScrapeUsersFromHtml(string recentUsersHtml)
        {
            List<string> users = new List<string>();
            MatchCollection userMatches = RecentOnlineUsersRegex.Matches(recentUsersHtml);
            foreach (Match userMatch in userMatches)
            {
                string username = userMatch.Groups["Username"].ToString();
                users.Add(username);
            }

            if (users.Count == 0)
            {
                throw new MalApiException("0 users found in recent users page html.");
            }

            return new RecentUsersResults(users);
        }

        private static readonly string AnimeDetailsUrlFormat = "https://myanimelist.net/anime/{0}";
        private static readonly string GenreIDRegex = "/genre/(?<genreID>[0-9]+)";
        
        /// <summary>
        /// Gets information from an anime's "details" page. This method uses HTML scraping and so may break if MAL changes the HTML.
        /// This method does not require a MAL API key.
        /// </summary>
        /// <param name="animeId"></param>
        /// <returns></returns>
        public AnimeDetailsResults GetAnimeDetails(int animeId)
        {
            string url = string.Format(AnimeDetailsUrlFormat, animeId);
            Logging.Log.InfoFormat("Getting anime details from {0}.", url);
            HttpWebRequest request = InitNewRequest(url, "GET");
            AnimeDetailsResults results = ProcessRequest(request, ScrapeAnimeDetailsFromHtml, animeId, httpErrorStatusHandler: GetAnimeDetailsHttpErrorStatusHandler,
                baseErrorMessage: string.Format("Failed getting anime details for anime ID {0}.", animeId));
            Logging.Log.InfoFormat("Successfully got details from {0}.", url);
            return results;
        }

        // If getting anime details page returned a 404, throw a MalAnimeNotFound exception instead of letting
        // a generic MalApiException be thrown.
        private static AnimeDetailsResults GetAnimeDetailsHttpErrorStatusHandler(WebException ex, int animeId, out bool handled)
        {
            if (ex.Response != null && ex.Response is HttpWebResponse && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
            {
                throw new MalAnimeNotFoundException(string.Format("No anime with id {0} exists.", animeId));
            }
            else
            {
                handled = false;
                return null;
            }
        }

        // internal for unit testing
        internal AnimeDetailsResults ScrapeAnimeDetailsFromHtml(string animeDetailsHtml, int animeId)
        {
            //Create a HtmlDocument, load the page into it and extract the all genre nodes
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(animeDetailsHtml);
            HtmlNodeCollection nodeCollection = htmlDoc.DocumentNode.SelectNodes("//div/a[contains(@href, \"genre\")]/@href");

            if (nodeCollection == null || nodeCollection.Count == 0)
            {
                throw new MalApiException(string.Format("Could not extract information from {0}.", string.Format(AnimeDetailsUrlFormat, animeId)));
            }

            //Extract one genre per node
            List<Genre> genres = new List<Genre>();
            foreach (HtmlNode node in nodeCollection)
            {
                string genreIdString = Regex.Match(node.Attributes["href"].Value, GenreIDRegex).Groups["genreID"].Value;
                int genreId = int.Parse(genreIdString);
                string genreName = node.InnerText;
                genres.Add(new Genre(genreId, genreName));
            }

            return new AnimeDetailsResults(genres);
        }

        public void Dispose()
        {
            ;
        }
    }
}

/*
 Copyright 2016 Greg Najda

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/