using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MalApi
{
    /// <summary>
    /// This class is thread-safe if the underlying API is. If the expiration time is null, anime lists are cached for the lifetime of
    /// the object. Expired cache entries are only actually removed when a new anime list is inserted into the cache. Cache expiration
    /// measurement is susceptible to changes to the system clock.
    /// 
    /// This class only caches user anime lists from GetAnimeListForUser(). Other functions are not cached.
    /// </summary>
    public class CachingMyAnimeListApi : IMyAnimeListApi
    {
        private IMyAnimeListApi m_underlyingApi;
        private bool m_ownUnderlyingApi;
        private AnimeListCache m_cache;

        public CachingMyAnimeListApi(IMyAnimeListApi underlyingApi, TimeSpan? expiration, bool ownApi = false)
        {
            m_underlyingApi = underlyingApi;
            m_ownUnderlyingApi = ownApi;
            m_cache = new AnimeListCache(expiration);
        }

        /// <summary>
        /// Gets a user's anime list.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MalUserLookupResults> GetAnimeListForUserAsync(string user, CancellationToken cancellationToken)
        {
            Logging.Log.InfoFormat("Checking cache for user {0}.", user);

            if (m_cache.GetListForUser(user, out MalUserLookupResults cachedAnimeList))
            {
                if (cachedAnimeList != null)
                {
                    Logging.Log.InfoFormat("Got anime list for {0} from cache.", user);
                    return cachedAnimeList;
                }
                else
                {
                    // User does not have an anime list/no such user exists
                    Logging.Log.InfoFormat("Cache indicates that user {0} does not have an anime list.", user);
                    throw new MalUserNotFoundException(string.Format("No MAL list exists for {0}.", user));
                }
            }
            else
            {
                Logging.Log.InfoFormat("Cache did not contain anime list for {0}.", user);

                try
                {
                    MalUserLookupResults animeList = await m_underlyingApi.GetAnimeListForUserAsync(user, cancellationToken)
                        .ConfigureAwait(continueOnCapturedContext: false);
                    m_cache.PutListForUser(user, animeList);
                    return animeList;
                }
                catch (MalUserNotFoundException)
                {
                    // Cache the fact that the user does not have an anime list
                    m_cache.PutListForUser(user, null);
                    throw;
                }
            }
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
            return m_underlyingApi.GetRecentOnlineUsersAsync(cancellationToken);
        }

        public Task<RecentUsersResults> GetRecentOnlineUsersAsync()
        {
            return m_underlyingApi.GetRecentOnlineUsersAsync();
        }

        public RecentUsersResults GetRecentOnlineUsers()
        {
            return m_underlyingApi.GetRecentOnlineUsers();
        }

        public Task<AnimeDetailsResults> GetAnimeDetailsAsync(int animeId, CancellationToken cancellationToken)
        {
            return m_underlyingApi.GetAnimeDetailsAsync(animeId, cancellationToken);
        }

        public Task<AnimeDetailsResults> GetAnimeDetailsAsync(int animeId)
        {
            return m_underlyingApi.GetAnimeDetailsAsync(animeId);
        }

        public AnimeDetailsResults GetAnimeDetails(int animeId)
        {
            return m_underlyingApi.GetAnimeDetails(animeId);
        }

        public void Dispose()
        {
            m_cache.Dispose();
            if (m_ownUnderlyingApi)
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
