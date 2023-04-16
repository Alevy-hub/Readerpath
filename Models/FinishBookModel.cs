namespace Readerpath.Models
{
	public class FinishBookModel
	{
		public int Id { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime FinishDate { get; set; }
		public string Rating { get; set; }
		public string Comment { get; set; }
	}
}
