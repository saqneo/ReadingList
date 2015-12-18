using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReadingList
{
    internal sealed class GoodreadsWork : GoodreadsObject
    {
        public readonly GoodreadsBook Book;
        public readonly float AverageRating;

        public override string ToString()
        {
            return this.Book.ToString();
        }

        private GoodreadsWork(int id, GoodreadsBook book) : this(id, book, Single.NaN)
        {
        }

        private GoodreadsWork(int id, GoodreadsBook book, float averageRating) : base(id)
        {
            this.Book = book;
            this.AverageRating = averageRating;
        }

        public static IEnumerable<GoodreadsWork> ParseMany(string xml)
        {
            XDocument doc;

            try
            {
                doc = XDocument.Parse(xml);
            }
            catch (Exception e) when (e is ArgumentNullException || e is InvalidOperationException)
            {
                yield break;
            }

            foreach (var workNode in doc.Descendants("work"))
            {
                GoodreadsWork work;
                if (GoodreadsWork.TryParse(workNode, out work))
                {
                    yield return work;
                }
            }
        }

        private static bool TryParse(XContainer xml, out GoodreadsWork result)
        {
            result = null;
            
            var workNode = (xml is XElement && (xml as XElement).Name == "work") ? xml as XElement : xml.Descendants("work").FirstOrDefault();

            var idNode = (workNode.Element("id")?.FirstNode as XText)?.Value;

            int id;
            if (idNode == null || !int.TryParse(idNode, out id))
            {
                id = int.MinValue;
            }

            var bookNode = workNode.Element("best_book");

            GoodreadsBook book;
            if (bookNode == null || !GoodreadsBook.TryParse(bookNode, out book))
            {
                return false;
            }

            var averageRatingNode = (workNode.Element("average_rating")?.FirstNode as XText)?.Value;

            float averageRating;
            if (averageRatingNode == null || !float.TryParse(averageRatingNode, out averageRating))
            {
                result = new GoodreadsWork(id, book);
            }
            else
            {
                result = new GoodreadsWork(id, book, averageRating);
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is GoodreadsWork))
            {
                return false;
            }

            return this.ToString() == (obj as GoodreadsWork).ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}