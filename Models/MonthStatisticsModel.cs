using Readerpath.Entities;

namespace Readerpath.Models
{

    public class MonthStatisticsModel
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public int BookCount { get; set; }
        public int PrevMonthBookCount { get; set; }
        public int? YearChallengeCount { get; set; }
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
        public List<BookWithRating> Books { get; set; }
        public List<GenreWithCount> Genres { get; set; }
        public List<PublisherWithCount> Publishers { get; set; }
    }

    public class GenreWithCount
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class BookWithRating
    {
        public string Title { get; set; }
        public Entities.Type Type { get; set; }
        public string StringType { get; set; }
        public float Rating { get; set; }
    }

    public class PublisherWithCount
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
