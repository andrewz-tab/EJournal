using EJournal.Data;
using EJournal.Models;
using EJournal.Models.ViewModels.DisciplineViewModels;
using EJournal.Models.ViewModels.EmployeeViewModels;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using System.Security.Claims;

namespace EJournal.Controllers
{
    public class DisciplinesController : Controller
    {
        private readonly IDisciplineRepository _dbContext;
        public DisciplinesController(IDisciplineRepository dbContext)
        {
            this._dbContext = dbContext;
        }
        [HttpGet]
        [Authorize(Policy = WC.PolicyOnlyForEmployee)]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Discipline> disciplines = await _dbContext.GetAllAsync(
                include: discipline =>
                discipline
                .Include(d => d.Class)
                .Include(d => d.Subject)
                .Include(d => d.Employee)
                    .ThenInclude(e => e.Account)
                    .ThenInclude(a => a.PersonalData)
                );
            return View(disciplines);
        }
        [HttpGet]
        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<SelectListItem> classList = _dbContext.GetAllClassesList();
            IEnumerable<SelectListItem> teacherList = _dbContext.GetAllTeachersList();
            IEnumerable<SelectListItem> subjectList = _dbContext.GetAllSubjectsList();
            UpsertDisciplineViewModel disciplineViewModel = new UpsertDisciplineViewModel
            {
                ClassDropDown = classList,
                EmployeeDropDown = teacherList,
                SubjectDropDown = subjectList
            };
            if (id != null)
            {
                Discipline discipline = await _dbContext.FirstOrDefaultAsync(filter: d => d.Id == id);
                disciplineViewModel.SetDiscipline(discipline);
            }
            else
            {
                disciplineViewModel.ClassMultpleSelectList = new MultiSelectList(classList.OrderBy(r => r.Text), "Value", "Text"); ;
            }
            return View(disciplineViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Upsert(UpsertDisciplineViewModel inputDiscipline)
        {
            bool isValid = true;
            ModelState.Remove("ClassDropDown");
            ModelState.Remove("EmployeeDropDown");
            ModelState.Remove("SubjectDropDown");
            ModelState.Remove("ClassMultpleSelectList");
            ModelState.Remove("ClassesIds");

            if (ModelState.IsValid)
            {
                if (!(isValid = !_dbContext.Any(d => inputDiscipline.EmployeeKey == d.EmployeeKey && inputDiscipline.ClassKey == d.ClassKey && inputDiscipline.SubjectKey == d.SubjectKey && inputDiscipline.Id != d.Id)))
                {
                    ViewData["DisciplineDuplicate"] = true;
                }
                if(isValid)
                {
                    if (inputDiscipline.Id == 0)
                    {
                        IEnumerable<Discipline> disciplines = inputDiscipline.CreateDisciplines();
                        foreach(var disciplineTemp in disciplines)
                        {
                            Discipline disciplineDuplicate = await _dbContext.FirstOrDefaultAsync(filter: d => disciplineTemp.EmployeeKey == d.EmployeeKey && disciplineTemp.ClassKey == d.ClassKey && disciplineTemp.SubjectKey == d.SubjectKey && disciplineTemp.Id != d.Id);
                            if (disciplineDuplicate == null)
                            {
                                await _dbContext.AddAsync(disciplineTemp);
                            }
                        }
                    }
                    else
                    {
                        Discipline discipline = await _dbContext.FirstOrDefaultAsync(filter: d => inputDiscipline.Id == d.Id);
                        inputDiscipline.GetCopy(discipline);
                        await _dbContext.UpdateAsync(discipline);
                    }
                    await _dbContext.SaveAsync();
                    return RedirectToAction("Index");
                }
            }
            inputDiscipline.SubjectDropDown = _dbContext.GetAllSubjectsList();
            inputDiscipline.EmployeeDropDown = _dbContext.GetAllTeachersList();
            inputDiscipline.ClassDropDown = _dbContext.GetAllClassesList();
            return View(inputDiscipline);
        }

        [Authorize(Policy = WC.PolicyOnlyForEmployee)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                Discipline discipline = await _dbContext.FirstOrDefaultAsync(isDetail: true, d => d.Id == id);
                if (discipline == null)
                {
                    return NotFound();
                }
                var user = User;
                var roles = user.FindAll(ClaimTypes.Role);
                if (roles.Count() == 1 && roles.FirstOrDefault()?.Value == WC.TeacherRole)
                {
                    if(!(user.FindFirstValue(WC.EmployeeId) == discipline.EmployeeKey.ToString() || user.FindFirstValue(WC.EmployeeId) == discipline.Class.EmployeeKey.ToString()))
                    {
                        return NotFound();
                    }
                }
                DetailsDisciplineViewModel detailsDiscipline = new DetailsDisciplineViewModel();
                detailsDiscipline.SetDiscipline(discipline);
                return View(detailsDiscipline);
            }
        }

        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Delete(int? id, string? redirectUrl)
        {
            if(id <= 0 || id == null)
            {
                return NotFound();
            }
            else
            {
                Discipline discipline = await _dbContext.FirstOrDefaultAsync(filter: d => d.Id == id);
                if(discipline == null)
                {
                    return NotFound();
                }
                _dbContext.Remove(discipline);
                await _dbContext.SaveAsync();
            }
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
