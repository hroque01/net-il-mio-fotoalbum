using Microsoft.AspNetCore.Mvc.Rendering;

namespace net_il_mio_fotoalbum.Models
{
    public class PhotoFormModel
    {
        public Photo? Photo { get; set; }

        //List for index
        public List<Photo>? ListPhotos {get; set;}
        public List<string>? ListImages { get; set; }

        public List<SelectListItem>? Categories { get; set; }

        public List<string>? SelectedCategories { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
