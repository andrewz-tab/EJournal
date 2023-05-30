using EJournal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EJournal.Repository.IRepository
{
    public interface IDisciplineRepository : IRepository<Discipline>
    {
        public Task UpdateAsync(Discipline discipline);
        new Task<Discipline> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<Discipline, bool>> filter = null,
            Func<IQueryable<Discipline>, IIncludableQueryable<Discipline, object>> include = null,
            bool isTracking = true
            );
    }
}
