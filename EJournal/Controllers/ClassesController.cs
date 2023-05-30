using EJournal.Data;
using EJournal.Models;
using EJournal.Models.ViewModels.ClassViewModels;
using EJournal.Models.ViewModels.EmployeeViewModels;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EJournal.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IClassRepository _dbContext;
        public ClassesController(IClassRepository dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Class> classes = await this._dbContext.GetAllAsync(
                include: c =>
                c.Include(ci => ci.Employee)
                .ThenInclude(e => e.Account)
                .ThenInclude(a => a.PersonalData)
                );
            return View(classes);
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            UpSertClassViewModel CurrentClass = new UpSertClassViewModel
            {
                EmployeeDropDown = _dbContext.GetAllTeachersList()
            };
        
            if (id == null)
            {
                return View(CurrentClass);
            }
            else
            {
                Class classData = await _dbContext.FirstOrDefaultAsync(filter: c => c.Id == id);
                if (classData == null)
                {
                    return NotFound();
                }
                CurrentClass.SetClass(classData);
                return View(CurrentClass);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UpSertClassViewModel inputClass)
        {
            bool isValid = true;
            ModelState.Remove("EmployeeDropDown");
            if (ModelState.IsValid)
            {
                if(!(isValid = !_dbContext.Any(c => c.Liter ==  inputClass.Liter && c.Number == inputClass.Number && c.Id != inputClass.Id)))
                {
                    ViewData["ClassDuplicate"] = true;
                }
                if (isValid)
                {
                    if (inputClass.Id == 0)
                    {
                        Class newClass = inputClass.CreateClass();
                        await _dbContext.AddAsync(newClass);
                    }
                    else
                    {
                        Class updateClass = await _dbContext.FirstOrDefaultAsync(filter: c => c.Id == inputClass.Id);
                        if (updateClass == null)
                        {
                            return NotFound();
                        }
                        inputClass.GetCopy(updateClass);
                        await _dbContext.UpdateAsync(updateClass);
                    }
                    await _dbContext.SaveAsync();
                    return RedirectToAction("Index");
                }
            }
            inputClass.EmployeeDropDown = _dbContext.GetAllTeachersList();
            return View(inputClass);

        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                Class classDetails = await _dbContext.FirstOrDefaultAsync(filter: c => c.Id == id, isDetail: true);
                if (classDetails == null)
                {
                    return NotFound();
                }         
                DetailsClassViewModel detailsClass = new DetailsClassViewModel();
                detailsClass.SetClass(classDetails);
                return View(detailsClass);
            }
        }

        public async Task<IActionResult> Delete(int? id, string? redirectUrl)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                Class Class = await _dbContext.FirstOrDefaultAsync(filter: e => e.Id == id);
                if (Class == null)
                {
                    return NotFound();
                }
                _dbContext.Remove(Class);
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
