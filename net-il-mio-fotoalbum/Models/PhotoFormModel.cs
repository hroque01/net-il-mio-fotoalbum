using Microsoft.AspNetCore.Mvc.Rendering;

namespace net_il_mio_fotoalbum.Models
{
    public class PhotoFormModel
    {
        public Photo Photo { get; set; }    
        public List<SelectListItem>? Category { get; set; }
        public List<string>? SelectedCategory { get; set; }
    }
}
