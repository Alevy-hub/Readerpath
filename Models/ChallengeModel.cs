using Readerpath.Entities;

namespace Readerpath.Models
{
    public class ChallengeModel
    {
        public YearChallenge? YearChallenge { get; set; }
        public List<BookInChallenge>? BooksInChallenge { get; set;}
        public ChallengeColors? ChallengeColors { get; set; }
        public bool IsPrevYearAvailable { get; set; }
        public bool IsNextYearAvailable { get; set; }
    }

    public class BookInChallenge
    {
		public int BookActionId { get; set; }
		public float Rating { get; set; }
		public DateTime? FinishDate { get; set; }
	}
}
