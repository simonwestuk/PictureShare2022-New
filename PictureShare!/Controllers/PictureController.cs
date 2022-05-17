using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PictureShare_.Data;
using PictureShare_.Helpers;
using PictureShare_.Models;

namespace PictureShare_.Controllers
{
    [Authorize]
    public class PictureController : Controller
    {
        private readonly ApplicationDbContext _db;
        private Images _image;
        private IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _user;
        public PictureController(ApplicationDbContext db, Images image, IWebHostEnvironment env, UserManager<IdentityUser> user)
        {
            _db = db;
            _image = image;
            _env = env;
            _user = user;
        }

        public async Task<IActionResult> Index()
        {
            var pictures = await _db.Pictures.Where(x => x.UserEmail == User.Identity.Name).Include("Category").ToListAsync();

            foreach (var picture in pictures)
            {
                picture.Comments = await _db.Comments.Where(x => x.PictureId == picture.Id).Include("User").ToListAsync();
            }

            return View(pictures);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return View(nameof(Index));
            }

            var picture = await _db.Pictures.Where(x => x.Id.ToString() == id).Include("Category").FirstOrDefaultAsync();
            picture.Comments = await _db.Comments.Where(x => x.PictureId == picture.Id).Include("User").OrderByDescending(x => x.Timestamp).ToListAsync();
            if (picture == null)
                return View(nameof(Index));

            return View(picture);

        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string comment, Guid id)
        {
            var pic =  await _db.Pictures.Where(x => x.Id == id).FirstOrDefaultAsync();
            pic.Comments.Add(new CommentModel
            {
                Comment = comment,
                PictureId = id,
                Timestamp = DateTime.Now,
                User = await _user.FindByEmailAsync(User.Identity.Name)
            }); 

            _db.Pictures.Update(pic);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = pic.Id });
        }

        [HttpGet]
        public IActionResult Create()
        {
            PictureViewModel pictureViewModel = new PictureViewModel()
            {
                Picture = new PictureModel(),
                CategoryList = _db.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString() 
                })
            };

            return View(pictureViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PictureViewModel model)
        {
            model.Picture.TimeStamp = DateTime.Now;
            model.Picture.UserEmail = User.Identity.Name;
            model.Picture.Public = false;
            var file = Request.Form.Files[0];

            Images images = new Images(_env);
            model.Picture.ImagePath = await images.Upload(file, "/Images/");

            await _db.Pictures.AddAsync(model.Picture);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }

        public async Task<IActionResult> Delete(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var model = await _db.Pictures.FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

            if (model == null)
            {
                return NotFound();
            }

            _db.Pictures.Remove(model);
            await _db.SaveChangesAsync();
            _image.Delete(model.ImagePath);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> makePublic(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _db.Pictures.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            model.Public = !model.Public;
            _db.Pictures.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
