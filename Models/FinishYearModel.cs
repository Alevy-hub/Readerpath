using Readerpath.Entities;

namespace Readerpath.Models
{
	public class FinishYearModel
	{
		public int Year { get; set; }
		public List<WorstBook> WorstBooks { get; set; }
		public List<BestBook> BestBooks { get; set; }
	}

	public class WorstBook
	{
		public int BookActionId { get; set; }
		public string BookTitle { get; set; }
		public string BookAuthor { get; set; }
		public string BookGenre { get; set; }
		public DateTime FinishDate { get; set; }
		public float? Rating { get; set; }
	}

	public class BestBook : WorstBook
	{

	}
}
