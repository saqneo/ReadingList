using System.Xml.Linq;

namespace ReadingList
{
    internal abstract class GoodreadsObject
    {
        protected readonly int Id;

        protected GoodreadsObject(int id)
        {
            this.Id = id;
        }
    }
}