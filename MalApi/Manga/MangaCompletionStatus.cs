using System.Xml.Serialization;

namespace MalApi
{
    // XML enum attributes are needed for proper serialization upon sending an update request
    public enum MangaCompletionStatus
    {
        [XmlEnum(Name = "reading")]
        Reading = 1,
        [XmlEnum(Name = "completed")]
        Completed = 2,
        [XmlEnum(Name = "onhold")]
        OnHold = 3,
        [XmlEnum(Name = "dropped")]
        Dropped = 4,
        [XmlEnum(Name = "plantoread")]
        PlanToRead = 6,
    }
}