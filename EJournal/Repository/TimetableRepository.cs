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

            await _dbContext.Disciplines.Include(d => d.Subject).Include(d => d.Lessons.Where(l => l.DateTime == date)).LoadAsync();
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

        public async Task<IEnumerable<Lesson>> GetTimetalbeDiariesByWeek(int classId, string? weekId, int? studentId)
        {
            DateTime dateBegin = ITimetableRepository.GetDateTimeByWeekId(weekId);
            List<DateTime> week = new List<DateTime>(6);
            for(int i = 0; i < 6; i++)
            {
                week.Add(dateBegin.AddDays(i));
            }
            IEnumerable<Lesson> lessons = _dbContext.Lessons
                .Where(l => l.Discipline.ClassKey == classId && week.Any(d => d == l.DateTime))
                .Include(l => l.Discipline)
                    .ThenInclude(d => d.Subject)
                .Include(l => l.Discipline)
                    .ThenInclude(d => d.Class)
                .Include(l => l.Discipline)
                    .ThenInclude(c => c.Employee)
                        .ThenInclude(e => e.Account)
                        .ThenInclude(a => a.PersonalData);
            if(studentId != null)
            {
                await _dbContext.Marks.Where(m => lessons.Contains(m.Lesson) && m.StudentKey == studentId).LoadAsync();
            }
            return lessons;
        }
        public async Task<IEnumerable<Lesson>> GetTimetableByWeekForTeacher(int teacherId, string? weekId)
        {

            DateTime dateBegin = ITimetableRepository.GetDateTimeByWeekId(weekId);
            List<DateTime> week = new List<DateTime>(6);
            for (int i = 0; i < 6; i++)
            {
                week.Add(dateBegin.AddDays(i));
            }
            IEnumerable<Lesson> lessons = _dbContext.Lessons
                .Where(l => l.Discipline.EmployeeKey == teacherId && week.Any(d => d == l.DateTime));
            await _dbContext.Lessons
                .Where(l => l.Discipline.EmployeeKey == teacherId && week.Any(d => d == l.DateTime))
                .Include(l => l.Discipline)
                    .ThenInclude(d => d.Subject)
                .Include(l => l.Discipline)
                    .ThenInclude(d => d.Class)
                .Include(l => l.Discipline)
                    .ThenInclude(c => c.Employee)
                        .ThenInclude(e => e.Account)
                        .ThenInclude(a => a.PersonalData).LoadAsync();
            

            return lessons;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

		public async Task<Employee> GetTeacher(int teacherId)
		{
            Employee employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == teacherId && e.Roles.Any(r => r.Name == WC.TeacherRole || r.Name == WC.HeadTeacherRole));

            if (employee == null)
                return employee;
            await _dbContext.Entry(employee).Reference(e => e.Account).LoadAsync();
            await _dbContext.Entry(employee.Account).Reference(a => a.PersonalData).LoadAsync();

            return employee;
		}
		public async Task<Class> GetClass(int classId)
		{
			Class classt = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == classId);

			return classt;
		}

	}
}
