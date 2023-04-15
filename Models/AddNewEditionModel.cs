using Readerpath.Entities;

namespace Readerpath.Models
{
    public class AddNewEditionModel
    {
        public int BookId { get; set; }
        public string Type { get; set; }
        public string? Publisher { get; set; }
        public int? Pages { get; set; }
        public int? Duration { get; set; }
    }
}
