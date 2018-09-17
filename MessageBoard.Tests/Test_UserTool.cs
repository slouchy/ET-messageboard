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
        public void isUserEmailExist_ShouldBeTrue()
        {
            IDbSet<UserList> mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            _mockDbContext.UserList.Returns(mockDbSet);
            UserTool userTool = new UserTool(_mockDbContext);
            string userName = "Test1";
            string userEmail = "CC1@com.tw";

            bool actual = userTool.isUserEmailExist(userName, userEmail);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void isUserEmailExist_ShouldBeFalse()
        {
            IDbSet<UserList> mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            _mockDbContext.UserList.Returns(mockDbSet);
            UserTool userTool = new UserTool(_mockDbContext);
            string userName = "Test1";
            string userEmail = "anyEmail";

            bool actual = userTool.isUserEmailExist(userName, userEmail);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void isUserEqulsDB_UseEncryptPw_ShouldBeTrue()
        {
            IDbSet<UserList> mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            ISaltPW saltPW = new SaltPW();
            _mockDbContext.UserList.Returns(mockDbSet);
            UserTool userTool = new UserTool(_mockDbContext, saltPW);
            string userName = "Test1";
            string userPw = "Test1";
            bool actual = false;

            // actual
            actual = userTool.isUserEqulsDB(userName, userPw);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void isUserEqulsDB_UseFakeData_ShouldBeTrue()
        {
            IDbSet<UserList> mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(GetDemoUserlist().AsQueryable());
            ISaltPW saltPW = new Stub_SaltPW();
            _mockDbContext.UserList.Returns(mockDbSet);
            UserTool userTool = new UserTool(_mockDbContext, saltPW);
            string userName = "Test2";
            string userPw = "Test2";
            bool actual = false;

            // actual
            actual = userTool.isUserEqulsDB(userName, userPw);

            Assert.IsTrue(actual);
        }

        private static List<UserList> GetDemoUserlist()
        {
            return new List<UserList>()
                {
                    new UserList(){ UserID=1, UserName="Test1", UserStatus=true, UserEmail="CC1@com.tw", UserPW="BVI0J7BZ039zjrgJZRiX6Q==" },
                    new UserList(){ UserID=2, UserName="Test2", UserStatus=true, UserEmail="CC2@com.tw", UserPW="Test2" },
                    new UserList(){ UserID=3, UserName="Test3", UserStatus=false, UserEmail="CC3@com.tw", UserPW="Test3" },
                };
        }
    }
}
