using Readerpath.Entities;

namespace Readerpath.Models
{
    public class BookDetailsModel
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public List<EditionModel> Editions { get; set; }
    }

    public class EditionModel
    {
        public string Name { get; set; }
        public Readerpath.Entities.Type Type { get; set; }
        public int? Pages { get; set; }
        public int? Duration { get; set; }
    }
}
