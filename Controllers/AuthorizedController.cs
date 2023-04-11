using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readerpath.Data;
using Readerpath.Entities;

using Readerpath.Models;

namespace Readerpath.Controllers
{
    [Authorize]
    public class AuthorizedController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
		private readonly DbContextOptions<ApplicationDbContext> _options;


		public AuthorizedController(UserManager<IdentityUser> userManager, DbContextOptions<ApplicationDbContext> options)
        {
            _userManager = userManager;
			_options = options;
        }

        public async Task<IActionResult> LoggedIndex()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            LoggedIndexModel model = new LoggedIndexModel();
            model.UserName = user.UserName;

            return View(model);
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            //using (var context = new ApplicationDbContext(_options))
            //{
            //    List<Book> model = context.Books.Where(b => b.Title.Contains(query)).ToList();
            //    return View(model);
            //}

            using (var context = new ApplicationDbContext(_options))
            {
                List<Book> books = context.Books.Where(b => b.Title.Contains(query)).ToList();
                List<SearchModel> modelList = new List<SearchModel>();

                foreach(Book book in books)
                {
                    SearchModel model = new SearchModel();
                    model.Title = book.Title;
                    model.Author = book.Author.Name;
                    model.Genre = book.Genre.Name;
                    modelList.Add(model);
                }

                return View(modelList);
            }

        }

        public IActionResult AddNewBook()
        {
            AddNewBookToViewModel model = new AddNewBookToViewModel();
            using (var context = new ApplicationDbContext(_options))
            {
                model.AuthorList = context.Authors.ToList();
                model.GenreList = context.Genres.ToList();
                model.PublisherList = context.Publishers.ToList();
            }
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddNewBook(AddNewBookModel model)
        {
            using (var context = new ApplicationDbContext(_options))
            {

                var user = await _userManager.GetUserAsync(HttpContext.User);

                Author author = context.Authors.FirstOrDefault(a => a.Name == model.Author);
                if (author == null)
                {
                    Author newAuthor = new Author();
                    newAuthor.Name = model.Author;
                    newAuthor.AddedBy = user.Id;
                    context.Add(newAuthor);
                    author = newAuthor;
                }

                Genre genre = context.Genres.FirstOrDefault(g => g.Name == model.Genre);
                if (genre == null)
                {
                    Genre newGenre = new Genre();
                    newGenre.Name = model.Genre;
                    newGenre.AddedBy = user.Id;
                    context.Add(newGenre);
                    genre = newGenre;
                }


                Book NewBook = new Book();
                NewBook.Title = model.Title;
                NewBook.Author = author;
                NewBook.Genre = genre;
                NewBook.AddedBy = user.Id;
                context.Add(NewBook);

				Publisher publisher = context.Publishers.FirstOrDefault(p => p.Name == model.Publisher);
				if (publisher == null)
				{
					Publisher newPublisher = new Publisher();
					newPublisher.Name = model.Publisher;
					newPublisher.AddedBy = user.Id;
					context.Add(newPublisher);
					publisher = newPublisher;
				}

				Edition NewEdition = new Edition();
                NewEdition.Book = NewBook;
                if(model.Type == "Książka papierowa")
                {
                    NewEdition.Type = Entities.Type.PaperBook;
                    NewEdition.Pages = model.Pages;
                }
                else if(model.Type == "Ebook")
                {
                    NewEdition.Type = Entities.Type.Ebook;
                    NewEdition.Pages = model.Pages;
                }
                else if(model.Type == "Audiobook")
                {
                    NewEdition.Type = Entities.Type.Audiobook;
                    NewEdition.Duration = model.Duration;
                }
                NewEdition.AddedBy = user.Id;
                context.Add(NewEdition);


				await context.SaveChangesAsync();
				return RedirectToAction(nameof(LoggedIndex));
            }
        }
    }
}
