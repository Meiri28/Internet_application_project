using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recycle.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public int FromUserID { get; set; }

        public int ToUserID { get; set; }

        public int Amount { get; set; }

        public int CartID { get; set; }

        public int NumTransaction { get; set; }

//starus(pending, approve, dicline)


        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }

    }
}




