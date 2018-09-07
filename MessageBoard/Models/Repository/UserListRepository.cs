using MessageBoard.Models.Interface;
using System;
using System.Linq;

namespace MessageBoard.Models.Repository
{
    public class UserListRepository : IUserList, IDisposable
    {
        public UserListRepository(MessageBoardEntities entities)
        {
            db = entities;
        }

        protected MessageBoardEntities db
        {
            get;
            private set;
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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public UserList GetUserInfo(string userName)
        {
            return db.UserList
                    .Where(r =>
                        r.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                        r.UserStatus == true
                    )
                    .FirstOrDefault() ?? null;
        }

        public UserList GetUserInfo(int userID)
        {
            return db.UserList
                .Where(r =>
                    r.UserID.Equals(userID) &&
                    r.UserStatus == true)
                .FirstOrDefault() ?? null;
        }

        public IQueryable<UserList> GetUserLists()
        {
            return db.UserList
                .Where(r => r.UserStatus)
                .OrderBy(r => r.UserID);
        }

        public bool isEmailExist(string email)
        {
            return db.UserList
                    .Where(r =>
                        r.UserEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                    .Any();
        }

        public bool isUserNameExist(string userName)
        {
            return db.UserList
                    .Where(r =>
                        r.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    .Any();
        }

        public void SaveChanges()
        {
            db.SaveChanges();
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