namespace Readerpath.Entities
{
    public class MonthBook
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Book BestBook { get; set; }
        public Book WorstBook { get; set; }
        public string User { get; set; }

    }
}
