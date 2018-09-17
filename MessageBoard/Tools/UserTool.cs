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
using System.Web.Security;
using MessageBoard.Models.Interface;
using MessageBoard.Models.Repository;

namespace MessageBoard.Tools
{
    public class UserTool
    {
        private IUserList _userList;
        private ISaltPW _saltPW;
        public UserTool()
        {
            _userList = new UserListRepository(new MessageBoardEntities());
            _saltPW = new SaltPW();
        }
        public UserTool(MessageBoardEntities entities, ISaltPW saltPW = null)
        {
            _userList = new UserListRepository(entities);
            _saltPW = saltPW ?? new SaltPW();
        }

        /// <summary>
        /// 透過 Cookies 取得使用者資訊
        /// </summary>
        /// <param name="httpRequest">Http Request</param>
        /// <param name="httpRequest">Http Request</param>
        /// <returns></returns>
        public UserList GetUserByCookie(HttpRequestBase httpRequest)
        {
            var userCookie = CookieTool.CheckUserNameExist(httpRequest);
            UserList userData = null;
            if (userCookie.isValid)
            {
                userData = GetLoginedUser(userCookie.userName);
            }

            return userData;
        }

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
                string saltedPw = _saltPW.GetSaltPW(userPw);
                //string saltedPw = GetSaltPW(userPw);
                var userData = _userList.GetUserInfo(userName);
                if (userData != null)
                {
                    if (userData.UserPW.Equals(saltedPw))
                    {
                        //DoUserLog(userData.UserID, "登入成功");
                        return true;
                    }

                    //DoUserLog(userData.UserID, "登入失敗");
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
        /// <para>Item4(UserList) 使用者資訊</para>
        /// </summary>
        /// <param name="userAccount">使用者名稱</param>
        /// <param name="userPw1">使用者密碼</param>
        /// <param name="userPw2">使用者密碼</param>
        /// <param name="email">Email</param>
        /// <returns></returns>
        public Tuple<bool, List<string>, string, UserList> RegisterUser(string userAccount, string userPw1, string userPw2, string email)
        {
            bool result = false;
            List<string> errorList = GetFieldCheckError(userAccount, userPw1, userPw2, email);
            string msg = "註冊失敗";
            UserList userList = new UserList();

            if (!errorList.Any())
            {
                try
                {
                    userList = new UserList
                    {
                        CreateDate = DateTime.Now,
                        CreateIP = PBTool.GetIP(),
                        UserEmail = email,
                        UserName = userAccount,
                        UserPW = GetSaltPW(userPw1),
                        UserAccess = 1,
                        UserStatus = true
                    };
                    _userList.Create(userList);
                    DoUserLog(userList.UserID, "註冊成功");
                    msg = "註冊成功，即將跳轉至登入頁";
                    result = true;
                }
                catch (Exception err)
                {
                    errorList.Add("發生錯誤");
                    LogTool.DoErrorLog($"{err.Message}\r\n{err.StackTrace}");
                }
            }
            return Tuple.Create(result, errorList, msg, userList);
        }

        /// <summary>
        /// 忘記密碼檢測使用者名稱是否和信箱匹配
        /// </summary>
        /// <param name="userName">使用者名稱</param>
        /// <param name="userEmail">使用者信箱</param>
        /// <returns></returns>
        public bool isUserEmailExist(string userName, string userEmail)
        {
            var userInfo = _userList.GetUserInfo(userName);
            return userInfo != null ?
                userInfo.UserEmail.Equals(userEmail, StringComparison.CurrentCultureIgnoreCase) :
                false;
        }

        public Tuple<bool, IQueryable<UserList>> UserPWCorrect(HttpRequestBase httpRequest, string pw)
        {
            var userCookie = CookieTool.CheckUserNameExist(httpRequest);
            UserList userData = null;
            if (userCookie.isValid)
            {
                userData = GetLoginedUser(userCookie.userName);
            }

            bool result = userData != null && userData.UserPW.Equals(GetSaltPW(pw));
            return Tuple.Create(result, userData.ToQueryable());
        }

        /// <summary>
        /// 檢查 Email 是否不存在
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>回傳檢查結果</returns>
        public bool isNotExistEmail(string email)
        {
            return !_userList.isEmailExist(email);
        }

        /// <summary>
        /// 檢查使用者是否不存在
        /// </summary>
        /// <param name="userName">使用者名稱</param>
        /// <returns>回傳檢查結果</returns>
        public bool isNotExistUserName(string userName)
        {
            return !_userList.isUserNameExist(userName);
        }

        /// <summary>
        /// 設定使用者新密碼
        /// </summary>
        /// <param name="userName">使用者名稱</param>
        /// <param name="userEmail">使用者信箱</param>
        /// <param name="newPW">新的密碼</param>
        /// <returns>回傳設定結果</returns>
        public bool isSetNewPW(string userName, string userEmail, string newPW)
        {
            bool result = false;
            if (isUserPWCorrect(newPW))
            {
                var userInfo = _userList.GetUserInfo(userName);
                if (userInfo != null && userInfo.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        userInfo.UserPW = GetSaltPW(newPW);
                        _userList.Update(userInfo);
                        result = true;
                        DoUserLog(userInfo.UserID, "改密碼成功");
                    }
                    catch (Exception err)
                    {
                        DoUserLog(userInfo.UserID, "改密碼失敗");
                        LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n");
                        throw;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 儲存使用者頭像
        /// </summary>
        /// <param name="userID">使用者 ID</param>
        /// <param name="iconPath">頭像路徑</param>
        public void SaveUserIconPath(int userID, string iconPath)
        {
            var userInfo = _userList.GetUserInfo(userID);
            userInfo.UserIcon = iconPath;
            _userList.Update(userInfo);
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
            Regex regPw = new Regex(@"(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])[\w\d]{0,12}$");
            return regPw.IsMatch(userPw) && userPw.Length >= 0 && userPw.Length <= 12;
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
        /// 取得登入的使用者資訊
        /// </summary>
        /// <param name="cookieUserName">使用者名稱</param>
        /// <returns>回傳使用者資訊</returns>
        private UserList GetLoginedUser(string cookieUserName)
        {
            if (cookieUserName?.Length == 0)
            {
                return new UserList();
            }

            return _userList.GetUserInfo(cookieUserName);
        }

        /// <summary>
        /// 儲存使用者 Log 檔
        /// </summary>
        /// <param name="userID">使用者 ID</param>
        /// <param name="userOperator">訊息</param>
        private void DoUserLog(int userID, string userOperator)
        {
            MessageBoardEntities messageBoardEntities = new MessageBoardEntities();
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
                LogTool.DoErrorLog($"#{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}:{err.Message}\r\n");
                throw;
            }
        }
    }

    public static class ObjectExtensionMethods
    {
        public static IQueryable<TEntityType> ToQueryable<TEntityType>(this TEntityType instance)
        {
            return new[] { instance }.AsQueryable();
        }
    }

    public interface ISaltPW
    {
        string GetSaltPW(string originPW);
    }

    public class SaltPW : ISaltPW
    {
        public string GetSaltPW(string originPW)
        {
            return "Test1";
            throw new NotImplementedException();
        }
    }
}