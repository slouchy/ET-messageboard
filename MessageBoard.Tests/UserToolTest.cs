using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using MessageBoard.DataInterface;
using MessageBoard.Tools;

namespace MessageBoard.Tests
{
    /// <summary>
    /// UserToolTest 的摘要說明
    /// </summary>
    [TestClass]
    public class UserToolTest
    {
        [TestMethod]
        public void GetAllUsers_Count()
        {
            var mockDBUserList = Substitute.For<IUserList>();
            mockDBUserList.GetUserLists().Returns(new List<Models.UserList>(){
                new Models.UserList(){ UserID=1, UserName="Test1", UserStatus=true },
                new Models.UserList(){ UserID=2, UserName="Test2", UserStatus=true },
                new Models.UserList(){ UserID=3, UserName="Test3", UserStatus=false },
            });

            UserTool2 userTool = new UserTool2(mockDBUserList);

            var expectedCount = 2;
            var assignResult = userTool.GetUserList();
            Assert.AreEqual(expected: expectedCount, actual: assignResult.Count);
        }

        [TestMethod]
        public void GetUniqueUser_Count()
        {
            var mockDBUserList = Substitute.For<IUserList>();
            mockDBUserList.GetUser(1).Returns(new List<Models.UserList>(){
                new Models.UserList(){ UserID=1, UserName="Test1", UserStatus=true },
                new Models.UserList(){ UserID=2, UserName="Test2", UserStatus=true},
                new Models.UserList(){ UserID=3, UserName="Test3", UserStatus=false },
            });
            // new Models.UserList() { UserID = 3, UserName = "Test3", UserStatus = false, UserPW = "pw", UserAccess = 1, UserEmail = "Email", UserIcon = "Icon", CreateDate = new DateTime(), CreateIP = "127.0.0.1" },

            UserTool2 userTool = new UserTool2(mockDBUserList);

            var expectedCount = 1;
            var assignResult = userTool.GetUniqueUser(1);
            Assert.AreEqual(expectedCount, assignResult.Count);
        }
    }
}
