using Microsoft.EntityFrameworkCore;
using RefugeeSkillsPlatform.Core.Interfaces;
using RefugeeSkillsPlatform.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly RefugeeSkillsDbContext context;
        private DbSet<T> dbSet;
        public GenericRepository(RefugeeSkillsDbContext _context)
        {
            context = _context;
            dbSet = context.Set<T>();
        }
        public void Add(T entity)
        {
             dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet;
        }

        

        public T FirstOrDefult(Expression<Func<T, bool>> predicate)
        {
            return dbSet.FirstOrDefault(predicate);
        }


    }
}
