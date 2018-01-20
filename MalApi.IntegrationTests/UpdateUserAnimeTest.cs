using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MalApi.IntegrationTests
{
    // Environment variables in the Initialize method have to be set up properly
    // In order for the test to succeed they need to be executed at the same (so the Environment variable can be set and used).
    public class UpdateUserAnimeTest
    {
        [Fact]
        public void UpdateAnimeForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate baseInfo = new AnimeUpdate()
            {
                Episode = 1,
                Status = AnimeCompletionStatus.Watching,
                Score = 3,
                StorageType = 3, // VHS
                StorageValue = 1,
                TimesRewatched = 1,
                RewatchValue = 2, // low
                DateStart = new DateTime(2017, 10, 1),
                DateFinish = new DateTime(2017, 10, 5),
                Priority = 0, // low
                EnableDiscussion = 0,
                EnableRewatching = 0,
                Comments = "base comment,base comment 2",
                Tags = "test base tag, test base tag 2"
            };

            AnimeUpdate updateInfo = new AnimeUpdate()
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
                string result = api.UpdateAnimeForUser(animeId, baseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(baseInfo.Episode, baseReults.Episode);
                Assert.Equal(baseInfo.Status, baseReults.Status);
                Assert.Equal(baseInfo.Score, baseReults.Score);
                Assert.Equal(baseInfo.StorageType, baseReults.StorageType);
                Assert.Equal(baseInfo.StorageValue, baseReults.StorageValue);
                Assert.Equal(baseInfo.TimesRewatched, baseReults.TimesRewatched);
                Assert.Equal(baseInfo.RewatchValue, baseReults.RewatchValue);
                Assert.Equal(baseInfo.DateStart, baseReults.DateStart);
                Assert.Equal(baseInfo.DateFinish, baseReults.DateFinish);
                Assert.Equal(baseInfo.Priority, baseReults.Priority);
                Assert.Equal(baseInfo.EnableDiscussion, baseReults.EnableDiscussion);
                Assert.Equal(baseInfo.EnableRewatching, baseReults.EnableRewatching);
                Assert.Equal(baseInfo.Comments, baseReults.Comments);
                Assert.Equal(baseInfo.Tags, baseReults.Tags);

                result = api.UpdateAnimeForUser(animeId, updateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(updateInfo.Episode, updatedResults.Episode);
                Assert.Equal(updateInfo.Status, updatedResults.Status);
                Assert.Equal(updateInfo.Score, updatedResults.Score);
                Assert.Equal(updateInfo.StorageType, updatedResults.StorageType);
                Assert.Equal(updateInfo.StorageValue, updatedResults.StorageValue);
                Assert.Equal(updateInfo.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(updateInfo.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(updateInfo.DateStart, updatedResults.DateStart);
                Assert.Equal(updateInfo.DateFinish, updatedResults.DateFinish);
                Assert.Equal(updateInfo.Priority, updatedResults.Priority);
                Assert.Equal(updateInfo.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(updateInfo.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(updateInfo.Comments, updatedResults.Comments);
                Assert.Equal(updateInfo.Tags, updatedResults.Tags);

                // Assert all values have been changed
                Assert.NotEqual(baseReults.Episode, updatedResults.Episode);
                Assert.NotEqual(baseReults.Status, updatedResults.Status);
                Assert.NotEqual(baseReults.Score, updatedResults.Score);
                Assert.NotEqual(baseReults.StorageType, updatedResults.StorageType);
                Assert.NotEqual(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.NotEqual(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.NotEqual(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.NotEqual(baseReults.DateStart, updatedResults.DateStart);
                Assert.NotEqual(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.NotEqual(baseReults.Priority, updatedResults.Priority);
                Assert.NotEqual(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.NotEqual(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.NotEqual(baseReults.Comments, updatedResults.Comments);
                Assert.NotEqual(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeEpisodeForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                Episode = 1
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                Episode = 26
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.Episode, baseReults.Episode);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.Episode, updatedResults.Episode);

                // Assert that only the episode has been changed
                Assert.NotEqual(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeStatusForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                Status = AnimeCompletionStatus.Watching
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                Status = AnimeCompletionStatus.Completed
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.Status, baseReults.Status);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.Status, updatedResults.Status);

                // Assert that only the status has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.NotEqual(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeScoreForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                Score = 3
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                Score = 8
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.Score, baseReults.Score);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.Score, updatedResults.Score);

                // Assert that only the score has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.NotEqual(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeStorageTypeForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                StorageType = 3 // VHS
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                StorageType = 5 // VHS
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.StorageType, baseReults.StorageType);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.StorageType, updatedResults.StorageType);

                // Assert that only the storage type has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.NotEqual(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeStorageValueForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                StorageValue = 1
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                StorageValue = 123
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.StorageValue, baseReults.StorageValue);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.StorageValue, updatedResults.StorageValue);

                // Assert that only the storage value has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.NotEqual(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeTimesRewatchedForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                TimesRewatched = 1
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                TimesRewatched = 2
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.TimesRewatched, baseReults.TimesRewatched);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.TimesRewatched, updatedResults.TimesRewatched);

                // Assert that only the times rewatched has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.NotEqual(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeRewatchValueForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                RewatchValue = 2 // low
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                RewatchValue = 4 // high
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.RewatchValue, baseReults.RewatchValue);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.RewatchValue, updatedResults.RewatchValue);

                // Assert that only the rewatch value has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.NotEqual(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeDateStartForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                DateStart = new DateTime(2017, 10, 1)
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                DateStart = new DateTime(2017, 12, 10)
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.DateStart, baseReults.DateStart);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.DateStart, updatedResults.DateStart);

                // Assert that only the date start has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.NotEqual(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeDateFinishForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            { 
                DateFinish = new DateTime(2017, 10, 5)
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                DateFinish = new DateTime(2017, 12, 15)
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.DateFinish, baseReults.DateFinish);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.DateFinish, updatedResults.DateFinish);

                // Assert that only the date finish has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.NotEqual(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimePriorityForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                Priority = 0 // low
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                Priority = 1 // medium
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.Priority, baseReults.Priority);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.Priority, updatedResults.Priority);

                // Assert that only the priority has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.NotEqual(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeEnableDiscussionForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                EnableDiscussion = 0
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                EnableDiscussion = 1
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.EnableDiscussion, baseReults.EnableDiscussion);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.EnableDiscussion, updatedResults.EnableDiscussion);

                // Assert that only the enable discussion has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.NotEqual(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeEnableRewatchingForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                EnableRewatching = 0
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                EnableRewatching = 1
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.EnableRewatching, baseReults.EnableRewatching);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.EnableRewatching, updatedResults.EnableRewatching);

                // Assert that only the enable rewatching has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.NotEqual(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeCommentsForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                Comments = "base comment,base comment 2"
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                Comments = "test updated comment, test updated comment2"
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.Comments, baseReults.Comments);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.Comments, updatedResults.Comments);

                // Assert that only the comments has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.NotEqual(baseReults.Comments, updatedResults.Comments);
                Assert.Equal(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void UpdateAnimeTagsForUserTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                Tags = "test base tag, test base tag 2"
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                Tags = "test updated tag, test updated tag 2"
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert first update against base info
                Assert.Equal(partialBaseInfo.Tags, baseReults.Tags);


                result = api.UpdateAnimeForUser(animeId, partialUpdateInfo, username, password);
                Assert.Equal("Updated", result);

                AnimeUpdate updatedResults = api.GetUserAnimeDetails(username, password, animeId);

                // Assert second update with update info
                Assert.Equal(partialUpdateInfo.Tags, updatedResults.Tags);

                // Assert that only the tags has been changed
                Assert.Equal(baseReults.Episode, updatedResults.Episode);
                Assert.Equal(baseReults.Status, updatedResults.Status);
                Assert.Equal(baseReults.Score, updatedResults.Score);
                Assert.Equal(baseReults.StorageType, updatedResults.StorageType);
                Assert.Equal(baseReults.StorageValue, updatedResults.StorageValue);
                Assert.Equal(baseReults.TimesRewatched, updatedResults.TimesRewatched);
                Assert.Equal(baseReults.RewatchValue, updatedResults.RewatchValue);
                Assert.Equal(baseReults.DateStart, updatedResults.DateStart);
                Assert.Equal(baseReults.DateFinish, updatedResults.DateFinish);
                Assert.Equal(baseReults.Priority, updatedResults.Priority);
                Assert.Equal(baseReults.EnableDiscussion, updatedResults.EnableDiscussion);
                Assert.Equal(baseReults.EnableRewatching, updatedResults.EnableRewatching);
                Assert.Equal(baseReults.Comments, updatedResults.Comments);
                Assert.NotEqual(baseReults.Tags, updatedResults.Tags);
            }
        }

        [Fact]
        public void GetAnimeListForUserCanceled()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate updateInfo = new AnimeUpdate()
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
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Task<string> userLookupTask = api.UpdateAnimeForUserAsync(animeId, updateInfo, username, password, tokenSource.Token);
                tokenSource.Cancel();
                Assert.Throws<TaskCanceledException>(() => userLookupTask.GetAwaiter().GetResult());
            }
        }

        [Fact]
        public void IncorrectUsernameAnimeEpisodeUpdateTest()
        {
            string username = System.Environment.GetEnvironmentVariable("MAL_USERNAME");
            username += "test";
            string password = System.Environment.GetEnvironmentVariable("MAL_PASSWORD");

            // Cowboy Bebop
            int animeId = 1;

            AnimeUpdate partialBaseInfo = new AnimeUpdate()
            {
                Episode = 1
            };

            AnimeUpdate partialUpdateInfo = new AnimeUpdate()
            {
                Episode = 26
            };

            using (MyAnimeListApi api = new MyAnimeListApi())
            {
                try
                {
                    string result = api.UpdateAnimeForUser(animeId, partialBaseInfo, username, password);

                }
                catch (Exception ex)
                {
                    Assert.Equal(typeof(MalApiRequestException), ex.GetType());
                }

                try
                {
                    AnimeUpdate baseReults = api.GetUserAnimeDetails(username, password, animeId);

                }
                catch (Exception ex)
                {
                    Assert.Equal(typeof(MalApiRequestException), ex.GetType());
                    Assert.Equal("Failed to log in. Recheck credentials.", ex.Message);
                }
            }
        }
    }
}
