using eShop.Core.Contracts;
using eShop.Core.Models;
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

        public UserAccountController(IRepository<UserAccount> userAccount)
        {
            _contextUserAccount = userAccount;
        }

        /// <summary>
        /// Method to create a SHA256 
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private static Byte[] ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                return bytes;
                // Convert byte array to a string   
                //StringBuilder builder = new StringBuilder();
                //for (int i = 0; i < bytes.Length; i++)
                //{
                //    builder.Append(bytes[i].ToString("x2"));
                //}
                //return builder.ToString();
            }
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
            UserAccount model = new UserAccount();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                UserAccount userAccount = new UserAccount();
                userAccount.UserAccountID = Guid.NewGuid();
                userAccount.UserName = formCollection["UserName"];
                userAccount.UserPassword = ComputeSha256Hash(formCollection["UserPassword"]);
                userAccount.FirstName = formCollection["FirstName"];
                userAccount.LastName = formCollection["LastName"];
                userAccount.Email = formCollection["Email"];
                _contextUserAccount.Insert(userAccount);
                _contextUserAccount.Commit();
            }
            return RedirectToAction("Login");
        }

        public ActionResult Login(string returnUrl)
        {
            UserAccount model = new UserAccount();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection formCollection, string returnUrl)
        {
            byte[] inputtedPassword = ComputeSha256Hash(formCollection["UserPassword"]);
            string inputtedUserName = formCollection["UserName"];
            UserAccount user = _contextUserAccount.Collection().SingleOrDefault(ua => ua.UserName == inputtedUserName && ua.UserPassword == inputtedPassword);
            if (user != null)
            {
                Session["UserAccountID"] = user.UserAccountID;
                return RedirectToLocal(returnUrl);
            }
            ViewBag.LoginMessage = "Wrong Username or Password";
            return RedirectToLocal(returnUrl);
        }

        public ActionResult Logout(string returnUrl)
        {
            Session["UserAccountID"] = null;
            return RedirectToLocal(returnUrl);
        }

    }
}