using Microsoft.CodeAnalysis.Differencing;

namespace Readerpath.Models
{
    public class SearchModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public bool Paper { get; set; }
        public bool Audiobook { get; set; }
        public bool Ebook { get; set; }
    }
}
