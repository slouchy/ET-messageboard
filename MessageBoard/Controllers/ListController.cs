using MessageBoard.Models;
using MessageBoard.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MessageBoard.Controllers
{
    //[Authorize]
    public class ListController : Controller
    {
        MessageBoardEntities messageBoardEntities = new MessageBoardEntities();
        UserTool userTool = new UserTool();
        ReturnJSON returnJSON = new ReturnJSON()
        {
            isOK = true,
            msg = ""
        };
        // GET: List
        public ActionResult Index()
        {
            var userData = userTool.GetLoginedUser(HttpContext.Request);
            TempData["userLogined"] = userData != null ? "1" : "0";
            TempData["userName"] = userData != null ? userData.FirstOrDefault().UserName : "訪客";
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("index", "login");
        }

        public JsonResult AddMessage(string content, int majorID)
        {
            CheckUserData(out int userID, out int userAccess);
            if (content != null && content != "" && userID != 0)
            {
                Message message;
                if (majorID == 0)   // 建立新主題
                {
                    MajorMessageList majorMessage = new MajorMessageList()
                    {
                        CreateUserID = userID,
                        MessageStatus = true,
                        CreateDate = DateTime.Now
                    };

                    messageBoardEntities.MajorMessageList.Add(majorMessage);
                    messageBoardEntities.SaveChanges();
                    InsertMessage(content, userID, majorMessage.MajorID, out message);
                }
                else
                {                   // 回覆留言
                    InsertMessage(content, userID, majorID, out message);
                }

                // 儲存圖片
                if (Request.Files.Count > 0)
                {
                    PicTool.SaveMessagePic(Request.Files[0], userID, message.MessageID);
                }
            }

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMessage(int messageID)
        {
            var message = messageBoardEntities.Message.Find(messageID);
            message.MessageStatus = false;
            messageBoardEntities.SaveChanges();
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMessagePic(int picID)
        {
            var pic = messageBoardEntities.MessagePic.Find(picID);
            messageBoardEntities.MessagePic.Remove(pic);
            messageBoardEntities.SaveChanges();
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMajorMessage()
        {
            CheckUserData(out int userID, out int userAccess);
            var majorMessage = from l in messageBoardEntities.MajorMessageList
                               join m in messageBoardEntities.Message on l.MajorID equals m.MajorID
                               where (l.MessageStatus == true && (m.MessageCount == 0 && m.MessageStatus == true))
                               join u in messageBoardEntities.UserList on m.CreateUserID equals u.UserID
                               orderby m.CreateDate descending
                               select new
                               {
                                   l.MajorID,
                                   m.MessageID,
                                   m.CreateDate,
                                   ip = m.IP,
                                   isShowDelete = m.CreateUserID == userID || userAccess == 0,
                                   isShowEdit = m.CreateUserID == userID,
                                   content = m.Message1,
                                   userIcon = u.UserIcon,
                                   userName = u.UserName,
                                   replyCount = messageBoardEntities.Message.Where(r => r.MajorID == m.MajorID && r.MessageStatus).Count() - 1,
                                   pics = from pic in messageBoardEntities.MessagePic
                                          where pic.MessageID == m.MessageID
                                          select new
                                          {
                                              pic.PicID,
                                              pic.PicURL
                                          }
                               };
            return Json(majorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMessageList(int majorID)
        {
            CheckUserData(out int userID, out int userAccess);
            var messageList = from m in messageBoardEntities.Message
                              where m.MajorID == majorID && m.MessageStatus && m.MessageCount > 0
                              join u in messageBoardEntities.UserList on m.CreateUserID equals u.UserID
                              orderby m.CreateDate ascending
                              select new
                              {
                                  m.MajorID,
                                  m.MessageID,
                                  m.CreateDate,
                                  ip = m.IP,
                                  isShowDelete = m.CreateUserID == userID || userAccess == 0,
                                  isShowEdit = m.CreateUserID == userID,
                                  content = m.Message1,
                                  userIcon = u.UserIcon,
                                  userName = u.UserName,
                                  replyCount = messageBoardEntities.Message.Where(r => r.MajorID == m.MajorID).Count() - 1,
                                  pics = from pic in messageBoardEntities.MessagePic
                                         where pic.MessageID == m.MessageID
                                         select new
                                         {
                                             pic.PicID,
                                             pic.PicURL
                                         }
                              };
            return Json(messageList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUniqueMessage(int messageID)
        {
            CheckUserData(out int userID, out int userAccess);
            var message = (from m in messageBoardEntities.Message
                           where m.MessageID == messageID && m.CreateUserID == userID
                           select new
                           {
                               m.MajorID,
                               m.MessageID,
                               m.Message1,
                               pics = from pic in messageBoardEntities.MessagePic
                                      where pic.MessageID == messageID
                                      select new
                                      {
                                          pic.PicID,
                                          pic.PicURL
                                      }
                           });

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateMessage(string content, int messageID)
        {
            CheckUserData(out int userID, out int userAccess);
            if (content != null && content != "" && userID != 0)
            {
                Message message = messageBoardEntities.Message.Find(messageID);
                message.Message1 = content;
                messageBoardEntities.SaveChanges();

                // 儲存圖片
                if (Request.Files.Count > 0)
                {
                    PicTool.SaveMessagePic(Request.Files[0], userID, message.MessageID);
                }
            }

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 檢查登入的使用者資料
        /// </summary>
        /// <param name="userID">回傳使用者 ID</param>
        /// <param name="userAccess">回傳使用者權限</param>
        private void CheckUserData(out int userID, out int userAccess)
        {
            var userData = userTool.GetLoginedUser(HttpContext.Request);
            userID = -1;
            userAccess = -1;
            if (userData != null)
            {
                userID = userData.FirstOrDefault().UserID;
                userAccess = userData.FirstOrDefault().UserAccess;
            }
        }

        /// <summary>
        /// 新增留言
        /// </summary>
        /// <param name="content">留言內容</param>
        /// <param name="userID">使用者 ID</param>
        /// <param name="majorID">留言主 ID</param>
        /// <param name="message">回傳新增的留言</param>
        /// <returns>回傳異動筆數</returns>
        private int InsertMessage(string content, int userID, int majorID, out Message message)
        {
            message = new Message()
            {
                MajorID = majorID,
                MessageCount = messageBoardEntities.Message.Where(r => r.MajorID == majorID).Count(),
                Message1 = content,
                MessageStatus = true,
                IP = PBTool.GetIP(),
                CreateDate = DateTime.Now,
                CreateUserID = userID,
            };
            messageBoardEntities.Message.Add(message);
            return messageBoardEntities.SaveChanges();
        }

        private class ReturnJSON
        {
            public bool isOK { get; set; }
            public string msg { get; set; }
        }
    }
}