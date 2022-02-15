using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PictureShare_.Data;

namespace PictureShare_.Controllers
{
    public class PictureController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PictureController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var pictures = await _db.Pictures.ToListAsync();
            return View(pictures);
        }


        public IActionResult Create()
        {
            return View();
        }
    }
}
