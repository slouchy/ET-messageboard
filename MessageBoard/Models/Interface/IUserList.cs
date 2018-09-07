using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBoard.Models.Interface
{
    public interface IUserList : IDisposable
    {
        void Create(UserList userList);

        void Update(UserList userList);

        UserList GetUserInfo(int userID);

        UserList GetUserInfo(string userName);

        bool isEmailExist(string email);
        bool isUserNameExist(string userName);

        IQueryable<UserList> GetUserLists();
    }
}
