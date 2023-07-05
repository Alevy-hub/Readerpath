using Readerpath.Entities;
using System.ComponentModel.DataAnnotations;

namespace Readerpath.Models
{
	public class AddNewBookModel
	{
		public int BookId { get; set; }
		[Required(ErrorMessage = "Tytuł jest wymagany")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Autor jest wymagany")]
		public string Author { get; set; }

		[Required(ErrorMessage = "Gatunek jest wymagany")]
		public string Genre { get; set; }
		public string Type { get; set; }
		public string? Publisher { get; set; }

		
		public int? Pages { get; set; }
		public int? Duration { get; set; }
		public int? DurationH { get; set; }
		public int? DurationM { get; set; }

		public List<Author>? AuthorList { get; set; }
        public List<Genre>? GenreList { get; set; }
        public List<Publisher>? PublisherList { get; set; }
    }
}
