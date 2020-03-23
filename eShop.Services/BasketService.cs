using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Services
{
    public class BasketService
    {
        private IRepository<Basket> _contextBasket;
        private IRepository<BasketItem> _contextBasketItem;

        public BasketService(IRepository<Basket> basket, IRepository<BasketItem> basketItem)
        {
            _contextBasket = basket;
            _contextBasketItem = basketItem;
        }

        public Basket GetBasket(string userAccountID)
        {
            if(userAccountID != null)
            {
                // user
                Guid UserAccountID = Guid.Parse(userAccountID);
                Basket selectedBasket = _contextBasket.Collection().SingleOrDefault(b => b.UserAccountID == UserAccountID);
                if (selectedBasket == null)
                {
                    Basket newBasket = new Basket();
                    newBasket.BasketID = Guid.NewGuid();
                    newBasket.UserAccountID = UserAccountID;
                    _contextBasket.Insert(newBasket);
                    _contextBasket.Commit();
                    return newBasket;
                }
                return selectedBasket;
            }
            else
            {
                //// non-user
                //ObjectCache cache = MemoryCache.Default;
                //BasketInMemory basketInMemory = cache["basket"] as BasketInMemory;
                //if (basketInMemory == null)
                //    basketInMemory = new BasketInMemory();
                //// ------
                return new Basket();
            }

            
        }

        public void AddItemToBasket(string productID, string sizeQuantityID, string userAccountID)
        {
            Guid ProductID = Guid.Parse(productID);
            Guid SizeQuantityID = Guid.Parse(sizeQuantityID);

            //Get Basket
            Basket selectedBasket = GetBasket(userAccountID);

            //Check existance of productID in the basket
            BasketItem selectedBasketItem = _contextBasketItem.Collection().SingleOrDefault(bi => bi.BasketID == selectedBasket.BasketID && bi.ProductID == ProductID);
            if (selectedBasketItem != null)
            {
                //exist
                selectedBasketItem.Quantity += 1;
            }
            else
            {
                //not exist
                BasketItem basketItem = new BasketItem();
                basketItem.BasketItemID = Guid.NewGuid();
                basketItem.Quantity = 1;
                basketItem.BasketID = selectedBasket.BasketID;
                basketItem.SizeQuantityID = SizeQuantityID;
                basketItem.ProductID = ProductID;
                _contextBasketItem.Insert(basketItem);
            }
            _contextBasketItem.Commit();
        }
    }
}
