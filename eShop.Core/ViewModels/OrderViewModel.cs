using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class OrderViewModel
    {
        public IEnumerable<BasketItem> basketItems { get; set; }
        public IEnumerable<UserAddress> userAddresses { get; set; }
        public decimal basketSubTotal { get; set; }
        public string InputtedDiscountCode { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = ("Address"))]
        public System.Guid selectedAddressID { get; set; }
    }
}
