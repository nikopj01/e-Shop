using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class BasketItemCountViewModel
    {
        public BasketItemCountViewModel()
        {
            this.basketItemCount = 0;
        }
        public int basketItemCount { get; set; }
    }
}
