using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBoard.Models.Interface
{
    public interface IMessagePic
    {
        void Create(MessagePic pic);
        void Update(MessagePic pic);

        IQueryable<MessagePic> GetMessagePics(int messageID);
    }
}
