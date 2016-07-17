using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.IO;

namespace MalApi.Tests
{
    [TestFixture]
    public class MyAnimeListApiTests
    {
        [Test]
        public void TestScrapeAnimeDetailsFromHtml()
        {
            string thisAssemblyPath = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
            Console.WriteLine(thisAssemblyPath);
            string htmlFilePath = Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Eureka_Seven.htm");
            string html = File.ReadAllText(htmlFilePath);

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
                Assert.That(results.Genres, Is.EquivalentTo(expectedGenres));
            }
        }
    }
}

/*
 Copyright 2016 Greg Najda

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