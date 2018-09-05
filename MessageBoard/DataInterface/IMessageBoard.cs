using MessageBoard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBoard.DataInterface
{
    public interface IMessageBoard
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : MessageBoardEntities;
        int SaveChanges();
    }
}
