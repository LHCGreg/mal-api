using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace MalApi.IntegrationTests
{
    public class GetAnimeDetailsTest
    {
        [Fact]
        public void GetAnimeDetails()
        {
            int animeId = 237; // Eureka Seven
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                AnimeDetailsResults results = api.GetAnimeDetails(animeId);
                List<Genre> expectedGenres = new List<Genre>()
                {
                    new Genre(2, "Adventure"),
                    new Genre(8, "Drama"),
                    new Genre(18, "Mecha"),
                    new Genre(22, "Romance"),
                    new Genre(24, "Sci-Fi"),
                };
                results.Genres.Should().BeEquivalentTo(expectedGenres);
            }
        }

        [Fact]
        public void GetAnimeDetailsCanceled()
        {
            int animeId = 237; // Eureka Seven
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Task<AnimeDetailsResults> task = api.GetAnimeDetailsAsync(animeId, tokenSource.Token);
                tokenSource.Cancel();
                Assert.Throws<TaskCanceledException>(() => task.GetAwaiter().GetResult());
            }
        }

        [Fact]
        public void GetAnimeDetailsForInvalidAnimeId()
        {
            int animeId = 99999;
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                Assert.Throws<MalAnimeNotFoundException>(() => api.GetAnimeDetails(animeId));
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