using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);
        IEnumerable<T> GetAll();

       
        T FirstOrDefult(Expression<Func<T, bool>> predicate);

    }
}
