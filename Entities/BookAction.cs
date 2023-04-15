namespace Readerpath.Entities
{
    public class BookAction
    {
        public int Id { get; set; }
        public Edition Edition { get; set; }
        public string User { get; set; }
        public DateTime? DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }
        public float? Rating { get; set; }
        public string? Opinion { get; set; }
        public DateTime DateAdded { get; set; }

        public BookAction()
        {
            DateAdded = DateTime.Now;
        }
    }

}
