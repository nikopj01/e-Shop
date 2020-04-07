using eShop.Core.ViewModels;
using eShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eShop.UI.Controllers
{
    public class OrderController : Controller
    {
        private BasketService basketService;
        private OrderService orderService;

        public OrderController(BasketService basketS, OrderService orderS)
        {
            basketService = basketS;
            orderService = orderS;
        }

        /// <summary>
        /// Method to display order page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                Guid? userAddressID = Session["UserAccountID"] as Guid?;

                OrderViewModel viewModel = new OrderViewModel()
                {
                    basketItems = basketService.GetBasketItems(userAddressID),
                    basketSubTotal = basketService.GetBasketSubTotal(userAddressID),
                    userAddresses = orderService.GetUserAddress(userAddressID)
                };
                if (viewModel.basketItems.Count() > 0)
                {
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Index", "Basket");
                }
            }
            else
            {
                return RedirectToAction("Login", "UserAccount", new { returnUrl = "/Order" });
            }
        }

        /// <summary>
        /// Method to apply inputted discount code to subtotal
        /// </summary>
        /// <param name="discountCode"></param>
        /// <returns></returns>
        public JsonResult ApplyDiscountCode(string discountCode)
        {
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                decimal? subTotal = orderService.ApplyDiscountCode(Session["UserAccountID"] as Guid?, discountCode);
                return Json(subTotal, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to place order
        /// </summary>
        /// <param name="orderViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(OrderViewModel orderViewModel)
        {
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                string discountCode = orderViewModel.InputtedDiscountCode;
                Guid? userAccountID = Session["UserAccountID"] as Guid?;
                Guid? selectedAddressID = orderViewModel.selectedAddressID;
                orderService.PlaceOrder(userAccountID, discountCode, selectedAddressID);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount", new { returnUrl = "/Order" });
            }

        }



    }
}