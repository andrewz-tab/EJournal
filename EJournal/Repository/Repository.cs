using EJournal.Data;
using EJournal.Models;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EJournal.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly JournalDbContext _dbContext;
        internal DbSet<T> dbSet { get; set; }
        public Repository(JournalDbContext dbContext)
        {
            _dbContext = dbContext;
            this.dbSet = _dbContext.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task<T> FindAsycn(int id)
        {
            return await dbSet.FindAsync(id);
        }


        public async Task<T> FirstOrDefaultAsync(bool isDetail = false, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (include != null)
            {
                query = include(query);
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if(filter != null)
            {
                query = query.Where(filter);
            }
            if(include != null)
            {
                query = include(query);
            }
            if(orderBy != null)
            {
                query = orderBy(query);
            }
            if(!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
        public bool Any(Func<T, bool> predicate)
        {
            return dbSet.Any(predicate);
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public IEnumerable<SelectListItem> GetAllTeachersList()
        {
            return _dbContext.Employees
            .Where(e => e.Roles.Any(r => r.Name == "Учитель" || r.Name == "Завуч"))
            .Select(e => new SelectListItem
            {
                Text = e.Account.PersonalData.FullName,
                Value = e.Id.ToString()
            });
        }
        public IEnumerable<SelectListItem> GetAllSubjectsList()
        {
            return _dbContext.Subjects
            .Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });
        }
        public IEnumerable<SelectListItem> GetAllClassesList()
        {
            return _dbContext.Classes
            .Select(c => new SelectListItem
            {
                Text = c.Number.ToString() + c.Liter.ToString(),
                Value = c.Id.ToString()
            });
        }
        public IEnumerable<SelectListItem> GetAllRolesList()
        {
            return _dbContext.Roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();
        }
    }
}
