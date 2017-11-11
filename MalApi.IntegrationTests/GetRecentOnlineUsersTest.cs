using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MalApi.IntegrationTests
{
    public class GetRecentOnlineUsersTest
    {
        [Fact]
        public void GetRecentOnlineUsers()
        {
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                RecentUsersResults results = api.GetRecentOnlineUsers();
                Assert.NotEmpty(results.RecentUsers);
            }
        }

        [Fact]
        public void GetRecentOnlineUsersCanceled()
        {
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Task<RecentUsersResults> task = api.GetRecentOnlineUsersAsync(tokenSource.Token);
                tokenSource.Cancel();
                Assert.Throws<TaskCanceledException>(() => task.GetAwaiter().GetResult());
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