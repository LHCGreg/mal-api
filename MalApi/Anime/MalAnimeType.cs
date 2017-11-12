using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MalApi
{
    public enum MalAnimeType
    {
        /// <summary>
        /// The type has not been entered in MAL's database yet.
        /// </summary>
        Unknown = 0,
        Tv = 1,
        Ova = 2,
        Movie = 3,
        Special = 4,
        Ona = 5,
        Music = 6
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