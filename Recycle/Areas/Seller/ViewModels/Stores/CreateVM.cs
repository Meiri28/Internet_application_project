using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Recycle.Areas.Seller.ViewModels.Stores
{
    public class CreateVM
    {

        /// <summary>
        /// The name of the Store.
        /// </summary>
        /// <remarks>[Unique]</remarks>
        [Display(Name = "Name *")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        [Remote("CheckNameAvailability", "Stores", "Seller", ErrorMessage = "Name already taken.")]
        public string Name { get; set; }

        /// <summary>
        /// The address of the Store.
        /// </summary>
        [Display(Name = "Address *")]
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Maximum length allowed is 50 characters.")]
        public string Address { get; set; }

        /// <summary>
        /// The phone number of the Store.
        /// </summary>
        [Display(Name = "Phone Number *")]
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20, ErrorMessage = "Maximum length allowed is 20 characters.")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The opening hours of the Store.
        /// </summary>
        [Display(Name = "Opening Hours *")]
        [Required(ErrorMessage = "Opening hours is required.")]
        [StringLength(200, ErrorMessage = "Maximum length allowed is 200 characters.")]
        [DataType(DataType.MultilineText)]
        public string OpeningHours { get; set; }

        /// <summary>
        /// The latitude of the Store location.
        /// </summary>
        [Display(Name = "Location Latitude *")]
        [Required(ErrorMessage = "Location latitude is required.")]
        public double LocationLatitude { get; set; }

        /// <summary>
        /// The longitude of the Store location.
        /// </summary>
        [Display(Name = "Location Longitude *")]
        [Required(ErrorMessage = "Location longitude is required.")]
        public double LocationLongitude { get; set; }

        public int UserId { get; set; }

    }
}
