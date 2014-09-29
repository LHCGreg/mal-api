using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MalApi
{
    /// <summary>
    /// Retries failed requests after waiting for a short period. After a certain number of failures, it gives up and propagates the
    /// thrown exception.
    /// </summary>
    public class RetryOnFailureMyAnimeListApi : IMyAnimeListApi
    {
        private IMyAnimeListApi m_underlyingApi;
        private bool m_ownApi;
        private int m_numTriesBeforeGivingUp;
        private int m_timeBetweenRetriesInMs;

        public RetryOnFailureMyAnimeListApi(IMyAnimeListApi underlyingApi, int numTriesBeforeGivingUp, int timeBetweenRetriesInMs, bool ownApi = false)
        {
            m_underlyingApi = underlyingApi;
            m_numTriesBeforeGivingUp = numTriesBeforeGivingUp;
            m_timeBetweenRetriesInMs = timeBetweenRetriesInMs;
            m_ownApi = ownApi;
        }

        private TResult DoActionWithRetry<TResult>(Func<TResult> action, string baseErrorMessage)
        {
            int numTries = 0;
            while (true)
            {
                try
                {
                    return action();
                }
                catch (MalApiRequestException ex)
                {
                    numTries++;
                    Logging.Log.ErrorFormat("{0} (failure {1}): {2}", ex, baseErrorMessage, numTries, ex.Message);

                    if (numTries < m_numTriesBeforeGivingUp)
                    {
                        Logging.Log.InfoFormat("Waiting {0} ms before trying again.", m_timeBetweenRetriesInMs);
                        Thread.Sleep(m_timeBetweenRetriesInMs);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public MalUserLookupResults GetAnimeListForUser(string user)
        {
            return DoActionWithRetry(() => m_underlyingApi.GetAnimeListForUser(user),
                baseErrorMessage: string.Format("Error getting anime list for user {0}", user));
        }

        public RecentUsersResults GetRecentOnlineUsers()
        {
            return DoActionWithRetry(() => m_underlyingApi.GetRecentOnlineUsers(),
                baseErrorMessage: "Error getting recently active MAL users");
        }

        public AnimeDetailsResults GetAnimeDetails(int animeId)
        {
            return DoActionWithRetry(() => m_underlyingApi.GetAnimeDetails(animeId),
                baseErrorMessage: string.Format("Error getting details for anime id {0}", animeId));
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
 Copyright 2012 Greg Najda

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