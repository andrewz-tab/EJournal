using EJournal.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EJournal.Repository.IRepository
{
    public interface ILessonRepository : IRepository<Lesson>
    {
        public Task UpdateAsync(Lesson lesson);
        new Task<Lesson> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<Lesson, bool>> filter = null,
            Func<IQueryable<Lesson>, IIncludableQueryable<Lesson, object>> include = null,
            bool isTracking = true
            );
    }
}
