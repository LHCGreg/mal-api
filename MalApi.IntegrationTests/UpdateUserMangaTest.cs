using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MalApi.IntegrationTests
{
    public class UpdateUserMangaTest
    {
        [Fact]
        public void UpdateMangaForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate baseInfo = new MangaUpdate()
            {
                Chapter = 1,
                Volume = 2,
                Status = MangaCompletionStatus.Reading,
                Score = 3,
                TimesReread = 1,
                RereadValue = 2, // low
                DateStart = new DateTime(2017, 10, 1),
                DateFinish = new DateTime(2017, 10, 5),
                Priority = 0, // low
                EnableDiscussion = 0,
                EnableRereading = 0,
                Comments = "base comment,base comment 2",
                // ScanGroup = "scan_group",
                Tags = "test base tag, test base tag 2"
            };

            MangaUpdate updateInfo = new MangaUpdate()
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
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, baseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(baseInfo.Chapter, baseReults.Chapter);
                    Assert.Equal(baseInfo.Volume, baseReults.Volume);
                    Assert.Equal(baseInfo.Status, baseReults.Status);
                    Assert.Equal(baseInfo.Score, baseReults.Score);
                    Assert.Equal(baseInfo.TimesReread, baseReults.TimesReread);
                    Assert.Equal(baseInfo.RereadValue, baseReults.RereadValue);
                    Assert.Equal(baseInfo.DateStart, baseReults.DateStart);
                    Assert.Equal(baseInfo.DateFinish, baseReults.DateFinish);
                    Assert.Equal(baseInfo.Priority, baseReults.Priority);
                    Assert.Equal(baseInfo.EnableDiscussion, baseReults.EnableDiscussion);
                    Assert.Equal(baseInfo.EnableRereading, baseReults.EnableRereading);
                    Assert.Equal(baseInfo.Comments, baseReults.Comments);
                    Assert.Equal(baseInfo.Tags, baseReults.Tags);

                    result = api.UpdateMangaForUser(mangaId, updateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(updateInfo.Chapter, updatedResults.Chapter);
                    Assert.Equal(updateInfo.Volume, updatedResults.Volume);
                    Assert.Equal(updateInfo.Status, updatedResults.Status);
                    Assert.Equal(updateInfo.Score, updatedResults.Score);
                    Assert.Equal(updateInfo.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(updateInfo.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(updateInfo.DateStart, updatedResults.DateStart);
                    Assert.Equal(updateInfo.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(updateInfo.Priority, updatedResults.Priority);
                    Assert.Equal(updateInfo.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(updateInfo.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(updateInfo.Comments, updatedResults.Comments);
                    Assert.Equal(updateInfo.Tags, updatedResults.Tags);

                    // Assert all values have been changed
                    Assert.NotEqual(baseReults.Chapter, updatedResults.Chapter);
                    Assert.NotEqual(baseReults.Volume, updatedResults.Volume);
                    Assert.NotEqual(baseReults.Status, updatedResults.Status);
                    Assert.NotEqual(baseReults.Score, updatedResults.Score);
                    Assert.NotEqual(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.NotEqual(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.NotEqual(baseReults.DateStart, updatedResults.DateStart);
                    Assert.NotEqual(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.NotEqual(baseReults.Priority, updatedResults.Priority);
                    Assert.NotEqual(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.NotEqual(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.NotEqual(baseReults.Comments, updatedResults.Comments);
                    Assert.NotEqual(baseReults.Tags, updatedResults.Tags);

                }
            }
        }

        [Fact]
        public void UpdateMangaChapterForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                Chapter = 1
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                Chapter = 162
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.Chapter, baseReults.Chapter);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.Chapter, updatedResults.Chapter);

                    // Assert that only the chapter has been changed
                    Assert.NotEqual(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaVolumeForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                Volume = 2
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                Volume = 18
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.Volume, baseReults.Volume);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.Volume, updatedResults.Volume);

                    // Assert that only the volume has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.NotEqual(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaStatusForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                Status = MangaCompletionStatus.Reading
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                Status = MangaCompletionStatus.Completed
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.Status, baseReults.Status);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.Status, updatedResults.Status);

                    // Assert that only the status has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.NotEqual(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaScoreForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                Score = 3
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                Score = 10
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.Score, baseReults.Score);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.Score, updatedResults.Score);

                    // Assert that only the score has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.NotEqual(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaTimesRereadForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                TimesReread = 1
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                TimesReread = 2
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.TimesReread, baseReults.TimesReread);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.TimesReread, updatedResults.TimesReread);

                    // Assert that only the times reread has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.NotEqual(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaRereadValueForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                RereadValue = 2 // high
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                RereadValue = 4, // high
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.RereadValue, baseReults.RereadValue);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.RereadValue, updatedResults.RereadValue);

                    // Assert that only the rearead value has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.NotEqual(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaDateStartForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                DateStart = new DateTime(2017, 10, 1)
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                DateStart = new DateTime(2017, 12, 10)
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.DateStart, baseReults.DateStart);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.DateStart, updatedResults.DateStart);

                    // Assert that only the date start has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.NotEqual(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaDateFinishForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                DateFinish = new DateTime(2017, 10, 5)
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                DateFinish = new DateTime(2017, 12, 15)
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.DateFinish, baseReults.DateFinish);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.DateFinish, updatedResults.DateFinish);

                    // Assert that only the date finish has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.NotEqual(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaPriorityForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                Priority = 0, // low
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                Priority = 1, // medium
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.Priority, baseReults.Priority);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.Priority, updatedResults.Priority);

                    // Assert that only the priority has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.NotEqual(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaEnableDiscussionForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                EnableDiscussion = 0
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                EnableDiscussion = 1
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.EnableDiscussion, baseReults.EnableDiscussion);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.EnableDiscussion, updatedResults.EnableDiscussion);

                    // Assert that only the enable discussion has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.NotEqual(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaEnableRereadingForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                EnableRereading = 0
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                EnableRereading = 1
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.EnableRereading, baseReults.EnableRereading);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.EnableRereading, updatedResults.EnableRereading);

                    // Assert that only the enable rereading has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.NotEqual(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaCommentsorUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                Comments = "base comment,base comment 2"
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                Comments = "test updated comment, test updated comment2"
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.Comments, baseReults.Comments);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.Comments, updatedResults.Comments);

                    // Assert that only the comments has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.NotEqual(baseReults.Comments, updatedResults.Comments);
                    Assert.Equal(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaTagsForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                Tags = "test base tag, test base tag 2"
            };

            MangaUpdate partialUpdateInfo = new MangaUpdate()
            {
                Tags = "test updated tag, test updated tag 2"
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    string result = api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate baseReults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert first update against base info
                    Assert.Equal(partialBaseInfo.Tags, baseReults.Tags);

                    result = api.UpdateMangaForUser(mangaId, partialUpdateInfo, username, password);
                    Assert.Equal("Updated", result);

                    MangaUpdate updatedResults = helper.GetUserMangaDetailsAsync(username, mangaId);

                    // Assert second update with update info
                    Assert.Equal(partialUpdateInfo.Tags, updatedResults.Tags);

                    // Assert that only the tags has been changed
                    Assert.Equal(baseReults.Chapter, updatedResults.Chapter);
                    Assert.Equal(baseReults.Volume, updatedResults.Volume);
                    Assert.Equal(baseReults.Status, updatedResults.Status);
                    Assert.Equal(baseReults.Score, updatedResults.Score);
                    Assert.Equal(baseReults.TimesReread, updatedResults.TimesReread);
                    Assert.Equal(baseReults.RereadValue, updatedResults.RereadValue);
                    Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                    Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                    Assert.Equal(baseReults.Priority, updatedResults.Priority);
                    Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                    Assert.Equal(baseReults.EnableRereading, updatedResults.EnableRereading);
                    Assert.Equal(baseReults.Comments, updatedResults.Comments);
                    Assert.NotEqual(baseReults.Tags, updatedResults.Tags);
                }
            }
        }

        [Fact]
        public void UpdateMangaForUserCanceled()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate updateInfo = new MangaUpdate()
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
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Task<string> userLookupTask =
                    api.UpdateMangaForUserAsync(mangaId, updateInfo, username, password, tokenSource.Token);
                tokenSource.Cancel();
                Assert.Throws<TaskCanceledException>(() => userLookupTask.GetAwaiter().GetResult());
            }
        }

        [Fact]
        public void IncorrectUsernameMangaChapterUpdateTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            username += "test";
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Monster
            int mangaId = 1;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                Chapter = 1
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    Assert.Throws<MalApiRequestException>(() => helper.Login(username, password));

                    Assert.Throws<MalApiRequestException>(() => api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password));
                }
            }
        }

        [Fact]
        public void WrongMangaIdUpdateTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            int mangaId = 5;

            MangaUpdate partialBaseInfo = new MangaUpdate()
            {
                Chapter = 1
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                using (GetUserMangaDetailsTest helper = new GetUserMangaDetailsTest())
                {
                    helper.Login(username, password);

                    Assert.Throws<MalApiRequestException>(() => api.UpdateMangaForUser(mangaId, partialBaseInfo, username, password));
                }
            }
        }
    }
}
