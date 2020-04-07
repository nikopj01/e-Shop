using eShop.Core.Contracts;
using eShop.Core.Models;
using eShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eShop.UI.Controllers
{
    public class CollectionsController : Controller
    {
        private IRepository<Product> _contextProduct;
        private IRepository<SizeQuantity> _contextSizeQuantity;

        public CollectionsController(IRepository<Product> product, IRepository<SizeQuantity> sizeQuantity)
        {
            _contextProduct = product;
            _contextSizeQuantity = sizeQuantity;
        }

        public ActionResult MenTops()
        {
            IEnumerable<Product> model = _contextProduct.Collection()
                .Where(p => p.ProductType.ProductTypeName == "Top" && p.IsActive == true).ToList();
            //IEnumerable<Product> model = _contextProduct.Collection()
            //    .Where(p => p.ProductType.ProductTypeName.Contains("op") && p.IsActive == true).ToList();
            return View(model);
        }

        public ActionResult Product(Guid Id)
        {
            ProductDisplayViewModel viewModel = new ProductDisplayViewModel()
            {
                product = _contextProduct.Collection().SingleOrDefault(p => p.ProductID == Id && p.IsActive == true),
                sizeQuantities = _contextSizeQuantity.Collection().Include(sq => sq.Product).Where(sq => sq.ProductID == Id && sq.Quantity > 0 && sq.Product.IsActive == true).OrderBy(sq => sq.SizeName).ToList()
            };
            return View(viewModel);
        }
    }
}