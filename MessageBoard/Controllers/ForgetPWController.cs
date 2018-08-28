using MessageBoard.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard.Controllers
{
    public class ForgetPWController : Controller
    {
        UserTool userTool = new UserTool();

        // GET: ForgetPW
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CheckUserNameEmail(string userName, string userEmail)
        {
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = false,
                msg = string.Empty
            };

            returnJSON.isOK = userTool.isUserEmailExist(HttpUtility.UrlDecode(userName),
                HttpUtility.UrlDecode(userEmail));
            returnJSON.msg = returnJSON.isOK ? "匹配正確，請重新設定密碼" : "使用者名稱和信箱並不匹配";
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResetPW(string userName, string userEmail, string pw1, string pw2)
        {
            userName = HttpUtility.UrlDecode(userName);
            userEmail = HttpUtility.UrlDecode(userEmail);
            pw1 = HttpUtility.UrlDecode(pw1);
            pw2 = HttpUtility.UrlDecode(pw2);
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = false,
                msg = "更新密碼發生非預期的錯誤"
            };

            if (pw1 != pw2)
            {
                returnJSON.msg = "前後兩次密碼輸入不一致";
            }
            else if (userTool.isSetNewPW(userName, userEmail, pw1))
            {
                returnJSON.msg = "更新密碼成功，返回登入頁重新登入";
                returnJSON.isOK = true;
            }

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        private class ReturnJSON
        {
            public bool isOK { get; set; }
            public string msg { get; set; }
        }
    }
}