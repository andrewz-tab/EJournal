using EJournal.Data;
using EJournal.Models;
using EJournal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EJournal.Controllers
{
    public class ClassesController : Controller
    {
        private readonly JournalDbContext _dbContext;
        public ClassesController(JournalDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Class> classes = this._dbContext.Classes.Include(c => c.Employee).ThenInclude(e => e.Account).ThenInclude(a => a.PersonalData);
            return View(classes);
        }

        public async Task<IActionResult> Create()
        {
            UpSertClassViewModel AddClass = new UpSertClassViewModel
            {
                EmployeeDropDown = _dbContext.Employees
            .Where(e => e.Roles.Any(r => r.Name == "Учитель"))
            .Select(e => new SelectListItem
            {
                Text = e.Account.PersonalData.FullName,
                Value = e.Id.ToString()
            })
        }; 
            return View(AddClass);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UpSertClassViewModel inputClass)
        {
            Employee employee = _dbContext.Employees.Find(inputClass.EmployeeKey);
            Class newClacc = new Class
            {
                Number = inputClass.Number,
                Liter = inputClass.Liter,
                Employee = employee
            };
            ModelState.Remove("EmployeeDropDown");
            if (ModelState.IsValid)
            {
                _dbContext.Classes.Add(newClacc);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(inputClass);
        }
    }
}
