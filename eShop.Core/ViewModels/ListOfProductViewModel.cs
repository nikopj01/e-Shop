using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class ListOfProductViewModel
    {
        public IEnumerable<Product> products { get; set; }
        public IEnumerable<ProductCategory> productCategories { get; set; }
        public IEnumerable<ProductType> productTypes { get; set; }
    }
}
