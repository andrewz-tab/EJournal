using EJournal.Data;
using EJournal.Models;
using EJournal.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EJournal.Repository
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        private readonly JournalDbContext _dbContext;
        public SubjectRepository(JournalDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }


        new public async Task<Subject> FirstOrDefaultAsync(bool isDetail = false, Expression<Func<Subject, bool>> filter = null, Func<IQueryable<Subject>, IIncludableQueryable<Subject, object>> include = null, bool isTracking = true)
        {
            IQueryable<Subject> query = dbSet;
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
            Subject subject = await query.FirstOrDefaultAsync();

            if (subject != null && isDetail)
            {
                await _dbContext.Entry(subject).Collection(c => c.Disciplines).LoadAsync();
                await _dbContext.Employees.Where(e => e.Disciplines.Any(d => d.SubjectKey == subject.Id)).Include(s => s.Account).ThenInclude(a => a.PersonalData).LoadAsync(); ;
                await _dbContext.Disciplines.Where(d => d.SubjectKey == subject.Id).Include(d => d.Class).LoadAsync(); ;
            }
            return subject;
        }

        public async Task UpdateAsync(Subject subject)
        {
            await Task.Run(() => dbSet.Update(subject));
        }
    }
}
