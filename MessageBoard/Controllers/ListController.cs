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
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = false,
                msg = string.Empty
            };

            returnJSON.msg = CheckCreateMesaage(content, userID);
            if (string.IsNullOrWhiteSpace(returnJSON.msg))
            {
                try
                {
                    Message message;
                    if (majorID == 0)   // 建立新主題
                    {
                        MajorMessageList majorMessage = InsertMajorMessage(userID);
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

                    returnJSON.isOK = true;
                    returnJSON.msg = "文章新增成功";
                }
                catch (Exception err)
                {
                    LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n");
                    returnJSON.msg = "<br/> * 儲存時發生錯誤";
                }
            }

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMessage(int messageID)
        {
            CheckUserData(out int userID, out int userAccess);
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = false,
                msg = string.Empty
            };

            if (userID == 0)
            {
                returnJSON.msg += "<br/> * 請先登入";
            }
            else
            {
                try
                {
                    var message = messageBoardEntities.Message.Find(messageID);
                    message.MessageStatus = false;
                    messageBoardEntities.SaveChanges();
                    returnJSON.msg = "文章刪除成功";
                    returnJSON.isOK = true;
                }
                catch (Exception err)
                {
                    LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n");
                    returnJSON.msg += "<br/> * 刪除文章時發生錯誤";
                }
            }

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMessagePic(int picID)
        {
            CheckUserData(out int userID, out int userAccess);
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = false,
                msg = string.Empty
            };

            if (userID == 0)
            {
                returnJSON.msg += "<br/> * 請先登入";
            }
            else
            {
                try
                {
                    var pic = messageBoardEntities.MessagePic.Find(picID);
                    messageBoardEntities.MessagePic.Remove(pic);
                    messageBoardEntities.SaveChanges();
                    returnJSON.msg = "圖片刪除成功";
                    returnJSON.isOK = true;
                }
                catch (Exception err)
                {
                    LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n");
                    returnJSON.msg += "<br/> * 刪除圖片時發生錯誤";
                }
            }

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
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = true,
                msg = string.Empty
            };

            if (userID == 0)
            {
                returnJSON.isOK = false;
                returnJSON.msg += "<br/> * 請先登入";
            }

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

            returnJSON.data = message;
            if (!message.Any())
            {
                returnJSON.isOK = false;
                returnJSON.msg += "<br/> * 找不到指定文章";
            }

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateMessage(string content, int messageID)
        {
            CheckUserData(out int userID, out int userAccess);
            ReturnJSON returnJSON = new ReturnJSON()
            {
                isOK = false,
                msg = ""
            };

            returnJSON.msg = CheckCreateMesaage(content, userID);
            if (string.IsNullOrWhiteSpace(returnJSON.msg))
            {
                try
                {
                    Message message = messageBoardEntities.Message.Find(messageID);
                    message.Message1 = content;
                    messageBoardEntities.SaveChanges();
                    // 儲存圖片
                    if (Request.Files.Count > 0)
                    {
                        PicTool.SaveMessagePic(Request.Files[0], userID, message.MessageID);
                    }

                    returnJSON.isOK = true;
                    returnJSON.msg = "文章儲存成功";
                }
                catch (Exception err)
                {
                    LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n");

                    returnJSON.msg = "<br/> * 儲存時生錯誤";
                }
            }

            return Json(returnJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 檢查建立文章時的內容和使用者
        /// </summary>
        /// <param name="content">內容</param>
        /// <param name="userID">使用者</param>
        /// <returns>回傳檢查字串</returns>
        private string CheckCreateMesaage(string content, int userID)
        {
            string errorMsg = string.Empty;
            if (content == null || content == "")
            {
                errorMsg += "<br/> * 請輸入留言內容";
            }

            if (userID == 0)
            {
                errorMsg = "<br/> * 請先登入";
            }

            return errorMsg;
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
        /// 主留言清單新增一筆紀錄
        /// </summary>
        /// <param name="userID">使用者 ID</param>
        /// <returns>回傳新增的紀錄</returns>
        private MajorMessageList InsertMajorMessage(int userID)
        {
            MajorMessageList majorMessage = new MajorMessageList()
            {
                CreateUserID = userID,
                MessageStatus = true,
                CreateDate = DateTime.Now
            };

            messageBoardEntities.MajorMessageList.Add(majorMessage);
            messageBoardEntities.SaveChanges();
            return majorMessage;
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
            public IQueryable data { get; set; }
        }
    }
}