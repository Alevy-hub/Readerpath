namespace Readerpath.Entities
{
    public class TBR
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string User { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime DateAdded { get; set; }

        public TBR()
        {
            DateAdded = DateTime.Now;
        }
    }
}
