using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class RegisterFormViewModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = ("User Name"))]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = ("Password"))]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = ("User Name"))]
        public string FirstName { get; set; }

        [Display(Name = ("Last Name"))]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-mail is required")]
        [Display(Name = ("E-mail"))]
        public string Email { get; set; }
    }
}
