using System.IO;
using System.Reflection;
using System.Text;

namespace MalApi.UnitTests
{
    static class Helpers
    {
        private static string GetResourceName(string fileName)
        {
            return "MalApi.UnitTests." + fileName;
        }

        public static StreamReader GetResourceStream(string fileName)
        {
            string resourceName = GetResourceName(fileName);
            return new StreamReader(typeof(Helpers).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName), Encoding.UTF8);
        }

        public static string GetResourceText(string fileName)
        {
            using (StreamReader resourceStream = GetResourceStream(fileName))
            {
                return resourceStream.ReadToEnd();
            }
        }
    }
}

/*
 Copyright 2017 Greg Najda

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
