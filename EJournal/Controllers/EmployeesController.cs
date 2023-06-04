using EJournal.Data;
using EJournal.Models;
using EJournal.Models.ViewModels.EmployeeViewModels;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EJournal.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _dbContext;
        public EmployeesController(IEmployeeRepository dbContext)
        {
            this._dbContext = dbContext;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Employee> employees = await _dbContext.GetAllAsync(
                include: employee =>
                employee
                .Include(e => e.Account)
                .ThenInclude(a => a.PersonalData)
                .Include(e => e.Roles)
                );
            return View(employees);
        }


        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Upsert(int? id)
        {
            UpSertEmployeeViewModel employeeVM = new UpSertEmployeeViewModel();
            IEnumerable<SelectListItem> roleList = _dbContext.GetAllRolesList();
            MultiSelectList roleMultpleSelectList = new MultiSelectList(roleList.OrderBy(r => r.Text), "Value", "Text");
            employeeVM = new UpSertEmployeeViewModel
            {
                RoleMultpleSelectList = roleMultpleSelectList
            };
            if (id  == null)
            {
                employeeVM.Id = 0;
                return View(employeeVM);
            }
            else
            {
                Employee employee = await _dbContext.FirstOrDefaultAsync(filter: e => e.Id == id);
                if(employee == null)
                {
                    return NotFound();
                }
                employeeVM.SetEmployee(employee, roleMultpleSelectList);
                return View(employeeVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Upsert(UpSertEmployeeViewModel inputAccountEmployee)
        {
            bool isValid = true;
            ModelState.Remove("RoleMultpleSelectList");
            if (ModelState.IsValid)
            {
                IEnumerable<Role> SelectedRoles = _dbContext.GetSelectedRoles(inputAccountEmployee.RoleIds.ToArray());
                    if (_dbContext.Any(e => e.Account.EMail == inputAccountEmployee.EMail && e.Id != inputAccountEmployee.Id) && inputAccountEmployee.EMail != null)
                    {
                        ViewData["EmailDuplicate"] = true;
                        isValid = false;
                    }
                    if (_dbContext.Any(e => e.Account.PhoneNumber == inputAccountEmployee.PhoneNumber && e.Id != inputAccountEmployee.Id) && inputAccountEmployee.PhoneNumber != null)
                    {
                        ViewData["PhoneDuplicate"] = true;
                        isValid = false;
                    }
                    if (_dbContext.Any(e => e.Account.PersonalData.PassId == inputAccountEmployee.PassId && e.Id != inputAccountEmployee.Id) && inputAccountEmployee.PassId != null)
                    {
                        ViewData["PassDuplicate"] = true;
                        isValid = false;
                    }
                    if (_dbContext.Any(e => e.Account.PersonalData.SNILS == inputAccountEmployee.SNILS && e.Id != inputAccountEmployee.Id) && inputAccountEmployee.SNILS != null)
                    {
                        ViewData["SNILSDuplicate"] = true;
                        isValid = false;
                    }

                if (isValid)
                {
                    if (inputAccountEmployee.Id == 0)
                    {
                        Employee newEmployee = inputAccountEmployee.CreateEmployee(SelectedRoles);
                        await _dbContext.AddAsync(newEmployee);
                    }
                    else
                    {
                        Employee employee = await _dbContext.FirstOrDefaultAsync(filter: e => e.Id == inputAccountEmployee.Id);
                        if (employee == null)
                        {
                            return NotFound();
                        }
                        inputAccountEmployee.GetCopy(employee, SelectedRoles);
                        employee.Account.isChanged = true;
                        await _dbContext.UpdateAsync(employee);
                    }
                    await _dbContext.SaveAsync();

                    return RedirectToAction("Index");
                }
            }
            IEnumerable<SelectListItem> roleList = _dbContext.GetAllRolesList();
            MultiSelectList roleMultpleSelectList = new MultiSelectList(roleList.OrderBy(r => r.Text), "Value", "Text");
            inputAccountEmployee.RoleMultpleSelectList = roleMultpleSelectList;
            return View(inputAccountEmployee);
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            else
            {
                Employee employee = await _dbContext.FirstOrDefaultAsync(isDetail: true, e => e.Id == id);
                if(employee == null)
                {
                    return NotFound();
                }
                ///////////////
                var user = User;
                var roles = user.FindAll(ClaimTypes.Role);
                if (user.FindFirstValue(WC.TypeUser) == WC.StudentUser && employee.Roles.Count() == 1 && employee.Roles.First().Name == WC.AdminRole)
                {
                    return NotFound();
                }
                //////////////
                DetailsEmloyeeViewModel detailsEmloyee = new DetailsEmloyeeViewModel();
                detailsEmloyee.SetEmployee(employee);
                return View(detailsEmloyee);
            }
        }

        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Delete(int? id, string? redirectUrl)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                Employee employee = await _dbContext.FirstOrDefaultAsync(filter: e => e.Id == id);
                if (employee == null)
                {
                    return NotFound();
                }
                _dbContext.Remove(employee);
                await _dbContext.SaveAsync();
                if (redirectUrl == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Redirect(redirectUrl);
                }
            }
        }

    }
}
