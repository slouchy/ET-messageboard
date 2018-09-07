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

        [TestInitialize]
        public void InitTest()
        {
            var mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist());
            _mockDbContext.UserList.Returns(mockDbSet);
        }

        [TestMethod]
        public void GetUserLists_AllowedUsersCount()
        {
            var mockDbUserList = new UserListRepository(_mockDbContext);

            var expectedCount = 2;
            var assignResult = mockDbUserList.GetUserLists();
            Assert.AreEqual(expectedCount, assignResult.Count());
        }

        [TestMethod]
        public void GetUserInfo_ByUserNameWithUserAllowed()
        {
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string userName = "Test1";
            var assignResult = mockDbUserList.GetUserInfo(userName);
            Assert.AreEqual(userName, assignResult.UserName);
        }

        [TestMethod]
        public void GetUserInfo_ByUserNameWithUserNotAllowed()
        {
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string userName = "Test3";
            var assignResult = mockDbUserList.GetUserInfo(userName);
            Assert.AreEqual(null, null);
        }

        [TestMethod]
        public void isEmailExist_EmailExistInDB()
        {
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string email = "CC1@com.tw";
            var assignResult = mockDbUserList.isEmailExist(email);
            Assert.IsTrue(assignResult);
        }

        [TestMethod]
        public void isEmailExist_EmailNotInDB()
        {
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string email = "CC0@com.tw";
            var assignResult = mockDbUserList.isEmailExist(email);
            Assert.IsFalse(assignResult);
        }

        [TestMethod]
        public void isUserNameExist_UserNameExistInDB()
        {
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
            var mockDbUserList = new UserListRepository(_mockDbContext);
            var assignResult = mockDbUserList.isUserNameExist("Test0");
            Assert.IsFalse(assignResult);
        }

        private IQueryable<UserList> GetDemoUserlist()
        {
            return
                new List<UserList>()
                {
                    new UserList(){ UserID=1, UserName="Test1", UserStatus=true, UserEmail="CC1@com.tw" },
                    new UserList(){ UserID=2, UserName="Test2", UserStatus=true, UserEmail="CC2@com.tw" },
                    new UserList(){ UserID=3, UserName="Test3", UserStatus=false, UserEmail="CC3@com.tw" },
                }.AsQueryable();
        }
    }


}
