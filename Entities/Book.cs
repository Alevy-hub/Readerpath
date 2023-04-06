namespace Readerpath.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Author Author { get; set; }
        public Genre Genre { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsAccepted { get; set; }

        public Book()
        {
            DateAdded = DateTime.Now;
        }
    }
}
