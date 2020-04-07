using eShop.Core.Contracts;
using eShop.Core.Models;
using eShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace eShop.Services
{
    public class BasketService
    {
        private IRepository<Product> _contextProduct;
        private IRepository<SizeQuantity> _contextSizeQuantities;
        private IRepository<Basket> _contextBasket;
        private IRepository<BasketItem> _contextBasketItem;

        public BasketService(IRepository<SizeQuantity> sizeQuantity, IRepository<Product> product, IRepository<Basket> basket, IRepository<BasketItem> basketItem)
        {
            _contextSizeQuantities = sizeQuantity;
            _contextProduct = product;
            _contextBasket = basket;
            _contextBasketItem = basketItem;
        }

        /// <summary>
        /// Method to get basket based on UserAccountID, if none, then basket will be created
        /// </summary>
        /// <param name="userAccountID"></param>
        /// <returns></returns>
        public Basket GetBasket(string userAccountID)
        {
            if (userAccountID != null)
            {
                // user
                Guid UserAccountID = Guid.Parse(userAccountID);
                Basket selectedBasket = _contextBasket.Collection().SingleOrDefault(b => b.UserAccountID == UserAccountID);
                if (selectedBasket == null)
                {
                    Basket newBasket = new Basket();
                    newBasket.BasketID = Guid.NewGuid();
                    newBasket.UserAccountID = UserAccountID;
                    newBasket.CreatedAt = DateTime.Now;
                    _contextBasket.Insert(newBasket);
                    _contextBasket.Commit();
                    return newBasket;
                }
                return selectedBasket;
            }
            else
            {
                return new Basket();
            }
        }

        /// <summary>
        /// Method to add or substract quantity of basket item
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="sizeQuantityID"></param>
        /// <param name="userAccountID"></param>
        /// <param name="quantity"></param>
        /// <param name="isUpdate"></param>
        public void UpdateBasketItem(string productID, string sizeQuantityID, string userAccountID, int? quantity, bool? isUpdate)
        {
            Guid ProductID = Guid.Parse(productID);
            Guid SizeQuantityID = Guid.Parse(sizeQuantityID);

            //Get Basket
            Basket selectedBasket = GetBasket(userAccountID);

            //Check existance of BasketItem based on productID & SizeQuantityID
            BasketItem selectedBasketItem = _contextBasketItem.Collection().
                SingleOrDefault(bi => bi.BasketID == selectedBasket.BasketID
                                        && bi.ProductID == ProductID
                                        && bi.SizeQuantityID == SizeQuantityID);

            if (selectedBasketItem != null)
            {
                //exist
                if (isUpdate == true)
                    selectedBasketItem.Quantity = quantity;
                else
                    selectedBasketItem.Quantity += quantity;
            }
            else
            {
                //not exist
                BasketItem basketItem = new BasketItem();
                basketItem.BasketItemID = Guid.NewGuid();
                basketItem.Quantity = quantity;
                basketItem.BasketID = selectedBasket.BasketID;
                basketItem.SizeQuantityID = SizeQuantityID;
                basketItem.ProductID = ProductID;
                basketItem.CreatedAt = DateTime.Now;
                _contextBasketItem.Insert(basketItem);
            }
            _contextBasketItem.Commit();
        }

        /// <summary>
        /// Method to delete selected basket item
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="sizeQuantityID"></param>
        /// <param name="userAccountID"></param>
        public void RemoveBasketItem(string productID, string sizeQuantityID, string userAccountID)
        {
            Guid ProductID = Guid.Parse(productID);
            Guid SizeQuantityID = Guid.Parse(sizeQuantityID);

            //Get Basket
            Basket selectedBasket = GetBasket(userAccountID);

            //Check existance of BasketItem based on productID & SizeQuantityID
            BasketItem selectedBasketItem = _contextBasketItem.Collection().
                SingleOrDefault(bi => bi.BasketID == selectedBasket.BasketID
                                        && bi.ProductID == ProductID
                                        && bi.SizeQuantityID == SizeQuantityID);
            if (selectedBasketItem != null)
            {
                //exist
                _contextBasketItem.Delete(selectedBasketItem.BasketItemID);
                _contextBasketItem.Commit();
            }
        }

        /// <summary>
        /// Method to get list of basket Item based on UserAccountID
        /// </summary>
        /// <param name="userAccountID"></param>
        /// <returns></returns>
        public List<BasketItem> GetBasketItems(Guid? userAccountID)
        {
            List<BasketItem> selectedBasketItems = new List<BasketItem>();

            Basket selectedBasket = _contextBasket.Collection().SingleOrDefault(b => b.UserAccountID == userAccountID);
            if(selectedBasket != null)
                selectedBasketItems = _contextBasketItem.Collection().Include(bi => bi.Product).Include(bi => bi.SizeQuantity).Include(bi => bi.Product).
                    Where(bi => bi.BasketID == selectedBasket.BasketID && bi.Product.IsActive == true).
                    OrderBy(bi => bi.Product.ProductName).ThenBy(bi => bi.SizeQuantity.SizeName).ToList();
            return selectedBasketItems;
        }

        /// <summary>
        /// Method to get subtotal of all basket item based on UserAccountID
        /// </summary>
        /// <param name="userAccountID"></param>
        /// <returns></returns>
        public decimal GetBasketSubTotal(Guid? userAccountID)
        {
            List<BasketItem> selectedBasketItems = new List<BasketItem>();
            decimal basketSubTotal = 0;
            Basket selectedBasket = _contextBasket.Collection().SingleOrDefault(b => b.UserAccountID == userAccountID);
            if (selectedBasket != null)
            {
                selectedBasketItems = _contextBasketItem.Collection().Include(bi => bi.Product).Where(bi => bi.BasketID == selectedBasket.BasketID).ToList();
                foreach (var basketItem in selectedBasketItems)
                {
                    basketSubTotal = basketSubTotal + (basketItem.Quantity * basketItem.Product.ProductPrice).Value;
                }
            }
            return basketSubTotal;
        }
    }
}
