using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eShop.Services;
using eShop.Core.ViewModels;
using System.Data.Entity;

namespace eShop.UI.Controllers
{
    public class BasketController : Controller
    {
        //private IRepository<Basket> _contextBasket;
        //private IRepository<BasketItem> _contextBasketItem;
        private BasketService basketService;

        public BasketController(IRepository<Basket> basket, IRepository<BasketItem> basketItem, BasketService basketS)
        {
            //_contextBasket = basket;
            //_contextBasketItem = basketItem;
            basketService = basketS;
        }

        public ActionResult Index()
        {
            if (Session["UserAccountID"] != null)
            {
                List<BasketItem> model = basketService.GetBasketItems(Guid.Parse(Session["UserAccountID"].ToString()));
                return View(model);
            }   
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        //[HttpPost]
        //public ActionResult AddBasketItem(FormCollection formCollection)
        //{
        //    string ProductID = formCollection["product.ProductID"];
        //    string SizeQuantityID = formCollection["Size"];
        //    string UserAccountID = Session["UserAccountID"].ToString();
        //    basketService.AddBasketItem(ProductID, SizeQuantityID, UserAccountID);
        //    return RedirectToAction("Product", "Collections", new { Id = Guid.Parse(ProductID) });
        //}

        public JsonResult AddBasketItem(string productID, string sizeQuantityID)
        {
            int selectedBasketItemQuantity= 0;
            if (Session["UserAccountID"] != null)
            {
                string UserAccountID = Session["UserAccountID"].ToString();
                selectedBasketItemQuantity = basketService.AddBasketItem(productID, sizeQuantityID, UserAccountID);
            }

            return Json(selectedBasketItemQuantity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReduceBasketItem(string productID, string sizeQuantityID)
        {
            int selectedBasketItemQuantity = 0;
            if (Session["UserAccountID"] != null)
            {
                string UserAccountID = Session["UserAccountID"].ToString();
                selectedBasketItemQuantity = basketService.ReduceBasketItem(productID, sizeQuantityID, UserAccountID);
            }

            return Json(selectedBasketItemQuantity, JsonRequestBehavior.AllowGet);
        }

        public void RemoveBasketItem(string productID, string sizeQuantityID)
        {
            if (Session["UserAccountID"] != null)
            {
                string UserAccountID = Session["UserAccountID"].ToString();
                basketService.RemoveBasketItem(productID, sizeQuantityID, UserAccountID);
            }
        }
    }
}