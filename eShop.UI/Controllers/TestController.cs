﻿using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eShop.UI.Controllers
{
    public class TestController : Controller
    {
        private IRepository<Product> _contextProduct;


        //Table with pagination and filter
        public ActionResult Index()
        {
            return View();
        }

        //Card with filtering
        public ActionResult test2()
        {
            return View();
        }
    }
}