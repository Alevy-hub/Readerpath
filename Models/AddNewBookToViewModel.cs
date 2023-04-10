using Readerpath.Entities;

namespace Readerpath.Models
{
	public class AddNewBookToViewModel
	{
		public List<Author> AuthorList { get; set; }
		public List<Genre> GenreList { get; set; }
		public List<Publisher> PublisherList { get; set; }
	}
}
