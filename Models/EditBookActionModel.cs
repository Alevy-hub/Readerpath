namespace Readerpath.Models
{
	public class EditBookActionModel
	{
		public int BookActionId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime FinishDate { get; set; }
		public string Rating { get; set; }
		public string Comment { get; set; }
		public string Status { get; set; }
	}
}
