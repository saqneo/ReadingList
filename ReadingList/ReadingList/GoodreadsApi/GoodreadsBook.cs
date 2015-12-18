using System;
using System.Linq;
using System.Xml.Linq;

namespace ReadingList
{
    internal sealed class GoodreadsBook : GoodreadsObject
    {
        public readonly string Title;
        public readonly GoodreadsAuthor Author;

        public override string ToString()
        {
            var str = this.Title;

            if (this.Author != null)
            {
                str += " by " + this.Author.ToString();
            }

            return str;
        }

        private GoodreadsBook(int id, string title, GoodreadsAuthor author) : base (id)
        {
            this.Title = title;
            this.Author = author;
        }

        private GoodreadsBook(int id, string title) : this(id, title, null)
        {
            this.Title = title;
        }

        public static bool TryParse(XContainer xml, out GoodreadsBook result)
        {
            result = null;

            var bookNode = (xml is XElement && (xml as XElement).Name == "best_book") ? xml as XElement : xml.Descendants("best_book").FirstOrDefault();
            var idNode = (bookNode.Element("id")?.FirstNode as XText)?.Value;
            var titleNode = bookNode.Element("title");
            var authorNode = bookNode.Element("author");

            var title = titleNode?.Value;
            if (string.IsNullOrEmpty(title))
            {
                return false;
            }

            int id;
            if (idNode == null || !int.TryParse(idNode, out id))
            {
                id = int.MinValue;
            }
            
            GoodreadsAuthor author;
            if (authorNode == null || !GoodreadsAuthor.TryParse(authorNode, out author))
            {
                result = new GoodreadsBook(id, title);
            }
            else
            {
                result = new GoodreadsBook(id, title, author);
            }

            return true;
        }
    }
}