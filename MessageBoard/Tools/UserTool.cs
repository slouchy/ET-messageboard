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
        /// <param name="userAccount">使用者姓名</param>
        /// <param name="userPw">使用者密碼</param>
        /// <returns>回傳檢驗結果</returns>
        public bool isUserEqulsDB(string userAccount, string userPw)
        {
            if (isUserNameCorrect(userAccount) && isUserPWCorrect(userPw))
            {
                string saltedPw = GetSaltPW(userPw);
                var userData = messageBoardEntities.UserList
                    .Where(r => r.UserName.Equals(userAccount, StringComparison.CurrentCultureIgnoreCase));
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
        /// 註冊使用者
        /// <para>回傳項目</para>
        /// <para>Item1(bool) 註冊結果</para>
        /// <para>Item2(List<stirng>) 錯誤代碼</para>
        /// <para>Item3(string) 相關訊息</para>
        /// </summary>
        /// <param name="userAccount">使用者名稱</param>
        /// <param name="userPw1">使用者密碼</param>
        /// <param name="userPw2">使用者密碼</param>
        /// <param name="email">Email</param>
        /// <returns></returns>
        public Tuple<bool, List<string>, string> RegisterUser(string userAccount, string userPw1, string userPw2, string email)
        {
            bool result = false;
            List<string> errorList = GetFieldCheckError(userAccount, userPw1, userPw2, email);
            string msg = "註冊失敗";

            if (!errorList.Any())
            {
                try
                {
                    UserList userList = new UserList
                    {
                        CreateDate = DateTime.Now,
                        CreateIP = PBTool.GetIP(),
                        UserEmail = email,
                        UserName = userAccount,
                        UserPW = GetSaltPW(userPw1),
                        UserAccess = 1,
                        UserStatus = true
                    };
                    messageBoardEntities.UserList.Add(userList);
                    messageBoardEntities.SaveChanges();
                    DoUserLog(userList.UserID, "註冊成功");
                    msg = "註冊成功，即將跳轉至登入頁";
                }
                catch (Exception err)
                {
                    errorList.Add("發生錯誤");
                    result = false;
                    LogTool.DoErrorLog($"{err.Message}\r\n{err.StackTrace}");
                }
            }
            return Tuple.Create(result, errorList, msg);
        }

        /// <summary>
        /// 檢查 Email 是否不存在
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>回傳檢查結果</returns>
        public bool isNotExistEmail(string email)
        {
            return !messageBoardEntities.UserList
                .Where(r => r.UserEmail.Equals(email, StringComparison.CurrentCultureIgnoreCase))
                .Any();
        }


        /// <summary>
        /// 檢查使用者是否不存在
        /// </summary>
        /// <param name="userAccount">使用者名稱</param>
        /// <returns>回傳檢查結果</returns>
        public bool isNotExistUserName(string userAccount)
        {
            return !messageBoardEntities.UserList
                .Where(r => r.UserName.Equals(userAccount, StringComparison.CurrentCultureIgnoreCase))
                .Any();
        }

        /// <summary>
        /// 取得註冊時檢查欄位的正確性
        /// </summary>
        /// <param name="userAccount">使用者名稱</param>
        /// <param name="userPw1">使用者密碼</param>
        /// <param name="userPw2">使用者密碼</param>
        /// <param name="email">電子信箱</param>
        /// <returns>回傳檢查結果</returns>
        private List<string> GetFieldCheckError(string userAccount, string userPw1, string userPw2, string email)
        {
            List<string> errorList = new List<string>();

            if (!isUserNameCorrect(userAccount))
            {
                errorList.Add("使用者名稱不得空白或長度超過 20 字");
            }

            if (!isNotExistUserName(userAccount))
            {
                errorList.Add("使用者名稱已存在");
            }

            if (!isUserPWCorrect(userPw1))
            {
                errorList.Add("密碼檢驗失敗");
            }

            if (userPw1 != userPw2)
            {
                errorList.Add("前後密碼不同");
            }

            if (!isEmailCorrect(email))
            {
                errorList.Add("email 格式錯誤");
            }

            if (!isNotExistEmail(email))
            {
                errorList.Add("email 已經存在");
            }

            return errorList;
        }

        /// <summary>
        /// 檢查使用者名稱長度
        /// </summary>
        /// <param name="userAccount">使用者名稱</param>
        /// <returns>回傳檢查結果</returns>
        private bool isUserNameCorrect(string userAccount)
        {
            return userAccount.Length > 0 && userAccount.Length <= 20;
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
        /// 檢查 E-mail 格式
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>回傳檢查結果</returns>
        private bool isEmailCorrect(string email)
        {
            Regex regEmail = new Regex(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$");
            return email != string.Empty && regEmail.IsMatch(email);
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
            try
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
            catch (Exception err)
            {
                LogTool.DoErrorLog($"{err.Message}\r\n");
                throw;
            }
        }
    }
}