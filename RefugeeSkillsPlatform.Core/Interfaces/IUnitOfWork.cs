using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Interfaces
{
    public interface IUnitOfWork
    {
        int Commit();
        IGenericRepository<T> GetRepository<T>() where T : class;
        List<TEntity> SpListRepository<TEntity>(string spName, params object[] parameters) where TEntity : class;
    }
}
