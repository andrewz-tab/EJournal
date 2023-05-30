using EJournal.Data;
using EJournal.Models;
using EJournal.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EJournal.Repository
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {

        private readonly JournalDbContext _dbContext;
        public LessonRepository(JournalDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }
        new public async Task<Lesson> FirstOrDefaultAsync(bool isDetail = false, Expression<Func<Lesson, bool>> filter = null, Func<IQueryable<Lesson>, IIncludableQueryable<Lesson, object>> include = null, bool isTracking = true)
        {
            IQueryable<Lesson> query = dbSet;
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
            Lesson lesson = await query.FirstOrDefaultAsync();
            if (lesson != null)
            {
                await dbSet.Entry(lesson).Reference(l => l.Discipline).LoadAsync();
                await _dbContext.Entry(lesson.Discipline).Reference(d => d.Subject).LoadAsync();
                await _dbContext.Entry(lesson.Discipline).Reference(d => d.Class).LoadAsync();
                await _dbContext.Entry(lesson.Discipline).Reference(d => d.Employee).LoadAsync();
                await _dbContext.Entry(lesson.Discipline.Employee).Reference(e => e.Account).LoadAsync();
                await _dbContext.Entry(lesson.Discipline.Employee.Account).Reference(a => a.PersonalData).LoadAsync();
                if (isDetail)
                {
                    await _dbContext.Students.Where(s => s.ClassKey == lesson.Discipline.ClassKey)
                        .Include(a => a.Account).ThenInclude(a => a.PersonalData)
                        .Include(s => s.Marks).ThenInclude(m => m.Lesson)
                        .LoadAsync();
                }
            }

            return lesson;
        }
        public async Task UpdateAsync(Lesson lesson)
        {
            await Task.Run(() => dbSet.Update(lesson));
        }
    }
}
