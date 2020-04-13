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
    public class HomeController : Controller
    {
        private IRepository<Product> _contextProduct;
        private IRepository<Subscriber> _contextSubscriber;

        public HomeController(IRepository<Product> product,IRepository<Subscriber> subscriber)
        {
            _contextProduct = product;
            _contextSubscriber = subscriber;
        }

        public ActionResult Index()
        {
            HomepageViewModel model = new HomepageViewModel();
            DateTime currentDate = DateTime.Now.AddDays(30);
            model.products = _contextProduct.Collection().Where(p => p.CreatedAt <= currentDate && p.IsActive == true).Take(8).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(HomepageViewModel homepageViewModel)
        {
            if (ModelState.IsValid)
            {
                Subscriber selectedSubscriber = _contextSubscriber.Collection().SingleOrDefault(s => s.Email == homepageViewModel.email);
                if (selectedSubscriber == null)
                {
                    Subscriber inputtedSubscriber = new Subscriber();
                    inputtedSubscriber.SubscriberID = Guid.NewGuid();
                    inputtedSubscriber.Email = homepageViewModel.email;
                    inputtedSubscriber.CreatedAt = DateTime.Now;
                    inputtedSubscriber.IsActive = true;
                    _contextSubscriber.Insert(inputtedSubscriber);
                    _contextSubscriber.Commit();
                    TempData["LayoutMessage"] = "Succesfully Add Subscriber E-mail";
                }
                else
                {
                    TempData["LayoutMessage"] = "You have subscribed";
                }
                homepageViewModel.email = "";
            }
            DateTime currentDate = DateTime.Now.AddDays(30);
            homepageViewModel.products = _contextProduct.Collection().Where(p => p.CreatedAt <= currentDate && p.IsActive == true).Take(8).ToList();
            return View(homepageViewModel);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}