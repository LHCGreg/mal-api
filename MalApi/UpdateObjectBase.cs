using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MalApi
{
    public class UpdateObjectBase
    {
        /// <summary>
        /// Generates an XML with the current information stored in the object. XML can later be used to update records on MAL.
        /// </summary>
        /// <returns>String representation of object-generated XML.</returns>
        public string GenerateXml()
        {
            var emptyNamespace = new XmlSerializerNamespaces();
            emptyNamespace.Add("", "");

            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
            var xml = string.Empty;

            using (var strWriter = new Utf8StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(strWriter))
                {
                    xmlSerializer.Serialize(xmlWriter, this, emptyNamespace);
                    xml = strWriter.ToString();

                    return xml;
                }
            }
        }

        /// <summary>
        /// Used to override the encoding of the writer to UTF-8.
        /// </summary>
        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
