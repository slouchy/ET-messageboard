using MessageBoard.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
            ReturnJSON returnJSON = new ReturnJSON();
            List<string> errorList = new List<string>();
            bool isRegisterOK = false;
            string returnMsg = "註冊失敗";

            if (Request.Files.Count == 0)
            {
                errorList.Add("請上傳頭像圖檔");
            }
            else
            {
                HttpPostedFileBase postFile = Request.Files[0];
                if (!PicTool.isFileSizeAllow(postFile, 1))
                {
                    errorList.Add("檔案大於 1MB");
                }

                if (!PicTool.isFileExtensionAllow(postFile.FileName, @"/(jpg|gif|png|bmp|jpeg|jpg2000|svg)$"))
                {
                    errorList.Add("不是圖像檔案");
                }
            }

            if (!errorList.Any())
            {
                HttpPostedFileBase postFile = Request.Files[0];
                userAccount = HttpUtility.UrlDecode(userAccount);
                userPW1 = HttpUtility.UrlDecode(userPW1);
                userPW2 = HttpUtility.UrlDecode(userPW2);
                userEmail = HttpUtility.UrlDecode(userEmail);
                var registerResult = userTool.RegisterUser(userAccount, userPW1, userPW2, userEmail);
                isRegisterOK = registerResult.Item1;
                errorList = registerResult.Item2;
                returnMsg = registerResult.Item3;

                if (registerResult.Item1)
                {
                    PicTool.SaveUserPic(postFile, $"UserIcom_{registerResult.Item4.UserID}{Path.GetExtension(postFile.FileName)}");
                }
            }

            returnJSON = new ReturnJSON()
            {
                isOK = isRegisterOK,
                errorList = errorList,
                msg = returnMsg
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