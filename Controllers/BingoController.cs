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
    public class BingoController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DbContextOptions<ApplicationDbContext> _options;


        public BingoController(UserManager<IdentityUser> userManager, DbContextOptions<ApplicationDbContext> options)
        {
            _userManager = userManager;
            _options = options;
        }

        public async Task<IActionResult> Bingo()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            using (var context = new ApplicationDbContext(_options))
            {
                List<Bingo> model = context.Bingos.Where(b => b.User == user.Id).ToList();
                return View(model);
            }
        }

        public async Task<IActionResult> AddNewBingo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewBingo(AddNewBingoModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            model.BingoField = model.BingoField.OrderBy(bf => Random.Shared.Next()).ToList();

            using (var context = new ApplicationDbContext(_options))
            {
                Bingo bingo = new();
                bingo.Name = model.Title;
                bingo.User = user.Id;

                context.Add(bingo);
                int index = 1;
                foreach (string field in model.BingoField)
                {
                    BingoField bingoField = new();
                    bingoField.Name = field;
                    bingoField.Bingo = bingo;
                    bingoField.IsChecked = false;
                    bingoField.Index = index;
                    index++;

                    context.Add(bingoField);
                }
                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Bingo));
        }

        [HttpGet]
        [Route("[controller]/BingoDetails/{bingoId}")]
        public async Task<IActionResult> BingoDetails(int bingoId)
        {
            using (var context = new ApplicationDbContext(_options))
            {
                BingoDetailsModel model = new();
                model.Title = context.Bingos.Where(b => b.Id == bingoId).Select(b => b.Name).FirstOrDefault();
                model.bingoFields = context.BingoFields.Where(b => b.Bingo.Id == bingoId).ToList();

                return View(model);
            }
        }

        [HttpPost]
        [Route("[controller]/CheckBingo/{bingoDetailId}")]
        public async Task<IActionResult> CheckBingo(int bingoDetailId)
        {
            using (var context = new ApplicationDbContext(_options))
            {
                BingoField bingoField = context.BingoFields.Find(bingoDetailId);
                if (bingoField == null)
                    return NotFound();

                if (bingoField.IsChecked)
                {
                    bingoField.IsChecked = false;
                }
                else
                {
                    bingoField.IsChecked = true;
                }
                context.Update(bingoField);
                await context.SaveChangesAsync();
                return Ok();
            }
        }
    }
}
