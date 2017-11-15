using System.IO;
using System.Text;

namespace MalApi
{
    /// <summary>
    /// Used to override the encoding of the writer to UTF-8.
    /// </summary>
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
