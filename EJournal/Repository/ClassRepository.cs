using EJournal.Data;
using EJournal.Models;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;
using System.Linq.Expressions;

namespace EJournal.Repository
{
    public class ClassRepository : Repository<Class>, IClassRepository
    {
        private readonly JournalDbContext _dbContext;
        public ClassRepository(JournalDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }


        new public async Task<Class> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<Class, bool>> filter = null,
            Func<IQueryable<Class>, IIncludableQueryable<Class, object>> include = null,
            bool isTracking = true
            )
        {
            IQueryable<Class> query = dbSet;
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
            Class Class = await query.FirstOrDefaultAsync();

            if (Class != null && isDetail)
            {
                await _dbContext.Entry(Class).Collection(c => c.Students).LoadAsync();
                await _dbContext.Entry(Class).Reference(c => c.Employee).LoadAsync();
                if (Class.Employee != null)
                {
                    await _dbContext.Entry(Class.Employee).Reference(e => e.Account).LoadAsync();
                    await _dbContext.Entry(Class.Employee.Account).Reference(a => a.PersonalData).LoadAsync();
                }
                await _dbContext.Students.Include(s => s.Account).ThenInclude(a => a.PersonalData).LoadAsync();
                await _dbContext.Disciplines.Include(d => d.Subject).Include(d => d.Employee).ThenInclude(e => e.Account).ThenInclude(a => a.PersonalData).LoadAsync();
            }
            return Class;
        }

        public async Task UpdateAsync(Class Class)
        {
            await Task.Run(() => dbSet.Update(Class));
        }
    }
}
