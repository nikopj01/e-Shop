using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class ProductSizeQuantityFormViewModel
    {
        public SizeQuantity sizeQuantity { get; set; }
        public Guid productID { get; set; }
    }
}
