using eShop.Core.Contracts;
using eShop.Core.Models;
using eShop.Core.ViewModels;
using eShop.Services;
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

        public ProductManagerController(IRepository<Product> product, IRepository<ProductCategory> productCategory, 
            IRepository<ProductType> productType, IRepository<SizeQuantity> sizeQuantity)
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
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                var viewModel = new ListOfProductViewModel()
                {
                    products = _contextProduct.Collection().Where(p => p.IsActive == true).ToList(),
                    productCategories = _contextProductCategory.Collection().ToList(),
                    productTypes = _contextProductType.Collection().ToList()
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }

        }

        /// <summary>
        /// Method to display "Create product" page
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateProduct()
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                ProductFormViewModel viewModel = new ProductFormViewModel()
                {
                    product = new Product(),
                    sizeQuantity = new SizeQuantity(),
                    productCategories = _contextProductCategory.Collection().ToList(),
                    sizeQuantities = new List<SizeQuantity>()
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// Method to get list of ProductCategory
        /// </summary>
        /// <param name="productCategoryID"></param>
        /// <returns></returns>
        public JsonResult GetProductType(string productCategoryID)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                List<ProductType> productTypeList = _contextProductType.Collection().Where(pt => pt.ProductCategoryID.ToString() == productCategoryID).ToList();
                List<SelectListItem> productTypeNames = new List<SelectListItem>();
                productTypeList.ForEach(pt =>
                {
                    productTypeNames.Add(new SelectListItem { Text = pt.ProductTypeName, Value = pt.ProductTypeID.ToString() });
                });
                return Json(productTypeNames, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to Create a product
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(FormCollection formCollection, HttpPostedFileBase file)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
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
                if (formCollection["product.ProductTypeID"] != null)
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
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// Method to delete product based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProduct(Guid id)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                Product product = _contextProduct.Find(id);
                product.ModifiedAt = DateTime.Now;
                product.IsActive = false;
                _contextProduct.Commit();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// method to display "edit product" page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProduct(Guid id)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                ProductFormViewModel viewModel = new ProductFormViewModel
                {
                    product = _contextProduct.Find(id),
                    productCategories = _contextProductCategory.Collection().ToList()
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
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
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
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
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// method to Display list of product Category
        /// </summary>
        /// <returns></returns>
        public ActionResult ListOfProductCategory()
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                IEnumerable<ProductCategory> model = _contextProductCategory.Collection().ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// method to Create product Category Page
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateProductCategory()
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                ProductCategory model = new ProductCategory();
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
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
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                //Find product category that has a same "product category name"
                ProductCategory selectedProductCategory = _contextProductCategory.Collection().SingleOrDefault(pc => pc.ProductCategoryName.ToLower() == productCategory.ProductCategoryName.ToLower());
                if (selectedProductCategory == null)
                {
                    productCategory.ProductCategoryID = Guid.NewGuid();
                    productCategory.CreatedAt = DateTime.Now;
                    _contextProductCategory.Insert(productCategory);
                    _contextProductCategory.Commit();
                }
                return RedirectToAction("ListOfProductCategory");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// Method to delete product based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProductCategory(Guid id)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                _contextProductCategory.Delete(id);
                _contextProductCategory.Commit();
                return RedirectToAction("ListOfProductCategory");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// method to display "edit product" page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProductCategory(Guid id)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                ProductCategory productCategory = _contextProductCategory.Find(id);
                return View(productCategory);
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
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
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                ProductCategory selectedProduct = _contextProductCategory.Find(productCategory.ProductCategoryID);
                selectedProduct.ProductCategoryName = productCategory.ProductCategoryName;
                selectedProduct.ModifiedAt = DateTime.Now;
                _contextProductCategory.Commit();

                return RedirectToAction("ListOfProductCategory");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// method to Display list of product Type
        /// </summary>
        /// <returns></returns>
        public ActionResult ListOfProductType()
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                ListOfProductTypeViewModel viewModel = new ListOfProductTypeViewModel()
                {
                    productTypes = _contextProductType.Collection().ToList(),
                    productCategories = _contextProductCategory.Collection().ToList()
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// method to Create product Type Page
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateProductType()
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                ProductTypeFormViewModel viewModel = new ProductTypeFormViewModel()
                {
                    productType = new ProductType(),
                    productCategories = _contextProductCategory.Collection().ToList()
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
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
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                //Find product category that has a same "product category name"
                ProductType selectedProductType = _contextProductType.Collection().SingleOrDefault(pc => pc.ProductTypeName.ToLower() == productType.ProductTypeName.ToLower());
                if (selectedProductType == null)
                {
                    ProductType AddedProductType = new ProductType();
                    AddedProductType.ProductTypeID = Guid.NewGuid();
                    AddedProductType.ProductTypeName = productType.ProductTypeName;
                    AddedProductType.ProductCategoryID = productType.ProductCategoryID;
                    AddedProductType.CreatedAt = DateTime.Now;
                    _contextProductType.Insert(AddedProductType);
                    _contextProductType.Commit();
                }
                return RedirectToAction("ListOfProductType");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// Method to delete product based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProductType(Guid id)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                _contextProductType.Delete(id);
                _contextProductType.Commit();
                return RedirectToAction("ListOfProductType");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// method to display "edit product" page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProductType(Guid id)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                ProductType productType = _contextProductType.Find(id);
                return View(productType);
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
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
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                ProductType selectedProduct = _contextProductType.Find(productType.ProductTypeID);
                selectedProduct.ProductTypeName = productType.ProductTypeName;
                selectedProduct.ModifiedAt = DateTime.Now;
                _contextProductType.Commit();

                return RedirectToAction("ListOfProductType");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// method to display page to create size and quantity for product
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult CreateProductSizeQuantity(Guid productId)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {

                if (productId != null && (_contextProduct.Find(productId) != null))
                {
                    ProductSizeQuantityFormViewModel viewModel = new ProductSizeQuantityFormViewModel()
                    {
                        sizeQuantity = new SizeQuantity(),
                        productID = productId
                    };

                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
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
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                SizeQuantity sizeQuantity = new SizeQuantity();
                sizeQuantity.SizeQuantityID = Guid.NewGuid();
                sizeQuantity.SizeName = formCollection["sizeQuantity.SizeName"];
                sizeQuantity.ProductID = Guid.Parse(formCollection["productID"]);
                sizeQuantity.Quantity = int.Parse(formCollection["sizeQuantity.Quantity"]);
                sizeQuantity.CreatedAt = DateTime.Now;
                _contextSizeQuantity.Insert(sizeQuantity);
                _contextSizeQuantity.Commit();

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

        /// <summary>
        /// Method to display page to edit the size and the quantity of the product
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult EditProductSizeQuantity(Guid Id)
        {
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                if (Id != null && (_contextProduct.Find(Id) != null))
                {
                    List<SizeQuantity> model = _contextSizeQuantity.Collection().Where(sq => sq.ProductID == Id).ToList();
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
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
            if (Session["UserRole"] != null && Session["UserRole"] as string == "Admin")
            {
                foreach (var item in sizeQuantity)
                {
                    SizeQuantity selectedSizeQuantity = _contextSizeQuantity.Find(item.SizeQuantityID);
                    selectedSizeQuantity.SizeName = item.SizeName;
                    selectedSizeQuantity.Quantity = item.Quantity;
                    selectedSizeQuantity.ModifiedAt = DateTime.Now;
                }
                _contextSizeQuantity.Commit();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "UserAccount");
            }
        }

    }
}