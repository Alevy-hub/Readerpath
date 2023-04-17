namespace Readerpath.Models
{
	public class AllReadBooksModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string Genre { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? FinishDate { get; set; }
	}
}
