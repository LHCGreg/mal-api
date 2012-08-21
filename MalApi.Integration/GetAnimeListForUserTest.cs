using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MalApi;

namespace MalApi.Integration
{
    [TestFixture]
    public class GetAnimeListForUserTest
    {
        [Test]
        public void GetAnimeListForUser()
        {
            string username = "lordhighcaptain";
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                MalUserLookupResults userLookup = api.GetAnimeListForUser(username);
                
                // Just a smoke test that checks that getting an anime list returns something
                Assert.That(userLookup.AnimeList, Is.Not.Empty);
            }
        }
    }
}
