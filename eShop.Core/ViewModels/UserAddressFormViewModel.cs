using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.Models
{
    public class UserAddressFormModel
    {
        public System.Guid UserAddressID { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Region is required")]
        public string Region { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
    }
}
