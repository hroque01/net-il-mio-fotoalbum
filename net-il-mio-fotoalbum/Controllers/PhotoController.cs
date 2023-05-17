using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;
using System.Drawing;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotoController : Controller
    {
        //Index, Recupera tutte le foto
        [HttpGet]
        public IActionResult Index()
        {
            using (PhotoContext db = new PhotoContext())
            {
                List<Photo> photos = db.Photos.Where(ph => ph.Visibility == true).ToList();

                // Creiamo una lista di stringhe chiamata "imagesData" per memorizzare la rappresentazione in formato Base64 dei dati dell'immagine di ogni foto presente nella lista "photos"
                List<string> imagesData = new List<string>();

                foreach (Photo photo in photos)
                {
                    imagesData.Add(Convert.ToBase64String(photo.Image));
                }

                // Salviamo l'elenco "imagesData" nel dizionario ViewData con la chiave "Images"
                ViewData["Images"] = imagesData;

                return View(photos);
            }
        }

        //HTTPGET, DETAILS
        //Prende dal server i dettagli della card
        [HttpGet]
        public IActionResult Details(long id)
        {
            using (PhotoContext db = new PhotoContext())
            {
                Photo photo = db.Photos.Where(ph => ph.Id == id).Include(ph => ph.Categories).FirstOrDefault();

                // Se la foto non esiste, viene restituita una view di errore
                if (photo == null)
                {
                    return View("Error", "La foto cercata non esiste");
                }

                // Qui viene convertita l'immagine della foto in una stringa Base64 e salvata nella ViewData
                string imagesData = Convert.ToBase64String(photo.Image);
                ViewData["Image"] = imagesData;

                return View("Details", photo);
            }
        }

        //HTTPGET CREATE
        // Questo metodo risponde ad una richiesta GET all'endpoint '/Create'
        [HttpGet]
        public IActionResult Create()
        {
            using (PhotoContext db = new PhotoContext())
            {
                // Qui viene istanziato un nuovo oggetto PhotoFormModel, che sarà utilizzato per passare i dati alla view
                PhotoFormModel model = new PhotoFormModel();

                // Qui viene estratta la lista di tutte le categorie dal database e salvata in una variabile
                List<Category> categories = db.Categories.ToList();

                // Qui viene istanziata una nuova lista di SelectListItem, che sarà utilizzata per popolare una dropdown list nella view
                List<SelectListItem> listCategories = new List<SelectListItem>();

                // Questo ciclo for-each scorre la lista di categorie e per ogni categoria crea un nuovo oggetto SelectListItem, 
                // che viene aggiunto alla lista listCategories
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

        //HTTPPOST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhotoFormModel model)
        {
            // Se il modello non è valido (cioè se ci sono errori di validazione), viene gestita la richiesta GET alla pagina di creazione della foto
            if (!ModelState.IsValid)
            {
                using (PhotoContext db = new PhotoContext())
                {
                    // Qui vengono recuperate dal database tutte le categorie disponibili
                    List<Category> categories = db.Categories.ToList();

                    // Qui viene creata una lista di SelectListItem a partire dalle categorie recuperate
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
                    return View(model);
                }
            }

            using (PhotoContext db = new PhotoContext())
            {

                // Verifica se l'utente ha selezionato un'immagine, in caso contrario restituisce un messaggio di errore
                if (model.ImageFile == null)
                    return View("Error", "Immagine non selezionata");

                // Converte l'immagine in un array di byte
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(model.ImageFile.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)model.ImageFile.Length);
                }


                if (model.SelectedCategories == null)
                    return View("Error", "Categorie non selezionate");

                // Recupera le categorie selezionate dall'utente dal database e le aggiunge alla lista categories
                List<Category> categories = new List<Category>();
                foreach (var categoryid in model.SelectedCategories)
                {
                    int intcategoryid = int.Parse(categoryid);
                    var category = db.Categories.Where(c => c.Id == intcategoryid).FirstOrDefault();
                    categories.Add(category);
                }

                Photo photo = new Photo()
                {
                    Title = model.Photo.Title,
                    Description = model.Photo.Description,
                    Visibility = model.Photo.Visibility,
                    Image = imageData,
                    Categories = categories
                };

                db.Photos.Add(photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        //HTTPGET UPDATE
        [HttpGet]
        public IActionResult Update(long id)
        {
            using (PhotoContext db = new PhotoContext())
            {
                Photo photo = db.Photos.FirstOrDefault(ph => ph.Id == id);

                if (photo == null)
                {
                    return NotFound();
                }

                PhotoFormModel model = new PhotoFormModel();
                List<Category> categories = db.Categories.ToList();

                List<SelectListItem> listCategories = new List<SelectListItem>();

                photo.Categories = new List<Category>();
                foreach (Category category in categories)
                {
                    listCategories.Add(new SelectListItem
                    {
                        Text = category.Name,
                        Value = category.Id.ToString(),
                        Selected = photo.Categories.Any(c => c.Id == category.Id)
                    });
                }

                model.Categories = listCategories;
                model.Photo = photo;
                ViewData["Image"] = Convert.ToBase64String(photo.Image);

                return View(model);
            }
        }

        //HTTPPOST UPDATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(long id, PhotoFormModel model)
        {
            if (!ModelState.IsValid)
            {
                using (PhotoContext db = new PhotoContext())
                {
                    Photo photo = db.Photos.FirstOrDefault(ph => ph.Id == id);

                    if (photo == null)
                    {
                        return NotFound();
                    }

                    List<Category> categories = db.Categories.ToList();

                    List<SelectListItem> listCategories = new List<SelectListItem>();

                    photo.Categories = new List<Category>();
                    foreach (Category category in categories)
                    {
                        listCategories.Add(new SelectListItem
                        {
                            Text = category.Name,
                            Value = category.Id.ToString(),
                            Selected = photo.Categories.Any(c => c.Id == category.Id)
                        });
                    }

                    model.Categories = listCategories;
                    model.Photo = photo;
                    ViewData["Image"] = Convert.ToBase64String(photo.Image);

                    return View(model);
                }
            }

            using (PhotoContext db = new PhotoContext())
            {
                Photo photo = db.Photos
                    .Include(ph => ph.Categories)
                    .FirstOrDefault(ph => ph.Id == id);

                if (photo == null)
                {
                    return NotFound();
                }

                // Se è stata fornita un'immagine, la converte in byte array e la assegna alla foto
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(model.ImageFile.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)model.ImageFile.Length);
                    }

                    photo.Image = imageData;
                }

                // Aggiorna le proprietà della foto con i valori forniti dal form
                photo.Title = model.Photo.Title;
                photo.Description = model.Photo.Description;
                photo.Visibility = model.Photo.Visibility;

                // Se sono state selezionate delle categorie, aggiorna quelle associate alla foto
                if (model.SelectedCategories != null && model.SelectedCategories.Any())
                {
                    photo.Categories.Clear();

                    foreach (var categoryId in model.SelectedCategories)
                    {
                        int intCategoryId = int.Parse(categoryId);
                        Category category = db.Categories.FirstOrDefault(c => c.Id == intCategoryId);
                        if (category != null)
                        {
                            photo.Categories.Add(category);
                        }
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }


        }

        //HTTPPOST DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(long id)
        {
            using (PhotoContext db = new PhotoContext())
            {
                Photo photo = db.Photos.Where(ph => ph.Id == id).FirstOrDefault();

                if (photo == null)
                    return View("Error", "La foto cercata non esiste");

                db.Photos.Remove(photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
