using MessageBoard.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard.Controllers
{
    public class LoginController : Controller
    {
        UserTool userTool = new UserTool();

        [AllowAnonymous]
        [HttpGet]
        // GET: Login
        public ActionResult Index()
        {


            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(string userAccount, string userPw)
        {
            if (userTool.isUserEqulsDB(userAccount, userPw))
            {
                System.Web.Security.FormsAuthentication.SetAuthCookie(userAccount, false);
                return RedirectToAction("Index", "List");
            }

            TempData["userAccount"] = userAccount;
            return RedirectToAction("Index");
        }
    }
}