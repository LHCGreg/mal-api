using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MalApi
{
    public interface IMyAnimeListApi : IDisposable
    {
        /// <summary>
        /// Gets a user's anime list.
        /// </summary>
        /// <returns></returns>
        /// <param name="user"></param>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        Task<MalUserLookupResults> GetAnimeListForUserAsync(string user);

        /// <summary>
        /// Gets a user's anime list.
        /// </summary>
        /// <returns></returns>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        Task<MalUserLookupResults> GetAnimeListForUserAsync(string user, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a user's anime list.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        MalUserLookupResults GetAnimeListForUser(string user);

        /// <summary>
        /// Gets a list of users that have been on MAL recently.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        Task<RecentUsersResults> GetRecentOnlineUsersAsync();

        /// <summary>
        /// Gets a list of users that have been on MAL recently.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        Task<RecentUsersResults> GetRecentOnlineUsersAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets a list of users that have been on MAL recently.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        RecentUsersResults GetRecentOnlineUsers();

        /// <summary>
        /// Gets information from an anime's "details" page.
        /// </summary>
        /// <param name="animeId"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        Task<AnimeDetailsResults> GetAnimeDetailsAsync(int animeId);

        /// <summary>
        /// Gets information from an anime's "details" page.
        /// </summary>
        /// <param name="animeId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        Task<AnimeDetailsResults> GetAnimeDetailsAsync(int animeId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets information from an anime's "details" page.
        /// </summary>
        /// <param name="animeId"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalApiException"></exception>
        AnimeDetailsResults GetAnimeDetails(int animeId);
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
