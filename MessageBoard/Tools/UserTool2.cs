using MessageBoard.DataInterface;
using MessageBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageBoard.Tools
{
    public class UserTool2
    {
        IUserList _iDBUserList;

        public UserTool2(IUserList iDBUserList)
        {
            _iDBUserList = iDBUserList;
        }

        public List<UserList> GetUserList()
        {
            var userList = _iDBUserList.GetUserLists()
                .Where(r => r.UserStatus)
                .ToList();
            return userList;
        }

        public List<UserList> GetUniqueUser(int userID)
        {
            var user = _iDBUserList.GetUser(userID)
                .Where(r=>r.UserID== userID)
                .ToList();
            return user;
        }
    }


}