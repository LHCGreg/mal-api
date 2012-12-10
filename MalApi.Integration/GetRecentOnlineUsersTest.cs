using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MalApi.Integration
{
    [TestFixture]
    public class GetRecentOnlineUsersTest
    {
        [Test]
        public void GetRecentOnlineUsers()
        {
            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                RecentUsersResults results = api.GetRecentOnlineUsers();
                Assert.That(results.RecentUsers.Count, Is.GreaterThan(0));
            }
        }
    }
}
