using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proniam.DAL;
using Proniam.Models;
using Proniam.Utilities.Enums;
using Proniam.Utilities.Extentions;
using Proniam.ViewModel;

namespace Proniam.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetProductVM> productVMs = await _context.Products.Select(p => new GetProductVM
            {
                Name = p.Name,
                Price = p.Price,
                SKU = p.SKU,
                CategoryName = p.Category.Name,
                MainImage = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                Id = p.Id

            }).ToListAsync();
            return View();
        }
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync()
            };

            return View(productVM);
        }
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "Category does not exists");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "file type is incorrect");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateSize(FileSize.KB, 500))
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "file size must be less than 500 kb");
                return View(productVM);
            }
            bool nameresult = await _context.Products.AnyAsync(p => p.Name == productVM.Name);
            if (!productVM.MainPhoto.ValidateSize(FileSize.KB, 500))
            {
                ModelState.AddModelError(nameof(CreateProductVM.Name), "already exist");
                return View(productVM);
            }
            ProductImage main = new ProductImage
            {
                Image = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = true,
                CreatedAt = DateTime.Now
            };

            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price.Value,
                SKU = productVM.SKU,
                Description = productVM.Description,
                CategoryId = productVM.CategoryId.Value,
                ProductImages = new List<ProductImage> { main }

            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            Product? product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                PrimaryImage = product.ProductImages.FirstOrDefault(p => p.IsPrimary == true).Image,
                Categories = await _context.Categories.ToListAsync()

            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            if (ModelState.IsValid)
            {
                return View(productVM);
            }
            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.CategoryId), "file type is incorrect");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(FileSize.KB, 500))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.CategoryId), "file size must be less than 500 kb");
                    return View(productVM);
                }
            }
            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.CategoryId), "Category does not exists");
                return View(productVM);
            }
            bool nameResult = await _context.Products.AnyAsync(p => p.Name == productVM.Name && p.Id != id);
            if (nameResult)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.Name), "Product already exists");
                return View(productVM);
            }
            Product? existed = await _context.Products.Include(p=>p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (productVM.MainPhoto is null)
            {
                ProductImage main = new ProductImage
                {
                    Image = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                    IsPrimary =true,
                    CreatedAt=DateTime.Now
                };
                ProductImage existedMain = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                existedMain.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(existedMain);
                existed.ProductImages.Add(main);
            }
            existed.Name = productVM.Name;
            existed.ProductImages = productVM.Price.Value;
            existed.Description = productVM.Description;
            existed.SKU = productVM.SKU;
            existed.Category = productVM.CategoryId.Value;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Dalete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            Product? product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            foreach (ProductImage proImage in product.ProductImages) 
            {
                proImage.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
    }
}           
        
    