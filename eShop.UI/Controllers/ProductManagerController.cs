using eShop.Core.Contracts;
using eShop.Core.Models;
using eShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eShop.UI.Controllers
{
    public class ProductManagerController : Controller
    {
        private IRepository<Product> _contextProduct;
        private IRepository<ProductCategory> _contextProductCategory;

        public ProductManagerController(IRepository<Product> product, IRepository<ProductCategory> productCategory)
        {
            _contextProduct = product;
            _contextProductCategory = productCategory;
        }

        /// <summary>
        /// Method to Display list of product
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(_contextProduct.Collection().ToList());
        }

        /// <summary>
        /// Method to display "Create product" page
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateProduct()
        {
            ProductForm viewModel = new ProductForm()
            {
                product = new Product(),
                productCategories = _contextProductCategory.Collection().ToList()

            };

            return View(viewModel);
        }

        /// <summary>
        /// Method to Create a product
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product product)
        {
            if (product.Id == null)
            {
                product.Id = Guid.NewGuid().ToString();
                product.CreatedAt = DateTime.Now;
            }

            _contextProduct.Insert(product);
            _contextProduct.Commit();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Method to delete product based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProduct(string id)
        {
            _contextProduct.Delete(id);
            _contextProduct.Commit();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// method to display "edit product" page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProduct(string id)
        {
            ProductForm viewModel = new ProductForm
            {
                product = _contextProduct.Find(id),
                productCategories = _contextProductCategory.Collection().ToList()
            };
            
            return View(viewModel);
        }

        /// <summary>
        /// method to edit selected product 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product product)
        {
            Product selectedProduct = _contextProduct.Find(product.Id);
            selectedProduct.ProductName = product.ProductName;
            selectedProduct.ProductDescription = product.ProductDescription;
            selectedProduct.ProductPrice = product.ProductPrice;
            selectedProduct.ProductImage = product.ProductImage;
            selectedProduct.ProductDiscount = product.ProductDiscount;
            selectedProduct.productCategory_Id = product.productCategory_Id;
            _contextProduct.Commit();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// method to Display list of product Category
        /// </summary>
        /// <returns></returns>
        public ActionResult ListOfProductCategory()
        {
            return View(_contextProductCategory.Collection().ToList());
        }

        /// <summary>
        /// method to Create product Category Page
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateProductCategory()
        {
            ProductCategory model = new ProductCategory();
            return View(model);
        }

        /// <summary>
        /// method to Create product category
        /// </summary>
        /// <param name="productCategory"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProductCategory(ProductCategory productCategory)
        {
            if (productCategory.Id == null)
            {
                productCategory.Id = Guid.NewGuid().ToString();
                productCategory.CreatedAt = DateTime.Now;
            }
            _contextProductCategory.Insert(productCategory);
            _contextProductCategory.Commit();

            return RedirectToAction("ListOfProductCategory");
        }

        /// <summary>
        /// Method to delete product based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProductCategory(string id)
        {
            _contextProductCategory.Delete(id);
            _contextProductCategory.Commit();
            return RedirectToAction("ListOfProductCategory");
        }

        /// <summary>
        /// method to display "edit product" page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProductCategory(string id)
        {
            ProductCategory productCategory = _contextProductCategory.Find(id);
            return View(productCategory);
        }

        /// <summary>
        /// method to edit selected product 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductCategory(ProductCategory productCategory)
        {
            ProductCategory selectedProduct = _contextProductCategory.Find(productCategory.Id);
            selectedProduct.ProductCategoryDescription = productCategory.ProductCategoryDescription;
            _contextProductCategory.Commit();

            return RedirectToAction("ListOfProductCategory");
        }
    }
}