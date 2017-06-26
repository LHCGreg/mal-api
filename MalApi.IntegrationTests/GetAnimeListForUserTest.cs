using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MalApi;
using System.Threading.Tasks;
using Xunit;

namespace MalApi.IntegrationTests
{
    public class GetAnimeListForUserTest
    {
        [Fact]
        public void GetAnimeListForUser()
        {
            string username = "lordhighcaptain";
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                MalUserLookupResults userLookup = api.GetAnimeListForUser(username);

                // Just a smoke test that checks that getting an anime list returns something
                Assert.NotEmpty(userLookup.AnimeList);
            }
        }

        [Fact]
        public void GetAnimeListForNonexistentUserThrowsCorrectException()
        {
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                Assert.Throws<MalUserNotFoundException>(() => api.GetAnimeListForUser("oijsfjisfdjfsdojpfsdp"));
            }
        }

        [Fact]
        public void GetAnimeListForNonexistentUserThrowsCorrectExceptionAsync()
        {
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                Assert.ThrowsAsync<MalUserNotFoundException>(() => api.GetAnimeListForUserAsync("oijsfjisfdjfsdojpfsdp"));
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