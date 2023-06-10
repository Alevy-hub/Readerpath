namespace Readerpath.Models
{
	public class FinishMonthModel
	{
		public int month { get; set; }
		public int year { get; set; }
		public List<ReadBook> ReadBooks { get; set; }
	}

	public class ReadBook
	{
		public int BookActionId {get; set;}
		public string BookTitle { get; set;}
		public float? Rating { get; set;}

	}
}
