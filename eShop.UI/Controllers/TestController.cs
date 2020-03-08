using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eShop.UI.Controllers
{
    public class TestController : Controller
    {
        private IRepository<Product> _contextProduct;

        public TestController(IRepository<Product> product)
        {
            _contextProduct = product;
        }

        public ActionResult Index()
        {
            return View(_contextProduct.Collection().ToList());
        }
    }
}