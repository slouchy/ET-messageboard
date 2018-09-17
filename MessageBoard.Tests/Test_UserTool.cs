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

        [TestMethod]
        public void isUserEqulsDB_UseRealData_ShouldBeTrue()
        {
            UserTool userTool = new UserTool();
            string userName = "Test";
            string userPw = "Test1";
            bool actual = false;

            // actual
            actual = userTool.isUserEqulsDB(userName, userPw);

            Assert.IsTrue(actual);
            Assert.Inconclusive("建立依賴注入後需要移除的測試方法");

        }

        [TestMethod]
        public void isUserEqulsDB_UseFakeData_ShouldBeTrue()
        {
            List<UserList> userLists = new List<UserList>()
                {
                    new UserList(){ UserID=1, UserName="Test1", UserStatus=true, UserEmail="CC1@com.tw", UserPW="Test1" },
                    new UserList(){ UserID=2, UserName="Test2", UserStatus=true, UserEmail="CC2@com.tw", UserPW="Test2" },
                    new UserList(){ UserID=3, UserName="Test3", UserStatus=false, UserEmail="CC3@com.tw", UserPW="Test3" },
                };
            IDbSet<UserList> _mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(userLists.AsQueryable());
            _mockDbContext.UserList.Returns(_mockDbSet);
            UserTool userTool = new UserTool(_mockDbContext);
            string userName = "Test1";
            string userPw = "Test1";
            bool actual = false;

            // actual
            actual = userTool.isUserEqulsDB(userName, userPw);

            Assert.IsTrue(actual);
            Assert.Inconclusive("驗證這個測試方法的正確性。");

        }
    }
}
