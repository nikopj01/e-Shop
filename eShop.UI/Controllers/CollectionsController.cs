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

        /// <summary>
        /// Method to display collection of Men-Top product
        /// </summary>
        /// <returns></returns>
        public ActionResult MenTops()
        {
            IEnumerable<Product> model = _contextProduct.Collection()
                .Where(p => p.ProductType.ProductTypeName == "Top" && p.IsActive == true).ToList();
            //IEnumerable<Product> model = _contextProduct.Collection()
            //    .Where(p => p.ProductType.ProductTypeName.Contains("op") && p.IsActive == true).ToList();
            return View(model);
        }

        /// <summary>
        /// Method to display certain product based on productID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public ActionResult Product(Guid productID)
        {
            ProductDisplayViewModel viewModel = new ProductDisplayViewModel()
            {
                product = _contextProduct.Collection().SingleOrDefault(p => p.ProductID == productID && p.IsActive == true),
                sizeQuantities = _contextSizeQuantity.Collection().Include(sq => sq.Product).Where(sq => sq.ProductID == productID && sq.Quantity > 0 && sq.Product.IsActive == true).OrderBy(sq => sq.SizeName).ToList()
            };
            return View(viewModel);
        }
    }
}