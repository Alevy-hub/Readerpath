using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> LoggedIndex()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            LoggedIndexModel model = new LoggedIndexModel();
            model.UserName = user.UserName;

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


                return View(model);
			}

        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            if(query == null)
            {
                query = "";
            }
            using (var context = new ApplicationDbContext(_options))
            {
                List<Book> books = context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Where(b => b.Title.Contains(query))
                    .ToList();
                List<SearchModel> modelList = new List<SearchModel>();

                foreach(Book book in books)
                {
                    bool paper = false;
                    bool audiobook = false;
                    bool ebook = false;

                    List<Edition> editions = context.Editions
                        .Where(e => e.Book == book)
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
            using(var context = new ApplicationDbContext(_options))
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
                    .Where(e => e.Book == book)
                    .Select(e => new EditionModel
                    {
                        Id = e.Id,
                        Name = e.Publisher.Name,
                        Pages = e.Pages,
                        Duration = e.Duration,
                        Type = e.Type
                    })
                    .ToList();

                model.Actions = context.BookActions
                    .Include(a => a.Edition.Publisher)
                    .Where(a => a.Edition.Book == book)
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

                return View(model);
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
                NewEdition.Publisher = publisher;
                context.Add(NewEdition);


				await context.SaveChangesAsync();
				return RedirectToAction(nameof(LoggedIndex));
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
                if (publisher == null)
                {
                    Entities.Publisher newPublisher = new Publisher();
                    newPublisher.Name = model.Publisher;
                    newPublisher.AddedBy = user.Id;
                    context.Add(newPublisher);
                    publisher = newPublisher;
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
                    NewEdition.Duration = model.Duration;
                }
                NewEdition.AddedBy = user.Id;
                NewEdition.Publisher = publisher;
                context.Add(NewEdition);

                await context.SaveChangesAsync();
            
                return RedirectToAction("BookDetails", new {id = model.BookId});
            }
        }

		[Route("{bookId}/{editionId}/AddToMyBooks")]
		public IActionResult AddToMyBooks(int bookId, int editionId)
        {
            using (var context = new ApplicationDbContext(_options))
            {
                AddToMyBooksModel model = new AddToMyBooksModel();
                model.book = context.Books.Find(bookId);
                model.bookId = bookId;
                model.editionId = editionId;

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
                    bookAction.Opinion = model.comment;
                }
                context.Add(bookAction);
                await context.SaveChangesAsync();

				return RedirectToAction("LoggedIndex");
            }
        }

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
                bookAction.DateFinished = finishBook.FinishDate;
                bookAction.Rating = float.Parse(finishBook.Rating, CultureInfo.InvariantCulture);
                bookAction.Opinion = finishBook.Comment;

                context.Update(bookAction);
                await context.SaveChangesAsync();
                return RedirectToAction("LoggedIndex");
            }
		}

        public async Task<IActionResult> AllReadBooks()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                List<AllReadBooksModel> model = context.BookActions
                    .Include(ba => ba.Edition.Book)
                    .Include(ba => ba.Edition.Book.Author)
                    .Include(ba => ba.Edition.Book.Genre)
                    .Include(ba => ba.Edition.Publisher)
                    .Select(ba => new AllReadBooksModel
                    {
                        Id = ba.Id,
                        BookId = ba.Edition.Book.Id,
                        Title = ba.Edition.Book.Title,
                        Author = ba.Edition.Book.Author.Name,
                        Genre = ba.Edition.Book.Genre.Name,
                        Rating = ba.Rating,
                        StartDate = (DateTime)ba.DateStarted,
                        FinishDate = ba.DateFinished
                    })
                    .ToList();
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
                model.BookCount = context.BookActions
                    .Count(a => a.User == user.Id && a.DateFinished != null && a.DateFinished.Value.Month.ToString() == month && a.DateFinished.Value.Year.ToString() == year);
                 
                model.PrevMonthBookCount = context.BookActions
                    .Count(a => a.User == user.Id && a.DateFinished != null && a.DateFinished.Value.Month.ToString() == prevMonth && a.DateFinished.Value.Year.ToString() == year);

                model.YearChallengeCount = (int)(context.YearChallenges
                        .SingleOrDefault(c => c.User == user.Id && c.Year.ToString() == year)?
                        .Count);

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
                        && a.DateFinished.Value.Year.ToString() == year)
                    .GroupBy(a => a.Edition.Book.Genre.Name)
                    .Select(group => new
                    {
                        GenreName = group.Key,
                        Count = group.Count()
                    })
                    .OrderByDescending(g => g.Count)
                    .FirstOrDefault()?.GenreName;

                model.Genres = context.BookActions
                    .Where(a => a.DateFinished != null
                        && a.DateFinished.Value.Month.ToString() == month
                        && a.DateFinished.Value.Year.ToString() == year)
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
                        && a.DateFinished.Value.Year.ToString() == year)
                    .Select(a => a.Edition.Book)
                    .ToList();
            }

            return View("MonthStatistics", model);
        }

        [Route("Statistics/{year}/Details")]
        public async Task<IActionResult> YearStatisticsDetails(string year)
        {
            return View();
        }
	}
}
