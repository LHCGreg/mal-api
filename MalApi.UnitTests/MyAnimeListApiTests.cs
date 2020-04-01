using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using FluentAssertions;
using Xunit;

namespace MalApi.UnitTests
{
    public class MyAnimeListApiTests
    {
        [Fact]
        public void TestScrapeAnimeDetailsFromHtml()
        {
            string html;
            using (StreamReader reader = Helpers.GetResourceStream("Eureka_Seven.htm"))
            {
                html = reader.ReadToEnd();
            }

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                AnimeDetailsResults results = api.ScrapeAnimeDetailsFromHtml(html, 237);
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
        public void TestScrapeUserAnimeDetailsFromHtml()
        {
            string html;
            using (StreamReader reader = Helpers.GetResourceStream("Cowboy_Bebop.htm"))
            {
                html = reader.ReadToEnd();
            }

            AnimeUpdate info = new AnimeUpdate()
            {
                Episode = 26,
                Status = AnimeCompletionStatus.Completed,
                Score = 8,
                StorageType = 5, // VHS
                StorageValue = 123,
                TimesRewatched = 2,
                RewatchValue = 4, // high
                DateStart = new DateTime(2017, 12, 10),
                DateFinish = new DateTime(2017, 12, 15),
                Priority = 1, // medium
                EnableDiscussion = 1,
                EnableRewatching = 1,
                Comments = "test updated comment, test updated comment2",
                Tags = "test updated tag, test updated tag 2"
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                AnimeUpdate results = MalDetailScrapingUtils.ScrapeUserAnimeDetailsFromHtml(html);

                Assert.Equal(info.Episode, results.Episode);
                Assert.Equal(info.Status, results.Status);
                Assert.Equal(info.Score, results.Score);
                Assert.Equal(info.StorageType, results.StorageType);
                Assert.Equal(info.StorageValue, results.StorageValue);
                Assert.Equal(info.TimesRewatched, results.TimesRewatched);
                Assert.Equal(info.RewatchValue, results.RewatchValue);
                Assert.Equal(info.DateStart, results.DateStart);
                Assert.Equal(info.DateFinish, results.DateFinish);
                Assert.Equal(info.Priority, results.Priority);
                Assert.Equal(info.EnableDiscussion, results.EnableDiscussion);
                Assert.Equal(info.EnableRewatching, results.EnableRewatching);
                Assert.Equal(info.Comments, results.Comments);
                Assert.Equal(info.Tags, results.Tags);
            }
        }

        [Fact]
        public void TestScrapeUserMangaDetailsFromHtml()
        {
            string html;
            using (StreamReader reader = Helpers.GetResourceStream("Monster.htm"))
            {
                html = reader.ReadToEnd();
            }

            MangaUpdate info = new MangaUpdate()
            {
                Chapter = 162,
                Volume = 18,
                Status = MangaCompletionStatus.Completed,
                Score = 10,
                TimesReread = 2,
                RereadValue = 4, // high
                DateStart = new DateTime(2017, 12, 10),
                DateFinish = new DateTime(2017, 12, 15),
                Priority = 1, // medium
                EnableDiscussion = 1,
                EnableRereading = 1,
                Comments = "test updated comment, test updated comment2",
                // ScanGroup = "scan_group_updated",
                Tags = "test updated tag, test updated tag 2"
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                MangaUpdate results = MalDetailScrapingUtils.ScrapeUserMangaDetailsFromHtml(html);

                Assert.Equal(info.Chapter, results.Chapter);
                Assert.Equal(info.Volume, results.Volume);
                Assert.Equal(info.Status, results.Status);
                Assert.Equal(info.Score, results.Score);
                Assert.Equal(info.TimesReread, results.TimesReread);
                Assert.Equal(info.RereadValue, results.RereadValue);
                Assert.Equal(info.DateStart, results.DateStart);
                Assert.Equal(info.DateFinish, results.DateFinish);
                Assert.Equal(info.Priority, results.Priority);
                Assert.Equal(info.EnableDiscussion, results.EnableDiscussion);
                Assert.Equal(info.EnableRereading, results.EnableRereading);
                Assert.Equal(info.Comments, results.Comments);
                Assert.Equal(info.Tags, results.Tags);
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
