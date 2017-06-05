using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MalApi
{
    /// <summary>
    /// Limits MAL requests by waiting a given time period betwen each request.
    /// The time is measured from after one request is complete to before the next request is made.
    /// You might use this to be nice to MAL and avoid making requests as fast as possible.
    /// This class is thread-safe if the underlying API is.
    /// </summary>
    public class RateLimitingMyAnimeListApi : IMyAnimeListApi
    {
        private IMyAnimeListApi m_underlyingApi;
        private bool m_ownApi;
        public TimeSpan TimeBetweenRequests { get; private set; }

        private Stopwatch m_stopwatchStartedAtLastRequest = null;
        private SemaphoreSlim m_lock;

        public RateLimitingMyAnimeListApi(IMyAnimeListApi underlyingApi, TimeSpan timeBetweenRequests, bool ownApi = false)
        {
            m_underlyingApi = underlyingApi;
            TimeBetweenRequests = timeBetweenRequests;
            m_ownApi = ownApi;
            m_lock = new SemaphoreSlim(1, 1);
        }

        private async Task<TResult> DoActionWithRateLimitingAsync<TResult>(Func<Task<TResult>> asyncAction, CancellationToken cancellationToken)
        {
            // Wait your turn to get the lock. Only one MAL request can be outstanding at a time.
            await m_lock.WaitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

            // Make sure to release the lock no matter what happens.
            try
            {
                // Sleep if needed
                if (m_stopwatchStartedAtLastRequest != null)
                {
                    TimeSpan timeSinceLastRequest = m_stopwatchStartedAtLastRequest.Elapsed;
                    if (timeSinceLastRequest < TimeBetweenRequests)
                    {
                        TimeSpan timeToWait = TimeBetweenRequests - timeSinceLastRequest;
                        Logging.Log.InfoFormat("Waiting {0} before making request.", timeToWait);
                        await Task.Delay(timeToWait).ConfigureAwait(continueOnCapturedContext: false);
                    }
                }

                // Do the MAL request
                TResult result = await asyncAction().ConfigureAwait(continueOnCapturedContext: false);

                // Start the stopwatch so the next request knows how long it has to wait
                if (m_stopwatchStartedAtLastRequest == null)
                {
                    m_stopwatchStartedAtLastRequest = new Stopwatch();
                }
                m_stopwatchStartedAtLastRequest.Restart();

                return result;
            }
            finally
            {
                m_lock.Release();
            }
        }

        public Task<MalUserLookupResults> GetAnimeListForUserAsync(string user, CancellationToken cancellationToken)
        {
            return DoActionWithRateLimitingAsync(() => m_underlyingApi.GetAnimeListForUserAsync(user, cancellationToken), cancellationToken);
        }

        public Task<MalUserLookupResults> GetAnimeListForUserAsync(string user)
        {
            return GetAnimeListForUserAsync(user, CancellationToken.None);
        }

        public MalUserLookupResults GetAnimeListForUser(string user)
        {
            return GetAnimeListForUserAsync(user).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        public Task<RecentUsersResults> GetRecentOnlineUsersAsync(CancellationToken cancellationToken)
        {
            return DoActionWithRateLimitingAsync(() => m_underlyingApi.GetRecentOnlineUsersAsync(cancellationToken), cancellationToken);
        }

        public Task<RecentUsersResults> GetRecentOnlineUsersAsync()
        {
            return GetRecentOnlineUsersAsync(CancellationToken.None);
        }

        public RecentUsersResults GetRecentOnlineUsers()
        {
            return GetRecentOnlineUsersAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        public Task<AnimeDetailsResults> GetAnimeDetailsAsync(int animeId, CancellationToken cancellationToken)
        {
            return DoActionWithRateLimitingAsync(() => m_underlyingApi.GetAnimeDetailsAsync(animeId, cancellationToken), cancellationToken);
        }

        public Task<AnimeDetailsResults> GetAnimeDetailsAsync(int animeId)
        {
            return GetAnimeDetailsAsync(animeId, CancellationToken.None);
        }

        public AnimeDetailsResults GetAnimeDetails(int animeId)
        {
            return GetAnimeDetailsAsync(animeId).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            if (m_ownApi && m_underlyingApi != null)
            {
                m_underlyingApi.Dispose();
            }
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
