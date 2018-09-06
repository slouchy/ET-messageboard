using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MessageBoard.Models;
using MessageBoard.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MessageBoard.Tests
{
    [TestClass]
    public class Test_UserTool
    {
        private MessageBoardEntities _mockDbContext = Substitute.For<MessageBoardEntities>();

        [TestInitialize]
        public void InitTest()
        {
            var mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist());
            _mockDbContext.UserList.Returns(mockDbSet);
        }

        [TestMethod]
        public void GetLoginUserByUserNameWithUserAllowed()
        {
            var mockUserTool = new UserTool(_mockDbContext);

            var userName = "test1";
            var assignResult = mockUserTool.GetLoginedUser(userName);
            Assert.AreEqual(userName, assignResult.UserName, true);
        }

        [TestMethod]
        public void GetLoginUserByUserNameWithUserNotAllowed()
        {
            var mockUserTool = new UserTool(_mockDbContext);

            var userName = "test3";
            var assignResult = mockUserTool.GetLoginedUser(userName);
            Assert.AreEqual(null, assignResult.UserName, true);
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
