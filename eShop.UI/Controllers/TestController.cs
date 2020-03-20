using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace eShop.UI.Controllers
{
    public class TestController : Controller
    {
        
        // Create a SHA256 
        static string ComputeSha256Hash(string rawData)
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

        //Table with pagination and filter
        public ActionResult Index()
        {
            string var1 = "Mahesh2132";
            string hashedvar1 = ComputeSha256Hash(var1);
            string var2 = "Mahesh2132";
            string hashedvar2 = ComputeSha256Hash(var2);
            string var3 = "Mahesh2132.";
            string hashedvar3 = ComputeSha256Hash(var3);


            return View();
        }

        //Card with filtering
        public ActionResult test2()
        {
            return View();
        }
    }
}