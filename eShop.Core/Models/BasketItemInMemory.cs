using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.Models
{
    public class BasketItemInMemory
    {
        public Guid BasketItemID { get; set; }
        public int Quantity { get; set; }
        public Guid BasketID { get; set; }
        public Guid ProductID { get; set; }
        public Guid SizeQuantityID { get; set; }
    }
}
