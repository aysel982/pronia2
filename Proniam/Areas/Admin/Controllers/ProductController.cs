using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proniam.DAL;
using Proniam.ViewModel;

namespace Proniam.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetProductVM> productVMs = await _context.Products.Select(p => new GetProductVM
            {
                Name = p.Name,
                Price = p.Price,
                SKU = p.SKU,
                CategoryName=p.Category.Name,
                MainImage = p.ProductImages.FirstOrDefault(pi=>pi.IsPrimary==true).Image,
                Id = p.Id

            }).ToListAsync();
            return View();
        }
    }
}
