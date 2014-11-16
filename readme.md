MalApi is a .NET library written in C# for accessing the myanimelist.net API or using scraping methods where no official API is available. Using it is easy:

```C#
using (MyAnimeListApi api = new MyAnimeListApi())
{
	api.UserAgent = "my_app"; // MAL now requires applications to be whitelisted. Whitelisted applications identify themselves by their user agent.
	MalUserLookupResults userLookup = api.GetAnimeListForUser("LordHighCaptain");
	foreach (MyAnimeListEntry listEntry in userLookup.AnimeList)
	{
		Console.WriteLine("Rating for {0}: {1}", listEntry.AnimeInfo.Title, listEntry.Score);
	}
}
```

Binaries are available as a [NuGet package](https://www.nuget.org/packages/MalApi/) called MalApi.

MalApi currently contains these MAL functions:
- Getting a user's anime list via http://myanimelist.net/malappinfo.php?status=all&type=anime&u=username. MalApi handles even malformed XML that MAL can generate when users put certain characters in tags.
- Getting a list of recently online users from http://myanimelist.net/users.php
- Getting an anime's genres from the anime's page.

Also included are some useful implementations of IMyAnimeListApi that wrap another IMyAnimeListApi.
- CachingMyAnimeListApi caches user lookups for a configurable amount of time.
- RateLimitingMyAnimeListApi limits MAL requests to once every N milliseconds so you can throttle your requests if you are making a large number of them.
- RetryOnFailureMyAnimeListApi waits a short period before retrying a request if a request fails. After a certain number of failures, it will give up.

MalApi can be configured to log using any logging library compatible with Common.Logging. See App.config in the MalApi.Example project. MalApi will use the logger name "MAL API". Consult the Common.Logging and NLog documentation for more information about logging.

MalApi is compatible with Mono.

MalApi is licensed under the Apache License 2.0.