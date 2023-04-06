namespace Readerpath.Entities
{
    public class YearBook
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public Book BestBook { get; set; }
        public Book WorstBook { get; set; }
        public string User { get; set; }
    }
}
