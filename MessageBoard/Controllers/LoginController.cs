using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard.Controllers
{
    public class LoginController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(string userName, string userPw)
        {
            if ((userName == "1" && userPw == "1"))
            {
                
                System.Web.Security.FormsAuthentication.SetAuthCookie(userName, false);
                return RedirectToAction("Index", "List");
            }

            TempData["Message"] = "Login failed.User name or password supplied doesn't exist.";
            return RedirectToAction("Index");
        }
    }
}