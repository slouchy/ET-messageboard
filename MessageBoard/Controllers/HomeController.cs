using MessageBoard.Models;
using MessageBoard.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Login");
        }
    }
}