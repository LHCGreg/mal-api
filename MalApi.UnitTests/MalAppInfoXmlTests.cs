using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Xunit;

namespace MalApi.UnitTests
{
    public partial class MalAppInfoXmlTests
    {
        [Fact]
        public void ParseWithTextReaderTest()
        {
            using (TextReader reader = Helpers.GetResourceStream("test_anime.xml"))
            {
                MalUserLookupResults results = MalAppInfoXml.Parse(reader);
                DoAnimeAsserts(results);
            }
        }

        [Fact]
        public void ParseWithTextReaderMangaTest()
        {
            using (TextReader reader = Helpers.GetResourceStream("test_manga.xml"))
            {
                MalUserLookupResults results = MalAppInfoXml.Parse(reader);
                DoMangaAsserts(results);
            }
        }

        [Fact]
        public void ParseWithXElementAnimeTest()
        {
            XDocument doc = XDocument.Parse(Helpers.GetResourceText("test_anime_clean.xml"));
            MalUserLookupResults results = MalAppInfoXml.ParseResults(doc);
            DoAnimeAsserts(results);
        }

        [Fact]
        public void ParseWithXElementMangaTest()
        {
            XDocument doc = XDocument.Parse(Helpers.GetResourceText("test_manga_clean.xml"));
            MalUserLookupResults results = MalAppInfoXml.ParseResults(doc);
            DoMangaAsserts(results);
        }

        [Fact]
        public void ParseInvalidUserWithTextReaderTest()
        {
            using (TextReader reader = Helpers.GetResourceStream("test_no_such_user.xml"))
            {
                Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.Parse(reader));
            }
        }

        [Fact]
        public void ParseInvalidUserWithXElementTest()
        {
            XDocument doc = XDocument.Parse(Helpers.GetResourceText("test_no_such_user.xml"));
            Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.ParseResults(doc));
        }

        [Fact]
        public void ParseOldInvalidUserWithTextReaderTest()
        {
            using (TextReader reader = Helpers.GetResourceStream("test_no_such_user_old.xml"))
            {
                Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.Parse(reader));
            }
        }

        [Fact]
        public void ParseOldInvalidUserWithXElementTest()
        {
            XDocument doc = XDocument.Parse(Helpers.GetResourceText("test_no_such_user_old.xml"));
            Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.ParseResults(doc));
        }

        private void DoAnimeAsserts(MalUserLookupResults results)
        {
            Assert.Equal("LordHighCaptain", results.CanonicalUserName);
            Assert.Equal(158667, results.UserId);
            Assert.Equal(163, results.AnimeList.Count);

            MyAnimeListEntry entry = results.AnimeList.Where(anime => anime.AnimeInfo.AnimeId == 853).First();
            Assert.Equal("Ouran Koukou Host Club", entry.AnimeInfo.Title);
            Assert.Equal(MalAnimeType.Tv, entry.AnimeInfo.Type);
            entry.AnimeInfo.Synonyms.Should().BeEquivalentTo(new List<string>() { "Ohran Koko Host Club", "Ouran Koukou Hosutobu", "Ouran High School Host Club" });

            Assert.Equal(7, entry.NumEpisodesWatched);
            Assert.Equal(7, entry.Score);
            Assert.Equal(AnimeCompletionStatus.Watching, entry.Status);

            // Test tags with Equal, not equivalent, because order in tags matters
            Assert.Equal(new List<string>() { "duck", "goose" }, entry.Tags);

            entry = results.AnimeList.Where(anime => anime.AnimeInfo.AnimeId == 7311).First();
            Assert.Equal("Suzumiya Haruhi no Shoushitsu", entry.AnimeInfo.Title);
            Assert.Equal(MalAnimeType.Movie, entry.AnimeInfo.Type);
            entry.AnimeInfo.Synonyms.Should().BeEquivalentTo(new List<string>() { "The Vanishment of Haruhi Suzumiya", "Suzumiya Haruhi no Syoshitsu", "Haruhi movie", "The Disappearance of Haruhi Suzumiya" });
            Assert.Equal((decimal?)null, entry.Score);
            Assert.Equal(0, entry.NumEpisodesWatched);
            Assert.Equal(AnimeCompletionStatus.PlanToWatch, entry.Status);
            Assert.Equal(new List<string>(), entry.Tags);

            entry = results.AnimeList.Where(anime => anime.AnimeInfo.AnimeId == 889).First();
            Assert.Equal("Black Lagoon", entry.AnimeInfo.Title);

            // Make sure synonyms that are the same as the real name get filtered out
            entry.AnimeInfo.Synonyms.Should().BeEquivalentTo(new List<string>());

            entry = results.AnimeList.Where(anime => anime.AnimeInfo.Title == "Test").First();
            // Make sure that <series_synonyms/> is the same as <series_synonyms></series_synonyms>
            entry.AnimeInfo.Synonyms.Should().BeEquivalentTo(new List<string>());
            Assert.Equal(new UncertainDate(2010, 2, 6), entry.AnimeInfo.StartDate);
            Assert.Equal(UncertainDate.Unknown, entry.AnimeInfo.EndDate);
            Assert.Equal("https://cdn.myanimelist.net/images/anime/9/24646.jpg", entry.AnimeInfo.ImageUrl);
            Assert.Equal(new UncertainDate(year: null, month: 2, day: null), entry.MyStartDate);
            Assert.Equal(UncertainDate.Unknown, entry.MyFinishDate);
            Assert.Equal(new DateTime(year: 2011, month: 4, day: 2, hour: 22, minute: 50, second: 58, kind: DateTimeKind.Utc), entry.MyLastUpdate);
            Assert.Equal(new List<string>() { "test&test", "< less than", "> greater than", "apos '", "quote \"", "hex ö", "dec !", "control character" }, entry.Tags);

        }

        private void DoMangaAsserts(MalUserLookupResults results)
        {
            Assert.Equal("naps250", results.CanonicalUserName);
            Assert.Equal(5544903, results.UserId);
            Assert.Equal(6, results.MangaList.Count);

            MyMangaListEntry entry = results.MangaList.Where(manga => manga.MangaInfo.MangaId == 2).First();
            Assert.Equal("Berserk", entry.MangaInfo.Title);
            Assert.Equal(MalMangaType.Manga, entry.MangaInfo.Type);
            entry.MangaInfo.Synonyms.Should().BeEquivalentTo(new List<string>() { "Berserk: The Prototype" });

            Assert.Equal(352, entry.NumChaptersRead);
            Assert.Equal(10, entry.Score);
            Assert.Equal(MangaCompletionStatus.Reading, entry.Status);

            // Test tags with Equal, not equivalent, because order in tags matters
            Assert.Equal(new List<string>() { "CLANG", "Miura pls" }, entry.Tags);

            entry = results.MangaList.Where(manga => manga.MangaInfo.MangaId == 9115).First();
            Assert.Equal("Ookami to Koushinryou", entry.MangaInfo.Title);
            Assert.Equal(MalMangaType.Novel, entry.MangaInfo.Type);
            entry.MangaInfo.Synonyms.Should().BeEquivalentTo(new List<string>() { "Okami to Koshinryo", "Spice and Wolf", "Spice & Wolf" });
            Assert.Equal((decimal?)null, entry.Score);
            Assert.Equal(0, entry.NumChaptersRead);
            Assert.Equal(MangaCompletionStatus.Completed, entry.Status);
            Assert.Equal(new List<string>(), entry.Tags);

            entry = results.MangaList.Where(manga => manga.MangaInfo.MangaId == 1).First();
            Assert.Equal("Monster", entry.MangaInfo.Title);

            // Make sure synonyms that are the same as the real name get filtered out
            entry.MangaInfo.Synonyms.Should().BeEquivalentTo(new List<string>());

            entry = results.MangaList.Where(manga => manga.MangaInfo.Title == "Test").First();
            // Make sure that <series_synonyms/> is the same as <series_synonyms></series_synonyms>
            entry.MangaInfo.Synonyms.Should().BeEquivalentTo(new List<string>());
            Assert.Equal(new UncertainDate(2010, 2, 6), entry.MangaInfo.StartDate);
            Assert.Equal(UncertainDate.Unknown, entry.MangaInfo.EndDate);
            Assert.Equal("https://myanimelist.cdn-dena.com/images/manga/2/159423.jpg", entry.MangaInfo.ImageUrl);
            Assert.Equal(new UncertainDate(year: null, month: 2, day: null), entry.MyStartDate);
            Assert.Equal(UncertainDate.Unknown, entry.MyFinishDate);
            Assert.Equal(new DateTime(year: 2011, month: 4, day: 2, hour: 22, minute: 50, second: 58, kind: DateTimeKind.Utc), entry.MyLastUpdate);
            Assert.Equal(new List<string>() { "test&test", "< less than", "> greater than", "apos '", "quote \"", "hex ö", "dec !", "control character" }, entry.Tags);
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
