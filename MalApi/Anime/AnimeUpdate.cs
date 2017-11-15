using System;
using System.Xml;
using System.Xml.Linq;
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
    public class AnimeUpdate
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

        /// <summary>
        /// Generates an XML with the current information stored in the object. XML can later be used to update records on MAL.
        /// </summary>
        /// <returns>String representation of object-generated XML.</returns>
        public string GenerateXml()
        {
            XDocument document = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("entry",
                    new XElement("episode", Episode),
                    new XElement("status", (int?)Status),
                    new XElement("score", Score),
                    new XElement("storage_type", StorageType),
                    new XElement("storage_value", StorageValue),
                    new XElement("times_rewatched", TimesRewatched),
                    new XElement("rewatch_value", RewatchValue),
                    new XElement("date_start", FormattedDateStart),
                    new XElement("date_finish", FormattedDateFinish),
                    new XElement("priority", Priority),
                    new XElement("enable_discussion", EnableDiscussion),
                    new XElement("enable_rewatching", EnableRewatching),
                    new XElement("comments", Comments),
                    new XElement("tags", Tags)
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
