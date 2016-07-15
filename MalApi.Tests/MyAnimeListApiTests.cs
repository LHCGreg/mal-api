using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

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

        // For showing that the old version is broken
        [Test]
        public void TestScrapeAnimeDetailsFromHtmlFail()
        {
            string thisAssemblyPath = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
            Console.WriteLine(thisAssemblyPath);
            string htmlFilePath = Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Eureka_Seven.htm");
            string html = File.ReadAllText(htmlFilePath);

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                Assert.Throws<MalApiException>( delegate { ScrapeAnimeDetailsFromHtml(html, 237); });
            }
        }

        // For showing that the old version is broken
        private static readonly string AnimeDetailsUrlFormat = "http://myanimelist.net/anime/{0}";
        private static Lazy<Regex> s_animeDetailsRegex = new Lazy<Regex>(() => new Regex(
            @"Genres:</span> \n.*?(?:<a href=""http://myanimelist.net/anime.php\?genre\[\]=(?<GenreId>\d+)"">(?<GenreName>.*?)</a>(?:, )?)*</div>",
            RegexOptions.Compiled));
        private static Regex AnimeDetailsRegex { get { return s_animeDetailsRegex.Value; } }
        // internal for unit testing
        internal AnimeDetailsResults ScrapeAnimeDetailsFromHtml(string animeDetailsHtml, int animeId)
        {
            if (animeDetailsHtml.Contains("<div class=\"badresult\">No series found, check the series id and try again.</div>"))
            {
                throw new MalAnimeNotFoundException(string.Format("No anime with id {0} exists.", animeId));
            }

            Match match = AnimeDetailsRegex.Match(animeDetailsHtml);
            if (!match.Success)
            {
                throw new MalApiException(string.Format("Could not extract information from {0}.", string.Format(AnimeDetailsUrlFormat, animeId)));
            }

            Group genreIds = match.Groups["GenreId"];
            Group genreNames = match.Groups["GenreName"];
            List<Genre> genres = new List<Genre>();
            for (int i = 0; i < genreIds.Captures.Count; i++)
            {
                string genreIdString = genreIds.Captures[i].Value;
                int genreId = int.Parse(genreIdString);
                string genreName = genreNames.Captures[i].Value;
                genres.Add(new Genre(genreId: genreId, name: genreName));
            }

            return new AnimeDetailsResults(genres);
        }
    }
}

/*
 Copyright 2012 Greg Najda

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