using eShop.Core.Contracts;
using eShop.Core.Models;
using eShop.Core.ViewModels;
using eShop.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace eShop.UI.Controllers
{
    public class UserAccountController : Controller
    {
        private IRepository<UserAccount> _contextUserAccount;
        private UserAccountService userAccountS;

        public UserAccountController(IRepository<UserAccount> userAccount,UserAccountService userAccountService)
        {
            _contextUserAccount = userAccount;
            userAccountS = userAccountService;
        }

        /// <summary>
        /// Method to redirect to default home page if the return url not valid
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Method to display UserAccount Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserAccountID"] != null)
            {
                Guid? userAccountID = Session["UserAccountID"] as Guid?;
                UserAccountViewModel viewModel = userAccountS.GetUserAccountViewModel(userAccountID);
                return View(viewModel);
            } 
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Method to display Edit Profile Page
        /// </summary>
        /// <returns></returns>
        public ActionResult EditProfile()
        {
            if (Session["UserAccountID"] != null)
            {
                Guid? userAccountID = Session["UserAccountID"] as Guid?;
                EditProfileFormModel model = userAccountS.GetEditProfileFormModel(userAccountID);
                return View(model);
            }
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Method to edit profile data
        /// </summary>
        /// <param name="editProfileFormModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EditProfileFormModel editProfileFormModel)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserAccountID"] != null)
                {
                    UserAccount selectedUserAccount = _contextUserAccount.Collection().Include(ua => ua.UserRole).SingleOrDefault(ua => ua.UserAccountID == editProfileFormModel.UserAccountID);

                    selectedUserAccount.FirstName = editProfileFormModel.FirstName;
                    selectedUserAccount.LastName = editProfileFormModel.LastName;
                    if (editProfileFormModel.UserPassword != null)
                        selectedUserAccount.UserPassword = userAccountS.ComputeSha256Hash(editProfileFormModel.UserPassword);
                    selectedUserAccount.ModifiedAt = DateTime.Now;
                    _contextUserAccount.Commit();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Login");
            }
            return View(editProfileFormModel);
        }

        public ActionResult AddAddress()
        {
            if(Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                UserAddressFormModel viewModel = new UserAddressFormModel();
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAddress(UserAddressFormModel UserAddressFormModel)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
                {
                    userAccountS.AddUserAddress(UserAddressFormModel, Session["UserAccountID"] as Guid?);
                }
                return RedirectToAction("Index");
            }
            return View(UserAddressFormModel);
        }

        public ActionResult EditAddress(Guid userAddressID)
        {
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                UserAddressFormModel viewModel = userAccountS.GetEditUserAddressForm(userAddressID);
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAddress(UserAddressFormModel userAddressFormModel)
        {
            
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                if (ModelState.IsValid)
                {
                    userAccountS.EditUserAddress(userAddressFormModel);
                    return RedirectToAction("Index");
                }
                return View(userAddressFormModel);
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteAddress(Guid userAddressID)
        {
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                userAccountS.DeleteUserAddress(userAddressID);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Method to display register page
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            RegisterFormModel model = new RegisterFormModel();
            return View(model);
        }

        /// <summary>
        /// Method to register new user
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterFormModel registerFormModel)
        {
            string registerMessage = null;
            if (ModelState.IsValid)
            {
                //Validate inputted username & email and Register User
                registerMessage = userAccountS.RegisterUser(registerFormModel);

                if (registerMessage == null)
                {
                    //Valid username & email
                    return RedirectToAction("Login");
                }
                else
                {
                    //Invalid username & email
                    ViewBag.RegisterMessage = registerMessage;
                    return View(registerFormModel);
                }
            }
            return View(registerFormModel);
        }

        /// <summary>
        /// method to display Login page
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Login(string returnUrl)
        {
            if (Session["UserAccountID"] == null)
            {
                LoginFormModel model = new LoginFormModel();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Method to validate inputted login form
        /// </summary>
        /// <param name="loginFormModel"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginFormModel loginFormModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Guid? userAccountID = userAccountS.LoginUser(loginFormModel);
                if (userAccountID != null)
                {
                    //Existed user
                    Session["UserAccountID"] = userAccountID;
                    Session["UserRole"] = userAccountS.CheckUserRole(userAccountID);
                    if (Session["UserRole"] as string == "Customer")
                        return RedirectToLocal(returnUrl);
                    else
                        return RedirectToAction("Index", "ProductManager");
                }
                //Not existed user
                ViewBag.LoginMessage = "Wrong Username or Password";
            }
            return View(loginFormModel);
        }

        /// <summary>
        /// Method to log out an user
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Logout(string returnUrl)
        {
            string userRole = Session["UserRole"] as string;
            Session["UserRole"] = null;
            Session["UserAccountID"] = null;

            if (userRole == "Admin")
                //Admin user
                return RedirectToAction("Index", "Home");

            //Customer user
            return RedirectToLocal(returnUrl);
        }

    }
}