using EJournal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Linq.Expressions;

namespace EJournal.Repository.IRepository
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        public Task UpdateAsync(Employee employee);
        new public Task<Employee> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<Employee, bool>> filter = null,
            Func<IQueryable<Employee>, IIncludableQueryable<Employee, object>> include = null,
            bool isTracking = true
            );
        new public void Remove(Employee employee);
        public IEnumerable<Role> GetSelectedRoles(int[] RoleIds);
        public Task<TypeUser> GetEmployeeTypeUserAsync();
        new bool Any(Func<Employee, bool> predicate);
    }
}
