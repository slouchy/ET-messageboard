using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MessageBoard.Models;
using MessageBoard.Models.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MessageBoard.Tests
{
    [TestClass]
    public class Test_DBMessagePic
    {
        private MessageBoardEntities _mockDbContext = Substitute.For<MessageBoardEntities>();

        [TestInitialize]
        public void InitTest()
        {
            var mockDbSet = Substitute.For<IDbSet<MessagePic>, DbSet<MessagePic>>().Initialize(GetDemoMessagePics());
            _mockDbContext.MessagePic.Returns(mockDbSet);
        }

        [TestMethod]
        public void GetMessagePics_MessageID6Count()
        {
            var mockDbMessagePics = new MessagePicRepository(_mockDbContext);
            var assignResult = mockDbMessagePics.GetMessagePics(6);
            Assert.AreEqual(0, assignResult.Count());
        }

        [TestMethod]
        public void GetMessagePics_MessageID1Count()
        {
            var mockDbMessagePics = new MessagePicRepository(_mockDbContext);
            var assignResult = mockDbMessagePics.GetMessagePics(1);
            Assert.AreEqual(1, assignResult.Count());
        }

        [TestMethod]
        public void GetMessagePics_MessageID2Count()
        {
            var mockDbMessagePics = new MessagePicRepository(_mockDbContext);
            var assignResult = mockDbMessagePics.GetMessagePics(2);
            Assert.AreEqual(0, assignResult.Count());
        }

        [TestMethod]
        public void GetMessagePics_MessageID5Count()
        {
            var mockDbMessagePics = new MessagePicRepository(_mockDbContext);
            var assignResult = mockDbMessagePics.GetMessagePics(5);
            Assert.AreEqual(2, assignResult.Count());
        }

        private IQueryable<MessagePic> GetDemoMessagePics()
        {
            return
                new List<MessagePic>()
                {
                    new MessagePic(){ PicID=1, MessageID=5, picStatus=true },
                    new MessagePic(){ PicID=2, MessageID=5, picStatus=false },
                    new MessagePic(){ PicID=3, MessageID=5, picStatus=true },
                    new MessagePic(){ PicID=4, MessageID=3, picStatus=true },
                    new MessagePic(){ PicID=6, MessageID=2, picStatus=false},
                    new MessagePic(){ PicID=8, MessageID=1, picStatus=true },
                }.AsQueryable();
        }
    }
}
