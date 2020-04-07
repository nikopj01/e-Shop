using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Services
{
    public class OrderService
    {
        private BasketService basketS;
        private IRepository<DiscountCode> _contextDiscountCode;
        private IRepository<Order> _contextOrder;
        private IRepository<OrderItem> _contextOrderItem;
        private IRepository<Basket> _contextBasket;
        private IRepository<BasketItem> _contextBasketItem;
        private IRepository<UserAddress> _contextUserAddress;
        
        public OrderService(BasketService basketService, IRepository<DiscountCode> DiscountCode,
            IRepository<Order> order, IRepository<OrderItem> orderItem,IRepository<Basket> basket, 
            IRepository<BasketItem> basketItem, IRepository<UserAddress> userAddress)
        {
            basketS = basketService;
            _contextUserAddress = userAddress;
            _contextDiscountCode = DiscountCode;
            _contextOrder = order;
            _contextOrderItem = orderItem;
            _contextBasket = basket;
            _contextBasketItem = basketItem;
        }


        /// <summary>
        /// Method to get list of address of selected user
        /// </summary>
        /// <param name="userAccountID"></param>
        /// <returns></returns>
        public IEnumerable<UserAddress> GetUserAddress(Guid? userAccountID)
        {
            IEnumerable<UserAddress> userAddress = null;
            if (userAccountID != null)
                userAddress = _contextUserAddress.Collection().Where(ua => ua.UserAccountID == userAccountID).ToList();
            return userAddress;
        }

        /// <summary>
        /// Method to get discount percentage of inputted discount code
        /// </summary>
        /// <param name="discountCode"></param>
        /// <returns></returns>
        public decimal? GetDiscountPercentage(string discountCode)
        {
            decimal? discountPercentage = 0;
            DiscountCode selectedDiscountCode = _contextDiscountCode.Collection().SingleOrDefault(dc => dc.Code == discountCode);
            if (selectedDiscountCode != null)
            {
                discountPercentage = selectedDiscountCode.DiscountPercentage;
            }
            return discountPercentage;
        }

        /// <summary>
        /// Method to apply discount code to subtotal
        /// </summary>
        /// <param name="userAccountID"></param>
        /// <param name="discountCode"></param>
        /// <returns></returns>
        public decimal? ApplyDiscountCode(Guid? userAccountID, string discountCode)
        {
            decimal? discountPercentage = GetDiscountPercentage(discountCode);
            decimal? subTotal = 0;
            if (discountPercentage != 0)
            {
                // Valid discount code (Discount code is exist and within period of discount)
                subTotal = basketS.GetBasketSubTotal(userAccountID);
                subTotal = subTotal - (subTotal * (discountPercentage / 100));
            }
            return subTotal;
        }

        /// <summary>
        /// Method to add order and all basket item to order item & to delete basket and basket item data
        /// </summary>
        /// <param name="userAccountID"></param>
        /// <param name="discountCode"></param>
        /// <param name="selectedAddressID"></param>
        public void PlaceOrder(Guid? userAccountID, string discountCode, Guid? selectedAddressID)
        {
            //***Add new order***
            DiscountCode selectedDiscountCode = _contextDiscountCode.Collection().SingleOrDefault(dc => dc.Code == discountCode);
            decimal Total = basketS.GetBasketSubTotal(userAccountID);
            Guid newOrderID = Guid.NewGuid();
            Order newOrder = new Order();
            newOrder.OrderID = newOrderID;
            newOrder.UserAccountID = userAccountID;
            newOrder.UserAddressID = selectedAddressID;
            newOrder.Status = "Waiting For Confirmation";
            newOrder.OrderDate = DateTime.Now;
            newOrder.CreatedAt = DateTime.Now;
            if (selectedDiscountCode != null)
            {
                //Discount code is not empty
                newOrder.DiscountCodeID = selectedDiscountCode.DiscountCodeID;
                //Total with applied discount Code
                newOrder.Total = ApplyDiscountCode(userAccountID, discountCode);
            }
            else
            {
                //Discount code is empty 
                //Total without discount Code
                newOrder.Total = basketS.GetBasketSubTotal(userAccountID);
            }
            _contextOrder.Insert(newOrder);

            //Delete Basket
            Basket selectedBasket = _contextBasket.Collection().SingleOrDefault(b => b.UserAccountID == userAccountID);
            if (selectedBasket != null)
            {
                _contextBasket.Delete(selectedBasket.BasketID);
            }
                

            //***Add new Order Item***
            List<BasketItem> basketItems = basketS.GetBasketItems(userAccountID);
            if(basketItems != null)
            {
                foreach(var item in basketItems)
                {
                    OrderItem newOrderItem = new OrderItem();
                    newOrderItem.OrderItemID = Guid.NewGuid();
                    newOrderItem.Quantity = item.Quantity;
                    newOrderItem.SizeQuantityID = item.SizeQuantityID;
                    newOrderItem.ProductID = item.ProductID;
                    newOrderItem.OrderID = newOrderID;
                    newOrderItem.CreatedAt = DateTime.Now;
                    _contextOrderItem.Insert(newOrderItem);

                    //Delete Basket Item
                    _contextBasketItem.Delete(item.BasketItemID);
                }
            }

            //Commit changes on Order, OrderItem, Basket, BasketItem DbSet
            _contextBasketItem.Commit();
            _contextBasket.Commit();
            _contextOrder.Commit();
            _contextOrderItem.Commit();


        }


    }

}
