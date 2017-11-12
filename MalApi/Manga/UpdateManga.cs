using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MalApi
{
    /// <summary>
    /// The update object sent to MAL when updating a manga entry.
    /// Only specified values will be changed. The rest will remain unchanged.
    /// More details: https://myanimelist.net/modules.php?go=api#mangavalues
    /// </summary>
    [XmlRoot("entry")]
    public class UpdateManga : UpdateObjectBase, IXmlSerializable
    {
        [XmlElement("chapter")]
        public int? Chapter { get; set; } = null;

        [XmlElement("volume")]
        public int? Volume { get; set; } = null;

        [XmlElement("status")]
        public MangaCompletionStatus? Status { get; set; } = null;

        [XmlElement("score")]
        public int? Score { get; set; } = null;

        [XmlElement("times_reread")]
        public int? TimesReread { get; set; } = null;

        [XmlElement("reread_value")]
        public int? RereadValue { get; set; } = null;

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

        [XmlElement("enable_rereading")]
        public int? EnableRereading { get; set; } = null;

        [XmlElement("comments")]
        public string Comments { get; set; } = null;

        [XmlElement("scan_group")]
        public string ScanGroup { get; set; } = null;

        [XmlElement("tags")]
        public string Tags { get; set; } = null;

        [XmlElement("retail_volumes")]
        public int? RetailVolumes { get; set; } = null;

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
            writer.WriteStartElement("chapter");
            if (Chapter != null)
            {
                writer.WriteValue(Chapter);
            }
            writer.WriteEndElement();

            // Volume
            writer.WriteStartElement("volume");
            if (Volume != null)
            {
                writer.WriteValue(Volume);
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

            // Times reread
            writer.WriteStartElement("times_reread");
            if (TimesReread != null)
            {
                writer.WriteValue(TimesReread);
            }
            writer.WriteEndElement();

            // Reread value
            writer.WriteStartElement("reread_value");
            if (RereadValue != null)
            {
                writer.WriteValue(RereadValue);
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

            // Enable rereading
            writer.WriteStartElement("enable_rereading");
            if (EnableRereading != null)
            {
                writer.WriteValue(EnableRereading);
            }
            writer.WriteEndElement();

            // Comments
            writer.WriteStartElement("comments");
            if (Comments != null)
            {
                writer.WriteString(Comments);
            }
            writer.WriteEndElement();

            // Scan group
            writer.WriteStartElement("scan_group");
            if (ScanGroup != null)
            {
                writer.WriteString(ScanGroup);
            }
            writer.WriteEndElement();

            // Tags
            writer.WriteStartElement("tags");
            if (Tags != null)
            {
                writer.WriteString(Tags);
            }
            writer.WriteEndElement();

            // Retail volumes
            writer.WriteStartElement("retail_volumes");
            if (RetailVolumes != null)
            {
                writer.WriteValue(RetailVolumes);
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
