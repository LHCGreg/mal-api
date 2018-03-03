using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MalApi.IntegrationTests
{
    class GetMalDetails : IDisposable
    {
        protected static readonly string NoRedirectMalUri = "https://myanimelist.net/pressroom";
        protected static readonly string LoginUri = "https://myanimelist.net/login.php";

        private HttpClientHandler m_httpHandler;
        private HttpClient m_httpClient;

        public string UserAgent { get; set; }

        private string CsrfToken { get; set; }

        /// <summary>
        /// Timeout in milliseconds for requests to MAL. Defaults to 15000 (15s).
        /// </summary>
        public int TimeoutInMs
        {
            get { return m_httpClient.Timeout.Milliseconds; }
            set { m_httpClient.Timeout = TimeSpan.FromMilliseconds(value); }
        }

        public GetMalDetails()
        {
            m_httpHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                UseCookies = true,

                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            m_httpClient = new HttpClient(m_httpHandler);
            TimeoutInMs = 15 * 1000;
        }

        protected HttpRequestMessage InitNewRequest(string uri, HttpMethod method)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, uri);

            if (UserAgent != null)
            {
                request.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
            }

            return request;
        }

        protected async Task<TReturn> ProcessRequestAsync<TReturn>(HttpRequestMessage request, Func<string, TReturn> processingFunc, string baseErrorMessage, CancellationToken cancellationToken)
        {
            try
            {
                // Need to read the entire content at once here because response.Content.ReadAsStringAsync doesn't support cancellation.
                using (HttpResponseMessage response = await m_httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
                        return processingFunc(responseBody);
                    }
                    throw new MalApiRequestException(string.Format("{0} Status code was {1}", baseErrorMessage, (int)response.StatusCode));
                }
            }
            catch (Exception ex)
            {
                    // If we didn't read a response, then there was an error with the request/response that may be fixable with a retry.
                    throw new MalApiRequestException(string.Format("{0} {1}", baseErrorMessage, ex.Message), ex);
            }
        }

        protected string ParseCsrfTokenFromHtml(string html)
        {
            Match match = Regex.Match(html, @"<meta name='csrf_token' content='(.+)'>");
            if (match.Success)
            {
                CsrfToken = match.Groups[1].ToString();
            }

            return CsrfToken;
        }

        // internal for unit testing
        protected string WebFormLoginHandler(string responseBody)
        {
            if (responseBody.Contains("Your username or password is incorrect."))
            {
                throw new MalApiRequestException("Failed to log in. Recheck credentials.");
            }

            return null;
        }

        public void Login(string username, string password)
        {
            Login(username, password, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        private async Task Login(string username, string password, CancellationToken cancellationToken)
        {
            string url = NoRedirectMalUri;

            HttpRequestMessage request = InitNewRequest(url, HttpMethod.Get);

            // Get CSRF token
            await ProcessRequestAsync(request, ParseCsrfTokenFromHtml,
                    cancellationToken: cancellationToken,
                    baseErrorMessage: "Failed connecting to MAL")
                .ConfigureAwait(continueOnCapturedContext: false);

            // Login through the web form
            url = LoginUri;
            request = InitNewRequest(url, HttpMethod.Post);

            var contentPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("user_name", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("cookie", "1"),
                new KeyValuePair<string, string>("sublogin", "Login"),
                new KeyValuePair<string, string>("submit", "1"),
                new KeyValuePair<string, string>("csrf_token", CsrfToken)
            };
            var content = new FormUrlEncodedContent(contentPairs);
            request.Content = content;

            await ProcessRequestAsync(request, WebFormLoginHandler,
                    cancellationToken: cancellationToken,
                    baseErrorMessage: string.Format("Failed to log in for user {0}.", username))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public void Dispose()
        {
            m_httpClient.Dispose();
            m_httpHandler.Dispose();
        }
    }
}
