using MessageBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBoard.DataInterface
{
    public interface IUserList
    {
        /// <summary>
        /// 取得使用者清單
        /// </summary>
        /// <returns></returns>
        List<UserList> GetUserLists();

        /// <summary>
        /// 取得單一使用者
        /// </summary>
        /// <param name="userID">使用者 ID</param>
        /// <returns></returns>
        List<UserList> GetUser(int userID);

    }
}
