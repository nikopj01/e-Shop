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

        /// <summary>
        /// Method to display basket page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                BasketViewModel viewModel = new BasketViewModel()
                {
                    basketItems = basketService.GetBasketItems(Guid.Parse(Session["UserAccountID"].ToString())),
                    basketSubTotal = basketService.GetBasketSubTotal(Guid.Parse(Session["UserAccountID"].ToString()))
                };
                return View(viewModel);
            }   
            else
            {
                return RedirectToAction("Login", "UserAccount", new { returnUrl = "/Basket"});
            }
        }

        /// <summary>
        /// Method to update quantity of basket item (Add 1 / Delete 1 / Add n <inputted quantity>)
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="sizeQuantityID"></param>
        /// <param name="quantity"></param>
        /// <param name="isUpdate"></param>
        public void UpdateBasketItem(string productID, string sizeQuantityID, int? quantity, bool? isUpdate)
        {
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer" 
                && productID != null && sizeQuantityID != null && quantity != null && isUpdate != null)
            {
                string userAccountID = Session["UserAccountID"].ToString();
                basketService.UpdateBasketItem(productID, sizeQuantityID, userAccountID, quantity, isUpdate);
            }
            else
            {
                RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Method to delete selected basket item
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="sizeQuantityID"></param>
        public void RemoveBasketItem(string productID, string sizeQuantityID)
        {
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer"
                && productID != null && sizeQuantityID != null)
            {
                string userAccountID = Session["UserAccountID"].ToString();
                basketService.RemoveBasketItem(productID, sizeQuantityID, userAccountID);
            }
            else
            {
                RedirectToAction("Index");
            }
        }

        public PartialViewResult BasketPartial()
        {
            BasketItemCountViewModel model = new BasketItemCountViewModel();
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                IEnumerable<BasketItem> basketItems = basketService.GetBasketItems(Guid.Parse(Session["UserAccountID"].ToString()));
                
                if(basketItems.Count() > 0)
                {
                    foreach (var item in basketItems)
                        model.basketItemCount += 1;
                }
            }
            return PartialView(model);
        }
    }
}