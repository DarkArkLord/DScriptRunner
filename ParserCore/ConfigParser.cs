using System.IO;
using System.Xml.Linq;

namespace ParserCore
{
    public static class ConfigParser
    {
        public static XDocument ReadXml(string path)
        {
            var file = File.ReadAllText(path);
            var xDocument = XDocument.Parse(file);
            return xDocument;
        }
    }
}
