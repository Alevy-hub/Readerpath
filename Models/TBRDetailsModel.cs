using Readerpath.Entities;

namespace Readerpath.Models
{
    public class TBRDetailsModel
    {
        public List<TBRBook> TBRBooks { get; set; }
        public string Title { get; set; }
        public int TBRId { get; set; }
    }
}
