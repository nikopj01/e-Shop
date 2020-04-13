using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace eShop.Core.ViewModels
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = ("NAME"))]
        public string name { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [Display(Name = ("EMAIL ADDRESS"))]
        public string email { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [Display(Name = ("SELECT A REASON"))]
        public string reason { get; set; }

        [Display(Name = ("SUBJECT"))]
        public string subject { get; set; }

        [Display(Name = ("ORDER NUMBER"))]
        public string ordernumber { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [Display(Name = ("MESSAGE"))]
        public string message { get; set; }

        public List<SelectListItem> selectReasons = new List<SelectListItem>();

        public ContactUsViewModel()
        {
            selectReasons.Add(new SelectListItem { Text = "General Inquiry", Value = "General Inquiry", Selected = true });
            selectReasons.Add(new SelectListItem { Text = "Order Status", Value = "Order Status" });
            selectReasons.Add(new SelectListItem { Text = "Product Question/Feedback", Value = "Product Question/Feedback" });
            selectReasons.Add(new SelectListItem { Text = "Billing/Payment Inquiry", Value = "Billing/Payment Inquiry" });
            selectReasons.Add(new SelectListItem { Text = "ReturnAssisstance", Value = "ReturnAssisstance" });
            selectReasons.Add(new SelectListItem { Text = "Other", Value = "Other" });
        }
    }
}
