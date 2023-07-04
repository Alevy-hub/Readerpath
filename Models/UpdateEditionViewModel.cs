using Readerpath.Entities;

namespace Readerpath.Models
{
    public class UpdateEditionViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public List<Publisher>? PublisherList { get; set; }
        public Edition? Edition { get; set; }
    }
}
