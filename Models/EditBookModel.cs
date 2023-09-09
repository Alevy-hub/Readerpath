using Readerpath.Entities;
using System.ComponentModel.DataAnnotations;


namespace Readerpath.Models
{
	public class EditBookModel
	{
		public int BookId { get; set; }
		[Required(ErrorMessage = "Tytuł jest wymagany")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Autor jest wymagany")]
		public string Author { get; set; }

		[Required(ErrorMessage = "Gatunek jest wymagany")]
		public string Genre { get; set; }

		public string GenreRadios { get; set; }
		public string AuthorRadios { get; set; }
		public string OldAuthor { get; set; }
		public string OldGenre { get; set; }


		public List<Author>? AuthorList { get; set; }
		public List<Genre>? GenreList { get; set; }
		public List<Publisher>? PublisherList { get; set; }
	}
}
