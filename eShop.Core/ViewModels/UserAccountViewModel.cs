using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class UserAccountViewModel
    {
        public UserAccount userAccount { get; set; }
        public IEnumerable<UserAddress> userAddresses { get; set; }
        public IEnumerable<Order> orders { get; set; }
    }
}
