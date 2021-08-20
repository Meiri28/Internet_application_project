using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace Recycle.Models
{
    public class Products
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Store ID")]
        public int StoreId { get; set; }
        public Store Store{ get; set; }

        [Required]
        public string ItemName { get; set; }

        //[Required]
        public string ItemDesc { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string Pictures { get; set; }

        public string VideoURL { get; set; }

        [Required]
        [Display(Name = "Is Product Active?")]
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }


        // Tables relationship
        public List<ProductsComment> comments { get; set; }
    }
}
