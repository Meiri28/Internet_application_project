using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recycle.Models
{
    public class Transaction
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int FromUserID { get; set; }
        [NotMapped]
        public User FromUser { get; set; }

        [Required]
        public int ToUserID { get; set; }
        [NotMapped]
        public User ToUser { get; set; }


        public enum StatusEnum
        {
            pending,
            approve,
            dicline
        };

        public StatusEnum Status { get; set; } = StatusEnum.pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public uint Amount { get; set; }

        [Required]
        public uint PriceForOneProduct { get; set; }
    }
}