//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eShop.Core.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BasketItem
    {
        public System.Guid BasketItemID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<System.Guid> BasketID { get; set; }
        public Nullable<System.Guid> SizeQuantityID { get; set; }
        public Nullable<System.Guid> ProductID { get; set; }
    
        public virtual Basket Basket { get; set; }
        public virtual Product Product { get; set; }
        public virtual SizeQuantity SizeQuantity { get; set; }
    }
}
