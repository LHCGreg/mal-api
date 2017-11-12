using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MalApi
{
    /// <summary>
    /// The update object sent to MAL when updating an anime entry.
    /// Only specified values will be changed. The rest will remain unchanged.
    /// More details: https://myanimelist.net/modules.php?go=api#animevalues
    /// </summary>
    [XmlRoot("entry")]
    public class UpdateAnime : UpdateObjectBase, IXmlSerializable
    {
        [XmlElement("episode")]
        public int? Episode { get; set; } = null;

        [XmlElement("status")]
        public AnimeCompletionStatus? Status { get; set; } = null;

        [XmlElement("score")]
        public int? Score { get; set; } = null;

        [XmlElement("storage_type")]
        public int? StorageType { get; set; } = null;

        [XmlElement("storage_value")]
        public float? StorageValue { get; set; } = null;

        [XmlElement("times_rewatched")]
        public int? TimesRewatched { get; set; } = null;

        [XmlElement("rewatch_value")]
        public int? RewatchValue { get; set; } = null;

        [XmlIgnore]
        public DateTime? DateStart { get; set; } = null;

        [XmlElement("date_start")]
        private string FormattedDateStart
        {
            get
            {
                return DateStart?.ToString("MMddyyyy");
            }
            set
            {
                DateStart = DateTime.Parse(value);
            }
        }

        public DateTime? DateFinish { get; set; } = null;

        [XmlElement("date_finish")]
        private string FormattedDateFinish
        {
            get
            {
                return DateFinish?.ToString("MMddyyyy");
            }
            set
            {
                DateFinish = DateTime.Parse(value);
            }
        }

        [XmlElement("priority")]
        public int? Priority { get; set; } = null;

        [XmlElement("enable_discussion")]
        public int? EnableDiscussion { get; set; } = null;

        [XmlElement("enable_rewatching")]
        public int? EnableRewatching { get; set; } = null;

        [XmlElement("comments")]
        public string Comments { get; set; } = null;

        [XmlElement("tags")]
        public string Tags { get; set; } = null;

        #region IXmlSerializable implementation

        public XmlSchema GetSchema()
        {
            return null;
        }

        // 12.11.17 - MAL API does not return this structure
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            // Episode
            writer.WriteStartElement("episode");
            if (Episode != null)
            {
                writer.WriteValue(Episode);
            }
            writer.WriteEndElement();

            // Status
            writer.WriteStartElement("status");
            if (Status != null)
            {
                writer.WriteString(Status.Value.ToString().ToLower());
            }
            writer.WriteEndElement();

            // Score
            writer.WriteStartElement("score");
            if (Score != null)
            {
                writer.WriteValue(Score);
            }
            writer.WriteEndElement();

            // Storage type
            writer.WriteStartElement("storage_type");
            if (StorageType != null)
            {
                writer.WriteValue(StorageType);
            }
            writer.WriteEndElement();

            // Storage value
            writer.WriteStartElement("storage_value");
            if (StorageValue != null)
            {
                writer.WriteValue(StorageValue);
            }
            writer.WriteEndElement();

            // Times rewatched
            writer.WriteStartElement("times_rewatched");
            if (TimesRewatched != null)
            {
                writer.WriteValue(TimesRewatched);
            }
            writer.WriteEndElement();

            // Rewatch value
            writer.WriteStartElement("rewatch_value");
            if (RewatchValue != null)
            {
                writer.WriteValue(RewatchValue);
            }
            writer.WriteEndElement();

            // Date start (formatted)
            writer.WriteStartElement("date_start");
            if (FormattedDateStart != null)
            {
                writer.WriteString(FormattedDateStart);
            }
            writer.WriteEndElement();

            // Date finish (formatted)
            writer.WriteStartElement("date_finish");
            if (FormattedDateFinish != null)
            {
                writer.WriteString(FormattedDateFinish);
            }
            writer.WriteEndElement();

            // Priority
            writer.WriteStartElement("priority");
            if (Priority != null)
            {
                writer.WriteValue(Priority);
            }
            writer.WriteEndElement();

            // Enable discussion
            writer.WriteStartElement("enable_discussion");
            if (EnableDiscussion != null)
            {
                writer.WriteValue(EnableDiscussion);
            }
            writer.WriteEndElement();

            // Enable rewatching
            writer.WriteStartElement("enable_rewatching");
            if (EnableRewatching != null)
            {
                writer.WriteValue(EnableRewatching);
            }
            writer.WriteEndElement();

            // Enable comments
            writer.WriteStartElement("comments");
            if (Comments != null)

            {
                writer.WriteString(Comments);
            }
            writer.WriteEndElement();

            // Tags
            writer.WriteStartElement("tags");
            if (Tags != null)

            {
                writer.WriteString(Tags);
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
