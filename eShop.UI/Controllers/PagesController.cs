using eShop.Core.Contracts;
using eShop.Core.Models;
using eShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eShop.UI.Controllers
{
    public class PagesController : Controller
    {
        private IRepository<Inquiry> _contextInquiry;

        public PagesController(IRepository<Inquiry> inquiry)
        {
            _contextInquiry = inquiry;
        }

        public ActionResult ContactUs()
        {
            ContactUsViewModel viewModel = new ContactUsViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactUsViewModel contactUsViewModel)
        {
            if (ModelState.IsValid)
            {
                Inquiry newInquiry = new Inquiry();
                newInquiry.InquiryID = Guid.NewGuid();
                newInquiry.Name = contactUsViewModel.name;
                newInquiry.Email = contactUsViewModel.email;
                newInquiry.Reason = contactUsViewModel.reason;
                newInquiry.Subject = contactUsViewModel.subject;
                newInquiry.OrderNumber = contactUsViewModel.ordernumber;
                newInquiry.Message = contactUsViewModel.message;
                newInquiry.CreatedAt = DateTime.Now;
                newInquiry.IsActive = true;
                _contextInquiry.Insert(newInquiry);
                _contextInquiry.Commit();
                TempData["LayoutMessage"] = "Succesfully Send a Message";
                ContactUsViewModel viewModel = new ContactUsViewModel();
                return View(viewModel);
            }
            return View(contactUsViewModel);
        }

        public ActionResult ReturnPolicy()
        {
            return View();
        }

        public ActionResult ShippingPolicy()
        {
            return View();
        }

        public ActionResult TermsOfService()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }
    }   
}