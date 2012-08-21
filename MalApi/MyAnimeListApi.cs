﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MalApi
{
    /// <summary>
    /// Class for accessing myanimelist.net. Methods are thread-safe. Properties are not.
    /// </summary>
    public class MyAnimeListApi : IMyAnimeListApi
    {
        private const string MalAppInfoUri = "http://myanimelist.net/malappinfo.php?status=all&type=anime";
        private const string RecentOnlineUsersUri = "http://myanimelist.net/users.php";

        /// <summary>
        /// What to set the user agent http header to in API requests. Null to use the default .NET user agent.
        /// </summary>
        public string UserAgent { get; set; }

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
            string responseBody = null;
            try
            {
                Logging.Log.DebugFormat("Starting MAL request to {0}", request.RequestUri);
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

                Logging.Log.Debug("Read response body.");

                return processingFunc(responseBody);
            }
            catch (MalUserNotFoundException)
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
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="AnimeCompatibility.MalUserNotFoundException"></exception>
        /// <exception cref="AnimeCompatibility.MalApiException"></exception>
        public MalUserLookupResults GetAnimeListForUser(string user)
        {
            string userInfoUri = MalAppInfoUri + "&u=" + Uri.EscapeDataString(user);

            Logging.Log.InfoFormat("Getting anime list for MAL user {0} using URI {1}", user, userInfoUri);

            HttpWebRequest request = InitNewRequest(userInfoUri, "GET");

            Func<string, MalUserLookupResults> responseProcessingFunc = (xml) =>
            {
                using (TextReader xmlTextReader = new StringReader(xml))
                    return ParseAnimeListXml(xmlTextReader, user);
            };
            MalUserLookupResults parsedList = ProcessRequest(request, responseProcessingFunc,
                baseErrorMessage: string.Format("Failed getting anime list for user {0} using url {1}", user, userInfoUri));

            Logging.Log.InfoFormat("Successfully retrieved anime list for user {0}", user);
            return parsedList;
        }

        private static readonly string[] SynonymSeparator = new string[] { "; " };

        private XElement GetExpectedElement(XContainer container, string elementName)
        {
            XElement element = container.Element(elementName);
            if (element == null)
            {
                throw new MalApiException(string.Format("Did not find element {0}.", elementName));
            }
            return element;
        }

        private string GetElementValueString(XContainer container, string elementName)
        {
            XElement element = GetExpectedElement(container, elementName);

            try
            {
                return (string)element;
            }
            catch (FormatException ex)
            {
                throw new MalApiException(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
            }
        }

        private int GetElementValueInt(XContainer container, string elementName)
        {
            XElement element = GetExpectedElement(container, elementName);

            try
            {
                return (int)element;
            }
            catch (FormatException ex)
            {
                throw new MalApiException(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
            }
        }

        private long GetElementValueLong(XContainer container, string elementName)
        {
            XElement element = GetExpectedElement(container, elementName);

            try
            {
                return (long)element;
            }
            catch (FormatException ex)
            {
                throw new MalApiException(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
            }
        }

        private decimal GetElementValueDecimal(XContainer container, string elementName)
        {
            XElement element = GetExpectedElement(container, elementName);

            try
            {
                return (decimal)element;
            }
            catch (FormatException ex)
            {
                throw new MalApiException(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
            }
        }

        // Rumor has it that compiled regexes are far more performant than non-compiled regexes on large pieces of text.
        // I haven't profiled it though.
        private static Lazy<Regex> s_tagElementContentsRegex =
            new Lazy<Regex>(() => new Regex("<my_tags>(?<TagText>.*?)</my_tags>", RegexOptions.Compiled | RegexOptions.CultureInvariant ));
        private static Regex TagElementContentsRegex { get { return s_tagElementContentsRegex.Value; } }

        private static Lazy<Regex> s_tagTextReplacementRegex =
            new Lazy<Regex>(() => new Regex("&(?!lt;)(?!gt;)(?!amp;)(?!apos;)(?!quot;)(?!#x[0-9a-fA-f]+;)(?!#[0-9]+;)", RegexOptions.Compiled | RegexOptions.CultureInvariant));
        private static Regex TagTextReplacementRegex { get { return s_tagTextReplacementRegex.Value; } }

        // Replace & with &amp; only if the & is not part of &lt; &gt; &amp; &apos; &quot; &#x<hex digits>; &#<decimal digits>;
        private static MatchEvaluator TagElementContentsReplacer = (Match match) =>
            {
                string tagText = match.Groups["TagText"].Value;
                string replacementTagText = TagTextReplacementRegex.Replace(tagText, "&amp;");
                return "<my_tags>" + replacementTagText + "</my_tags>";
            };

        /// <summary>
        /// Sanitizes anime list XML which is not always well-formed. If a user uses &amp; characters in their tags,
        /// they will not be escaped in the XML.
        /// </summary>
        /// <param name="xmlTextReader"></param>
        /// <returns></returns>
        private TextReader SanitizeAnimeListXml(TextReader xmlTextReader)
        {
            string rawXml = xmlTextReader.ReadToEnd();
            string sanitizedXml = TagElementContentsRegex.Replace(rawXml, TagElementContentsReplacer);
            return new StringReader(sanitizedXml);
        }

        private static char[] TagSeparator = new char[] { ',' };

        // internal for unit testing
        internal MalUserLookupResults ParseAnimeListXml(TextReader xmlTextReader, string user)
        {
            Logging.Log.Trace("Parsing XML.");

            Logging.Log.Trace("Sanitizing XML.");
            using (xmlTextReader = SanitizeAnimeListXml(xmlTextReader))
            {
                Logging.Log.Trace("XML sanitized.");

                XDocument doc = XDocument.Load(xmlTextReader);

                XElement error = doc.Root.Element("error");
                if (error != null && (string)error == "Invalid username")
                {
                    throw new MalUserNotFoundException(string.Format("No MAL list exists for {0}.", user));
                }
                else if (error != null)
                {
                    throw new MalApiException((string)error);
                }

                XElement myinfo = GetExpectedElement(doc.Root, "myinfo");
                int userId = GetElementValueInt(myinfo, "user_id");
                string canonicalUserName = GetElementValueString(myinfo, "user_name");

                List<MyAnimeListEntry> entries = new List<MyAnimeListEntry>();

                IEnumerable<XElement> animes = doc.Root.Elements("anime");
                foreach (XElement anime in animes)
                {
                    int animeId = GetElementValueInt(anime, "series_animedb_id");
                    string title = GetElementValueString(anime, "series_title");

                    string synonymList = GetElementValueString(anime, "series_synonyms");
                    string[] rawSynonyms = synonymList.Split(SynonymSeparator, StringSplitOptions.RemoveEmptyEntries);

                    // filter out synonyms that are the same as the main title
                    HashSet<string> synonyms = new HashSet<string>(rawSynonyms.Where(synonym => !synonym.Equals(title, StringComparison.Ordinal)));

                    int seriesTypeInt = GetElementValueInt(anime, "series_type");
                    MalAnimeType seriesType = (MalAnimeType)seriesTypeInt;

                    int numEpisodes = GetElementValueInt(anime, "series_episodes");

                    int seriesStatusInt = GetElementValueInt(anime, "series_status");
                    MalSeriesStatus seriesStatus = (MalSeriesStatus)seriesStatusInt;

                    string seriesStartString = GetElementValueString(anime, "series_start");
                    UncertainDate seriesStart = UncertainDate.FromMalDateString(seriesStartString);

                    string seriesEndString = GetElementValueString(anime, "series_end");
                    UncertainDate seriesEnd = UncertainDate.FromMalDateString(seriesEndString);

                    string seriesImage = GetElementValueString(anime, "series_image");

                    MalAnimeInfoFromUserLookup animeInfo = new MalAnimeInfoFromUserLookup(animeId: animeId, title: title,
                        type: seriesType, synonyms: synonyms, status: seriesStatus, numEpisodes: numEpisodes, startDate: seriesStart,
                        endDate: seriesEnd, imageUrl: seriesImage);


                    int numEpisodesWatched = GetElementValueInt(anime, "my_watched_episodes");

                    string myStartDateString = GetElementValueString(anime, "my_start_date");
                    UncertainDate myStartDate = UncertainDate.FromMalDateString(myStartDateString);

                    string myFinishDateString = GetElementValueString(anime, "my_finish_date");
                    UncertainDate myFinishDate = UncertainDate.FromMalDateString(myFinishDateString);

                    decimal rawScore = GetElementValueDecimal(anime, "my_score");
                    decimal? myScore = rawScore == 0 ? (decimal?)null : rawScore;

                    int completionStatusInt = GetElementValueInt(anime, "my_status");
                    CompletionStatus completionStatus = (CompletionStatus)completionStatusInt;

                    long lastUpdatedUnixTimestamp = GetElementValueLong(anime, "my_last_updated");
                    DateTime lastUpdated = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(lastUpdatedUnixTimestamp);

                    string rawTagsString = GetElementValueString(anime, "my_tags");
                    string[] untrimmedTags = rawTagsString.Split(TagSeparator, StringSplitOptions.RemoveEmptyEntries);
                    List<string> tags = new List<string>(untrimmedTags.Select(tag => tag.Trim()));

                    MyAnimeListEntry entry = new MyAnimeListEntry(score: myScore, status: completionStatus, numEpisodesWatched: numEpisodesWatched,
                        myStartDate: myStartDate, myFinishDate: myFinishDate, myLastUpdate: lastUpdated, animeInfo: animeInfo, tags: tags);

                    entries.Add(entry);
                }

                MalUserLookupResults results = new MalUserLookupResults(userId: userId, canonicalUserName: canonicalUserName, animeList: entries);
                Logging.Log.Trace("Parsed XML.");
                return results;
            }
        }

        private static Lazy<Regex> s_recentOnlineUsersRegex =
            new Lazy<Regex>(() => new Regex("myanimelist.net/profile/(?<Username>[^\"]+)\">\\k<Username>",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase));
        public static Regex RecentOnlineUsersRegex { get { return s_recentOnlineUsersRegex.Value; } }

        /// <summary>
        /// Gets a list of users that have been on MAL recently. This scrapes the HTML on the recent users page and therefore
        /// can break if MAL changes the HTML on that page.
        /// </summary>
        /// <returns></returns>
        public RecentUsersResults GetRecentOnlineUsers()
        {
            Logging.Log.InfoFormat("Getting list of recent online MAL users using URI {0}", RecentOnlineUsersUri);

            HttpWebRequest request = InitNewRequest(RecentOnlineUsersUri, "GET");

            RecentUsersResults recentUsers = ProcessRequest(request, ScrapeUsersFromHtml,
                baseErrorMessage: string.Format("Failed getting list of recent MAL users."));

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

        public void Dispose()
        {
            ;
        }
    }
}

/*
 Copyright 2011 Greg Najda

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