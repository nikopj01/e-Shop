using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eShop.Services;

namespace eShop.UI.Controllers
{
    public class BasketController : Controller
    {
        private IRepository<Basket> _contextBasket;
        private BasketService basketService;

        public BasketController(IRepository<Basket> basketI, BasketService basketS)
        {
            _contextBasket = basketI;
            basketService = basketS;
        }

        [HttpPost]
        public ActionResult AddToBasket(FormCollection formCollection)
        {
            string ProductID = formCollection["product.ProductID"];
            string SizeQuantityID = formCollection["Size"];
            string UserAccountID = Session["UserAccountID"].ToString();
            basketService.AddItemToBasket(ProductID, SizeQuantityID, UserAccountID);
            return RedirectToAction("Product","Collections", new { Id = Guid.Parse(ProductID) });
        }
    }
}