using EJournal.Data;
using EJournal.Models;
using EJournal.Models.ViewModels.EmployeeViewModels;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EJournal.Controllers
{
    public class SubjectsController : Controller
    {
        private ISubjectRepository _dbContext;
        public SubjectsController(ISubjectRepository dbContext)
        {
            this._dbContext = dbContext;
        }

        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Subject> subjects = await _dbContext.GetAllAsync();
            return View(subjects);
        }

        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Upsert(int? id)
        {
            Subject subject = new Subject();
            if (id == null)
            {
                subject.Id = 0;
                return View(subject);
            }
            else
            {
                subject = await _dbContext.FirstOrDefaultAsync(filter: s => s.Id == id);
                if (subject == null)
                {
                    return NotFound();
                }

                return View(subject);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Upsert(Subject inputSubject)
        {
            bool isValid = true;
            if (ModelState.IsValid)
            {
                if(!(isValid = !_dbContext.Any(s => s.Id != inputSubject.Id && s.Name == inputSubject.Name))) 
                {
                    ViewData["SubjectDuplicate"] = true;
                }
                if (isValid)
                {
                    if (inputSubject.Id == 0)
                    {
                        await _dbContext.AddAsync(inputSubject);
                    }
                    else
                    {
                        Subject subject = await _dbContext.FirstOrDefaultAsync(filter: e => e.Id == inputSubject.Id);
                        if (subject == null)
                        {
                            return NotFound();
                        }
                        subject.Name = inputSubject.Name;
                        await _dbContext.UpdateAsync(subject);
                    }
                    await _dbContext.SaveAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(inputSubject);
        }

        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Details(int? id)
        {
            Subject subject = new Subject();
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                subject = await _dbContext.FirstOrDefaultAsync(
                    filter: s => s.Id == id,
                    isDetail: true
                    );
                if (subject == null)
                {
                    return NotFound();
                }
                return View(subject);
            }
        }

        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Delete(int? id, string? returnUrl)
        {
            Subject subject = await _dbContext.FirstOrDefaultAsync(filter: s => s.Id == id);
            if (subject == null)
            {
                return NotFound();
            }
            _dbContext.Remove(subject);
            await _dbContext.SaveAsync();
            if (returnUrl == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Redirect(returnUrl);
            }
        }
    }
}
