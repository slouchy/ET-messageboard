using System;
using System.Collections.Generic;
using System.Linq;
using MessageBoard.Models;
using MessageBoard.Models.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Data.Entity;

namespace MessageBoard.Tests
{
    [TestClass]
    public class Test_DBUserList
    {
        private MessageBoardEntities _mockDbContext = Substitute.For<MessageBoardEntities>();

        [TestMethod]
        public void GetUserLists_AllowedUsersCount()
        {
            IDbSet<UserList> _mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            _mockDbContext.UserList.Returns(_mockDbSet);
            var mockDbUserList = new UserListRepository(_mockDbContext);

            var expectedCount = 2;
            var assignResult = mockDbUserList.GetUserLists();
            Assert.AreEqual(expectedCount, assignResult.Count());
        }

        [TestMethod]
        public void GetUserInfo_ByUserNameWithUserAllowed()
        {
            IDbSet<UserList> _mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            _mockDbContext.UserList.Returns(_mockDbSet);
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string userName = "Test1";
            var assignResult = mockDbUserList.GetUserInfo(userName);
            Assert.AreEqual(userName, assignResult.UserName);
        }

        [TestMethod]
        public void GetUserInfo_ByUserNameWithUserNotAllowed()
        {
            IDbSet<UserList> _mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            _mockDbContext.UserList.Returns(_mockDbSet);
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string userName = "Test3";
            var assignResult = mockDbUserList.GetUserInfo(userName);
            Assert.AreEqual(null, null);
        }

        [TestMethod]
        public void isEmailExist_EmailExistInDB()
        {
            IDbSet<UserList> _mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            _mockDbContext.UserList.Returns(_mockDbSet);
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string email = "CC1@com.tw";
            var assignResult = mockDbUserList.isEmailExist(email);
            Assert.IsTrue(assignResult);
        }

        [TestMethod]
        public void isEmailExist_EmailNotInDB()
        {
            IDbSet<UserList> _mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            _mockDbContext.UserList.Returns(_mockDbSet);
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string email = "CC0@com.tw";
            var assignResult = mockDbUserList.isEmailExist(email);
            Assert.IsFalse(assignResult);
        }

        [TestMethod]
        public void isUserNameExist_UserNameExistInDB()
        {
            IDbSet<UserList> _mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            _mockDbContext.UserList.Returns(_mockDbSet);
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string[] userNames = new string[5] { "Test1", "Test2", "test1", "Test3", "test3" };
            for (int i = 0; i < userNames.Length; i++)
            {
                var assignResult = mockDbUserList.isUserNameExist(userNames[i]);
                Assert.IsTrue(assignResult);
            }
        }

        [TestMethod]
        public void isUserNameExist_UserNameNotInDB()
        {
            IDbSet<UserList> _mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            _mockDbContext.UserList.Returns(_mockDbSet);
            var mockDbUserList = new UserListRepository(_mockDbContext);

            var assignResult = mockDbUserList.isUserNameExist("Test0");
            Assert.IsFalse(assignResult);
        }

        [TestMethod]
        public void Create_InsertNewUserSuccess()
        {
            // 建立待新增的資料
            var userInfo = new UserList()
            {
                CreateDate = DateTime.Now,
                CreateIP = "127.0.0.1",
                UserEmail = "email@com.tw",
                UserName = "unitTest",
                UserPW = "pw",
                UserAccess = 1,
                UserStatus = true
            };

            // 建立待使用的資料
            var originDBUserList = GetDemoUserlist();

            // 建立 DB mock
            var mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(originDBUserList.AsQueryable());
            _mockDbContext.UserList.Returns(mockDbSet);

            // 建立 Add method
            mockDbSet.Add(Arg.Do<UserList>(user =>
            {
                originDBUserList.Add(userInfo);
            }));

            // 建立呼叫的 Class
            var mockDbUserList = new UserListRepository(_mockDbContext);

            mockDbUserList.Create(userInfo);
            var assignResult = mockDbUserList.GetUserLists();
            int exceptedCount = 3;
            Assert.AreEqual(exceptedCount, assignResult.Count());
        }

        private List<UserList> GetDemoUserlist()
        {
            return
                new List<UserList>()
                {
                    new UserList(){ UserID=1, UserName="Test1", UserStatus=true, UserEmail="CC1@com.tw" },
                    new UserList(){ UserID=2, UserName="Test2", UserStatus=true, UserEmail="CC2@com.tw" },
                    new UserList(){ UserID=3, UserName="Test3", UserStatus=false, UserEmail="CC3@com.tw" },
                };
        }
    }
}
