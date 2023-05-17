using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;
using System.Drawing;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotoController : Controller
    {
        //INDEX, stampa tutte le foto nella index
        [HttpGet]
        public IActionResult Index()
        {
            using(PhotoContext db = new PhotoContext())
            {
                List <Photo> photos = db.Photos.ToList();

                List<string> imagesData = new List<string>();

                foreach(Photo photo in photos)
                {
                    imagesData.Add(Convert.ToBase64String(photo.Image));
                }

                PhotoFormModel model = new PhotoFormModel();

                model.ListPhotos = photos;
                model.ListImages = imagesData;

                return View(model);
            }
        }

        //CREATE, HTTPGET
        [HttpGet]
        public IActionResult Create()
        {
            using (PhotoContext db = new PhotoContext())
            {
                PhotoFormModel model = new PhotoFormModel();
                List<Category> categories = db.Categories.ToList();

                List<SelectListItem> listCategories = new List<SelectListItem>();

                foreach (Category category in categories)
                {
                    listCategories.Add(new SelectListItem
                    {
                        Text = category.Name,
                        Value = category.Id.ToString()
                    });
                }

                Photo photo = new Photo();
                photo.Visibility = true;

                model.Categories = listCategories;
                model.Photo = photo;
                return View("Create", model);
            }
        }

        //CREATE, HTTPPOST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhotoFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Gestisci l'immagine solo se è stata fornita
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using (PhotoContext db = new PhotoContext())
                    {
                        // Leggi i dati dell'immagine come array di byte
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(model.ImageFile.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)model.ImageFile.Length);
                        }

                        List<Category> categories = new List<Category>();
                        foreach (var categoryid in model.SelectedCategories)
                        {
                            int intcategoryid = int.Parse(categoryid);
                            var category = db.Categories.Where(c => c.Id == intcategoryid).FirstOrDefault();
                            categories.Add(category);
                        }


                        // Crea un nuovo oggetto Photo e assegna i valori
                        Photo photo = new Photo
                        {
                            Title = model.Photo.Title,
                            Description = model.Photo.Description,
                            Image = imageData,
                            Visibility = model.Photo.Visibility,
                            Categories = categories
                            // Assegna altre proprietà necessarie
                        };

                        // Salva l'oggetto Photo nel database

                            db.Photos.Add(photo);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Index");
                }
            }

            // Se il modello non è valido, ritorna la vista "Create" con il modello
            return View(model);
        }
        
        //DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(long id)
        {
            using (PhotoContext db = new PhotoContext())
            {
                Photo photo = db.Photos.Where(ph => ph.Id == id).FirstOrDefault();
                if (photo != null)
                {
                    db.Photos.Remove(photo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }
        }

        //DETAILS
        [HttpGet]
        public IActionResult Details(long id)
        {
            using (PhotoContext db = new PhotoContext())
            {
                Photo photo = db.Photos.Where(ph => ph.Id == id).Include(ph => ph.Categories).FirstOrDefault();
                string imagesData = Convert.ToBase64String(photo.Image);

                PhotoFormModel model = new PhotoFormModel();

                model.Photo = photo;
                model.Image = imagesData;

                if (photo == null)
                {
                    return NotFound();
                }

                return View(model);
            }
        }
    }
}
