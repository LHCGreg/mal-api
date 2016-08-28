﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Xml.Linq;

namespace MalApi.Tests
{
    [TestFixture]
    public partial class MalAppInfoXmlTests
    {
        [Test]
        public void ParseWithTextReaderTest()
        {
            using (StringReader reader = new StringReader(TestXml))
            {
                MalUserLookupResults results = MalAppInfoXml.Parse(reader);
                DoAsserts(results);
            }
        }

        [Test]
        public void ParseWithXElementTest()
        {
            XDocument doc = XDocument.Parse(CleanTestXml);
            MalUserLookupResults results = MalAppInfoXml.Parse(doc);
            DoAsserts(results);
        }

        [Test]
        public void ParseInvalidUserWithTextReaderTest()
        {
            using (StringReader reader = new StringReader(InvalidUserXml))
            {
                Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.Parse(reader));
            }
        }

        [Test]
        public void ParseInvalidUserWithXElementTest()
        {
            XDocument doc = XDocument.Parse(InvalidUserXml);
            Assert.Throws<MalUserNotFoundException>(() => MalAppInfoXml.Parse(doc));
        }

        private void DoAsserts(MalUserLookupResults results)
        {
            Assert.That(results.CanonicalUserName, Is.EqualTo("LordHighCaptain"));
            Assert.That(results.UserId, Is.EqualTo(158667));
            Assert.That(results.AnimeList.Count, Is.EqualTo(163));

            MyAnimeListEntry entry = results.AnimeList.Where(anime => anime.AnimeInfo.AnimeId == 853).First();
            Assert.That(entry.AnimeInfo.Title, Is.EqualTo("Ouran Koukou Host Club"));
            Assert.That(entry.AnimeInfo.Type, Is.EqualTo(MalAnimeType.Tv));
            Assert.That(entry.AnimeInfo.Synonyms, Is.EquivalentTo(new List<string>() { "Ohran Koko Host Club", "Ouran Koukou Hosutobu", "Ouran High School Host Club" }));
            Assert.That(entry.NumEpisodesWatched, Is.EqualTo(7));
            Assert.That(entry.Score, Is.EqualTo(7));
            Assert.That(entry.Status, Is.EqualTo(CompletionStatus.Watching));
            Assert.That(entry.Tags, Is.EqualTo(new List<string>() { "duck", "goose" }));

            entry = results.AnimeList.Where(anime => anime.AnimeInfo.AnimeId == 7311).First();
            Assert.That(entry.AnimeInfo.Title, Is.EqualTo("Suzumiya Haruhi no Shoushitsu"));
            Assert.That(entry.AnimeInfo.Type, Is.EqualTo(MalAnimeType.Movie));
            Assert.That(entry.AnimeInfo.Synonyms, Is.EquivalentTo(new List<string>() { "The Vanishment of Haruhi Suzumiya", "Suzumiya Haruhi no Syoshitsu", "Haruhi movie", "The Disappearance of Haruhi Suzumiya" }));
            Assert.That(entry.Score, Is.EqualTo((decimal?)null));
            Assert.That(entry.NumEpisodesWatched, Is.EqualTo(0));
            Assert.That(entry.Status, Is.EqualTo(CompletionStatus.PlanToWatch));
            Assert.That(entry.Tags, Is.EqualTo(new List<string>()));

            entry = results.AnimeList.Where(anime => anime.AnimeInfo.AnimeId == 889).First();
            Assert.That(entry.AnimeInfo.Title, Is.EqualTo("Black Lagoon"));

            // Make sure synonyms that are the same as the real name get filtered out
            Assert.That(entry.AnimeInfo.Synonyms, Is.EquivalentTo(new List<string>()));

            entry = results.AnimeList.Where(anime => anime.AnimeInfo.Title == "Test").First();
            // Make sure that <series_synonyms/> is the same as <series_synonyms></series_synonyms>
            Assert.That(entry.AnimeInfo.Synonyms, Is.EquivalentTo(new List<string>()));
            Assert.That(entry.AnimeInfo.StartDate, Is.EqualTo(new UncertainDate(2010, 2, 6)));
            Assert.That(entry.AnimeInfo.EndDate, Is.EqualTo(UncertainDate.Unknown));
            Assert.That(entry.AnimeInfo.ImageUrl, Is.EqualTo("https://cdn.myanimelist.net/images/anime/9/24646.jpg"));
            Assert.That(entry.MyStartDate, Is.EqualTo(new UncertainDate(year: null, month: 2, day: null)));
            Assert.That(entry.MyFinishDate, Is.EqualTo(UncertainDate.Unknown));
            Assert.That(entry.MyLastUpdate, Is.EqualTo(new DateTime(year: 2011, month: 4, day: 2, hour: 22, minute: 50, second: 58, kind: DateTimeKind.Utc)));
            Assert.That(entry.Tags, Is.EqualTo(new List<string>() { "test&test", "< less than", "> greater than", "apos '", "quote \"", "hex ö", "dec !", "control character" }));

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