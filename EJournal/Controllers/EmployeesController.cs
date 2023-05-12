using EJournal.Data;
using EJournal.Models;
using EJournal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EJournal.Controllers
{
    public class EmployeesController : Controller
    {
        private JournalDbContext _dbContext;
        public EmployeesController(JournalDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<SelectListItem> roles = _dbContext.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }).ToList();
            IEnumerable<Employee> FullAccounts = _dbContext.Employees
                .Include(e => e.Account)
                .ThenInclude(a => a.PersonalData)
                .Include(e => e.Roles)
                .ToList();
            return View(FullAccounts);
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            UpSertEmployeeViewModel EmployeeAccount = new UpSertEmployeeViewModel();
            IEnumerable<SelectListItem> roles = _dbContext.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }).ToList();
            MultiSelectList roleMultpleSelectList = new MultiSelectList(roles.OrderBy(r => r.Text), "Value", "Text");
            EmployeeAccount = new UpSertEmployeeViewModel
            {
                RoleMultpleSelectList = roleMultpleSelectList
            };
            if (id  == null)
            {
                EmployeeAccount.Id = 0;
                return View(EmployeeAccount);
            }
            else
            {
                Employee employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
                if(employee == null)
                {
                    return NotFound();
                }
                await _dbContext.Entry(employee).Reference(e => e.Account).LoadAsync();
                await _dbContext.Entry(employee.Account).Reference(a => a.PersonalData).LoadAsync();
                await _dbContext.Entry(employee).Collection(e => e.Roles).LoadAsync();
                EmployeeAccount.SetEmployee(employee, roleMultpleSelectList);


                return View(EmployeeAccount);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UpSertEmployeeViewModel inputAccountEmployee)
        {

            ModelState.Remove("RoleMultpleSelectList");
            if (ModelState.IsValid)
            {
                List<Role> SelectedRoles = _dbContext.Roles.Where(r => inputAccountEmployee.RoleIds.Contains(r.Id)).ToList();
                TypeUser typeUser = await _dbContext.TypeUsers.FirstOrDefaultAsync(tu => tu.Name == "Сотрудник");
                if (inputAccountEmployee.Id == 0)
                {
                    Employee newEmployee = inputAccountEmployee.CreateEmployee(typeUser, SelectedRoles);
                    await _dbContext.Employees.AddAsync(newEmployee);
                }
                else
                {
                    Employee employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == inputAccountEmployee.Id);
                    if (employee == null)
                    {
                        return NotFound();
                    }
                    await _dbContext.Entry(employee).Reference(e => e.Account).LoadAsync();
                    await _dbContext.Entry(employee.Account).Reference(a => a.PersonalData).LoadAsync();
                    await _dbContext.Entry(employee).Collection(e => e.Roles).LoadAsync();
                    inputAccountEmployee.GetCopy(employee, SelectedRoles);
                    _dbContext.Employees.Update(employee);
                }
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(inputAccountEmployee);

        }

        public async Task<IActionResult> Create(UpSertEmployeeViewModel? inputAccountEmployee)
        {
            return RedirectToAction("Upsert");
        }

        
    }
}
