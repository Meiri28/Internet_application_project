﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recycle.Models
{
    public class ProductsComment
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User Writer { get; set; }

        [Required]
        public int ProductsId { get; set; }
        public Products Products { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
        public string body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
