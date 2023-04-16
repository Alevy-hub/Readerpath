namespace Readerpath.Models
{
    public class LoggedIndexModel
    {
        public string UserName { get; set; }
        public List<NowReadingBook> NowReadingBooks { get; set; }


    }

    public class NowReadingBook
    {
        public int ActionId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Type { get; set; }
        public DateTime? startDate { get; set; }
        public int? Pages { get; set; }
        public int? Duration { get; set; }
    }

}
