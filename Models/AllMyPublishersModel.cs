namespace Readerpath.Models
{
    public class AllMyPublishersModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ReadCount { get; set; }
        public int AudiobookCount { get; set; }
        public int PaperbookCount { get; set; }
        public int EbookCount { get; set; }
    }
}
