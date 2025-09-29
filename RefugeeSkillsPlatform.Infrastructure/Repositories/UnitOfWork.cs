using Microsoft.EntityFrameworkCore;
using RefugeeSkillsPlatform.Core.Interfaces;
using RefugeeSkillsPlatform.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RefugeeSkillsDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(RefugeeSkillsDbContext context)
        {
            _context = context;
        }
        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                var repoInstance = new GenericRepository<T>(_context);
                _repositories[type] = repoInstance;
            }

            return (IGenericRepository<T>)_repositories[type];
        }
        public List<TResult> SpListRepository<TResult>(string sql, params object[] parameters) where TResult : class
        {
            try
            {
                return _context.Set<TResult>().FromSqlRaw(sql, parameters).ToList();
            }
            catch
            {
                return new List<TResult>();
            }
        }
        public int Commit()
        {
            return _context.SaveChanges();
        }
       

    }
}
