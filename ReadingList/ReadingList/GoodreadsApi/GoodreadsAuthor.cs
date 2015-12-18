using System;
using System.Linq;
using System.Xml.Linq;

namespace ReadingList
{
    internal sealed class GoodreadsAuthor : GoodreadsObject
    {
        public readonly string Name;

        public override string ToString()
        {
            return this.Name;
        }

        private GoodreadsAuthor(int id, string name) : base (id)
        {
            this.Name = name;
        }

        public static bool TryParse(XContainer xml, out GoodreadsAuthor result)
        {
            result = null;

            var authorNode = (xml is XElement && (xml as XElement).Name == "author") ? xml as XElement : xml.Descendants("author").FirstOrDefault();
            var idNode = (authorNode.Element("id")?.FirstNode as XText)?.Value;
            var nameNode = (authorNode.Element("name")?.FirstNode as XText)?.Value;
            if (string.IsNullOrEmpty(nameNode))
            {
                return false;
            }

            int id;
            if (idNode == null || !int.TryParse(idNode, out id))
            {
                id = int.MinValue;
            }

            result = new GoodreadsAuthor(id, nameNode);

            return true;
        }
    }
}