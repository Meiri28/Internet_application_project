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

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }


        public User User { get; set; }
        public int UserId { get; set; }
    }
}
