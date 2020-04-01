﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace MalApi
{
    public static class MalAppInfoXml
    {
        /// <summary>
        /// Parses XML obtained from malappinfo.php. The XML is sanitized to account for MAL's invalid XML if, for example,
        /// a user has a &amp; character in their tags.
        /// </summary>
        /// <param name="xmlTextReader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        public static async Task<MalUserLookupResults> ParseAsync(TextReader xmlTextReader, CancellationToken cancellationToken)
        {
            Logging.Log.Trace("Sanitizing XML.");
            using (StringReader sanitizedXmlTextReader = await SanitizeAnimeListXmlAsync(xmlTextReader, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
            {
                Logging.Log.Trace("XML sanitized.");

                // No async version of XDocument.Load in the full framework yet.
                // StringReader won't block though.
                XDocument doc = XDocument.Load(sanitizedXmlTextReader);

                return ParseResults(doc);
            }
        }

        /// <summary>
        /// Parses XML obtained from malappinfo.php. The XML is sanitized to account for MAL's invalid XML if, for example,
        /// a user has a &amp; character in their tags.
        /// </summary>
        /// <param name="xmlTextReader"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        public static Task<MalUserLookupResults> ParseAsync(TextReader xmlTextReader)
        {
            return ParseAsync(xmlTextReader, CancellationToken.None);
        }

        /// <summary>
        /// Parses XML obtained from malappinfo.php. The XML is sanitized to account for MAL's invalid XML if, for example,
        /// a user has a &amp; character in their tags.
        /// </summary>
        /// <param name="xmlTextReader"></param>
        /// <returns></returns>
        /// <exception cref="MalApi.MalUserNotFoundException"></exception>
        /// <exception cref="MalApi.MalApiException"></exception>
        public static MalUserLookupResults Parse(TextReader xmlTextReader)
        {
            return ParseAsync(xmlTextReader).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        // Rumor has it that compiled regexes are far more performant than non-compiled regexes on large pieces of text.
        // I haven't profiled it though.
        private static Lazy<Regex> s_tagElementContentsRegex =
            new Lazy<Regex>(() => new Regex("<my_tags>(?<TagText>.*?)</my_tags>", RegexOptions.Compiled | RegexOptions.CultureInvariant));
        private static Regex TagElementContentsRegex { get { return s_tagElementContentsRegex.Value; } }

        private static Lazy<Regex> s_nonEntityAmpersandRegex =
            new Lazy<Regex>(() => new Regex("&(?!lt;)(?!gt;)(?!amp;)(?!apos;)(?!quot;)(?!#x[0-9a-fA-f]+;)(?!#[0-9]+;)", RegexOptions.Compiled | RegexOptions.CultureInvariant));
        private static Regex NonEntityAmpersandRegex { get { return s_nonEntityAmpersandRegex.Value; } }

        // Remove any code points not in: U+0009, U+000A, U+000D, U+0020–U+D7FF, U+E000–U+FFFD (see http://en.wikipedia.org/wiki/Xml)
        private static Lazy<Regex> s_invalidXmlCharacterRegex =
            new Lazy<Regex>(() => new Regex("[^\\u0009\\u000A\\u000D\\u0020-\\uD7FF\\uE000-\\uFFFD]", RegexOptions.Compiled | RegexOptions.CultureInvariant));
        private static Regex InvalidXmlCharacterRegex { get { return s_invalidXmlCharacterRegex.Value; } }

        // Replace & with &amp; only if the & is not part of &lt; &gt; &amp; &apos; &quot; &#x<hex digits>; &#<decimal digits>;
        private static MatchEvaluator TagElementContentsReplacer = (Match match) =>
        {
            string tagText = match.Groups["TagText"].Value;
            string replacementTagText = NonEntityAmpersandRegex.Replace(tagText, "&amp;");
            replacementTagText = InvalidXmlCharacterRegex.Replace(replacementTagText, "");
            return "<my_tags>" + replacementTagText + "</my_tags>";
        };

        /// <summary>
        /// Sanitizes anime list XML which is not always well-formed. If a user uses &amp; characters in their tags,
        /// they will not be escaped in the XML.
        /// </summary>
        /// <param name="xmlTextReader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<StringReader> SanitizeAnimeListXmlAsync(TextReader xmlTextReader, CancellationToken cancellationToken)
        {
            string rawXml = await xmlTextReader.ReadToEndAsync().ConfigureAwait(continueOnCapturedContext: false);
            string sanitizedXml = TagElementContentsRegex.Replace(rawXml, TagElementContentsReplacer);
            return new StringReader(sanitizedXml);
        }

        /// <summary>
        /// Parses XML obtained from malappinfo.php.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static MalUserLookupResults ParseResults(XDocument doc)
        {
            Logging.Log.Trace("Parsing XML.");

            XElement error = doc.Root.Element("error");
            if (error != null && (string)error == "Invalid username")
            {
                throw new MalUserNotFoundException("No MAL list exists for this user.");
            }
            else if (error != null)
            {
                throw new MalApiException((string)error);
            }

            if (!doc.Root.HasElements)
            {
                throw new MalUserNotFoundException("No MAL list exists for this user.");
            }

            XElement myinfo = GetExpectedElement(doc.Root, "myinfo");
            int userId = GetElementValueInt(myinfo, "user_id");
            string canonicalUserName = GetElementValueString(myinfo, "user_name");

            List<MyAnimeListEntry> animeEntries = new List<MyAnimeListEntry>();
            List<MyMangaListEntry> mangaEntries = new List<MyMangaListEntry>();

            // Anime entries
            if (doc.Root.Element("anime") != null)
            {
                IEnumerable<XElement> animes = doc.Root.Elements("anime");
                foreach (XElement anime in animes)
                {
                    int animeId = GetElementValueInt(anime, "series_animedb_id");
                    string title = GetElementValueString(anime, "series_title");

                    string synonymList = GetElementValueString(anime, "series_synonyms");
                    string[] rawSynonyms = synonymList.Split(SynonymSeparator, StringSplitOptions.RemoveEmptyEntries);

                    // filter out synonyms that are the same as the main title
                    HashSet<string> synonyms = new HashSet<string>(rawSynonyms.Where(synonym => !synonym.Equals(title, StringComparison.Ordinal)));

                    int seriesTypeInt = GetElementValueInt(anime, "series_type");
                    MalAnimeType seriesType = (MalAnimeType)seriesTypeInt;

                    int numEpisodes = GetElementValueInt(anime, "series_episodes");

                    int seriesStatusInt = GetElementValueInt(anime, "series_status");
                    MalAnimeSeriesStatus seriesStatus = (MalAnimeSeriesStatus)seriesStatusInt;

                    string seriesStartString = GetElementValueString(anime, "series_start");
                    UncertainDate seriesStart = UncertainDate.FromMalDateString(seriesStartString);

                    string seriesEndString = GetElementValueString(anime, "series_end");
                    UncertainDate seriesEnd = UncertainDate.FromMalDateString(seriesEndString);

                    string seriesImage = GetElementValueString(anime, "series_image");

                    MalAnimeInfoFromUserLookup animeInfo = new MalAnimeInfoFromUserLookup(animeId: animeId, title: title,
                        type: seriesType, synonyms: synonyms, status: seriesStatus, numEpisodes: numEpisodes, startDate: seriesStart,
                        endDate: seriesEnd, imageUrl: seriesImage);


                    int numEpisodesWatched = GetElementValueInt(anime, "my_watched_episodes");

                    string myStartDateString = GetElementValueString(anime, "my_start_date");
                    UncertainDate myStartDate = UncertainDate.FromMalDateString(myStartDateString);

                    string myFinishDateString = GetElementValueString(anime, "my_finish_date");
                    UncertainDate myFinishDate = UncertainDate.FromMalDateString(myFinishDateString);

                    decimal rawScore = GetElementValueDecimal(anime, "my_score");
                    decimal? myScore = rawScore == 0 ? (decimal?)null : rawScore;

                    int completionStatusInt = GetElementValueInt(anime, "my_status");
                    AnimeCompletionStatus completionStatus = (AnimeCompletionStatus)completionStatusInt;

                    long lastUpdatedUnixTimestamp = GetElementValueLong(anime, "my_last_updated");
                    DateTime lastUpdated = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(lastUpdatedUnixTimestamp);

                    string rawTagsString = GetElementValueString(anime, "my_tags");
                    string[] untrimmedTags = rawTagsString.Split(TagSeparator, StringSplitOptions.RemoveEmptyEntries);
                    List<string> tags = new List<string>(untrimmedTags.Select(tag => tag.Trim()));

                    MyAnimeListEntry entry = new MyAnimeListEntry(score: myScore, status: completionStatus, numEpisodesWatched: numEpisodesWatched,
                        myStartDate: myStartDate, myFinishDate: myFinishDate, myLastUpdate: lastUpdated, animeInfo: animeInfo, tags: tags);

                    animeEntries.Add(entry);
                }
            }

            // Manga entries
            if (doc.Root.Element("manga") != null)
            {

                IEnumerable<XElement> mangas = doc.Root.Elements("manga");
                foreach (XElement manga in mangas)
                {
                    int mangaId = GetElementValueInt(manga, "series_mangadb_id");
                    string title = GetElementValueString(manga, "series_title");

                    string synonymList = GetElementValueString(manga, "series_synonyms");
                    string[] rawSynonyms = synonymList.Split(SynonymSeparator, StringSplitOptions.RemoveEmptyEntries);

                    // filter out synonyms that are the same as the main title
                    HashSet<string> synonyms = new HashSet<string>(rawSynonyms.Where(synonym => !synonym.Equals(title, StringComparison.Ordinal)));

                    int seriesTypeInt = GetElementValueInt(manga, "series_type");
                    MalMangaType seriesType = (MalMangaType)seriesTypeInt;

                    int numChapters = GetElementValueInt(manga, "series_chapters");

                    int numVolumes = GetElementValueInt(manga, "series_volumes");

                    int seriesStatusInt = GetElementValueInt(manga, "series_status");
                    MalMangaSeriesStatus seriesStatus = (MalMangaSeriesStatus)seriesStatusInt;

                    string seriesStartString = GetElementValueString(manga, "series_start");
                    UncertainDate seriesStart = UncertainDate.FromMalDateString(seriesStartString);

                    string seriesEndString = GetElementValueString(manga, "series_end");
                    UncertainDate seriesEnd = UncertainDate.FromMalDateString(seriesEndString);

                    string seriesImage = GetElementValueString(manga, "series_image");

                    MalMangaInfoFromUserLookup mangaInfo = new MalMangaInfoFromUserLookup(mangaId: mangaId, title: title,
                        type: seriesType, synonyms: synonyms, status: seriesStatus, numChapters: numChapters, numVolumes: numVolumes, startDate: seriesStart,
                        endDate: seriesEnd, imageUrl: seriesImage);


                    int numChaptersRead = GetElementValueInt(manga, "my_read_chapters");

                    int numVolumesRead = GetElementValueInt(manga, "my_read_volumes");

                    string myStartDateString = GetElementValueString(manga, "my_start_date");
                    UncertainDate myStartDate = UncertainDate.FromMalDateString(myStartDateString);

                    string myFinishDateString = GetElementValueString(manga, "my_finish_date");
                    UncertainDate myFinishDate = UncertainDate.FromMalDateString(myFinishDateString);

                    decimal rawScore = GetElementValueDecimal(manga, "my_score");
                    decimal? myScore = rawScore == 0 ? (decimal?)null : rawScore;

                    int completionStatusInt = GetElementValueInt(manga, "my_status");
                    MangaCompletionStatus completionStatus = (MangaCompletionStatus)completionStatusInt;

                    long lastUpdatedUnixTimestamp = GetElementValueLong(manga, "my_last_updated");
                    DateTime lastUpdated = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(lastUpdatedUnixTimestamp);

                    string rawTagsString = GetElementValueString(manga, "my_tags");
                    string[] untrimmedTags = rawTagsString.Split(TagSeparator, StringSplitOptions.RemoveEmptyEntries);
                    List<string> tags = new List<string>(untrimmedTags.Select(tag => tag.Trim()));

                    MyMangaListEntry entry = new MyMangaListEntry(score: myScore, status: completionStatus, numChaptersRead: numChaptersRead, numVolumesRead: numVolumesRead,
                        myStartDate: myStartDate, myFinishDate: myFinishDate, myLastUpdate: lastUpdated, mangaInfo: mangaInfo, tags: tags);

                    mangaEntries.Add(entry);
                }
            }
            MalUserLookupResults results = new MalUserLookupResults(userId: userId, canonicalUserName: canonicalUserName, animeList: animeEntries, mangaList: mangaEntries);
            Logging.Log.Trace("Parsed XML.");
            return results;
        }

        private static readonly string[] SynonymSeparator = new string[] { "; " };
        private static readonly char[] TagSeparator = new char[] { ',' };

        private static XElement GetExpectedElement(XContainer container, string elementName)
        {
            XElement element = container.Element(elementName);
            if (element == null)
            {
                throw new MalApiException(string.Format("Did not find element {0}.", elementName));
            }
            return element;
        }

        private static string GetElementValueString(XContainer container, string elementName)
        {
            XElement element = GetExpectedElement(container, elementName);

            try
            {
                return (string)element;
            }
            catch (FormatException ex)
            {
                throw new MalApiException(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
            }
        }

        private static int GetElementValueInt(XContainer container, string elementName)
        {
            XElement element = GetExpectedElement(container, elementName);

            try
            {
                return (int)element;
            }
            catch (FormatException ex)
            {
                throw new MalApiException(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
            }
        }

        private static long GetElementValueLong(XContainer container, string elementName)
        {
            XElement element = GetExpectedElement(container, elementName);

            try
            {
                return (long)element;
            }
            catch (FormatException ex)
            {
                throw new MalApiException(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
            }
        }

        private static decimal GetElementValueDecimal(XContainer container, string elementName)
        {
            XElement element = GetExpectedElement(container, elementName);

            try
            {
                return (decimal)element;
            }
            catch (FormatException ex)
            {
                throw new MalApiException(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
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
