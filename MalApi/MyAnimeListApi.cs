using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;

namespace MalApi
{
    /// <summary>
    /// Class for accessing myanimelist.net. Methods are thread-safe. Properties are not.
    /// </summary>
    public class MyAnimeListApi : IMyAnimeListApi
    {
        private static readonly string AnimeDetailsUrlFormat = "https://myanimelist.net/anime/{0}";

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

        /// <summary>
        /// Timeout in milliseconds for requests to MAL. Defaults to 15000 (15s).
        /// </summary>
        public int TimeoutInMs
        {
            get { return m_httpClient.Timeout.Milliseconds; }
            set { m_httpClient.Timeout = TimeSpan.FromMilliseconds(value); }
        }

        private HttpClientHandler m_httpHandler;
        private HttpClient m_httpClient;

        public MyAnimeListApi()
        {
            m_httpHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                UseCookies = false,

                // Very important optimization! Time to get an anime list of ~150 entries 2.6s -> 0.7s
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            m_httpClient = new HttpClient(m_httpHandler);
            TimeoutInMs = 15 * 1000;
        }

        private HttpRequestMessage InitNewRequest(string uri, HttpMethod method)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, uri);

            if (UserAgent != null)
            {
                request.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
            }

            return request;
        }

        private HttpRequestMessage InitNewRequestWithCredentials(string uri, HttpMethod method, string user, string password)
        {
            HttpRequestMessage request = InitNewRequest(uri, method);

            // Requests with credentials require them to be encoded in base64
            string credentials;
            if (user != null && password != null)
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(user + ":" + password);
                credentials = Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                // This will always be true given the && condition above
                throw new ArgumentNullException(nameof(password));
            }

            // Adding the authorization header with the credentials
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            return request;
        }

        private Task<TReturn> ProcessRequestAsync<TReturn>(HttpRequestMessage request, Func<string, TReturn> processingFunc, string baseErrorMessage, CancellationToken cancellationToken)
        {
            return ProcessRequestAsync(request, (string html, object dummy) => processingFunc(html), (object)null,
                httpErrorStatusHandler: null, baseErrorMessage: baseErrorMessage, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Expected to do one of the following:
        /// 1) Set handled to true and return a valid result
        /// 2) Throw a new exception that better matches the abstraction level, for example a MalAnimeNotFoundException.
        /// 3) Set handled to false and return anything
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="response"></param>
        /// <param name="data"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        delegate TReturn HttpErrorStatusHandler<TData, TReturn>(HttpResponseMessage response, TData data, out bool handled);

        private async Task<TReturn> ProcessRequestAsync<TReturn, TData>(HttpRequestMessage request, Func<string, TData, TReturn> processingFunc, TData data, HttpErrorStatusHandler<TData, TReturn> httpErrorStatusHandler, string baseErrorMessage, CancellationToken cancellationToken)
        {
            string responseBody = null;
            try
            {
                Logging.Log.DebugFormat("Starting MAL request to {0}", request.RequestUri);

                // Need to read the entire content at once here because response.Content.ReadAsStringAsync doesn't support cancellation.
                using (HttpResponseMessage response = await m_httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
                {
                    Logging.Log.DebugFormat("Got response. Status code = {0}.", (int)response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
                        Logging.Log.Debug("Read response body.");
                        return processingFunc(responseBody, data);
                    }
                    else
                    {
                        if (httpErrorStatusHandler != null)
                        {
                            TReturn result = httpErrorStatusHandler(response, data, out bool handled);

                            // If the handler knew what to do and returned a result, return that result
                            if (handled)
                            {
                                return result;
                            }

                            // If the handler knew what to do and threw a better exception, it will get caught below and thrown further.
                        }

                        // If there was a handler and it did not know what to do with the http error,
                        // or if no handler was passed, throw an exception.
                        throw new MalApiRequestException(string.Format("{0} Status code was {1}", baseErrorMessage, (int)response.StatusCode));
                    }
                }
            }
            catch (MalUserNotFoundException)
            {
                throw;
            }
            catch (MalAnimeNotFoundException)
            {
                throw;
            }
            catch (OperationCanceledException)
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
        public Task<MalUserLookupResults> GetAnimeListForUserAsync(string user)
        {
            return GetAnimeListForUserAsync(user, CancellationToken.None);
        }

        /// <summary>
        /// Updates a user's anime list entry.
        /// </summary>
        /// <param name="animeId">ID of the anime</param>
        /// <param name="updateInfo">The updated information</param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<string> UpdateAnimeForUserAsync(int animeId, AnimeUpdate updateInfo, string user, string password)
        {
            return UpdateAnimeForUserAsync(animeId, updateInfo, user, password, CancellationToken.None);
        }

        /// <summary>
        /// Gets a user's manga list. This method requires a MAL API key.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        public Task<MalUserLookupResults> GetMangaListForUserAsync(string user)
        {
            return GetMangaListForUserAsync(user, CancellationToken.None);
        }

        /// <summary>
        /// Updates a user's manga list entry.
        /// </summary>
        /// <param name="animeId">ID of the manga</param>
        /// <param name="updateInfo">The updated information</param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<string> UpdateMangaForUserAsync(int mangaId, MangaUpdate updateInfo, string user, string password)
        {
            return UpdateMangaForUserAsync(mangaId, updateInfo, user, password, CancellationToken.None);
        }

        /// <summary>
        /// Gets a user's anime list. This method requires a MAL API key.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        public async Task<MalUserLookupResults> GetAnimeListForUserAsync(string user, CancellationToken cancellationToken)
        {
            const string malAppAnimeInfoUriFormatString = "https://myanimelist.net/malappinfo.php?status=all&type=anime&u={0}";

            string userInfoUri = string.Format(malAppAnimeInfoUriFormatString, Uri.EscapeDataString(user));

            Logging.Log.InfoFormat("Getting anime list for MAL user {0} using URI {1}", user, userInfoUri);

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

            try
            {
                HttpRequestMessage request = InitNewRequest(userInfoUri, HttpMethod.Get);
                MalUserLookupResults parsedList = await ProcessRequestAsync(request, responseProcessingFunc,
                    cancellationToken: cancellationToken,
                    baseErrorMessage: string.Format("Failed getting anime list for user {0} using url {1}", user,
                        userInfoUri)).ConfigureAwait(continueOnCapturedContext: false);

                Logging.Log.InfoFormat("Successfully retrieved anime list for user {0}", user);
                return parsedList;
            }
            catch (OperationCanceledException)
            {
                Logging.Log.InfoFormat("Canceled getting anime list for MAL user {0}", user);
                throw;
            }
        }

        /// <summary>
        /// Updates a user's anime list entry.
        /// </summary>
        /// <param name="animeId">ID of the anime</param>
        /// <param name="updateInfo">The updated information</param>
        /// <param name="cancellationToken"></param>
        /// <param name="user">MAL username</param>
        /// <param name="password">MAL password</param>
        /// <returns></returns>
        public async Task<string> UpdateAnimeForUserAsync(int animeId, AnimeUpdate updateInfo, string user, string password, CancellationToken cancellationToken)
        {
            const string malAnimeUpdateUriFormatString = "https://myanimelist.net/api/animelist/update/{0}.xml";

            string userInfoUri = string.Format(malAnimeUpdateUriFormatString, Uri.EscapeDataString(animeId.ToString()));

            Logging.Log.InfoFormat("Updating anime entry for MAL anime ID {0}, user {1} using URI {2}", animeId, user, userInfoUri);

            Func<string, string> responseProcessingFunc = (response) =>
            {
                return response;
            };

            try
            {
                HttpRequestMessage request = InitNewRequestWithCredentials(userInfoUri, HttpMethod.Post, user, password);

                // Encoding and adding the new information in the content(body) of the request
                string xml = updateInfo.GenerateXml();
                request.Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("data", xml) });

                string result = await ProcessRequestAsync(request, responseProcessingFunc, cancellationToken: cancellationToken,
                    baseErrorMessage: string.Format("Failed updating anime entry for anime ID {0}, user {0} using url {1}", animeId, user, userInfoUri)).ConfigureAwait(continueOnCapturedContext: false);

                Logging.Log.InfoFormat("Successfully updated anime entry for anime ID {0} and user {0}", animeId, user);

                return result;
            }
            catch (OperationCanceledException)
            {
                Logging.Log.InfoFormat("Canceled updating anime entry for MAL anime ID {0} and user {0}", animeId, user);
                throw;
            }
        }

        /// <summary>
        /// Gets a user's manga list. This method requires a MAL API key.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        public async Task<MalUserLookupResults> GetMangaListForUserAsync(string user,
            CancellationToken cancellationToken)
        {
            const string malAppMangaInfoUriFormatString = "https://myanimelist.net/malappinfo.php?status=all&type=manga&u={0}";

            string userInfoUri = string.Format(malAppMangaInfoUriFormatString, Uri.EscapeDataString(user));

            Logging.Log.InfoFormat("Getting manga list for MAL user {0} using URI {1}", user, userInfoUri);

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

            try
            {
                HttpRequestMessage request = InitNewRequest(userInfoUri, HttpMethod.Get);
                MalUserLookupResults parsedList = await ProcessRequestAsync(request, responseProcessingFunc,
                    cancellationToken: cancellationToken,
                    baseErrorMessage: string.Format("Failed getting manga list for user {0} using url {1}", user,
                        userInfoUri)).ConfigureAwait(continueOnCapturedContext: false);

                Logging.Log.InfoFormat("Successfully retrieved manga list for user {0}", user);
                return parsedList;
            }
            catch (OperationCanceledException)
            {
                Logging.Log.InfoFormat("Canceled getting manga list for MAL user {0}", user);
                throw;
            }
        }

        /// <summary>
        /// Updates a user's manga list entry.
        /// </summary>
        /// <param name="mangaId">ID of the manga</param>
        /// <param name="updateInfo">The updated information</param>
        /// <param name="cancellationToken"></param>
        /// <param name="user">MAL user</param>
        /// <param name="password">MAL password</param>
        /// <returns></returns>
        public async Task<string> UpdateMangaForUserAsync(int mangaId, MangaUpdate updateInfo, string user, string password, CancellationToken cancellationToken)
        {
            const string malMangaUpdateUriFormatString = "https://myanimelist.net/api/mangalist/update/{0}.xml";

            string userInfoUri = string.Format(malMangaUpdateUriFormatString, Uri.EscapeDataString(mangaId.ToString()));

            Logging.Log.InfoFormat("Updating manga entry for MAL manga ID {0}, user {1} using URI {2}", mangaId, user, userInfoUri);

            Func<string, string> responseProcessingFunc = (response) =>
            {
                return response;
            };

            try
            {
                HttpRequestMessage request = InitNewRequestWithCredentials(userInfoUri, HttpMethod.Post, user, password);

                // Encoding and adding the new information in the content(body) of the request
                string xml = updateInfo.GenerateXml();
                request.Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("data", xml) });

                string result = await ProcessRequestAsync(request, responseProcessingFunc, cancellationToken: cancellationToken,
                    baseErrorMessage: string.Format("Failed updating manga entry for manga ID {0}, user {0} using url {1}", mangaId, user, userInfoUri)).ConfigureAwait(continueOnCapturedContext: false);

                Logging.Log.InfoFormat("Successfully updated manga entry for manga ID {0} and user {0}", mangaId, user);

                return result;
            }
            catch (OperationCanceledException)
            {
                Logging.Log.InfoFormat("Canceled updating manga entry for MAL manga ID {0} and user {0}", mangaId, user);
                throw;
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
            return GetAnimeListForUserAsync(user).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Updates a user's anime entry. Required username and password or a base64 encrypted username and password.
        /// </summary>
        /// <param name="animeId">ID of the updated anime</param>
        /// <param name="xml">Required data to update the anime</param>
        /// <param name="user">Username</param>
        /// <param name="password">Password</param>
        /// <param name="base64Credentials">base64 encrypted username and password</param>
        /// <returns></returns>
        public string UpdateAnimeForUser(int animeId, AnimeUpdate updateInfo, string user, string password)
        {
            return UpdateAnimeForUserAsync(animeId, updateInfo, user, password).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets a user's manga list. This method requires a MAL API key.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        public MalUserLookupResults GetMangaListForUser(string user)
        {
            return GetMangaListForUserAsync(user).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Updates a user's manga entry. Required username and password or a base64 encrypted username and password.
        /// </summary>
        /// <param name="mangaId">ID of the updated manga</param>
        /// <param name="xml">Required data to update the manga</param>
        /// <param name="user">Username</param>
        /// <param name="password">Password</param>
        /// <param name="base64Credentials">base64 encrypted username and password</param>
        /// <returns></returns>
        public string UpdateMangaForUser(int mangaId, MangaUpdate updateInfo, string user, string password)
        {
            return UpdateMangaForUserAsync(mangaId, updateInfo, user, password).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        private static Lazy<Regex> s_recentOnlineUsersRegex =
            new Lazy<Regex>(() => new Regex("/profile/(?<Username>[^\"]+)\">\\k<Username>",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase));
        public static Regex RecentOnlineUsersRegex { get { return s_recentOnlineUsersRegex.Value; } }

        public Task<RecentUsersResults> GetRecentOnlineUsersAsync()
        {
            return GetRecentOnlineUsersAsync(CancellationToken.None);
        }

        /// <summary>
        /// Gets a list of users that have been on MAL recently. This scrapes the HTML on the recent users page and therefore
        /// can break if MAL changes the HTML on that page. This method does not require a MAL API key.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        public async Task<RecentUsersResults> GetRecentOnlineUsersAsync(CancellationToken cancellationToken)
        {
            Logging.Log.InfoFormat("Getting list of recent online MAL users using URI {0}", RecentOnlineUsersUri);

            HttpRequestMessage request = InitNewRequest(RecentOnlineUsersUri, HttpMethod.Get);

            try
            {
                RecentUsersResults recentUsers = await ProcessRequestAsync(request, ScrapeUsersFromHtml,
                    baseErrorMessage: "Failed getting list of recent MAL users.", cancellationToken: cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);

                Logging.Log.Info("Successfully got list of recent online MAL users.");
                return recentUsers;
            }
            catch (OperationCanceledException)
            {
                Logging.Log.Info("Canceled getting list of recent online MAL users.");
                throw;
            }
        }

        /// <summary>
        /// Gets a list of users that have been on MAL recently. This scrapes the HTML on the recent users page and therefore
        /// can break if MAL changes the HTML on that page. This method does not require a MAL API key.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        public RecentUsersResults GetRecentOnlineUsers()
        {
            return GetRecentOnlineUsersAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
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

        
        private static Lazy<Regex> s_animeDetailsRegex = new Lazy<Regex>(() => new Regex(
@"Genres:</span>\s*?(?:<a href=""/anime/genre/(?<GenreId>\d+)/[^""]+?""[^>]*?>(?<GenreName>.*?)</a>(?:, )?)*</div>",
RegexOptions.Compiled));
        private static Regex AnimeDetailsRegex { get { return s_animeDetailsRegex.Value; } }

        /// <summary>
        /// Gets information from an anime's "details" page. This method uses HTML scraping and so may break if MAL changes the HTML.
        /// This method does not require a MAL API key.
        /// </summary>
        /// <param name="animeId"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        public Task<AnimeDetailsResults> GetAnimeDetailsAsync(int animeId)
        {
            return GetAnimeDetailsAsync(animeId, CancellationToken.None);
        }

        /// <summary>
        /// Gets information from an anime's "details" page. This method uses HTML scraping and so may break if MAL changes the HTML.
        /// This method does not require a MAL API key.
        /// </summary>
        /// <param name="animeId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        public async Task<AnimeDetailsResults> GetAnimeDetailsAsync(int animeId, CancellationToken cancellationToken)
        {
            string url = string.Format(AnimeDetailsUrlFormat, animeId);
            Logging.Log.InfoFormat("Getting anime details for anime ID {0} from {1}.", animeId, url);

            HttpRequestMessage request = InitNewRequest(url, HttpMethod.Get);

            try
            {
                AnimeDetailsResults results = await ProcessRequestAsync(request, ScrapeAnimeDetailsFromHtml, animeId,
                    httpErrorStatusHandler: GetAnimeDetailsHttpErrorStatusHandler, cancellationToken: cancellationToken,
                    baseErrorMessage: string.Format("Failed getting anime details for anime ID {0}.", animeId))
                    .ConfigureAwait(continueOnCapturedContext: false);
                Logging.Log.InfoFormat("Successfully got details from {0}.", url);
                return results;
            }
            catch (OperationCanceledException)
            {
                Logging.Log.InfoFormat("Canceled getting anime details for anime ID {0}.", animeId);
                throw;
            }
        }

        /// <summary>
        /// Gets information from an anime's "details" page. This method uses HTML scraping and so may break if MAL changes the HTML.
        /// This method does not require a MAL API key.
        /// </summary>
        /// <param name="animeId"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        public AnimeDetailsResults GetAnimeDetails(int animeId)
        {
            return GetAnimeDetailsAsync(animeId).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        // If getting anime details page returned a 404, throw a MalAnimeNotFound exception instead of letting
        // a generic MalApiException be thrown.
        private static AnimeDetailsResults GetAnimeDetailsHttpErrorStatusHandler(HttpResponseMessage response, int animeId, out bool handled)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
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
            Match match = AnimeDetailsRegex.Match(animeDetailsHtml);

            if (!match.Success)
            {
                throw new MalApiException(string.Format("Could not extract information from {0}.", string.Format(AnimeDetailsUrlFormat, animeId)));
            }

            Group genreIds = match.Groups["GenreId"];
            Group genreNames = match.Groups["GenreName"];
            List<Genre> genres = new List<Genre>();
            for (int i = 0; i < genreIds.Captures.Count; i++)
            {
                string genreIdString = genreIds.Captures[i].Value;
                int genreId = int.Parse(genreIdString);
                string genreName = WebUtility.HtmlDecode(genreNames.Captures[i].Value);
                genres.Add(new Genre(genreId, genreName));
            }

            return new AnimeDetailsResults(genres);
        }

        public void Dispose()
        {
            m_httpClient.Dispose();
            m_httpHandler.Dispose();
        }
    }
}

/*
 Copyright 2017 Greg Najda

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
