using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Proniam.DAL;
using Proniam.Models;
using Proniam.Utilities.Enums;
using Proniam.Utilities.Extentions;
using Proniam.ViewModel;
using Proniam.ViewModel;
using System.Data;
using System.Threading.Tasks;

namespace Proniam.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    { 
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
           _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetSlideVM> slideVMs = await _context.Slides.Select(s =>
            
                new GetSlideVM
                {
                    Id = s.Id,
                    Title = s.Title,
                    Image = s.Image,
                    CreatedAt = s.CreatedAt,
                    Order = s.Order,
                }
            
            ).ToListAsync();
           
            
            return View(slideVMs);
        }

        
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!slideVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Photo), "File is incorrect");
                return View();
            }

            if (!slideVM.Photo.ValidateSize(FileSize.MB,1))
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Photo), "File size should be less that 2MB");
            }
          
            string fileName = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");

            Slide slide = new Slide
            {
                Title = slideVM.Title,
                Subtitle = slideVM.Subtitle,
                Description = slideVM.Description,
                Order = slideVM.Order,
                Image = fileName,
                CreatedAt = DateTime.Now

            };

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null||id<=0) return BadRequest();
            Slide? slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();
            UpdateSlideVM slidevm = new UpdateSlideVM
            {
                Title = slide.Title,
                Subtitle = slide.Subtitle,
                Order = slide.Order,
                Description = slide.Description,
                Image = slide.Image
            };
            return View(slidevm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id,UpdateSlideVM slidevm)
        {
            if (!ModelState.IsValid)
            {
               
                return View(slidevm);
            }
            Slide? existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
                if (existed is null) return NotFound();
            if(slidevm.Photo is not null)
            {
                if (!slidevm.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "File type is incorrect");
                    return View(slidevm);
                }
                if (!slidevm.Photo.ValidateSize(FileSize.MB, 1))
                {

                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "File size must be less that 1MB");
                    return View(slidevm);
                }
                string fileName= await slidevm.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.Image = fileName;
            }
            existed.Title = slidevm.Title;
            existed.Subtitle = slidevm.Subtitle;
            existed.Order = slidevm.Order;
            existed.Description = slidevm.Description;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Delete(int? id) 
        {

            if (id is null || id <= 0) return BadRequest();
            Slide? slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();
            slide.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            _context.Remove(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        

        

    }
}
