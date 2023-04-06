namespace Readerpath.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }

        public Genre()
        {
            DateAdded = DateTime.Now;
        }
    }
}
