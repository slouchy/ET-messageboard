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
        [TestMethod]
        public void GetAllowedUsers_Count()
        {
            // use RealDB
            //MessageBoardEntities messageBoardEntities = new MessageBoardEntities();
            //var realDBUserList = messageBoardEntities.UserList.AsQueryable();

            //arrange
            var data = new List<UserList>()
            {
                new UserList(){ UserID=1, UserName="Test1", UserStatus=true },
                new UserList(){ UserID=2, UserName="Test2", UserStatus=true },
                new UserList(){ UserID=3, UserName="Test3", UserStatus=false },
            }.AsQueryable();


            // use RealDB
            //var mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(realDBUserList);
            var mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>().Initialize(data);

            // 提出成擴充函式
            //var mockDbSet = Substitute.For<IDbSet<UserList>, DbSet<UserList>>();
            //mockDbSet.Provider.Returns(data.Provider);
            //mockDbSet.Expression.Returns(data.Expression);
            //mockDbSet.ElementType.Returns(data.ElementType);

            var mockDbContext = Substitute.For<MessageBoardEntities>();
            mockDbContext.UserList.Returns(mockDbSet);
            var mockDbUserList = new UserListRepository(mockDbContext);

            var expectedCount = 2;
            var assignResult = mockDbUserList.GetUserLists();
            Assert.AreEqual(expectedCount, assignResult.Count());
        }
    }

    public static class ExtentionMethods
    {
        public static IDbSet<T> Initialize<T>(this IDbSet<T> dbSet, IQueryable<T> data) where T : class
        {
            dbSet.Provider.Returns(data.Provider);
            dbSet.Expression.Returns(data.Expression);
            dbSet.ElementType.Returns(data.ElementType);
            dbSet.GetEnumerator().Returns(data.GetEnumerator());
            return dbSet;
        }
    }
}
