using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MalApi
{
    public class Genre : IEquatable<Genre>
    {
        public int GenreId { get; private set; }
        public string Name { get; private set; }

        public Genre(int genreId, string name)
        {
            GenreId = genreId;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Genre);
        }

        public bool Equals(Genre other)
        {
            return other != null && this.GenreId == other.GenreId && this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 23;
                hash = hash * 17 + GenreId.GetHashCode();
                hash = hash * 17 + Name.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return Name;
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