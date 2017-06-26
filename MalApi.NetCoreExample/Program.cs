using MalApi;
using System;

namespace MalApi.NetCoreExample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                api.UserAgent = "MalApiExample";
                api.TimeoutInMs = 15000;

                MalUserLookupResults userLookup = api.GetAnimeListForUser("LordHighCaptain");
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