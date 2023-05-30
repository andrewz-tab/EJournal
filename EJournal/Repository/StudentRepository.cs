using EJournal.Data;
using EJournal.Models;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using EJournal.Repository.IRepository;

namespace EJournal.Repository
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        private readonly JournalDbContext _dbContext;
        public StudentRepository(JournalDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        new public async Task<Student> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<Student, bool>> filter = null,
            Func<IQueryable<Student>, IIncludableQueryable<Student, object>> include = null,
            bool isTracking = true
            )
        {

            IQueryable<Student> query = dbSet;
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
            Student student = await query.FirstOrDefaultAsync();

            if (student != null)
            {
                await _dbContext.Entry(student).Reference(e => e.Account).LoadAsync();
                await _dbContext.Entry(student.Account).Reference(a => a.PersonalData).LoadAsync();
                await _dbContext.Entry(student).Reference(e => e.Class).LoadAsync();
                await _dbContext.Entry(student.Account).Reference(a => a.TypeUser).LoadAsync();

                if (isDetail)
                {
                    await _dbContext.Entry(student.Class).Collection(c => c.Disciplines).LoadAsync();
                    await _dbContext.Disciplines.Where(d => d.ClassKey == student.ClassKey).Include(d => d.Subject).Include(d => d.Lessons)
                        .Include(d => d.Employee).ThenInclude(e => e.Account).ThenInclude(a => a.PersonalData).LoadAsync();
                    await _dbContext.Marks.Where(m => m.StudentKey == student.Id).LoadAsync();
                }
            }
            return student;
        }

        public async Task UpdateAsync(Student student)
        {
            Student studentUpdate = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == student.Id);

            if (studentUpdate != null)
            {
                await _dbContext.Entry(student).Reference(e => e.Account).LoadAsync();
                await _dbContext.Entry(student.Account).Reference(a => a.PersonalData).LoadAsync();
                await _dbContext.Entry(student).Reference(e => e.Class).LoadAsync();
            }
            dbSet.Update(student);
        }
        new public void Remove(Student student)
        {
            _dbContext.Remove(student.Account);
        }

        new public bool Any(Func<Student, bool> predicate)
        {
            return dbSet.Include(s => s.Account).ThenInclude(s => s.PersonalData).Any(predicate);
        }
        public async Task<TypeUser> GetStudentTypeUserAsync()
        {
            return await _dbContext.TypeUsers.FirstOrDefaultAsync(tu => tu.Name == "Ученик");
        }
        new public async Task AddAsync(Student entity)
        {
            if (entity != null)
            {
                if (entity.Account != null)
                {
                    entity.Account.TypeUser = await GetStudentTypeUserAsync();
                    await dbSet.AddAsync(entity);
                }
            }
        }
    }
}
