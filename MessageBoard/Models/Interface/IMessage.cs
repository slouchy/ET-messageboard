using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBoard.Models.Interface
{
    interface IMessage
    {
        void Create(MajorMessageList major, Message message);
        void Update(Message message);
        IQueryable<Message> GetMessages(int majorID);
        Message GetUniqueMessage(int messageID);
    }
}
