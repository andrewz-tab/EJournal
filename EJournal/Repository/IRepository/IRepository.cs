using EJournal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EJournal.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> FindAsycn(int id);
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool isTracking = true
            );

        Task<T> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool isTracking = true
            );

        Task AddAsync(T entity);
        
        Task SaveAsync();
        void Remove(T entity);

        public IEnumerable<SelectListItem> GetAllEmployeesList();
        public IEnumerable<SelectListItem> GetAllTeachersList();
        public IEnumerable<SelectListItem> GetAllClassesList();
        public IEnumerable<SelectListItem> GetAllSubjectsList();
        public IEnumerable<SelectListItem> GetAllRolesList();
        bool Any(Func<T, bool> predicate);
    }
}
