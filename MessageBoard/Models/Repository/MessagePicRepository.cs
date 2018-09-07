using MessageBoard.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageBoard.Models.Repository
{
    public class MessagePicRepository : IMessagePic, IDisposable
    {
        public MessagePicRepository(MessageBoardEntities entities)
        {
            db = entities;
        }

        protected MessageBoardEntities db
        {
            get;
            private set;
        }

        public void Create(MessagePic pic)
        {
            if (pic == null)
            {
                throw new ArgumentNullException("MessagePic");
            }

            db.MessagePic.Add(pic);
            db.SaveChanges();
        }

        public void Update(MessagePic pic)
        {
            if (pic == null)
            {
                throw new ArgumentNullException("MessagePic");
            }

            db.SaveChanges();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IQueryable<MessagePic> GetMessagePics(int messageID)
        {
            return db.MessagePic.Where(r => r.MessageID == messageID && r.picStatus);
        }
    }
}