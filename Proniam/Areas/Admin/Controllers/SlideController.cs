using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proniam.DAL;
using Proniam.Models;
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
            List<Slide> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }

        public string Test()
        {

            return Guid.NewGuid().ToString();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if (!slide.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError(nameof(Slide.Photo), "File is incorrect");
                return View();
            }

            if (slide.Photo.Length > 2 * 1024 * 10124)
            {
                ModelState.AddModelError(nameof(Slide.Photo), "File size should be less that 2MB");
            }


           string fileName= string.Concat(Guid.NewGuid().ToString(), slide.Photo.FileName) ;
            String path= Path.Combine(_env.WebRootPath, "assets","images","website-images", fileName);
            FileStream fl = new FileStream(path,FileMode.Create);
            await slide.Photo.CopyToAsync(fl);

            slide.Image = fileName;

            slide.CreatedAt = DateTime.Now;
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }
    }
}
