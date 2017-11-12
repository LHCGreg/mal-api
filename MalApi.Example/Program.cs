using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace MalApi.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // MalApi uses the Common.Logging logging abstraction.
            // You can hook it up to any logging library that has a Common.Logging adapter.
            // See App.config for an example of hooking up MalApi to NLog.
            // Note that you will also need the appropriate NLog and Common.Logging.NLogXX packages installed.
            // Hooking up logging is not necessary but can be useful.
            // With the configuration in this example and with this example program, you will see lines like:

            // Logged from MalApi: Getting anime list for MAL user LordHighCaptain using URI https://myanimelist.net/malappinfo.php?status=all&type=anime&u=LordHighCaptain
            // Logged from MalApi: Successfully retrieved anime list for user LordHighCaptain

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                api.UserAgent = "MalApiExample";
                api.TimeoutInMs = 15000;

                var animeUpdateInfo = new UpdateAnime()
                {
                    Episode = 26,
                    Status = AnimeCompletionStatus.Completed,
                    Score = 9,
                    DateStart = new DateTime(1999, 01, 02)
                };
                string userUpdateAnime = api.UpdateAnimeForUser(1, animeUpdateInfo, "user", "password");

                var mangaUpdateInfo = new UpdateManga()
                {
                    Chapter = 20,
                    Volume = 3,
                    Score = 8,
                    Status = MangaCompletionStatus.Completed
                };
                string userUpdateManga = api.UpdateMangaForUser(952, mangaUpdateInfo, "user", "password");



                MalUserLookupResults userLookup = api.GetAnimeListForUser("user");
                foreach (MyAnimeListEntry listEntry in userLookup.AnimeList)
                {
                    Console.WriteLine("Rating for {0}: {1}", listEntry.AnimeInfo.Title, listEntry.Score);
                }

                Console.WriteLine();
                Console.WriteLine();

                RecentUsersResults recentUsersResults = api.GetRecentOnlineUsers();
                foreach (string user in recentUsersResults.RecentUsers)
                {
                    Console.WriteLine("Recent user: {0}", user);
                }

                Console.WriteLine();
                Console.WriteLine();

                int eurekaSevenID = 237;
                AnimeDetailsResults eurekaSeven = api.GetAnimeDetails(eurekaSevenID);
                Console.WriteLine("Eureka Seven genres: {0}", string.Join(", ", eurekaSeven.Genres));
            }
        }
    }
}
