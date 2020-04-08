using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.ViewModels
{
    public class EditProfileFormViewModel
    {
        public System.Guid UserAccountID { get; set; }


        [Display(Name = ("User Name"))]
        public string UserName { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = ("First Name"))]
        public string FirstName { get; set; }

        [Display(Name = ("Last Name"))]
        public string LastName { get; set; }

        [Display(Name = ("Password"))]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }
    }
}
