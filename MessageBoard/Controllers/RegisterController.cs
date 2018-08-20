using MessageBoard.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard.Controllers
{
    public class RegisterController : Controller
    {
        UserTool userTool = new UserTool();

        // GET: Register
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult CheckUserExist(string userAccount)
        {
            if (userAccount == null || userAccount == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = userTool.isNotExistUserName(HttpUtility.UrlDecode(userAccount))
            };
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult CheckUserEmail(string userEmail)
        {
            if (userEmail == null || userEmail == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = userTool.isNotExistEmail(HttpUtility.UrlDecode(userEmail))
            };
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UserRegister(string userAccount, string userPW1, string userPW2, string userEmail)
        {
            userAccount = HttpUtility.UrlDecode(userAccount);
            userPW1 = HttpUtility.UrlDecode(userPW1);
            userPW2 = HttpUtility.UrlDecode(userPW2);
            userEmail = HttpUtility.UrlDecode(userEmail);
            var registerResult = userTool.RegisterUser( userAccount, userPW1, userPW2, userEmail);
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = registerResult.Item1,
                errorList = registerResult.Item2,
                msg = registerResult.Item3
            };

            return Json(returnJSON);
        }

        public class ReturnJSON
        {
            public bool isOK { get; set; }
            public string msg { get; set; }
            public List<string> errorList { get; set; }
        }
    }
}