using System;
using System.Collections.Generic;
using MessageBoard.Models;
using MessageBoard.Models.Interface;
using MessageBoard.Models.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MessageBoard.Tests
{
    [TestClass]
    public class Test_UserList
    {
        [TestMethod]
        public void GetAllUsers_Count()
        {
            var mockDBUserList = Substitute.For<UserListRepository>();
            mockDBUserList.GetUserLists()
            //var mockDBUserList = Substitute.For<UserListRepository>();
            //mockDBUserList.
            //mockDBUserList.GetUserLists().Returns(new List<UserList>(){
            //    new UserList(){ UserID=1, UserName="Test1", UserStatus=true },
            //    new UserList(){ UserID=2, UserName="Test2", UserStatus=true },
            //    new UserList(){ UserID=3, UserName="Test3", UserStatus=false },
            //});

            //UserTool2 userTool = new UserTool2(mockDBUserList);

            //var expectedCount = 2;
            //var assignResult = userTool.GetUserList();
            //Assert.AreEqual(expected: expectedCount, actual: assignResult.Count);
        }
    }
}
