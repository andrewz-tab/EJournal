using EJournal.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EJournal.Repository.IRepository
{
    public interface IStudentRepository : IRepository<Student>
    {
        public Task UpdateAsync(Student student);
        new public Task<Student> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<Student, bool>> filter = null,
            Func<IQueryable<Student>, IIncludableQueryable<Student, object>> include = null,
            bool isTracking = true
            );
        new public void Remove(Student student);
        new bool Any(Func<Student, bool> predicate);
    }
}
