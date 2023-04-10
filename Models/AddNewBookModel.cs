namespace Readerpath.Models
{
	public class AddNewBookModel
	{
		public string Title { get; set; }
		public string Author { get; set; }
		public string Genre { get; set; }
		public string Type { get; set; }
		public string? Publisher { get; set; }
		public int? Pages { get; set; }
		public int? Duration { get; set; }
	}
}
