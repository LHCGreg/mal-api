using System.Xml.Serialization;

namespace MalApi
{
    [XmlRoot("entry")]
    public class UpdateMangaObject : UpdateObjectBase
    {
        [XmlElement("chapter")]
        public string Chapter { get; set; } = null;

        [XmlElement("volume")]
        public string Volume { get; set; } = null;

        [XmlElement("status")]
        public string Status { get; set; } = null;

        [XmlElement("score")]
        public string Score { get; set; } = null;

        [XmlElement("times_reread")]
        public string TimesReread { get; set; } = null;

        [XmlElement("reread_value")]
        public string RereadValue { get; set; } = null;

        [XmlElement("date_start")]
        public string DateStart { get; set; } = null;

        [XmlElement("date_finish")]
        public string DateFinish { get; set; } = null;

        [XmlElement("priority")]
        public string Priority { get; set; } = null;

        [XmlElement("enable_discussion")]
        public string EnableDiscussion { get; set; } = null;

        [XmlElement("enable_rereading")]
        public string EnableRereading { get; set; } = null;

        [XmlElement("comments")]
        public string Comments { get; set; } = null;

        [XmlElement("scan_group")]
        public string ScanGroup { get; set; } = null;

        [XmlElement("tags")]
        public string Tags { get; set; } = null;

        [XmlElement("retail_volumes")]
        public string RetailVolumes { get; set; } = null;

        public UpdateMangaObject()
        {
        }

        public UpdateMangaObject(string chapter = null,
                                 string volume = null,
                                 string status = null,
                                 string score = null,
                                 string timesReread = null,
                                 string rereadValue = null,
                                 string dateStart = null,
                                 string dateFinish = null,
                                 string priority = null,
                                 string enableDiscussion = null,
                                 string enableRereading = null,
                                 string comments = null,
                                 string scanGroup = null,
                                 string tags = null,
                                 string retailVolumes = null)
        {
            Chapter = chapter;
            Volume = volume;
            Status = status;
            Score = score;
            TimesReread = timesReread;
            RereadValue = rereadValue;
            DateStart = dateStart;
            DateFinish = dateFinish;
            Priority = priority;
            EnableDiscussion = enableDiscussion;
            EnableRereading = enableRereading;
            Comments = comments;
            ScanGroup = scanGroup;
            Tags = tags;
            RetailVolumes = retailVolumes;
        }
    }
}
