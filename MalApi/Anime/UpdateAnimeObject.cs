using System.Xml.Serialization;

namespace MalApi
{
    [XmlRoot("entry")]
    public class UpdateAnimeObject : UpdateObjectBase
    {
        [XmlElement("episode")]
        public string Episode { get; set; } = null;

        [XmlElement("status")]
        public string Status { get; set; } = null;

        [XmlElement("score")]
        public string Score { get; set; } = null;

        [XmlElement("storage_type")]
        public string StorageType { get; set; } = null;

        [XmlElement("storage_value")]
        public string StorageValue { get; set; } = null;

        [XmlElement("times_rewatched")]
        public string TimesRewatched { get; set; } = null;

        [XmlElement("rewatch_value")]
        public string RewatchValue { get; set; } = null;

        [XmlElement("date_start")]
        public string DateStart { get; set; } = null;

        [XmlElement("date_finish")]
        public string DateFinish { get; set; } = null;

        [XmlElement("priority")]
        public string Priority { get; set; } = null;

        [XmlElement("enable_discussion")]
        public string EnableDiscussion { get; set; } = null;

        [XmlElement("enable_rewatching")]
        public string EnableRewatching { get; set; } = null;

        [XmlElement("comments")]
        public string Comments { get; set; } = null;

        [XmlElement("tags")]
        public string Tags { get; set; } = null;

        private UpdateAnimeObject()
        {
        }

        public UpdateAnimeObject(string episode = null,
                                 string status = null,
                                 string score = null,
                                 string storageType = null,
                                 string storageValue = null,
                                 string timesRewatched = null,
                                 string rewatchValue = null,
                                 string dateStart = null,
                                 string dateFinish = null,
                                 string priority = null,
                                 string enableDiscussion = null,
                                 string enableRewatching = null,
                                 string comments = null,
                                 string tags = null)
        {
            Episode = episode;
            Status = status;
            Score = score;
            StorageType = storageType;
            StorageValue = storageValue;
            TimesRewatched = timesRewatched;
            RewatchValue = rewatchValue;
            DateStart = dateStart;
            DateFinish = dateFinish;
            Priority = priority;
            EnableDiscussion = enableDiscussion;
            EnableRewatching = enableRewatching;
            Comments = comments;
            Tags = tags;
        }
    }
}
