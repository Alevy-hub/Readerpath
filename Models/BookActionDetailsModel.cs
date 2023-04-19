namespace Readerpath.Models
{
    public class BookActionDetailsModel
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string? Publisher { get; set; }
        public string Type { get; set; }
        public float? Rating { get; set; }
        public string? Comment { get; set; }
        public int? Pages { get; set; }
        public int? Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }

    }
}
