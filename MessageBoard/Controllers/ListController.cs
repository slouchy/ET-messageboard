using MessageBoard.Models;
using MessageBoard.Tools;
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
        int userID = 3;
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
            return View();
        }

        public ActionResult AddMessage(string content, int majorID)
        {
            //int userID = userTool.GetLoginedUserID(HttpContext.Request);
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
                    SaveMessage(content, userID, majorMessage.MajorID, out message);
                }
                else
                {                   // 回覆留言
                    SaveMessage(content, userID, majorID, out message);
                }

                // 儲存圖片
                if (Request.Files.Count > 0)
                {
                    PicTool.SaveMessagePic(Request.Files[0], userID, message.MessageID);
                }
            }

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMessage(int messageID)
        {
            var message = messageBoardEntities.Message.Find(messageID);
            message.MessageStatus = false;
            messageBoardEntities.SaveChanges();
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMessagePic(int picID)
        {
            var pic = messageBoardEntities.MessagePic.Find(picID);
            messageBoardEntities.MessagePic.Remove(pic);
            messageBoardEntities.SaveChanges();
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMajorMessage()
        {
            //int userID = userTool.GetLoginedUserID(HttpContext.Request);
            // ToDo 20180823 使用者權限和管理員權限
            var majorMessage = from l in messageBoardEntities.MajorMessageList
                               join m in messageBoardEntities.Message on l.MajorID equals m.MajorID
                               where (l.MessageStatus == true && (m.MessageCount == 0 && m.MessageStatus == true))
                               join u in messageBoardEntities.UserList on m.CreateUserID equals u.UserID
                               orderby m.CreateDate descending
                               select new
                               {
                                   l.MajorID,
                                   m.MessageID,
                                   m.MessagePic,
                                   m.CreateDate,
                                   ip = m.IP,
                                   isShowDelete = m.CreateUserID == userID,
                                   isShowEdit = m.CreateUserID == userID,
                                   content = m.Message1,
                                   userIcon = u.UserIcon,
                                   userName = u.UserName,
                                   replyCount = messageBoardEntities.Message.Where(r => r.MajorID == m.MajorID).Count() - 1
                               };
            return Json(majorMessage, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMessageList(int majorID)
        {
            //int userID = userTool.GetLoginedUserID(HttpContext.Request);
            // ToDo 20180823 使用者權限和管理員權限
            var messageList = from m in messageBoardEntities.Message
                              where m.MajorID == majorID && m.MessageStatus
                              join u in messageBoardEntities.UserList on m.CreateUserID equals u.UserID
                              orderby m.CreateDate descending
                              select new
                              {
                                  m.MajorID,
                                  m.MessageID,
                                  m.MessagePic,
                                  m.CreateDate,
                                  ip = m.IP,
                                  isShowDelete = m.CreateUserID == userID,
                                  isShowEdit = m.CreateUserID == userID,
                                  content = m.Message1,
                                  userIcon = u.UserIcon,
                                  userName = u.UserName,
                                  replyCount = messageBoardEntities.Message.Where(r => r.MajorID == m.MajorID).Count() - 1
                              };
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateMessage(int messageID)
        {
            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        private int SaveMessage(string content, int userID, int majorID, out Message message)
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