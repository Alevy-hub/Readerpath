using Readerpath.Entities;

namespace Readerpath.Models
{
	public class PublisherDetailsModel
	{
		public string Name { get; set; }
		public int ReadCount { get; set; }
		public int PaperbookCount { get; set; }
		public int AudiobookCount { get; set; }
		public int EbookCount { get; set; }
		public float RatingAvg { get; set; }
		public List<ReadPublisher> AllReadOfPublisher { get; set; }
		public List<BookPublisher> AllBooksOfPublisher { get; set; }


}

	public class ReadPublisher
	{
		public int BookActionId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string Genre { get; set; }
		public float? Rating { get; set; }

	}

	public class BookPublisher
	{
		public int BookId { get; set; }
		public int EditionId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string Genre { get; set; }
		public Entities.Type Type { get; set; }
	}
}
