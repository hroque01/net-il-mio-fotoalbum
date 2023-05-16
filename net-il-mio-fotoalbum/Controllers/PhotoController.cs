using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using net_il_mio_fotoalbum.Models;
using System.Data;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        // Controller for CREATE PAGES
        public IActionResult Create()
        {
            //DATABASE
            using (PhotoContext db = new PhotoContext())
            {
                PhotoFormModel model = new PhotoFormModel();
                
                List<SelectListItem> listCategory = new List<SelectListItem>();
                foreach (Category category in db.Categories)
                {
                    listCategory.Add(new SelectListItem()
                    {
                        Text = category.Name, Value = category.Id.ToString()
                    });
                }

                model.Photo = new Photo();
                model.Category = listCategory;

                return View("Create", model);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhotoFormModel data)
        {
            using (PhotoContext db = new PhotoContext())
            {
                if (!ModelState.IsValid)
                {
                    List<Category> categories = db.Categories.ToList();
                    List<SelectListItem> listCategories = new List<SelectListItem>();

                    foreach (Category category in categories)
                    {
                        listCategories.Add(new SelectListItem()
                        {
                            Text = category.Name,
                            Value = category.Id.ToString()
                        });
                    }

                    data.Category = listCategories;

                    return View(data);
                }

                //Crea nuovo oggetto dal server. Contiene dati del form
                Photo photo = new Photo();
                photo.Title = data.Photo.Title;
                photo.Description = data.Photo.Description;
                photo.Image = data.Photo.Image;
                photo.Visible = data.Photo.Visible;

                photo.Categories = new List<Category>();

                if (data.SelectedCategory != null)
                {
                    foreach (string selectedCategoryId in data.SelectedCategory)
                    {
                        int categoryId = int.Parse(selectedCategoryId);
                        Category category = db.Categories.Where(category => category.Id == categoryId).FirstOrDefault();
                        photo.Categories.Add(category);

                    }
                }

                db.Photos.Add(photo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }
    }
}
