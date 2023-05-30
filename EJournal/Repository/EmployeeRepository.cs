using EJournal.Data;
using EJournal.Models;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;
using System.Linq.Expressions;

namespace EJournal.Repository
{

    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private readonly JournalDbContext _dbContext;
        public EmployeeRepository(JournalDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        new public async Task<Employee> FirstOrDefaultAsync(
            bool isDetail = false,
            Expression<Func<Employee, bool>> filter = null,
            Func<IQueryable<Employee>, IIncludableQueryable<Employee, object>> include = null,
            bool isTracking = true
            )
        {

            IQueryable<Employee> query = dbSet;
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
            Employee employee = await query.FirstOrDefaultAsync();

            if (employee != null)
            {
                await _dbContext.Entry(employee).Reference(e => e.Account).LoadAsync();
                await _dbContext.Entry(employee.Account).Reference(a => a.PersonalData).LoadAsync();
                await _dbContext.Entry(employee).Collection(e => e.Roles).LoadAsync();
                if (isDetail)
                {
                    await _dbContext.Entry(employee).Collection(e => e.Disciplines).LoadAsync();
                    await _dbContext.Disciplines.Where(d => d.EmployeeKey == employee.Id).Include(d => d.Subject).Include(d => d.Class).LoadAsync();
                    await _dbContext.Entry(employee).Collection(e => e.Classes).LoadAsync();
                }
            }
            return employee;
        }

        public async Task UpdateAsync(Employee employee)
        {
            Employee employeeUpdate = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);

            if (employeeUpdate != null)
            {
                await _dbContext.Entry(employee).Reference(e => e.Account).LoadAsync();
                await _dbContext.Entry(employee.Account).Reference(a => a.PersonalData).LoadAsync();
                await _dbContext.Entry(employee).Collection(e => e.Roles).LoadAsync();
            }

            _dbContext.Employees.Update(employee);
        }
        new public void Remove(Employee employee)
        {
            _dbContext.Remove(employee.Account);
        }
        public IEnumerable<Role> GetSelectedRoles(int[] RoleIds)
        {
            return _dbContext.Roles.Where(r => RoleIds.Contains(r.Id));
        }
        public async Task<TypeUser> GetEmployeeTypeUserAsync()
        {
            return await _dbContext.TypeUsers.FirstOrDefaultAsync(tu => tu.Name == "Сотрудник");
        }
        new public bool Any(Func<Employee, bool> predicate)
        {
            return dbSet.Include(e => e.Account).ThenInclude(a => a.PersonalData).Any(predicate);
        }
        new public async Task AddAsync(Employee entity)
        {
            if(entity !=null)
            {
                if(entity.Account != null)
                {
                    entity.Account.TypeUser = await GetEmployeeTypeUserAsync();
                    await dbSet.AddAsync(entity);
                }
            }
        }
    }
}
