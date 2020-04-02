using eShop.Core.Contracts;
using eShop.Core.Models;
using eShop.Services;
using System;
using System.Collections.Generic;
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

        public UserAccountController(IRepository<UserAccount> userAccount, UserAccountService userAccountService)
        {
            _contextUserAccount = userAccount;
            userAccountS = userAccountService;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
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
        public ActionResult Register(RegisterFormModel registerFormModel )
        {
            string registerMessage = null;
            if (ModelState.IsValid)
            {
                //Validate inputted username and email
                registerMessage = userAccountS.ValidateUsernameEmail(registerFormModel.UserName, registerFormModel.Email);

                if (registerMessage == null)
                {
                    //Valid username & email
                    userAccountS.RegisterUser(registerFormModel);
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

        public ActionResult Login(string returnUrl)
        {
            LoginFormModel model = new LoginFormModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginFormModel loginFormModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Guid? userAccountID = userAccountS.LoginUser(loginFormModel); 
                if (userAccountID != null)
                {
                    Session["UserAccountID"] = userAccountID;
                    return RedirectToLocal(returnUrl);
                }
                ViewBag.LoginMessage = "Wrong Username or Password";
            }
            return View(loginFormModel);
        }

        public ActionResult Logout(string returnUrl)
        {
            Session["UserAccountID"] = null;
            return RedirectToLocal(returnUrl);
        }

    }
}