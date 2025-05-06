using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proniam.DAL;
using Proniam.Models;
using Proniam.ViewModel;

namespace Proniam.Controllers
{
    public class HomeController : Controller
    {
        public readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()

        {

            HomeVM homevm = new HomeVM
            {
                Slides = await _context.Slides
                .OrderBy(s => s.Order)
                .Take(2)
                .ToListAsync(),

                Products = await _context.Products
                .Include(p => p.ProductImages)
                .ToListAsync()
            };

            return View(homevm);
        }
    }
}
