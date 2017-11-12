using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MalApi;
using System.Threading.Tasks;
using Xunit;
using System.Threading;

namespace MalApi.IntegrationTests
{
    // Environment variables in the Initialize method have to be set up properly
    // In order for the test to succeed they need to be executed at the same (so the Environment variable can be set and used).
    public class GetMangaListForUserTest
    {
        [Fact]
        public static void Initialize()
        {
            System.Environment.SetEnvironmentVariable("IntegrationTestMalUsername", "username");
        }

        [Fact]
        public void GetMangaListForUser()
        {
            string username = System.Environment.GetEnvironmentVariable("IntegrationTestMalUsername");
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                MalUserLookupResults userLookup = api.GetMangaListForUser(username);

                Assert.NotEmpty(userLookup.MangaList);
            }
        }

        [Fact]
        public void GetMangaListForUserCanceled()
        {
            string username = System.Environment.GetEnvironmentVariable("IntegrationTestMalUsername");
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Task<MalUserLookupResults> userLookupTask = api.GetMangaListForUserAsync(username, tokenSource.Token);
                tokenSource.Cancel();
                Assert.Throws<TaskCanceledException>(() => userLookupTask.GetAwaiter().GetResult());
            }
        }

        [Fact]
        public void GetMangaListForNonexistentUserThrowsCorrectException()
        {
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                Assert.Throws<MalUserNotFoundException>(() => api.GetMangaListForUser("oijsfjisfdjfsdojpfsdp"));
            }
        }

        [Fact]
        public void GetMangaListForNonexistentUserThrowsCorrectExceptionAsync()
        {
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                Assert.ThrowsAsync<MalUserNotFoundException>(() => api.GetMangaListForUserAsync("oijsfjisfdjfsdojpfsdp"));
            }
        }
    }
}