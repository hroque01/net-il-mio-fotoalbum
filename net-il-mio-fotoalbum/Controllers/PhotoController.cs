using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Controller for CREATE PAGES
        public IActionResult Create()
        {
            //DATABASE
            using (PhotoContext db = new PhotoContext())
            {

            }
            return View();
        }
    }
}
