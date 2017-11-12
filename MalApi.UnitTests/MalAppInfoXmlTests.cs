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
            using (TextReader reader = Helpers.GetResourceStream("test.xml"))
            {
                MalUserLookupResults results = MalAppInfoXml.Parse(reader);
                DoAsserts(results);
            }
        }

        [Fact]
        public void ParseWithXElementTest()
        {
            XDocument doc = XDocument.Parse(Helpers.GetResourceText("test_clean.xml"));
            MalUserLookupResults results = MalAppInfoXml.ParseAnimeResults(doc);
            DoAsserts(results);
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
            Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.ParseAnimeResults(doc));

            Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.ParseMangaResults(doc));
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
            Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.ParseAnimeResults(doc));

            Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.ParseMangaResults(doc));
        }

        private void DoAsserts(MalUserLookupResults results)
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
