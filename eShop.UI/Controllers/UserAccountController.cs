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
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Method to display register page
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
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
                _contextUserAccount.Insert(userAccount);
                _contextUserAccount.Commit();
            }
            return RedirectToAction("Index","ProductManager");
        }

        public ActionResult Login()
        {
            return View();
        }
    }
}