using MessageBoard.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard.Controllers
{
    public class UserProfileController : Controller
    {
        UserTool userTool = new UserTool();

        // GET: UserProfile
        [Authorize]
        public ActionResult Index()
        {
            var userData = userTool.GetLoginedUser(HttpContext.Request);
            TempData["userLogined"] = userData != null ? "1" : "0";
            TempData["userName"] = userData != null ? userData.FirstOrDefault().UserName : "訪客";
            TempData["userIcon"] = userData != null ? userData.FirstOrDefault().UserIcon : "";
            return View();
        }

        public JsonResult UserCheck(string pw)
        {
            pw = HttpUtility.UrlDecode(pw);
            var userCheckResult = userTool.UserPWCorrect(HttpContext.Request, pw);
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = userCheckResult.Item1,
                data = userCheckResult.Item2?.Select(r => new { r.UserIcon, r.UserID, r.UserName, r.UserEmail }),
                msg = userCheckResult.Item1 ? "密碼檢驗正確" : "密碼檢驗失敗"
            };

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult isUserLogined()
        {
            var userLogin = userTool.GetLoginedUser(HttpContext.Request);
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = userLogin != null,
                msg = userLogin == null ? "使用者已經被登出" : ""
            };

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePorfile(string pw1, string pw2, int userID)
        {
            pw1 = HttpUtility.UrlDecode(pw1);
            pw2 = HttpUtility.UrlDecode(pw2);
            var userLogin = userTool.GetLoginedUser(HttpContext.Request).FirstOrDefault();
            string dgMsg = "儲存成功";
            bool isCatch = false;
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = false,
                msg = "發生未預期的錯誤"
            };

            try
            {
                if (pw1 != pw2)
                {
                    dgMsg += "<br/> * 前後兩次密碼輸入不一致";
                }
                else if (userTool.isSetNewPW(userLogin.UserName, userLogin.UserEmail, pw1))
                {
                    dgMsg += "<br/> * 更新密碼成功";
                    returnJSON.isOK = true;
                }
            }
            catch (Exception)
            {
                isCatch = true;
            }

            try
            {
                if (Request.Files.Count > 0)
                {
                    bool isFileCorrect = true;
                    HttpPostedFileBase postFile = Request.Files[0];
                    var fileCheck = PicTool.CheckUplaodFiles(postFile, @"\.(?i:jpg|bmp|gif|ico|pcx|jpeg|tif|png|raw|tga|svg|jpeg2000)$", 1);
                    if (!fileCheck.Item1)
                    {
                        isFileCorrect = false;
                        foreach (var item in fileCheck.Item2)
                        {
                            dgMsg += $"<br/> * {item}";
                        }
                    }

                    if (isFileCorrect)
                    {
                        returnJSON.isOK = true;
                        dgMsg += "<br/> * 頭像更新成功";
                        PicTool.SaveUserPic(postFile, userLogin.UserID);
                    }
                }

                returnJSON.isOK = true;
            }
            catch (Exception)
            {
                isCatch = true;
            }

            returnJSON.msg = isCatch ? returnJSON.msg : dgMsg;
            return Json(returnJSON);
        }

        private class ReturnJSON
        {
            public bool isOK { get; set; }
            public string msg { get; set; }
            public IQueryable data { get; set; }
        }
    }
}