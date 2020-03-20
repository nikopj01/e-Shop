using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class ProductTypeFormViewModel
    {
        public ProductType productType { get; set; }
        public IEnumerable<ProductCategory> productCategories { get; set; }
    }
}
