using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using NuGet.Configuration;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Security.Certificates;
using Readerpath.Data;
using Readerpath.Entities;
using Readerpath.Models;
using System.Globalization;
using System.Security.Policy;
using Publisher = Readerpath.Entities.Publisher;

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

		public async Task<IActionResult> LoggedIndex(bool showBookAlreadyInDbPrompt = false)
		{
			int yearChallenge = 0;
			int booksInChallenge = 0;

			var user = await _userManager.GetUserAsync(HttpContext.User);
			LoggedIndexModel model = new LoggedIndexModel();
			model.UserName = user.UserName;

			if(showBookAlreadyInDbPrompt == true)
			{
				model.ShowBookAlreadyInDbPrompt = true;
			}

			using(var context = new ApplicationDbContext(_options))
			{
				model.NowReadingBooks = context.BookActions
					.Include(ba => ba.Edition)
					.Include(ba => ba.Edition.Book)
					.Include(ba => ba.Edition.Book.Genre)
					.Include(ba => ba.Edition.Book.Author)
					.Where(ba => ba.User == user.Id && ba.DateFinished == null)
					.Select(ba => new NowReadingBook
					{
						ActionId = ba.Id,
						Title = ba.Edition.Book.Title,
						Author = ba.Edition.Book.Author.Name,
						Genre = ba.Edition.Book.Genre.Name,
						Type = ba.Edition.Type.ToString(),
						startDate = ba.DateStarted,
						Pages = ba.Edition.Pages,
						Duration = ba.Edition.Duration

					})
					.ToList();

				var years = context.BookActions
					.Where(ba => ba.User == user.Id 
							&& !context.YearBooks.Any(yb => yb.User == user.Id && yb.Year == ba.DateFinished.Value.Year) 
							&& ba.DateFinished.HasValue)
					.Select(ba => ba.DateFinished.Value.Year)
					.Distinct()
					.OrderBy(y => y)
					.ToList();

				foreach (int year in years)
				{
					var monthsWithBook = context.BookActions 
						.Where(ba => ba.User == user.Id && ba.DateFinished.Value.Year == year)
						.Select(ba => ba.DateFinished.Value.Month)
						.Distinct()
						.ToList();

					var finishedMonths = context.MonthBooks
							.Where(mb => mb.User == user.Id && mb.Year == year)
							.Select(mb => mb.Month)
							.ToList();

					if(monthsWithBook.Count > 3 && monthsWithBook.Except(finishedMonths).IsNullOrEmpty() && model.YearToClose == null && year < DateTime.Now.Year)
					{
						model.YearToClose = year;
					}

					if (monthsWithBook.Except(finishedMonths).Any())
					{
						
						model.MonthToClose = monthsWithBook.Except(finishedMonths).Min();
						model.YearOfMonthToClose = year;

						if(model.MonthToClose == DateTime.Now.Month && year == DateTime.Now.Year)
						{
							model.MonthToClose = null;
							model.YearOfMonthToClose = null;
						}
						break;
					}



				}
				int currentYear = DateTime.Now.Year;

				bool congratsShowed = context.YearChallenges.Where(yc => yc.User == user.Id && yc.Year == currentYear).Select(yc => yc.CongratsShowed).FirstOrDefault();

				if (!congratsShowed)
				{
					yearChallenge = context.YearChallenges.Where(yc => yc.User == user.Id && yc.Year == currentYear).Select(yc => yc.Count).FirstOrDefault();
					booksInChallenge = context.BookActions.Where(ba => ba.User == user.Id && ((DateTime)ba.DateFinished).Year.ToString() == currentYear.ToString()).Count();
				}

				if(!congratsShowed && yearChallenge != 0 && booksInChallenge !=0 && booksInChallenge >= yearChallenge )
				{
					model.ShowCongrats = true;
				}

				if(context.UpdatePromptSeen.Where(ups => ups.UserId == user.Id).Any() == false)
				{
					model.ShowUpdate = true;
				}
				else
				{
					model.ShowUpdate = false;
				}

				return View(model);
			}

		}

		[HttpPost]
		[Route("Authorized/CongratsShowed")]
		public async Task<IActionResult> CongratsShowed()
		{
			using(var context = new ApplicationDbContext(_options))
			{
				int currentYear = DateTime.Now.Year;
				var user = await _userManager.GetUserAsync(HttpContext.User);
				var yearChallenge = new YearChallenge();

				yearChallenge = await context.YearChallenges.Where(yc => yc.User == user.Id && yc.Year == currentYear).FirstOrDefaultAsync();
				if (yearChallenge != null)
				{
					yearChallenge.CongratsShowed = true;
					context.Update(yearChallenge);
					await context.SaveChangesAsync();
					var data = new { Message = "Success!" };
					return Json(data);
				}
				else
				{
					var data = new { Message = "YearChallenge not found!" };
					return Json(data);
				}
			}
		}

		[HttpPost]
		[Route("Authorized/UpdateShowed")]
		public async Task<IActionResult> UpdateShowed()
		{
			using (var context = new ApplicationDbContext(_options))
			{
				var user = await _userManager.GetUserAsync(HttpContext.User);
				UpdatePromptSeen seen = new();
				seen.UserId = user.Id;
				context.UpdatePromptSeen.Add(seen);
				await context.SaveChangesAsync();
				var data = new { Message = "Success!" };
				return Ok();
			}
		}

		[HttpGet]
		public async Task<IActionResult> Search(string query)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			if (query == null)
			{
				query = "";
			}
			using (var context = new ApplicationDbContext(_options))
			{
				List<Book> books = context.Books
					.Include(b => b.Author)
					.Include(b => b.Genre)
					.Where(b => b.Title.Contains(query) && (b.IsAccepted == true || b.AddedBy == user.Id))
					.ToList();
				List<SearchModel> modelList = new List<SearchModel>();

				foreach(Book book in books)
				{
					bool paper = false;
					bool audiobook = false;
					bool ebook = false;

					List<Edition> editions = context.Editions
						.Where(e => e.Book == book && (e.isAccepted == true || e.AddedBy == user.Id))
						.ToList();
					foreach(Edition edition in editions)
					{
						if(edition.Type == Entities.Type.PaperBook)
						{
							paper = true;
						}
						else if(edition.Type == Entities.Type.Audiobook)
						{
							audiobook = true;
						}
						else if(edition.Type == Entities.Type.Ebook)
						{
							ebook = true;
						}
					}
					SearchModel model = new SearchModel();
					model.Id = book.Id;
					model.Title = book.Title;
					model.Author = book.Author.Name;
					model.Genre = book.Genre.Name;
					model.Paper = paper;
					model.Audiobook = audiobook;
					model.Ebook = ebook;

					modelList.Add(model);
				}

				return View(modelList);
			}

		}

		[Route("{id}/BookDetails")]
		public async Task<IActionResult> BookDetails(int id)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			using (var context = new ApplicationDbContext(_options))
			{
				Book? book = context.Books
					.Include(b => b.Author)
					.Include(b => b.Genre)
					.Where(b => b.Id == id)
					.FirstOrDefault();

				BookDetailsModel model = new BookDetailsModel();
				model.Id = book.Id;
				model.Title = book.Title;
				model.Author = book.Author.Name;
				model.Genre = book.Genre.Name;
				model.Editions = context.Editions
					.Include(e => e.Publisher)
					.Where(e => e.Book == book && (e.isAccepted == true || e.AddedBy == user.Id))
					.Select(e => new EditionModel
					{
						Id = e.Id,
						Name = e.Publisher.Name,
						Pages = e.Pages,
						Duration = e.Duration,
						Type = e.Type,
						AddedBy = e.AddedBy,
						User = user.Id
					})
					.ToList();

				model.Actions = context.BookActions
					.Include(a => a.Edition.Publisher)
					.Where(a => a.Edition.Book == book && a.User == user.Id)
					.Select(a => new ActionModel
					{
						Id = a.Id,
						Type = a.Edition.Type.ToString(),
						Publisher = a.Edition.Publisher.Name,
						StartDate = (DateTime)a.DateStarted,
						FinishDate = a.DateFinished,
						Rating = a.Rating
					})
					.ToList();

				if(User.IsInRole("Admin") || User.IsInRole("Editor") || context.Books.Find(id).AddedBy == user.Id)
				{
					model.CanEdit = true;
				}
				else
				{
					model.CanEdit = false;
				}

                if ((User.IsInRole("Admin") || User.IsInRole("Editor")) && !context.BookActions.Where(ba => ba.Edition.Book.Id == id).Any())
				{
					model.CanDelete = true;
				}
				else
				{
					model.CanDelete = false;
				}

				return View(model);
			}
		}

		public async Task<IActionResult> EditBook(int id)
		{
			EditBookModel model = new EditBookModel();
			using(var context = new ApplicationDbContext(_options))
			{
				model = await context.Books.Include(b => b.Author).Include(b => b.Genre).Where(b => b.Id == id).Select(b => new EditBookModel
				{
					BookId = b.Id,
					Title = b.Title,
					Author = b.Author.Name,
					Genre = b.Genre.Name
				}).FirstOrDefaultAsync();
				model.AuthorList = await context.Authors.ToListAsync();
				model.GenreList = await context.Genres.ToListAsync();
				model.PublisherList = await context.Publishers.ToListAsync();

				return View(model);
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditBook(EditBookModel model)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			if (!ModelState.IsValid)
			{
				AddNewBookToViewModel invalidModel = new AddNewBookToViewModel();
				using (var context = new ApplicationDbContext(_options))
				{
					model.AuthorList = context.Authors.ToList();
					model.GenreList = context.Genres.ToList();
					model.PublisherList = context.Publishers.ToList();
					return View(model);
				}
			}

			using (var context = new ApplicationDbContext(_options))
			{
				Book book = context.Books.Find(model.BookId);
				Author author = context.Authors.FirstOrDefault(a => a.Name == model.Author);
				Genre genre = context.Genres.FirstOrDefault(g => g.Name == model.Genre);

				if(model.AuthorRadios == "changingAuthor")
				{
					if (author == null)
					{
						Author newAuthor = new Author();
						newAuthor.Name = model.Author;
						newAuthor.AddedBy = user.Id;
						context.Add(newAuthor);
						author = newAuthor;
					}
				}
				else
				{
					author = context.Authors.FirstOrDefault(a => a.Name == model.OldAuthor);
					author.Name = model.Author;
					context.Authors.Update(author);
				}

				if(model.GenreRadios == "changingGenre")
				{
					if (genre == null)
					{
						Genre newGenre = new Genre();
						newGenre.Name = model.Genre;
						newGenre.AddedBy = user.Id;
						context.Add(newGenre);
						genre = newGenre;
					}
				}
				else
				{
					genre = context.Genres.FirstOrDefault(g => g.Name == model.OldGenre);
					genre.Name = model.Genre;
					context.Genres.Update(genre);
				}

				book.Title = model.Title;
				book.Author = author;
				book.Genre = genre;

				context.Books.Update(book);
				context.SaveChanges();

				return RedirectToAction("BookDetails", new { id = model.BookId });
			}
		}

		public async Task<IActionResult> DeleteBook(int bookId)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			using(var context = new ApplicationDbContext(_options))
			{
				if(!context.BookActions.Where(ba => ba.Edition.Book.Id == bookId).Any())
				{
					var book = context.Books.Find(bookId);
					var editions = context.Editions.Where(e => e.Book.Id == bookId).ToList();

					context.Editions.RemoveRange(editions);
					context.Books.Remove(book);
					context.SaveChanges();
				}
				return RedirectToAction(nameof(LoggedIndex));
			}

		}

		public IActionResult AddNewBook()
		{
			AddNewBookModel model = new AddNewBookModel();
			using (var context = new ApplicationDbContext(_options))
			{
				model.AuthorList = context.Authors.ToList();
				model.GenreList = context.Genres.ToList();
				model.PublisherList = context.Publishers.ToList();
				return View(model);
			}
		}


		[HttpPost]
		public async Task<IActionResult> AddNewBook(AddNewBookModel model)
		{
			if (!ModelState.IsValid)
			{
				AddNewBookToViewModel invalidModel = new AddNewBookToViewModel();
				using (var context = new ApplicationDbContext(_options))
				{
					model.AuthorList = context.Authors.ToList();
					model.GenreList = context.Genres.ToList();
					model.PublisherList = context.Publishers.ToList();
					return View(model);
				}
			}

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
				if(User.IsInRole("Admin") || User.IsInRole("Editor"))
				{
					NewBook.IsAccepted = true;
				}
				else
				{
					NewBook.IsAccepted = false;
				}

				context.Add(NewBook);

				if(context.Books.Where(b => b.Title == NewBook.Title && b.Author == NewBook.Author).Any())
				{
					return RedirectToAction("LoggedIndex", new {showBookAlreadyInDbPrompt = true});
				}

				Publisher publisher = context.Publishers.FirstOrDefault(p => p.Name == model.Publisher);
				if(model.Publisher != null)
				{
					if (publisher == null)
					{
						Publisher newPublisher = new Publisher();
						newPublisher.Name = model.Publisher;
						newPublisher.AddedBy = user.Id;
						context.Add(newPublisher);
						publisher = newPublisher;
					}
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
					NewEdition.Duration = model.DurationH * 60 + model.DurationM; ;
				}
				NewEdition.AddedBy = user.Id;
				NewEdition.Publisher = publisher;

				if (User.IsInRole("Admin") || User.IsInRole("Editor"))
				{
					NewEdition.isAccepted = true;
				}
				else
				{
					NewEdition.isAccepted = false;
				}

				context.Add(NewEdition);


				await context.SaveChangesAsync();

				if(model.toMyBooks == "false")
				{
					return RedirectToAction(nameof(LoggedIndex));
				}
				else
				{
					return RedirectToAction("AddToMyBooks", new {bookId = NewBook.Id, editionId = NewEdition.Id});
				}
			}
		}

		[Route("{id}/AddNewEdition")]
		public IActionResult AddNewEdition(int id)
		{
			AddNewBookModel model = new AddNewBookModel();
			using (var context = new ApplicationDbContext(_options))
			{
				model.PublisherList = context.Publishers.ToList();
				model.BookId = id;
				model.Title = context.Books.Find(id).Title;
				return View(model);
			}
		}

		[Route("{id}/AddNewEdition")]
		[HttpPost]
		public async Task<IActionResult> AddNewEdition(AddNewEditionModel model)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			using (var context = new ApplicationDbContext(_options))
			{
				Publisher publisher = context.Publishers.FirstOrDefault(p => p.Name == model.Publisher);
				if(model.Publisher != null)
				{
					if (publisher == null)
					{
						Publisher newPublisher = new Publisher();
						newPublisher.Name = model.Publisher;
						newPublisher.AddedBy = user.Id;
						context.Add(newPublisher);
						publisher = newPublisher;
					}
				}

				Book book = context.Books.Find(model.BookId);

				Edition NewEdition = new Edition();
				NewEdition.Book = book;
				if (model.Type == "Książka papierowa")
				{
					NewEdition.Type = Entities.Type.PaperBook;
					NewEdition.Pages = model.Pages;
				}
				else if (model.Type == "Ebook")
				{
					NewEdition.Type = Entities.Type.Ebook;
					NewEdition.Pages = model.Pages;
				}
				else if (model.Type == "Audiobook")
				{
					NewEdition.Type = Entities.Type.Audiobook;
					NewEdition.Duration = model.DurationH*60 + model.DurationM;
				}
				NewEdition.AddedBy = user.Id;
				NewEdition.Publisher = publisher;
				context.Add(NewEdition);

				await context.SaveChangesAsync();
			
				return RedirectToAction("BookDetails", new {id = model.BookId});
			}
		}

		[HttpGet]
		[Route("{id}/UpdateEdition")]
		public async Task<IActionResult> UpdateEdition(int editionId)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			UpdateEditionViewModel model = new();
			using (var context = new ApplicationDbContext(_options))
			{
				model.Edition = context.Editions.Include(e => e.Book).FirstOrDefault(e => e.Id == editionId);
				model.PublisherList = context.Publishers.ToList();
				model.BookId = model.Edition.Book.Id;
				model.Title = context.Books.Find(model.BookId).Title;
			}

			if(model.Edition.AddedBy != user.Id)
			{
				return RedirectToAction("BookDetails", new { id = model.BookId });
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateEdition(AddNewEditionModel model, int editionId)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			using (var context = new ApplicationDbContext(_options))
			{
				Publisher publisher = context.Publishers.FirstOrDefault(p => p.Name == model.Publisher);
				if (model.Publisher != null)
				{
					if (publisher == null)
					{
						Publisher newPublisher = new Publisher();
						newPublisher.Name = model.Publisher;
						newPublisher.AddedBy = user.Id;
						context.Add(newPublisher);
						publisher = newPublisher;
					}
				}

				Book book = context.Books.Find(model.BookId);

				Edition existingEdition = context.Editions.Find(editionId);
				existingEdition.Book = book;
				if (model.Type == "Książka papierowa")
				{
					existingEdition.Type = Entities.Type.PaperBook;
					existingEdition.Pages = model.Pages;
				}
				else if (model.Type == "Ebook")
				{
					existingEdition.Type = Entities.Type.Ebook;
					existingEdition.Pages = model.Pages;
				}
				else if (model.Type == "Audiobook")
				{
					existingEdition.Type = Entities.Type.Audiobook;
					existingEdition.Duration = model.DurationH*60 + model.DurationM;
				}
				existingEdition.AddedBy = user.Id;
				existingEdition.Publisher = publisher;
				context.Update(existingEdition);
				await context.SaveChangesAsync();
			}

			return RedirectToAction("BookDetails", new { id = model.BookId });
		}

		[Route("{bookId}/{editionId}/AddToMyBooks")]
		public async Task<IActionResult> AddToMyBooks(int bookId, int editionId)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			using (var context = new ApplicationDbContext(_options))
			{
				AddToMyBooksModel model = new AddToMyBooksModel();
				model.book = context.Books.Find(bookId);
				model.bookId = bookId;
				model.editionId = editionId;
				model.TBRs = context.TBRs.Where(t => t.User == user.Id).ToList();

				return View(model);
			}
		}

		[HttpPost]
		public async Task<IActionResult> AddToMyBooks(AddToMyBooksModel model)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			using (var context = new ApplicationDbContext(_options))
			{
				Edition edition = context.Editions.Find(model.editionId);
				BookAction bookAction = new BookAction();
				bookAction.Edition = edition;
				bookAction.User = user.Id;
				if(model.status == "Czytam" ||  model.status == "Przeczytano")
				{
					bookAction.DateStarted = model.startDate.Date;
				}

				if (model.status == "Przeczytano")
				{
					bookAction.DateFinished = model.finishDate.Date;
					bookAction.Rating = float.Parse(model.rating, CultureInfo.InvariantCulture);
					if (model.comment != null)
					{
						bookAction.Opinion = model.comment.Replace("\n", "<br>");
					}
					else
					{
						bookAction.Opinion = "";
					}

				}
				if (model.status == "Do przeczytania")
				{
					var tbrBook = new TBRBook();
					tbrBook.Title = context.Editions.Where(e => e.Id == edition.Id).Select(e => e.Book.Title).FirstOrDefault();
					tbrBook.LinkedEdition = edition;
					tbrBook.TBR = context.TBRs.Find(model.Tbr);
					context.Add(tbrBook);
					await context.SaveChangesAsync();
					return RedirectToAction("LoggedIndex");
				}
				context.Add(bookAction);
				await context.SaveChangesAsync();

				return RedirectToAction("LoggedIndex");
			}
		}

		[HttpPost]
		public async Task<IActionResult> DeleteFromMyBooks(int id)
		{
			using (var context = new ApplicationDbContext(_options))
			{
				BookAction actionToDelete = context.BookActions.Find(id);
				context.BookActions.Remove(actionToDelete);
				await context.SaveChangesAsync();
				return RedirectToAction("LoggedIndex");
			}
		}

		[HttpGet]
		[Route("{actionId}/AddCommentWhileReading")]
		public async Task<IActionResult> AddCommentWhileReading(int actionId)
		{
			AddCommentWhileReadingModel model = new();
			using (var context = new ApplicationDbContext(_options))
			{
				model.Id = actionId;

				var bookAction = context.BookActions
				.Include(ba => ba.Edition.Book)
				.FirstOrDefault(ba => ba.Id == actionId);

				model.Title = bookAction.Edition.Book.Title;
				if(bookAction.Opinion != null)
				{
					model.Comment = bookAction.Opinion.Replace("<br>","");
				}
				else
				{
					model.Comment = "";
				}
			}

				return View("AddComment", model);
		}

		[HttpPost]
		[Route("{actionId}/AddCommentWhileReadingPatch")]
		public async Task<IActionResult> AddCommentWhileReadingPatch(AddCommentWhileReadingModel model)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			using (var context = new ApplicationDbContext(_options))
			{
				BookAction bookAction = await context.BookActions.SingleOrDefaultAsync(ba => ba.User == user.Id && ba.Id == model.Id);
				if (model.Comment != null)
				{
					bookAction.Opinion = model.Comment.Replace("\n", "<br>");
				}
				else
				{
					bookAction.Opinion = "";
				}
				context.Update(bookAction);
				await context.SaveChangesAsync();
			}
			return RedirectToAction("LoggedIndex");
		}

		[Route("{actionId}/FinishBook")]
		public IActionResult FinishBook(int actionId)
		{
			using (var context = new ApplicationDbContext(_options))
			{
				FinishBookViewModel model = new FinishBookViewModel();
				model.Id = actionId;
				var bookAction = context.BookActions
					.Include(ba => ba.Edition.Book)
					.FirstOrDefault(ba => ba.Id == actionId);

				model.Title = bookAction.Edition.Book.Title;
				model.StartDate = (DateTime)bookAction.DateStarted;
				//model.Comment = bookAction.Opinion;
				if (bookAction.Opinion != null)
				{
					model.Comment = bookAction.Opinion.Replace("<br>", "");
				}
				else
				{
					model.Comment = "";
				}


				return View(model);
			}
		}

		[HttpPost]
		public async Task<IActionResult> FinishBook(FinishBookModel finishBook)
		{
			using (var context = new ApplicationDbContext(_options))
			{
				BookAction bookAction = context.BookActions.Find(finishBook.Id);

				bookAction.DateStarted = finishBook.StartDate;
				bookAction.DateFinished = finishBook.FinishDate.Date + DateTime.Now.TimeOfDay;
				bookAction.Rating = float.Parse(finishBook.Rating, CultureInfo.InvariantCulture);
				if (finishBook.Comment != null)
				{
					bookAction.Opinion = finishBook.Comment.Replace("\n", "<br>");
				}
				else
				{
					bookAction.Opinion = "";
				}

				context.Update(bookAction);
				await context.SaveChangesAsync();
				return RedirectToAction("LoggedIndex");
			}
		}

		public async Task<IActionResult> AllReadBooks()
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			using (var context = new ApplicationDbContext(_options))
			{
				List<AllReadBooksModel> model = context.BookActions
					.Include(ba => ba.Edition.Book)
					.Include(ba => ba.Edition.Book.Author)
					.Include(ba => ba.Edition.Book.Genre)
					.Include(ba => ba.Edition.Publisher)
					.Where(ba => ba.User == user.Id)
					.Select(ba => new AllReadBooksModel
					{
						Id = ba.Id,
						BookId = ba.Edition.Book.Id,
						Title = ba.Edition.Book.Title,
						Author = ba.Edition.Book.Author.Name,
						Genre = ba.Edition.Book.Genre.Name,
						Rating = ba.Rating,
						StartDate = (DateTime)ba.DateStarted,
						FinishDate = ba.DateFinished,
						Blocked = context.MonthBooks.Where(mb => mb.BestBook.Id == ba.Id || mb.WorstBook.Id == ba.Id).Any()
					})
					.ToList();
				return View(model);
			}
		}

		public async Task<IActionResult> AllMyPublishers()
		{
            var user = await _userManager.GetUserAsync(HttpContext.User);

			using (var context = new ApplicationDbContext(_options))
			{
				List<AllMyPublishersModel> model = context.Publishers
					.Select(p => new AllMyPublishersModel
					{
						Id = p.Id,
						Name = p.Name,
						ReadCount = context.BookActions
							.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == p.Id && ba.DateFinished != null)
							.Count(),
						AudiobookCount = context.BookActions
							.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == p.Id && ba.DateFinished != null && ba.Edition.Type == Entities.Type.Audiobook)
							.Count(),
						PaperbookCount = context.BookActions
							.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == p.Id && ba.DateFinished != null && ba.Edition.Type == Entities.Type.PaperBook)
							.Count(),
						EbookCount = context.BookActions
							.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == p.Id && ba.DateFinished != null && ba.Edition.Type == Entities.Type.Ebook)
							.Count(),
					})
					.ToList();

				model.RemoveAll(m => m.ReadCount == 0);
				return View(model);
			}
		}


		[Route("{actionId}/ActionDetails")]
		public IActionResult BookActionDetails(int actionId)
		{
			using(var context = new ApplicationDbContext(_options))
			{
				BookActionDetailsModel model = context.BookActions
					.Include(ba => ba.Edition.Book)
					.Include(ba => ba.Edition.Book.Author)
					.Include(ba => ba.Edition.Book.Genre)
					.Include(ba => ba.Edition.Publisher)
					.Where(ba => ba.Id == actionId)
					.Select(ba => new BookActionDetailsModel
					{
						Title = ba.Edition.Book.Title,
						Author = ba.Edition.Book.Author.Name,
						Genre = ba.Edition.Book.Genre.Name,
						Publisher = ba.Edition.Publisher.Name,
						Type = ba.Edition.Type.ToString(),
						Pages = ba.Edition.Pages,
						Duration = ba.Edition.Duration,
						StartDate = (DateTime)ba.DateStarted,
						FinishDate = ba.DateFinished,
						Comment = ba.Opinion,
						Rating = ba.Rating
					})
					.FirstOrDefault();                
				return View(model);
			}
		}

		public async Task<IActionResult> PublisherDetails(int publisherId)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			
			using (var context = new ApplicationDbContext(_options))
			{
				PublisherDetailsModel model = new();

				model.Name = context.Publishers.Find(publisherId).Name;
				model.ReadCount = context.BookActions
					.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == publisherId && ba.DateFinished != null)
					.Count();
				model.AudiobookCount = context.BookActions
					.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == publisherId && ba.DateFinished != null && ba.Edition.Type == Entities.Type.Audiobook)
					.Count();
				model.PaperbookCount = context.BookActions
					.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == publisherId && ba.DateFinished != null && ba.Edition.Type == Entities.Type.PaperBook)
					.Count();
				model.EbookCount = context.BookActions
					.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == publisherId && ba.DateFinished != null && ba.Edition.Type == Entities.Type.Ebook)
					.Count();

				model.RatingAvg = context.BookActions
					.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == publisherId && ba.DateFinished != null)
					.Average(a => a.Rating ?? 0); //do sprawdzenia


				model.AllReadOfPublisher = context.BookActions
					.Where(ba => ba.User == user.Id && ba.Edition.Publisher.Id == publisherId && ba.DateFinished != null)
					.Include(ba => ba.Edition.Book.Author)
					.Include(ba => ba.Edition.Book.Genre)
					.Select(ba => new ReadPublisher
					{
						BookActionId = ba.Id,
						Title = ba.Edition.Book.Title,
						Author = ba.Edition.Book.Author.Name,
						Genre = ba.Edition.Book.Genre.Name,
						Rating = ba.Rating ?? 0
					})
					.ToList();

				model.AllBooksOfPublisher = context.Editions
					.Where(e => e.Publisher.Id == publisherId)
					.Include(e => e.Book.Author)
					.Include(e => e.Book.Genre)
					.Select(ba => new BookPublisher
					{
						BookId = ba.Book.Id,
						EditionId = ba.Id,
						Title = ba.Book.Title,
						Author = ba.Book.Author.Name,
						Genre = ba.Book.Genre.Name,
						Type = ba.Type
					})
					.ToList();
				return View(model);
			}

		}

		[Route("Challenge/{year}")]
		public async Task<IActionResult> Challenge(string year)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			using (var context = new ApplicationDbContext(_options))
			{
				ChallengeModel model = new ChallengeModel();
				model.YearChallenge = context.YearChallenges
					.Where(yc => yc.User == user.Id && yc.Year.ToString() == year)
					.FirstOrDefault();

				model.BooksInChallenge = context.BookActions
					.Where(ba => ba.User == user.Id && ((DateTime)ba.DateFinished).Year.ToString() == year)
					.Select(ba => new BookInChallenge
					{
						BookActionId = ba.Id,
						Rating = (float)ba.Rating,
						FinishDate = (DateTime)ba.DateFinished
					})
					.ToList();


				if (model.YearChallenge == null)
				{
					model.YearChallenge = new YearChallenge();
					model.YearChallenge.User = user.Id;
					model.YearChallenge.Year = int.Parse(year);
				}

				model.ChallengeColors = context.ChallengeColors
					.Where(cc => cc.UserId == user.Id)
					.FirstOrDefault();

				model.IsPrevYearAvailable = context.YearChallenges.Any(yc => yc.User == user.Id && yc.Year == model.YearChallenge.Year - 1);
                model.IsNextYearAvailable = model.YearChallenge.Year + 1 == DateTime.Now.Year || context.YearChallenges.Any(yc => yc.User == user.Id && yc.Year == model.YearChallenge.Year + 1);

				return View("Challenge", model);
			}
		}

		[Route("AddChallenge/{year}")]
		public IActionResult AddChallenge(string year)
		{
			return View("AddChallenge", year);
		}

		[HttpPost]
		public async Task<IActionResult> AddChallenge(YearChallenge model)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			YearChallenge challenge = new YearChallenge();
			challenge.Year = model.Year;
			challenge.Count = model.Count;
			challenge.User = user.Id;

			using (var context = new ApplicationDbContext(_options))
			{
				bool challengeExists = context.YearChallenges.Any(yc => yc.User == user.Id && yc.Year == model.Year);
				if (challengeExists)
				{
					YearChallenge existingChallenge = await context.YearChallenges.SingleOrDefaultAsync(yc => yc.User == user.Id && yc.Year == model.Year);
					existingChallenge.Count = model.Count;
					context.Update(existingChallenge);
				}
				else
				{
					context.Add(challenge);
				}
				await context.SaveChangesAsync();
			}

			return RedirectToAction("Challenge", new { year = model.Year });
		}

		public async Task<IActionResult> SetChallengeColors()
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			using(var context = new ApplicationDbContext(_options))
			{
				ChallengeColors model = new ChallengeColors();
				model = context.ChallengeColors
					.Where(cc => cc.UserId == user.Id)
					.FirstOrDefault();
				return View(model);
			}            
		}

		[HttpPost]
		public async Task<IActionResult> SetChallengeColors(ChallengeColors model)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			ChallengeColors challengeColors = model;
			challengeColors.UserId = user.Id;

			using (var context = new ApplicationDbContext(_options))
			{
				bool challengeColorsExists = context.ChallengeColors.Any(cc => cc.UserId == user.Id);

				if (challengeColorsExists)
				{
					ChallengeColors existingChallengeColors = await context.ChallengeColors.SingleOrDefaultAsync(cc => cc.UserId == user.Id);
					existingChallengeColors.ColorForOne = model.ColorForOne;
					existingChallengeColors.ColorForTwo = model.ColorForTwo;
					existingChallengeColors.ColorForThree = model.ColorForThree;
					existingChallengeColors.ColorForFour = model.ColorForFour;
					existingChallengeColors.ColorForFive = model.ColorForFive;
					existingChallengeColors.ColorForNoRating = model.ColorForNoRating;
					
					context.Update(existingChallengeColors);
				}
				else
				{
					context.Add(challengeColors);
				}

				await context.SaveChangesAsync();
			}
			return RedirectToAction("Challenge", new { year = DateTime.Now.Year.ToString() });
		}

		[Route("Statistics/{year}")]
		public async Task<IActionResult> Statistics(string year)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			StatisticsModel model = new StatisticsModel();
			model.year = year;
			List<string> months = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };

			using (var context = new ApplicationDbContext(_options))
			{
				var finishedBooks = context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id)
					.ToList();

				var finishedMonths = finishedBooks
					.Select(a => a.DateFinished.Value.Month.ToString("D2"))
					.Distinct()
					.ToList();

				model.months = months.Select(month => finishedMonths.Contains(month)).ToList();

				model.IsPrevYearAvailable = context.BookActions.Any(a => a.User == user.Id && a.DateFinished.Value.Year == int.Parse(year) - 1);
				model.IsNextYearAvailable = context.BookActions.Any(a => a.User == user.Id && a.DateFinished.Value.Year == int.Parse(year) + 1);
			}

			return View("Statistics", model);
		}

		[Route("Statistics/{year}/{month}")]
		public async Task<IActionResult> MonthStatistics(string month, string year)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			MonthStatisticsModel model = new MonthStatisticsModel();
			model.Month = month;
			model.Year = year;

			string prevMonth, prevYear;

			if((int.Parse(month)-1) == 0)
			{
				prevMonth = "12";
				prevYear = (int.Parse(year) - 1).ToString();
			}
			else
			{
				prevMonth = (int.Parse(month) - 1).ToString();
				prevYear = year;
			}

			using (var context = new ApplicationDbContext(_options))
			{
				if(!context.BookActions.Any(ba => ba.DateFinished.Value.Month == int.Parse(month) && ba.DateFinished.Value.Year == int.Parse(year)))
				{
					return RedirectToAction(nameof(Statistics), new {year = DateTime.Now.Year});
				}

				model.BookCount = context.BookActions
					.Count(a => a.User == user.Id && a.DateFinished != null && a.DateFinished.Value.Month.ToString() == month && a.DateFinished.Value.Year.ToString() == year);
				 
				model.PrevMonthBookCount = context.BookActions
					.Count(a => a.User == user.Id && a.DateFinished != null && a.DateFinished.Value.Month.ToString() == prevMonth && a.DateFinished.Value.Year.ToString() == year);

				model.YearChallengeCount = (int)(context.YearChallenges
						.SingleOrDefault(c => c.User == user.Id && c.Year.ToString() == year)?
						.Count ?? 0);

				model.PaperBooksCount = await context.BookActions
						.CountAsync(a => a.User == user.Id && a.DateFinished != null
							&& a.DateFinished.Value.Month.ToString() == month
							&& a.DateFinished.Value.Year.ToString() == year
							&& a.Edition.Type == Entities.Type.PaperBook);
				
				model.EbooksCount = await context.BookActions
						.CountAsync(a => a.User == user.Id && a.DateFinished != null
							&& a.DateFinished.Value.Month.ToString() == month
							&& a.DateFinished.Value.Year.ToString() == year
							&& a.Edition.Type == Entities.Type.Ebook);
				
				model.AudiobooksCount = await context.BookActions
						.CountAsync(a => a.User == user.Id && a.DateFinished != null
							&& a.DateFinished.Value.Month.ToString() == month
							&& a.DateFinished.Value.Year.ToString() == year
							&& a.Edition.Type == Entities.Type.Audiobook);

				model.PagesCount = await context.BookActions
						.Where(a => a.DateFinished != null
							&& a.DateFinished.Value.Month.ToString() == month
							&& a.DateFinished.Value.Year.ToString() == year
							&& (a.Edition.Type == Entities.Type.Ebook || a.Edition.Type == Entities.Type.PaperBook)
							&& a.User == user.Id)
						.SumAsync(a => a.Edition.Pages ?? 0);


				model.AudiobooksMinutes = await context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Month.ToString() == month
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.Edition.Type == Entities.Type.Audiobook
						&& a.User == user.Id)
					.SumAsync(a => a.Edition.Duration ?? 0);

				model.RatingAverage = await context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Month.ToString() == month
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id
						&& a.Rating != 0)
					.AverageAsync(a => a.Rating ?? 0);


				var bookActions = await context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Month.ToString() == prevMonth
						&& a.DateFinished.Value.Year.ToString() == prevYear
						&& a.User == user.Id
						&& a.Rating != 0)
					.ToListAsync();

				model.PrevMonthRatingAverage = (float)bookActions
					.Select(a => a.Rating)
					.DefaultIfEmpty(0)
					.Average();

				model.FavouriteGenre = context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Month.ToString() == month
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id)
					.GroupBy(a => a.Edition.Book.Genre.Name)
					.Select(group => new
					{
						GenreName = group.Key,
						Count = group.Count()
					})
					.OrderByDescending(g => g.Count)
					.FirstOrDefault()?.GenreName ?? "Brak ulubionego gatunku";

				model.Genres = context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Month.ToString() == month
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id)
					.GroupBy(a => a.Edition.Book.Genre.Name)
					.Select(group => new GenreWithCount
					{
						Name = group.Key,
						Count = group.Count()
					})
					.ToList();

				model.Books = context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Month.ToString() == month
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id)
					.Select(a => new BookWithRating
					{
						Title = a.Edition.Book.Title,
						Type = a.Edition.Type,
						Rating = a.Rating ?? 0
					})
					.ToList();

				var monthBooks = context.MonthBooks
					.Include(mb => mb.BestBook.Edition.Book)
					.Include(mb => mb.WorstBook.Edition.Book)
					.FirstOrDefault(a => a.User == user.Id && a.Year.ToString() == year && a.Month.ToString() == month);

				model.Publishers = context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Month.ToString() == month
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id)
					.GroupBy(a => a.Edition.Publisher.Name)
					.Select(group => new PublisherWithCount
					{
						Name = group.Key,
						Count = group.Count()
					})
					.ToList();

				model.Genres = model.Genres.OrderByDescending(g => g.Count).ToList();
				model.Books = model.Books.OrderByDescending(b => b.Rating).ToList();
				model.Publishers = model.Publishers.OrderByDescending(p => p.Count).ToList();

				foreach(var book in model.Books)
				{
					if(book.Type == Entities.Type.PaperBook)
					{
						book.StringType = "Książka papierowa";
					}
					else if(book.Type == Entities.Type.Audiobook)
					{
						book.StringType = "Audiobook";
					}
					else
					{
						book.StringType = "Ebook";
					}
				}

				model.BestBook = monthBooks?.BestBook?.Edition?.Book?.Title;
				model.WorstBook = monthBooks?.WorstBook?.Edition?.Book?.Title;
			}

			return View("MonthStatistics", model);
		}

		[Route("Statistics/{year}/Details")]
		public async Task<IActionResult> YearStatisticsDetails(string year)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			YearStatisticsModel model = new ();
			model.Year = year;

			string prevYear = (int.Parse(year) - 1).ToString();


			using (var context = new ApplicationDbContext(_options))
			{
				if (!context.BookActions.Any(ba => ba.DateFinished.Value.Year == int.Parse(year)))
				{
					return RedirectToAction(nameof(Statistics), new { year = DateTime.Now.Year });
				}

				model.BookCount = context.BookActions
					.Count(a => a.User == user.Id && a.DateFinished != null && a.DateFinished.Value.Year.ToString() == year);

				model.PrevYearBookCount = context.BookActions
					.Count(a => a.User == user.Id && a.DateFinished != null && (a.DateFinished.Value.Year).ToString() == prevYear);

				model.YearChallengeCount = (int)(context.YearChallenges
						.SingleOrDefault(c => c.User == user.Id && c.Year.ToString() == year)?
						.Count ?? 0);

				model.PaperBooksCount = await context.BookActions
						.CountAsync(a => a.User == user.Id && a.DateFinished != null
							&& a.DateFinished.Value.Year.ToString() == year
							&& a.Edition.Type == Entities.Type.PaperBook);

				model.EbooksCount = await context.BookActions
						.CountAsync(a => a.User == user.Id && a.DateFinished != null
							&& a.DateFinished.Value.Year.ToString() == year
							&& a.Edition.Type == Entities.Type.Ebook);

				model.AudiobooksCount = await context.BookActions
						.CountAsync(a => a.User == user.Id && a.DateFinished != null
							&& a.DateFinished.Value.Year.ToString() == year
							&& a.Edition.Type == Entities.Type.Audiobook);

				model.PagesCount = await context.BookActions
						.Where(a => a.DateFinished != null
							&& a.DateFinished.Value.Year.ToString() == year
							&& (a.Edition.Type == Entities.Type.Ebook || a.Edition.Type == Entities.Type.PaperBook)
							&& a.User == user.Id)
						.SumAsync(a => a.Edition.Pages ?? 0);


				model.AudiobooksMinutes = await context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.Edition.Type == Entities.Type.Audiobook
						&& a.User == user.Id)
					.SumAsync(a => a.Edition.Duration ?? 0);

				model.RatingAverage = await context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id
						&& a.Rating != 0)
					.AverageAsync(a => a.Rating ?? 0);


				var bookActions = await context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Year.ToString() == prevYear
						&& a.User == user.Id
						&& a.Rating != 0)
					.ToListAsync();

				model.PrevYearRatingAverage = (float)bookActions
					.Select(a => a.Rating)
					.DefaultIfEmpty(0)
					.Average();

				model.FavouriteGenre = context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id)
					.GroupBy(a => a.Edition.Book.Genre.Name)
					.Select(group => new
					{
						GenreName = group.Key,
						Count = group.Count()
					})
					.OrderByDescending(g => g.Count)
					.FirstOrDefault()?.GenreName ?? "Brak ulubionego gatunku";

				model.Genres = context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id)
					.GroupBy(a => a.Edition.Book.Genre.Name)
					.Select(group => new GenreWithCount
					{
						Name = group.Key,
						Count = group.Count()
					})
					.ToList();

				model.Books = context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id)
					.Select(a => new BookWithRating
					{
						Title = a.Edition.Book.Title,
						Type = a.Edition.Type,
						Rating = a.Rating ?? 0
					})
					.ToList();

				var yearBooks = context.YearBooks
					.Include(yb => yb.BestBook.Edition.Book)
					.Include(yb => yb.WorstBook.Edition.Book)
					.FirstOrDefault(a => a.User == user.Id && a.Year.ToString() == year && a.Year.ToString() == year);

				model.Publishers = context.BookActions
					.Where(a => a.DateFinished != null
						&& a.DateFinished.Value.Year.ToString() == year
						&& a.User == user.Id)
					.GroupBy(a => a.Edition.Publisher.Name)
					.Select(group => new PublisherWithCount
					{
						Name = group.Key,
						Count = group.Count()
					})
					.ToList();

				model.Genres = model.Genres.OrderByDescending(g => g.Count).ToList();
				model.Books = model.Books.OrderByDescending(b => b.Rating).ToList();
				model.Publishers = model.Publishers.OrderByDescending(p => p.Count).ToList();

				foreach (var book in model.Books)
				{
					if (book.Type == Entities.Type.PaperBook)
					{
						book.StringType = "Książka papierowa";
					}
					else if (book.Type == Entities.Type.Audiobook)
					{
						book.StringType = "Audiobook";
					}
					else
					{
						book.StringType = "Ebook";
					}
				}

				model.BestBook = yearBooks?.BestBook?.Edition?.Book?.Title;
				model.WorstBook = yearBooks?.WorstBook?.Edition?.Book?.Title;

				model.MonthCounts = new();
				for(int i=1; i<=12; i++)
				{
					model.MonthCounts.Add(context.BookActions.Count(ba => ba.User == user.Id && ba.DateFinished.Value.Year == int.Parse(year) && ba.DateFinished.Value.Month == i));
				}

			}

			return View("YearStatistics", model);
		}

		[Route("FinishMonth/{month}/{year}")]
		public async Task<IActionResult> FinishMonth(string year, string month)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			var model = new FinishMonthModel();
			model.month = int.Parse(month);
			model.year = int.Parse(year);


			using (var context = new ApplicationDbContext(_options))
			{
				if(context.MonthBooks.Any(mb => mb.Month == int.Parse(month) && mb.Year == int.Parse(year) && mb.User == user.Id))
				{
					return RedirectToAction(nameof(MonthStatistics), new { year = year, month = month });
				}                    
					
				model.ReadBooks = context.BookActions
					.Include(ba => ba.Edition.Book)
					.Where(ba => ba.DateFinished.Value.Year == int.Parse(year) && ba.DateFinished.Value.Month == int.Parse(month) && ba.User == user.Id)
					.Select(ba => new ReadBook
					{
						BookActionId = ba.Id,
						BookTitle = ba.Edition.Book.Title,
						Rating = ba.Rating
					})
					.ToList();

				if(!model.ReadBooks.Any()) { return RedirectToAction(nameof(LoggedIndex)); }
			}

				return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> FinishMonthPost(int month, int year, int bestId, int worstId)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			using (var context = new ApplicationDbContext(_options))
			{
				var monthBook = new MonthBook();
				monthBook.Month = month;
				monthBook.Year = year;
				monthBook.BestBook = context.BookActions.Find(bestId);
				monthBook.WorstBook = context.BookActions.Find(worstId);
				monthBook.User = user.Id;

				context.Add(monthBook);
				await context.SaveChangesAsync();

			}
				return RedirectToAction(nameof(LoggedIndex));
		}

		[Route("FinishYear/{year}")]
		public async Task<IActionResult> FinishYear(string year)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			var model = new FinishYearModel();
			model.Year = int.Parse(year);

			using (var context = new ApplicationDbContext(_options))
			{
				model.WorstBooks = context.MonthBooks
					.Where(mb => mb.Year.ToString() == year && mb.User == user.Id)
					.Select(mb => new WorstBook
					{
						BookActionId = mb.WorstBook.Id,
						BookTitle = mb.WorstBook.Edition.Book.Title,
						Rating = mb.WorstBook.Rating,
					})
					.ToList();

				model.BestBooks = context.MonthBooks
					.Where(mb => mb.Year.ToString() == year && mb.User == user.Id)
					.Select(mb => new BestBook
					{
						BookActionId = mb.BestBook.Id,
						BookTitle = mb.BestBook.Edition.Book.Title,
						Rating = mb.BestBook.Rating,
					})
					.ToList();
			}

			return View(model);
		}

		public IActionResult UpdateLog()
		{
			return View();
		}

		[HttpGet]
		[Route("EditBookAction/{bookActionId}")]
		public IActionResult EditBookAction(int bookActionId)
		{
			using(var context = new ApplicationDbContext(_options))
			{
				if(context.MonthBooks.Where(mb => mb.WorstBook.Id == bookActionId || mb.BestBook.Id == bookActionId).Any())
				{
					return RedirectToAction(nameof(AllReadBooks));
				}

				EditBookActionViewModel model = new();

				model.BookAction = context.BookActions.Include(ba => ba.Edition.Book).FirstOrDefault(ba => ba.Id == bookActionId);
				model.Title = context.Books.Where(b => b.Id == model.BookAction.Edition.Book.Id).Select(b => b.Title).FirstOrDefault();

				if(model.BookAction.Opinion != null)
				{
					model.BookAction.Opinion = model.BookAction.Opinion.Replace("<br>", "");
				}

				return View(model);

			}
		}

		[HttpPost]
		public async Task<IActionResult> EditBookAction(EditBookActionModel model)
		{
			model.Rating = model.Rating ?? "0";
			using (var context = new ApplicationDbContext(_options))
			{
				BookAction bookAction = context.BookActions.Find(model.BookActionId);

				bookAction.DateStarted = model.StartDate;
				if(model.FinishDate != null && model.Status != "czytam")
				{
					bookAction.DateFinished = model.FinishDate.Date + DateTime.Now.TimeOfDay;
				}
				else
				{
					bookAction.DateFinished = null;
				}

				if(model.Status == "czytam")
				{
					bookAction.Rating = null;
				}
				else
				{
					bookAction.Rating = float.Parse(model.Rating, CultureInfo.InvariantCulture);
				}

				if (model.Comment != null)
				{
					bookAction.Opinion = model.Comment.Replace("\n", "<br>");
				}
				else
				{
					bookAction.Opinion = "";
				}

				context.Update(bookAction);
				await context.SaveChangesAsync();
				return RedirectToAction(nameof(AllReadBooks));
			}
		}

		public async Task<IActionResult> TBR()
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			using (var context = new ApplicationDbContext(_options))
			{
				var tbrs = context.TBRs.Where(t => t.User == user.Id).ToList();
                var model = new List<TBRModel>();

				foreach(var tbr in tbrs)
				{
					var oneForModel = new TBRModel();
					oneForModel.TBR = tbr;
					oneForModel.CountRead = context.TBRBooks.Where(t => t.TBR.Id == tbr.Id && t.IsRead).Count();
					oneForModel.CountToRead = context.TBRBooks.Where(t => t.TBR.Id == tbr.Id).Count();
					model.Add(oneForModel);
				}

				return View(model);
			}

		}

		public async Task<IActionResult> AddNewTBR()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddNewTBR(string title, DateTime deadline)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);

			using (var context = new ApplicationDbContext(_options))
			{
				TBR tbr = new TBR();
				tbr.Title = title;
				tbr.Deadline = deadline;
				tbr.User = user.Id;

				context.Add(tbr);
				await context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(TBR));
		}

		[Route("TBRDetails/{TBRId}")]
		public async Task<IActionResult> TBRDetails(int TBRId)
		{
            var user = await _userManager.GetUserAsync(HttpContext.User);

            using (var context = new ApplicationDbContext(_options))
            {
				var tbr = context.TBRs.Find(TBRId);

                var model = new TBRDetailsModel();
				model.TBRBooks = context.TBRBooks.Where(t => t.TBR == tbr).ToList();
				model.Title = tbr.Title;
				model.Deadline = tbr.Deadline.ToString("dd.MM.yyyy") ?? "0";
				model.TBRId = TBRId;
				model.IsFinished = tbr.IsFinished;
				return View(model);
            }
        }

		[HttpPost]
		[Route("Authorized/AddNewTBRBook")]
		public async Task<IActionResult> AddNewTBRBook([FromBody] AddTBRBookModel model)
		{
			using (var context = new ApplicationDbContext(_options))
			{
				var tbrBook = new TBRBook();
				tbrBook.Title = model.Title;
				tbrBook.TBR = context.TBRs.Find(model.TBRId);

				context.TBRBooks.Add(tbrBook);
				await context.SaveChangesAsync();
			}

			return Ok();
		}

		[HttpPost]
		[Route("Authorized/ChangeTBRStatus")]
		public async Task<IActionResult> ChangeTBRStatus([FromBody] int Id)
		{
			using (var context = new ApplicationDbContext(_options))
			{
				var tbrBook = context.TBRBooks.Find(Id);
				if (tbrBook.IsRead)
				{
					tbrBook.IsRead = false;
				}
				else
				{
					tbrBook.IsRead = true;
				}

				context.TBRBooks.Update(tbrBook);
				await context.SaveChangesAsync();
			}

				return Ok();
		}

		[HttpDelete]
		[Route("Authorized/DeleteTBRBook")]
		public async Task<IActionResult> DeleteTBRBook([FromBody] int Id)
		{
			using (var context = new ApplicationDbContext(_options))
			{
				var tbrBook = context.TBRBooks.Find(Id);
				context.TBRBooks.Remove(tbrBook);
				await context.SaveChangesAsync();
				return Ok();
			}
		}

		[HttpPost]
		[Route("Authorized/EditTBRBook")]
		public async Task<IActionResult> EditTBRBook([FromBody] EditTBRBook model)
		{
			using (var context = new ApplicationDbContext(_options))
			{
				var tbrbook = context.TBRBooks.Find(model.TBRBookId);
				tbrbook.Title = model.Title;

				context.Update(tbrbook);
				await context.SaveChangesAsync();

				return Ok();
			}
		}

		[HttpGet]
		public async Task<IActionResult> EditTBR(int TBRId)
		{
			using(var context = new ApplicationDbContext(_options))
			{
				var tbr = context.TBRs.Find(TBRId);
				var model = new EditTBRViewModel();
				model.TBRId = tbr.Id;
				model.TBRName = tbr.Title;
				model.TBRDeadline = tbr.Deadline;
				return View(model);
				
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditTBR(EditTBRModel model)
		{
			using(var context = new ApplicationDbContext(_options))
			{
				var tbr = context.TBRs.Find(model.TBRId);
				tbr.Title = model.Title;
				tbr.Deadline = model.Deadline;
				context.TBRs.Update(tbr);
				await context.SaveChangesAsync();
				return RedirectToAction("TBRDetails", new { model.TBRId });
			}
		}

		[HttpDelete]
		[Route("Authorized/RemoveTBR")]
		public async Task<IActionResult> RemoveTBR([FromBody] int TBRId)
		{
			using(var context = new ApplicationDbContext(_options))
			{
				var tbr = context.TBRs.Find(TBRId);
				var bookToRemove = context.TBRBooks.Where(t => t.TBR == tbr).ToList();
				context.TBRBooks.RemoveRange(bookToRemove);
				context.TBRs.Remove(tbr);
				await context.SaveChangesAsync();
				return Ok();
			}
		}

		[HttpGet]
		[Route("Authorized/CheckTBRFinishable")]
		public async Task<IActionResult> CheckTBRFinishable(int TBRId)
		{
			using(var context = new ApplicationDbContext(_options))
			{
				int CountRead = context.TBRBooks.Where(t => t.TBR.Id == TBRId && t.IsRead).Count();
				int CountToRead = context.TBRBooks.Where(t => t.TBR.Id == TBRId).Count();
				bool isFinished = context.TBRs.Find(TBRId).IsFinished;
				if(CountToRead != 0 && !isFinished && CountToRead == CountRead)
				{
					return Json(true);
				}
				else
				{
					return Json(false);
				}
			}
		}

		[HttpPost]
		[Route("Authorized/FinishTBR")]
		public async Task<IActionResult> FinishTBR([FromBody] int TBRId)
		{
			using(var context = new ApplicationDbContext(_options))
			{
				var tbr = context.TBRs.Find(TBRId);
				tbr.IsFinished = true;
				context.TBRs.Update(tbr);
				await context.SaveChangesAsync();
				return Ok();
			}
		}


	}
}
