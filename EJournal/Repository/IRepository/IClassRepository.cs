using EJournal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EJournal.Repository.IRepository
{
    public interface IClassRepository : IRepository<Class>
    {
        public Task UpdateAsync(Class Class);
        new Task<Class> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<Class, bool>> filter = null,
            Func<IQueryable<Class>, IIncludableQueryable<Class, object>> include = null,
            bool isTracking = true
            );
    }
}
