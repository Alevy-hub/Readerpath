namespace Readerpath.Entities
{
    public class TBR
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public string User { get; set; }
        public int Year { get; set; }
        public DateTime DateAdded { get; set; }

        public TBR()
        {
            DateAdded = DateTime.Now;
        }
    }
}
