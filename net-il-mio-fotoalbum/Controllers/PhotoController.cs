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
            
    }
}
