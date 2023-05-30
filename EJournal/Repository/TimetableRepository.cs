using EJournal.Data;
using EJournal.Models;
using EJournal.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace EJournal.Repository
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly JournalDbContext _dbContext;
        public TimetableRepository(JournalDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<Class>> GetClassesTimetableByDay(DateTime date)
        {

            IEnumerable<Class> classes = null;

            _dbContext.Disciplines.Include(d => d.Subject).Include(d => d.Lessons.Where(l => l.DateTime == date)).Load();
            await _dbContext.Employees.Where(e => _dbContext.Disciplines.Any(d => d.EmployeeKey == e.Id)).Include(e => e.Account).ThenInclude(a => a.PersonalData).LoadAsync();
            /*await _dbContext.Disciplines.ForEachAsync(d =>
            {
                _dbContext.Entry(d).Reference(d => d.Employee);
                _dbContext.Entry(d.Employee).Reference(e =>e.Account);
                _dbContext.Entry(d.Employee.Account).Reference(a => a.PersonalData);
            });*/
            //_dbContext.Lessons.Where(l => l.DateTime == currentDay);
            classes = _dbContext.Classes;
            return classes;
        }

        public async Task<IEnumerable<Lesson>> GetTimetalbeByWeek(int? weekId)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
