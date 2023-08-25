using Readerpath.Entities;

namespace Readerpath.Models
{

    public class YearStatisticsModel
    {
        public string Year { get; set; }
        public int BookCount { get; set; }
        public int PrevYearBookCount { get; set; }
        public int? YearChallengeCount { get; set; }
        public int PaperBooksCount { get; set; }
        public int EbooksCount { get; set; }
        public int AudiobooksCount { get; set; }
        public int PagesCount { get; set; }
        public int AudiobooksMinutes { get; set; }
        public float RatingAverage { get; set; }
        public float PrevYearRatingAverage { get; set; }
        public string FavouriteGenre { get; set; }
        public string BestBook { get; set; }
        public string WorstBook { get; set; }
        public List<BookWithRating> Books { get; set; }
        public List<GenreWithCount> Genres { get; set; }
        public List<PublisherWithCount> Publishers { get; set; }
    }
}
