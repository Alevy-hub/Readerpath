namespace Readerpath.Models
{
    public class MonthStatisticsModel
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public int BookCount { get; set; }
        public int PrevMonthBookCount { get; set; }
        public int YearChallengeCount { get; set; }
        public int PaperBooksCount { get; set; }
        public int EbooksCount { get; set; }
        public int AudiobooksCount { get; set; }
        public int PagesCount { get; set; }
        public int AudiobooksMinutes { get; set; }
        public float RatingAverage { get; set; }
        public float PrevMonthRatingAverage { get; set; }
        public string FavouriteGenre { get; set; }
        public string BestBook { get; set; }
        public string WorstBook { get; set; }

    }
}
