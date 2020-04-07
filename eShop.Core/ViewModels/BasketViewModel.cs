using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class BasketViewModel
    {
        public IEnumerable<BasketItem> basketItems { get; set; }
        public decimal basketSubTotal { get; set; }
    }
}
