using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using MessageBoard.Models;
using Newtonsoft.Json;

namespace MessageBoard.Tools
{
    public class UserTool
    {
        MessageBoardEntities messageBoardEntities = new MessageBoardEntities();

        /// <summary>
        /// 檢驗使用者是否登入成功
        /// </summary>
        /// <param name="userName">使用者姓名</param>
        /// <param name="userPw">使用者密碼</param>
        /// <returns>回傳檢驗結果</returns>
        public bool isUserEqulsDB(string userName, string userPw)
        {
            if (isUserNameCorrect(userName) && isUserPWCorrect(userPw))
            {
                string saltedPw = GetSaltPW(userPw);
                var userData = messageBoardEntities.UserList
                    .Where(r => r.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
                if (userData.Any())
                {
                    var loginUser = userData
                        .Where(r => r.UserPW.Equals(saltedPw));
                    if (loginUser.Any())
                    {
                        DoUserLog(userData.FirstOrDefault().UserID, "登入成功");
                        return true;
                    }

                    DoUserLog(userData.FirstOrDefault().UserID, "登入失敗");
                }
            }

            return false;
        }
               
        /// <summary>
        /// 檢查使用者名稱長度
        /// </summary>
        /// <param name="userName">使用者名稱</param>
        /// <returns>回傳檢查結果</returns>
        private bool isUserNameCorrect(string userName)
        {
            return userName.Length > 0 && userName.Length <= 20;
        }

        /// <summary>
        /// 檢查密碼格式
        /// </summary>
        /// <param name="userPw">使用者密碼</param>
        /// <returns>回傳檢查結果</returns>
        private bool isUserPWCorrect(string userPw)
        {
            Regex regPw = new Regex(@"(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])[\w\d]{0,12}");
            return regPw.IsMatch(userPw);
        }

        /// <summary>
        /// 取得加密密碼
        /// </summary>
        /// <param name="orignPW">原始密碼</param>
        /// <returns>加密後的密碼</returns>
        private string GetSaltPW(string orignPW)
        {
            // salt list
            char[] salts = new char[6] { 'A', 'B', 'C', 'D', '1', '2' };
            string saltPw = string.Empty;
            // Add salts
            for (int i = 0; i < orignPW.Length; i++)
            {
                saltPw += orignPW[i];
                if (i % 2 == 0)
                {
                    saltPw += salts[i / 2];
                }
            }

            MD5 md5 = MD5.Create();
            byte[] orignPWByte = Encoding.Default.GetBytes(saltPw);
            return Convert.ToBase64String(md5.ComputeHash(md5.ComputeHash(orignPWByte)));
        }

        /// <summary>
        /// 儲存使用者 Log 檔
        /// </summary>
        /// <param name="userID">使用者 ID</param>
        /// <param name="userOperator">訊息</param>
        private void DoUserLog(int userID, string userOperator)
        {
            UserLog userLog = new UserLog
            {
                UserID = userID,
                UserOperator = userOperator,
                IP = PBTool.GetIP(),
                CreateDate = DateTime.Now
            };

            messageBoardEntities.UserLog.Add(userLog);
            messageBoardEntities.SaveChanges();

            // ToDo 20180817 操作紀錄 
            //var userJSON = JsonConvert.SerializeObject(userLog);
        }
    }
}