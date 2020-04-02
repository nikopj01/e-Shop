using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.Models
{
    public class ProductMetadata
    {
        [Required]
        [Display(Name=("Product Name"))]
        public string ProductName;

        [Display(Name=("Product Description"))]
        public string ProductDescription;

        [Required]
        [Display(Name=("Product Price"))]
        public decimal ProductPrice;

        [Required]
        [Display(Name=("Product Category"))]
        public string ProductCategoryID;

        //[Required]
        //[Display(Name = ("Product Supplier"))]
        //public string ProductSupplierID;

        //[Required]
        //[Display(Name=("Product Type"))]
        //public string productType_Id;
    }

    public class ProductCategoryMetadata
    {
        [Required]
        [Display(Name = ("Product Category Name"))]
        public string ProductCategoryName;
    }

    public class ProductTypeMetadata
    {
        [Required]
        [Display(Name = ("Product Type Name"))]
        public string ProductTypeName;

        [Required]
        [Display(Name = ("Product Category"))]
        public string ProductCategoryID;
    }

    public class ProductSupplierMetadata
    {
        [Required]
        [Display(Name = ("Supplier Name"))]
        public string ProductSupplierName;
    }

    public class SizeQuantityMetadata
    {
        [Required]
        [Display(Name = ("Size Name"))]
        public string SizeName;
    }

    public class UserAccountMetadata
    {
       
    }
}
