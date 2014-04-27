using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MalApi.Example
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
            }
        }
    }
}
