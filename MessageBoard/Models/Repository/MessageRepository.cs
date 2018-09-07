using MessageBoard.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageBoard.Models.Repository
{
    public class MessageRepository : IMessage, IDisposable
    {
        private readonly MessageBoardEntities _db;
        private readonly IUserList _userList;
        private readonly IMessagePic _messagePic;

        public MessageRepository()
        {
            _userList = new UserListRepository(new MessageBoardEntities());
            _messagePic = new MessagePicRepository(new MessageBoardEntities());
        }
        public MessageRepository(MessageBoardEntities entities, IUserList userList, IMessagePic messagePic)
        {
            _db = entities;
            _userList = userList;
            _messagePic = messagePic;
        }

        public void Create(MajorMessageList major, Message message)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Message> GetMessages(int majorID)
        {
            throw new NotImplementedException();
        }

        public Message GetUniqueMessage(int messageID)
        {
            throw new NotImplementedException();
        }

        public void Update(Message message)
        {
            throw new NotImplementedException();
        }
    }
}