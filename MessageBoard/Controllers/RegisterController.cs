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
                isOK = userTool.isNotExistUserName(userAccount),
                msg = ""
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
                isOK = userTool.isNotExistEmail(userEmail),
                msg = ""
            };
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserRegister(string userAccount, string userPW1, string userPW2, string userEmail)
        {
            //var registerResult = userTool.RegisterUser(userAccount, userPW1, userPW2, userEmail);
            //if (registerResult.Item1)
            //{

            //}
            //else {
            //    TempData["registerMsg"] = "註冊失敗";
            //    TempData["registerMsg"] = "註冊失敗";
            //}
            //return Content($"{userAccount}, {userPW1}, {userPW2}, {userEmail}");

            //TempData["registerMsg"] = "註冊失敗";
            //TempData["registerError"] = "註冊失敗";
            //return RedirectToAction("Index");
        }

        public class ReturnJSON
        {
            public bool isOK { get; set; }
            public string msg { get; set; }
        }
    }
}