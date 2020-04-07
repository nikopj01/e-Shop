using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class LoginFormViewModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = ("User Name"))]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = ("Password"))]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }
    }
}
