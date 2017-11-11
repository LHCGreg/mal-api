﻿using System;


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

                var animeUpdateInfo = new UpdateAnimeObject(episode: "26", status: "2", score: "9");
                string userLookup2 = api.UpdateAnimeForUser(1, animeUpdateInfo, "user", "pass");

                var mangaUpdateInfo = new UpdateMangaObject(chapter: "20", volume: "3", score: "8");
                string userLookup3 = api.UpdateMangaForUser(952, mangaUpdateInfo, "user", "pass");



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
