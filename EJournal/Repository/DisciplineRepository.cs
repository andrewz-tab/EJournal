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
    public class DisciplineRepository : Repository<Discipline>, IDisciplineRepository
    {
        private readonly JournalDbContext _dbContext;
        public DisciplineRepository(JournalDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }


        new public async Task<Discipline> FirstOrDefaultAsync(bool isDetail = false, Expression<Func<Discipline, bool>> filter = null, Func<IQueryable<Discipline>, IIncludableQueryable<Discipline, object>> include = null, bool isTracking = true)
        {
            IQueryable<Discipline> query = dbSet;
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
            Discipline discipline = await query.FirstOrDefaultAsync();
            if(discipline != null)
            {
                await dbSet.Entry(discipline).Reference(d => d.Subject).LoadAsync();
                await dbSet.Entry(discipline).Reference(d => d.Class).LoadAsync();
                await dbSet.Entry(discipline).Reference(d => d.Employee).LoadAsync();
                await _dbContext.Entry(discipline.Employee).Reference(e => e.Account).LoadAsync();
                await _dbContext.Entry(discipline.Employee.Account).Reference(a => a.PersonalData).LoadAsync();
                if (isDetail)
                {
                    await dbSet.Entry(discipline).Collection(d => d.Lessons).LoadAsync();
                    await _dbContext.Students.Where(s => s.ClassKey == discipline.ClassKey)
                        .Include(a => a.Account).ThenInclude(a => a.PersonalData)
                        .Include(s => s.Marks).ThenInclude(m => m.Lesson)
                        .LoadAsync();
                    
                }
            }
            
            return discipline;
        }


        public async Task UpdateAsync(Discipline discipline)
        {
            await Task.Run(() => dbSet.Update(discipline));    
        }
    }
}
