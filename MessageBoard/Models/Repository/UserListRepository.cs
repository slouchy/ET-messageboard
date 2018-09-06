using MessageBoard.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageBoard.Models.Repository
{
    public class UserListRepository : IUserList, IDisposable
    {
        protected MessageBoardEntities db
        {
            get;
            private set;
        }

        public UserListRepository(MessageBoardEntities entities)
        {
            db = entities;
        }

        public void Create(UserList userInfo)
        {
            if (userInfo == null)
            {
                throw new ArgumentNullException("userInfo");
            }

            db.UserList.Add(userInfo);
            SaveChanges();
        }

        public void Update(UserList userInfo)
        {
            if (userInfo == null)
            {
                throw new ArgumentNullException("userInfo");
            }

            var dbUserData = db.UserList.Find(userInfo.UserID);
            if (dbUserData == null)
            {
                throw new ArgumentNullException("userInfo");
            }

            dbUserData = userInfo;
            SaveChanges();
        }

        public IQueryable<UserList> GetUserLists()
        {
            return db.UserList
                .Where(r => r.UserStatus)
                .OrderBy(r => r.UserID);
        }

        public UserList GetUserInfo(string userName)
        {
            var userInfo =
                db.UserList
                    .Where(r =>
                        r.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                        r.UserStatus == true
                    )
                    .FirstOrDefault() ??
                new UserList();
            return userInfo;
        }

        public UserList GetUserInfo(int userID)
        {
            return db.UserList.Find(userID);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.db != null)
                {
                    this.db.Dispose();
                    this.db = null;
                }
            }
        }
    }
}