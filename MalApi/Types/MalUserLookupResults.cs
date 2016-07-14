using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MalApi
{
    public class MalUserLookupResults
    {
        public ICollection<MyAnimeListEntry> AnimeList { get; private set; }
        public int UserId { get; private set; }

        /// <summary>
        /// The user name as it appears in MAL. This might differ in capitalization from the username that was searched for.
        /// </summary>
        public string CanonicalUserName { get; private set; }

        public MalUserLookupResults(int userId, string canonicalUserName, ICollection<MyAnimeListEntry> animeList)
        {
            UserId = userId;
            CanonicalUserName = canonicalUserName;
            AnimeList = animeList;
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