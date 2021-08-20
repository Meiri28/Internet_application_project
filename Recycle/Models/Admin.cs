using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recycle.Models
{
    public class Admin
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password address is required.")]
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
