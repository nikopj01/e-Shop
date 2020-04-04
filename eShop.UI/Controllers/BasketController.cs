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
        private BasketService basketService;

        public BasketController(BasketService basketS)
        {
            basketService = basketS;
        }

        public ActionResult Index()
        {
            if (Session["UserAccountID"] != null)
            {
                BasketItemViewModel viewModel = new BasketItemViewModel()
                {
                    basketItems = basketService.GetBasketItems(Guid.Parse(Session["UserAccountID"].ToString())),
                    basketSubTotal = basketService.GetBasketSummary(Guid.Parse(Session["UserAccountID"].ToString()))
                };
                return View(viewModel);
            }   
            else
            {
                return RedirectToAction("Login", "UserAccount", new { returnUrl = "/Basket/Index"});
            }
        }

        public void UpdateBasketItem(string productID, string sizeQuantityID, int? quantity, bool? isUpdate)
        {
            if (Session["UserAccountID"] != null && productID != null && sizeQuantityID != null && quantity != null && isUpdate != null)
            {
                string userAccountID = Session["UserAccountID"].ToString();
                basketService.UpdateBasketItem(productID, sizeQuantityID, userAccountID, quantity, isUpdate);
            }
            else
            {
                RedirectToAction("Index");
            }
        }

        public void RemoveBasketItem(string productID, string sizeQuantityID)
        {
            if (Session["UserAccountID"] != null && productID != null && sizeQuantityID != null)
            {
                string userAccountID = Session["UserAccountID"].ToString();
                basketService.RemoveBasketItem(productID, sizeQuantityID, userAccountID);
            }
            else
            {
                RedirectToAction("Index");
            }
        }
    }
}