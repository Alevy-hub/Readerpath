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
	public class ChallengeController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly DbContextOptions<ApplicationDbContext> _options;


		public ChallengeController(UserManager<IdentityUser> userManager, DbContextOptions<ApplicationDbContext> options)
		{
			_userManager = userManager;
			_options = options;
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
		[Route("Challenge/NewChallenge")]
		public async Task<IActionResult> NewChallenge(YearChallenge model)
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
			using (var context = new ApplicationDbContext(_options))
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
	}
}
