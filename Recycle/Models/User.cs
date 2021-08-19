using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recycle.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email address is required.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password address is required.")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Gender is required.")]
        public UserGender Gender { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Birth Date is required.")]
        public DateTime BDay { get; set; }

        public double Balance { get; set; } = 0.0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
    }
}
