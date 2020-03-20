using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class ProductFormViewModel
    {
        public Product product { get; set; }
        public SizeQuantity sizeQuantity { get; set; }
        public IEnumerable<ProductCategory> productCategories { get; set; }
        public List<SizeQuantity> sizeQuantities { get; set; }
    }
}
