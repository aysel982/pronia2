using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proniam.DAL;
using Proniam.Models;
using Proniam.ViewModel;

namespace Proniam.Controllers
{
    public class ShopController1 : Controller
    {
        private readonly AppDbContext _context;

        public ShopController1(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null || id <= 0)
            {
                return BadRequest();
            }
            Product? product = await _context.Products
                .Include(p => p.ProductImages.OrderByDescending(pi=>pi.IsPrimary))
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            DetailVM detailvm = new DetailVM
            {
                Product=product,
                RelatedProducts=await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(2)
                .Include(p=>p.ProductImages.Where(pi => pi.IsPrimary != null))
                .ToListAsync()

            };
            return View(detailvm);
        }
    }
}
