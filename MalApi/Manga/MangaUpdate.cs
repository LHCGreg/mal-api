using System;
using System.Xml;
using System.Xml.Linq;
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
    public class MangaUpdate
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

        /// <summary>
        /// Generates an XML with the current information stored in the object. XML can later be used to update records on MAL.
        /// </summary>
        /// <returns>String representation of object-generated XML.</returns>
        public string GenerateXml()
        {
            XDocument document = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("entry",
                    Chapter != null ? new XElement("chapter", Chapter) : null,
                    Volume != null ? new XElement("volume", Volume) : null,
                    Status != null ? new XElement("status", (int?)Status) : null,
                    Score != null ? new XElement("score", Score) : null,
                    TimesReread != null ? new XElement("times_reread", TimesReread) : null,
                    RereadValue != null ? new XElement("reread_value", RereadValue) : null,
                    FormattedDateStart != null ? new XElement("date_start", FormattedDateStart) : null,
                    FormattedDateFinish != null ? new XElement("date_finish", FormattedDateFinish) : null,
                    Priority != null ? new XElement("priority", Priority) : null,
                    EnableDiscussion != null ? new XElement("enable_discussion", EnableDiscussion) : null,
                    EnableRereading != null ? new XElement("enable_rereading", EnableRereading) : null,
                    Comments != null ? new XElement("comments", Comments) : null,
                    // ScanGroup != null ? new XElement("scan_group", ScanGroup) : null,
                    Tags != null ? new XElement("tags", Tags) : null,
                    RetailVolumes != null ? new XElement("retail_volumes", RetailVolumes) : null
                )
            );

            using (Utf8StringWriter writer = new Utf8StringWriter())
            {
                document.Save(writer);
                return writer.ToString();
            }
        }
    }
}
