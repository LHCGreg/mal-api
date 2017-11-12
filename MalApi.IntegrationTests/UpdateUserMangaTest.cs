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
    public class UpdateUserMangaTest
    {
        [Fact]
        public void AInitialize()
        {
            System.Environment.SetEnvironmentVariable("IntegrationTestMalUsername", "user");
            System.Environment.SetEnvironmentVariable("IntegrationTestMalPassword", "password");

            System.Environment.SetEnvironmentVariable("IntegrationTestMangaId", "952");

            System.Environment.SetEnvironmentVariable("IntegrationTestChapterNumber", "10");
            System.Environment.SetEnvironmentVariable("IntegrationTestScore", "8");
            System.Environment.SetEnvironmentVariable("IntegrationTestStatus", "1");
        }

        [Fact]
        public void UpdateMangaForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("IntegrationTestMalUsername");
            string password = System.Environment.GetEnvironmentVariable("IntegrationTestMalPassword");

            int mangaId = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestMangaId"));
            UpdateManga updateInfo = new UpdateManga()
            {
                Chapter = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestChapterNumber")),
                Score = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestScore")),
                Status = (MangaCompletionStatus)int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestStatus"))
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateMangaForUser(mangaId, updateInfo, username, password);

                Assert.Equal("Updated", result);
            }
        }

        [Fact]
        public void GetMangaListForUserCanceled()
        {
            string username = System.Environment.GetEnvironmentVariable("IntegrationTestMalUsername");
            string password = System.Environment.GetEnvironmentVariable("IntegrationTestMalPassword");

            int mangaId = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestMangaId"));
            UpdateManga updateInfo = new UpdateManga()
            {
                Chapter = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestChapterNumber")),
                Score = int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestScore")),
                Status = (MangaCompletionStatus)int.Parse(System.Environment.GetEnvironmentVariable("IntegrationTestStatus"))
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Task<string> userLookupTask = api.UpdateMangaForUserAsync(mangaId, updateInfo, username, password, tokenSource.Token);
                tokenSource.Cancel();
                Assert.Throws<TaskCanceledException>(() => userLookupTask.GetAwaiter().GetResult());
            }
        }
    }
}
