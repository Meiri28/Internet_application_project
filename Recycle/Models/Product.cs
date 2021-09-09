using DataAnnotationsExtensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace Recycle.Models
{
    public class Product
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

        [Required]
        public string ItemDesc { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        public uint Amount { get; set; }

        [Required]
        public uint Price { get; set; }

        [Required]
        public string Color { get; set; }

        public byte[] Video { get; set; }

        [NotMapped]
        public IFormFile VideoFile { get; set; }

        [Required]
        [Display(Name = "Is Product Active?")]
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }


        // Tables relationship
        public List<ProductComment> comments { get; set; }

        public List<ProductImage> Pictures { get; set; }

        [NotMapped]
        public List<IFormFile> PictursFiles { get; set; }
    }
}
