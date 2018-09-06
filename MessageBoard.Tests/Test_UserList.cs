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
    public class Test_UserList
    {
        private MessageBoardEntities _mockDbContext;

        [TestInitialize]
        public void InitTest()
        {
            var mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist());
            _mockDbContext = Substitute.For<MessageBoardEntities>();
            _mockDbContext.UserList.Returns(mockDbSet);
        }

        [TestMethod]
        public void GetAllowedUsersCount()
        {
            var mockDbUserList = new UserListRepository(_mockDbContext);

            var expectedCount = 2;
            var assignResult = mockDbUserList.GetUserLists();
            Assert.AreEqual(expectedCount, assignResult.Count());
        }

        [TestMethod]
        public void GetUniqueUserByUserNameWithUserAllowed()
        {
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string userName = "Test1";
            var assignResult = mockDbUserList.GetUserInfo(userName);
            Assert.AreEqual(userName, assignResult.UserName);
        }

        [TestMethod]
        public void GetUniqueUserByUserNameWithUserNotAllowed()
        {
            var mockDbUserList = new UserListRepository(_mockDbContext);

            string userName = "Test3";
            var assignResult = mockDbUserList.GetUserInfo(userName);
            Assert.AreEqual(null, assignResult.UserName);
        }

        private IQueryable<UserList> GetDemoUserlist()
        {
            return
                new List<UserList>()
                {
                    new UserList(){ UserID=1, UserName="Test1", UserStatus=true },
                    new UserList(){ UserID=2, UserName="Test2", UserStatus=true },
                    new UserList(){ UserID=3, UserName="Test3", UserStatus=false },
                }.AsQueryable();
        }
    }
}
