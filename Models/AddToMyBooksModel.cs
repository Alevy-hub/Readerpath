namespace Readerpath.Models
{
	public class AddToMyBooksModel
	{
		public int bookId { get; set; }
		public int editionId { get; set; }
		public Readerpath.Entities.Book book { get; set; }

		public string status { get; set; }
		public DateTime startDate { get; set; }
		public DateTime finishDate { get; set; }
		public float rating { get; set; }
		public string comment { get; set; }
	}
}
