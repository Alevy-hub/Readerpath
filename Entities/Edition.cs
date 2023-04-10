using System.Security.Policy;

namespace Readerpath.Entities
{
    public enum Type
    {
        PaperBook,
        Ebook,
        Audiobook
    }

    public class Edition
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public Type Type { get; set; }
        public int? Pages { get; set; }
        public int? Duration { get; set; }
        public Publisher? Publisher { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public bool isAccepted { get; set; }

        public Edition()
        {
			DateAdded = DateTime.Now;
		}
	}
}
