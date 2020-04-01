using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MalApi
{
    public class MyMangaListEntry : IEquatable<MyMangaListEntry>
    {
        public decimal? Score { get; private set; }
        public MangaCompletionStatus Status { get; private set; }
        public int NumChaptersRead { get; private set; }
        public int NumVolumesRead { get; private set; }
        public UncertainDate MyStartDate { get; private set; }
        public UncertainDate MyFinishDate { get; private set; }
        public DateTime MyLastUpdate { get; private set; }
        public MalMangaInfoFromUserLookup MangaInfo { get; private set; }
        public ICollection<string> Tags { get; private set; }

        public MyMangaListEntry(decimal? score, MangaCompletionStatus status, int numChaptersRead, int numVolumesRead, UncertainDate myStartDate,
            UncertainDate myFinishDate, DateTime myLastUpdate, MalMangaInfoFromUserLookup mangaInfo, ICollection<string> tags)
        {
            Score = score;
            Status = status;
            NumChaptersRead = numChaptersRead;
            NumVolumesRead = numVolumesRead;
            MyStartDate = myStartDate;
            MyFinishDate = myFinishDate;
            MyLastUpdate = myLastUpdate;
            MangaInfo = mangaInfo;
            Tags = tags;
        }

        public bool Equals(MyMangaListEntry other)
        {
            if (other == null) return false;
            return this.MangaInfo.MangaId == other.MangaInfo.MangaId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MyMangaListEntry);
        }

        public override int GetHashCode()
        {
            return MangaInfo.MangaId.GetHashCode();
        }

        public override string ToString()
        {
            return MangaInfo.Title;
        }
    }
}
