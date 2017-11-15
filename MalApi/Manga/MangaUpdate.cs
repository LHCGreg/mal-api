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
                    new XElement("chapter", Chapter),
                    new XElement("volume", Volume),
                    new XElement("status", (int?)Status),
                    new XElement("score", Score),
                    new XElement("times_reread", TimesReread),
                    new XElement("reread_value", RereadValue),
                    new XElement("date_start", FormattedDateStart),
                    new XElement("date_finish", FormattedDateFinish),
                    new XElement("priority", Priority),
                    new XElement("enable_discussion", EnableDiscussion),
                    new XElement("enable_rereading", EnableRereading),
                    new XElement("comments", Comments),
                    new XElement("scan_group", ScanGroup),
                    new XElement("tags", Tags),
                    new XElement("retail_volumes", RetailVolumes)
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
