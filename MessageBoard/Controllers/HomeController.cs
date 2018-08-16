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
        DBTool dBTool = new DBTool();
        string userConnection = ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString;
        // GET: Home
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Index() {
            return View();
        }
    }
}