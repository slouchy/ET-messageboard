using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBoard.Models.Interface
{
    interface IUserList : IDisposable
    {
        void Create(UserList userList);

        void Update(UserList userList);

        UserList GetUserInfo(int userID);

        IQueryable<UserList> GetUserLists();


    }
}
