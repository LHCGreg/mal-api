using System;
using System.Collections.Generic;

namespace MalApi
{
    public class MalMangaInfoFromUserLookup : IEquatable<MalMangaInfoFromUserLookup>
    {
        public int MangaId { get; private set; }
        public string Title { get; private set; }

        /// <summary>
        /// Could be something other than the enumerated values if MAL adds new types!
        /// </summary>
        public MalMangaType Type { get; private set; }

        public ICollection<string> Synonyms { get; private set; }
        public MalMangaSeriesStatus Status { get; private set; }

        public int NumChapters { get; private set; }

        public int NumVolumes { get; private set; }

        public UncertainDate StartDate { get; private set; }
        public UncertainDate EndDate { get; private set; }
        public string ImageUrl { get; private set; }

        public MalMangaInfoFromUserLookup(int mangaId, string title, MalMangaType type, ICollection<string> synonyms, MalMangaSeriesStatus status,
            int numChapters, int numVolumes, UncertainDate startDate, UncertainDate endDate, string imageUrl)
        {
            MangaId = mangaId;
            Title = title;
            Type = type;
            Synonyms = synonyms;
            Status = status;
            NumChapters = numChapters;
            NumVolumes = numVolumes;
            StartDate = startDate;
            EndDate = endDate;
            ImageUrl = imageUrl;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MalMangaInfoFromUserLookup);
        }

        public bool Equals(MalMangaInfoFromUserLookup other)
        {
            if (other == null) return false;
            return this.MangaId == other.MangaId;
        }

        public override int GetHashCode()
        {
            return MangaId.GetHashCode();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
