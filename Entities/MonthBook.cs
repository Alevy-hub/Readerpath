namespace Readerpath.Entities
{
    public class MonthBook
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public BookAction BestBook { get; set; }
        public BookAction WorstBook { get; set; }
        public string User { get; set; }

    }
}
