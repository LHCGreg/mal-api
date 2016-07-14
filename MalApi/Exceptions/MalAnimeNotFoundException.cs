﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MalApi
{
    /// <summary>
    /// Indicates that the anime that was searched for does not exist.
    /// </summary>
    [Serializable]
    public class MalAnimeNotFoundException : MalApiException
    {
        public MalAnimeNotFoundException() { }
        public MalAnimeNotFoundException(string message) : base(message) { }
        public MalAnimeNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected MalAnimeNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
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