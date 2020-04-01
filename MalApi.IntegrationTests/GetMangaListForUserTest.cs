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
    public class GetMangaListForUserTest
    {
        [Fact]
        public void GetMangaListForUser()
        {
            string username = "naps250";
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                MalUserLookupResults userLookup = api.GetMangaListForUser(username);

                Assert.NotEmpty(userLookup.MangaList);
            }
        }

        [Fact]
        public void GetMangaListForUserCanceled()
        {
            string username = "naps250";
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