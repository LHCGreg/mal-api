﻿using System;
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
