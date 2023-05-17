using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_il_mio_fotoalbum.Models
{
    [Table("category")]
    public class Category
    {
        [Key]
        public long Id { get; set; }
        
        public string Name { get; set; }

        //Relation MtoM [Photo]
        public List<Photo> Photos { get; set; }
    }
}
