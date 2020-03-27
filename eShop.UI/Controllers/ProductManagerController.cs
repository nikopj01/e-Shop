using eShop.Core.Contracts;
using eShop.Core.Models;
using eShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eShop.UI.Controllers
{
    public class ProductManagerController : Controller
    {
        private IRepository<Product> _contextProduct;
        private IRepository<ProductCategory> _contextProductCategory;
        private IRepository<ProductType> _contextProductType;
        private IRepository<SizeQuantity> _contextSizeQuantity;

        public ProductManagerController(IRepository<Product> product, IRepository<ProductCategory> productCategory, IRepository<ProductType> productType, IRepository<SizeQuantity> sizeQuantity)
        {
            _contextProduct = product;
            _contextProductCategory = productCategory;
            _contextProductType = productType;
            _contextSizeQuantity = sizeQuantity;
        }

        /// <summary>
        /// Method to Display list of product
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //List<Product> model = _contextProduct.Collection().Where(pc => pc.IsActive == true).ToList();
            var viewModel = new ListOfProductViewModel()
            {
                products = _contextProduct.Collection().Where(p => p.IsActive == true).ToList(),
                productCategories = _contextProductCategory.Collection().Where(pc => pc.IsActive == true).ToList(),
                productTypes = _contextProductType.Collection().Where(pt => pt.IsActive == true).ToList()
            };
            return View(viewModel);

        }

        /// <summary>
        /// Method to display "Create product" page
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateProduct()
        {
            ProductFormViewModel viewModel = new ProductFormViewModel()
            {
                product = new Product(),
                sizeQuantity = new SizeQuantity(),
                productCategories = _contextProductCategory.Collection().Where(pc => pc.IsActive == true).ToList(),
                sizeQuantities = new List<SizeQuantity>()
            };
            return View(viewModel);
        }

        /// <summary>
        /// Method to get list of ProductCategory
        /// </summary>
        /// <param name="productCategoryID"></param>
        /// <returns></returns>
        public JsonResult GetProductType(string productCategoryID)
        {
            List<ProductType> productTypeList = _contextProductType.Collection().Where(pt => pt.IsActive == true && pt.ProductCategoryID.ToString() == productCategoryID).ToList();
            List<SelectListItem> productTypeNames = new List<SelectListItem>();
            productTypeList.ForEach(pt =>
            {
                productTypeNames.Add(new SelectListItem { Text = pt.ProductTypeName, Value = pt.ProductTypeID.ToString() });
            });
            return Json(productTypeNames, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to Create a product
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult CreateProduct(FormCollection formCollection, HttpPostedFileBase file)
        {
            //Assign formcollection value to Product object
            Product newProduct = new Product();
            var productId = Guid.NewGuid();
            newProduct.ProductID = productId;
            newProduct.ProductName = formCollection["product.ProductName"];
            newProduct.ProductDescription = formCollection["product.ProductDescription"];
            newProduct.ProductPrice = decimal.Parse(formCollection["product.ProductPrice"]);
            newProduct.ProductImage = productId + Path.GetExtension(file.FileName);
            file.SaveAs(Server.MapPath("//Content//ProductImages//") + newProduct.ProductImage);

            if (formCollection["product.ProductDiscount"] != null)
                newProduct.ProductDiscount = decimal.Parse(formCollection["product.ProductDiscount"]);

            newProduct.ProductCategoryID = Guid.Parse(formCollection["product.ProductCategoryID"]);
            if(formCollection["product.ProductTypeID"] != null)
                newProduct.ProductTypeID = Guid.Parse(formCollection["product.ProductTypeID"]);
            newProduct.CreatedAt = DateTime.Now;
            newProduct.IsActive = true;
            //add new product
            _contextProduct.Insert(newProduct);
            _contextProduct.Commit();


            //Dynamic Add size & quantity
            //if (!string.IsNullOrWhiteSpace(formCollection["sizeQuantity.SizeDescription"].Replace(",", "")))
            //    if (formCollection["sizeQuantity.SizeName"] != null)
            //    {
            //        add size of product
            //        string[] listofSize = formCollection["sizeQuantity.SizeDescription"].Split(',');
            //        var listofQuantity = formCollection["sizeQuantity.Quantity"].Replace(",", "").ToCharArray();

            //        int currentIndex = 0;
            //        foreach (var item in listofSize)
            //        {
            //            SizeQuantity sizeQuantity = new SizeQuantity();
            //            sizeQuantity.SizeQuantityID = Guid.NewGuid();
            //            sizeQuantity.SizeName = item.ToString();
            //            sizeQuantity.SizeQuantityID = productId;
            //            sizeQuantity.Quantity = int.Parse(listofQuantity[currentIndex].ToString());
            //            sizeQuantity.CreatedAt = DateTime.Now;
            //            _contextSizeQuantity.Insert(sizeQuantity);
            //            _contextSizeQuantity.Commit();

            //            currentIndex += 1;
            //        }
            //    }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Method to delete product based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProduct(Guid id)
        {
            Product product = _contextProduct.Find(id);
            product.IsActive = false;
            _contextProduct.Commit();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// method to display "edit product" page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProduct(Guid id)
        {
            ProductFormViewModel viewModel = new ProductFormViewModel
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
            Product selectedProduct = _contextProduct.Find(product.ProductID);
            selectedProduct.ProductName = product.ProductName;
            selectedProduct.ProductDescription = product.ProductDescription;
            selectedProduct.ProductPrice = product.ProductPrice;
            selectedProduct.ProductImage = product.ProductImage;
            selectedProduct.ProductDiscount = product.ProductDiscount;
            selectedProduct.ProductCategoryID = product.ProductCategoryID;
            selectedProduct.ModifiedAt = DateTime.Now;
            _contextProduct.Commit();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// method to Display list of product Category
        /// </summary>
        /// <returns></returns>
        public ActionResult ListOfProductCategory()
        {
            IEnumerable<ProductCategory> model = _contextProductCategory.Collection().Where(pc => pc.IsActive == true).ToList();
            return View(model);
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
            //Find product category that has a same "product category name"
            ProductCategory selectedProductCategory = _contextProductCategory.Collection().SingleOrDefault(pc => pc.ProductCategoryName.ToLower() == productCategory.ProductCategoryName.ToLower());
            if (selectedProductCategory != null)
            {
                selectedProductCategory.IsActive = true;
            }
            else
            {
                productCategory.ProductCategoryID = Guid.NewGuid();
                productCategory.CreatedAt = DateTime.Now;
                productCategory.IsActive = true;
                _contextProductCategory.Insert(productCategory);
                _contextProductCategory.Commit();
            }

            return RedirectToAction("ListOfProductCategory");
        }

        /// <summary>
        /// Method to delete product based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProductCategory(Guid id)
        {
            ProductCategory productCategory = _contextProductCategory.Find(id);
            productCategory.IsActive = false;
            _contextProductCategory.Commit();
            return RedirectToAction("ListOfProductCategory");
        }

        /// <summary>
        /// method to display "edit product" page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProductCategory(Guid id)
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
            ProductCategory selectedProduct = _contextProductCategory.Find(productCategory.ProductCategoryID);
            selectedProduct.ProductCategoryName = productCategory.ProductCategoryName;
            selectedProduct.ModifiedAt = DateTime.Now;
            _contextProductCategory.Commit();

            return RedirectToAction("ListOfProductCategory");
        }

        /// <summary>
        /// method to Display list of product Type
        /// </summary>
        /// <returns></returns>
        public ActionResult ListOfProductType()
        {
            ListOfProductTypeViewModel viewModel = new ListOfProductTypeViewModel()
            {
                productTypes = _contextProductType.Collection().Where(pt => pt.IsActive == true).ToList(),
                productCategories = _contextProductCategory.Collection().Where(pc => pc.IsActive == true).ToList()
            };
            return View(viewModel);
        }

        /// <summary>
        /// method to Create product Type Page
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateProductType()
        {
            ProductTypeFormViewModel viewModel = new ProductTypeFormViewModel()
            {
                productType = new ProductType(),
                productCategories = _contextProductCategory.Collection().ToList()
        };
            return View(viewModel);
        }

        /// <summary>
        /// method to Create product Type
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProductType(ProductType productType)
        {
            //Find product category that has a same "product category name"
            ProductType selectedProductType = _contextProductType.Collection().SingleOrDefault(pc => pc.ProductTypeName.ToLower() == productType.ProductTypeName.ToLower());
            if (selectedProductType != null)
            {
                selectedProductType.IsActive = true;
            }
            else
            {
                ProductType AddedProductType = new ProductType();
                AddedProductType.ProductTypeID = Guid.NewGuid();
                AddedProductType.ProductTypeName = productType.ProductTypeName;
                AddedProductType.ProductCategoryID = productType.ProductCategoryID;
                AddedProductType.CreatedAt = DateTime.Now;
                AddedProductType.IsActive = true;
                _contextProductType.Insert(AddedProductType);
                _contextProductType.Commit();
            }
            return RedirectToAction("ListOfProductType");
        }

        /// <summary>
        /// Method to delete product based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProductType(Guid id)
        {
            ProductType productType = _contextProductType.Find(id);
            productType.IsActive = false;
            _contextProductType.Commit();
            return RedirectToAction("ListOfProductType");
        }

        /// <summary>
        /// method to display "edit product" page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProductType(Guid id)
        {
            ProductType productType = _contextProductType.Find(id);
            return View(productType);
        }

        /// <summary>
        /// method to edit selected product 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductType(ProductType productType)
        {
            ProductType selectedProduct = _contextProductType.Find(productType.ProductTypeID);
            selectedProduct.ProductTypeName = productType.ProductTypeName;
            selectedProduct.ModifiedAt = DateTime.Now;
            _contextProductType.Commit();

            return RedirectToAction("ListOfProductType");
        }

        /// <summary>
        /// method to display page to create size and quantity for product
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult CreateProductSizeQuantity(Guid Id)
        {
            if(Id != null && (_contextProduct.Find(Id) != null))
            {
                ProductSizeQuantityFormViewModel viewModel = new ProductSizeQuantityFormViewModel()
                {
                    sizeQuantity = new SizeQuantity(),
                    productID = Id
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// method to create size and quantity for product
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProductSizeQuantity(FormCollection formCollection)
        {
            SizeQuantity sizeQuantity = new SizeQuantity();
            sizeQuantity.SizeQuantityID = Guid.NewGuid();
            sizeQuantity.SizeName = formCollection["sizeQuantity.SizeName"];
            sizeQuantity.ProductID = Guid.Parse(formCollection["productID"]);
            sizeQuantity.Quantity = int.Parse(formCollection["sizeQuantity.Quantity"]);
            sizeQuantity.CreatedAt = DateTime.Now;
            sizeQuantity.IsActive = true;
            _contextSizeQuantity.Insert(sizeQuantity);
            _contextSizeQuantity.Commit();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Method to display page to edit the size and the quantity of the product
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult EditProductSizeQuantity(Guid Id)
        {
            if (Id != null && (_contextProduct.Find(Id) != null))
            {
                List<SizeQuantity> model = _contextSizeQuantity.Collection().Where(sq => sq.ProductID == Id && sq.IsActive == true).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Method to edit size and quantity of the product
        /// </summary>
        /// <param name="sizeQuantity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductSizeQuantity(List<SizeQuantity> sizeQuantity)
        {
            foreach(var item in sizeQuantity)
            {
                SizeQuantity selectedSizeQuantity = _contextSizeQuantity.Find(item.SizeQuantityID);
                selectedSizeQuantity.SizeName = item.SizeName;
                selectedSizeQuantity.Quantity = item.Quantity;
                selectedSizeQuantity.ModifiedAt = DateTime.Now;
            }
            _contextSizeQuantity.Commit();
            return RedirectToAction("Index");
        }

    }
}