using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MalApi.IntegrationTests
{
    // Environment variables in the Initialize method have to be set up properly
    // In order for the test to succeed they need to be executed at the same (so the Environment variable can be set and used).
    public class UpdateUserAnimeTest
    {
        [Fact]
        public void Initialize()
        {
            System.Environment.SetEnvironmentVariable("IntegrationTestMalUsername", "user");
            System.Environment.SetEnvironmentVariable("IntegrationTestMalPassword", "password");

            System.Environment.SetEnvironmentVariable("IntegrationTestAnimeId", "1");

            System.Environment.SetEnvironmentVariable("IntegrationTestEpisodeNumber", "26");
            System.Environment.SetEnvironmentVariable("IntegrationTestScore", "9");
            System.Environment.SetEnvironmentVariable("IntegrationTestStatus", "2");
        }

        [Fact]
        public void UpdateAnimeForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("IntegrationTestMalUsername");
            string password = System.Environment.GetEnvironmentVariable("IntegrationTestMalPassword");

            int animeId = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestAnimeId"));
            AnimeUpdate updateInfo = new AnimeUpdate()
            {
                Episode = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestEpisodeNumber")),
                Score = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestScore")),
                Status = (AnimeCompletionStatus) int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestStatus"))
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, updateInfo, username, password);

                Assert.Equal("Updated", result);
            }
        }

        [Fact]
        public void GetAnimeListForUserCanceled()
        {
            string username = System.Environment.GetEnvironmentVariable("IntegrationTestMalUsername");
            string password = System.Environment.GetEnvironmentVariable("IntegrationTestMalPassword");

            int animeId = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestAnimeId"));
            AnimeUpdate updateInfo = new AnimeUpdate()
            {
                Episode = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestEpisodeNumber")),
                Score = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestScore")),
                Status = (AnimeCompletionStatus)int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestStatus"))
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Task<string> userLookupTask = api.UpdateAnimeForUserAsync(animeId, updateInfo, username, password, tokenSource.Token);
                tokenSource.Cancel();
                Assert.Throws<TaskCanceledException>(() => userLookupTask.GetAwaiter().GetResult());
            }
        }
    }
}
