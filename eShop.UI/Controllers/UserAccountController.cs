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
                EditProfileFormViewModel model = userAccountS.GetEditProfileFormModel(userAccountID);
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
        public ActionResult EditProfile(EditProfileFormViewModel editProfileFormViewModel)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserAccountID"] != null)
                {
                    UserAccount selectedUserAccount = _contextUserAccount.Collection().Include(ua => ua.UserRole).SingleOrDefault(ua => ua.UserAccountID == editProfileFormViewModel.UserAccountID);

                    selectedUserAccount.FirstName = editProfileFormViewModel.FirstName;
                    selectedUserAccount.LastName = editProfileFormViewModel.LastName;
                    if (editProfileFormViewModel.UserPassword != null)
                        selectedUserAccount.UserPassword = userAccountS.ComputeSha256Hash(editProfileFormViewModel.UserPassword);
                    selectedUserAccount.ModifiedAt = DateTime.Now;
                    _contextUserAccount.Commit();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Login");
            }
            return View(editProfileFormViewModel);
        }

        /// <summary>
        /// Method to display add address page
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult AddAddress(string returnUrl = null)
        {
            if(Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                UserAddressFormModel viewModel = new UserAddressFormModel();
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Method to add address to user account
        /// </summary>
        /// <param name="UserAddressFormModel"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAddress(UserAddressFormModel UserAddressFormModel, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
                {
                    userAccountS.AddUserAddress(UserAddressFormModel, Session["UserAccountID"] as Guid?);
                }
                if(returnUrl == null)
                {
                    return RedirectToAction("Index");
                }
                else if (returnUrl != null)
                {
                    return RedirectToLocal(returnUrl);
                }
            }
            return View(UserAddressFormModel);
        }

        /// <summary>
        /// Method to display edit address page
        /// </summary>
        /// <param name="userAddressID"></param>
        /// <returns></returns>
        public ActionResult EditAddress(Guid userAddressID)
        {
            if (Session["UserAccountID"] != null && Session["UserRole"] as string == "Customer")
            {
                UserAddressFormModel viewModel = userAccountS.GetEditUserAddressForm(userAddressID);
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Method to edit certain address
        /// </summary>
        /// <param name="userAddressFormModel"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Method to Delete certain address
        /// </summary>
        /// <param name="userAddressID"></param>
        /// <returns></returns>
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
            RegisterFormViewModel model = new RegisterFormViewModel();
            return View(model);
        }

        /// <summary>
        /// Method to register new user
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterFormViewModel registerFormViewModel)
        {
            string registerMessage = null;
            if (ModelState.IsValid)
            {
                //Validate inputted username & email and Register User
                registerMessage = userAccountS.RegisterUser(registerFormViewModel);

                if (registerMessage == null)
                {
                    //Valid username & email
                    return RedirectToAction("Login");
                }
                else
                {
                    //Invalid username & email
                    ViewBag.RegisterMessage = registerMessage;
                    return View(registerFormViewModel);
                }
            }
            return View(registerFormViewModel);
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
                LoginFormViewModel model = new LoginFormViewModel();
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
        public ActionResult Login(LoginFormViewModel loginFormViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                UserAccount userAccount = userAccountS.LoginUser(loginFormViewModel);
                if (userAccount != null)
                {
                    if (userAccount.IsActive == true)
                    {
                        //Existed user - active user
                        Session["UserAccountID"] = userAccount.UserAccountID;
                        Session["UserRole"] = userAccountS.CheckUserRole(userAccount.UserAccountID);
                        if (Session["UserRole"] as string == "Customer")
                            return RedirectToLocal(returnUrl);
                        else
                            return RedirectToAction("Index", "ProductManager");
                    }
                    else if (userAccount.IsActive == false)
                    {
                        //Existed user - Deactivate user
                        ViewBag.LoginMessage = "You have been deactived by admin";
                        return View(loginFormViewModel);
                    }
                }
                //Not existed user
                ViewBag.LoginMessage = "Wrong Username or Password";
            }
            return View(loginFormViewModel);
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