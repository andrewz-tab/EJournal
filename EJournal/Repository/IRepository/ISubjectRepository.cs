using EJournal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EJournal.Repository.IRepository
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        public Task UpdateAsync(Subject subject);
        new Task<Subject> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<Subject, bool>> filter = null,
            Func<IQueryable<Subject>, IIncludableQueryable<Subject, object>> include = null,
            bool isTracking = true
            );
    }
}
