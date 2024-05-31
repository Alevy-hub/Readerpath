using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readerpath.Data;
using Readerpath.Models;

namespace Readerpath.Controllers
{
	[Authorize]
	public class FinishYearController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly DbContextOptions<ApplicationDbContext> _options;


		public FinishYearController(UserManager<IdentityUser> userManager, DbContextOptions<ApplicationDbContext> options)
		{
			_userManager = userManager;
			_options = options;
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
						BookAuthor = mb.WorstBook.Edition.Book.Author.Name,
						BookGenre = mb.WorstBook.Edition.Book.Genre.Name,
						FinishDate = (DateTime)mb.WorstBook.DateFinished,
						Rating = mb.WorstBook.Rating,
					})
					.ToList();

				model.BestBooks = context.MonthBooks
					.Where(mb => mb.Year.ToString() == year && mb.User == user.Id)
					.Select(mb => new BestBook
					{
						BookActionId = mb.WorstBook.Id,
						BookTitle = mb.WorstBook.Edition.Book.Title,
						BookAuthor = mb.WorstBook.Edition.Book.Author.Name,
						BookGenre = mb.WorstBook.Edition.Book.Genre.Name,
						FinishDate = (DateTime)mb.WorstBook.DateFinished,
						Rating = mb.WorstBook.Rating,
					})
					.ToList();
			}

			return View(model);
		}
	}
}
