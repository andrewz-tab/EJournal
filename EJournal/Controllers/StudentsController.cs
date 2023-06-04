using EJournal.Models.ViewModels.EmployeeViewModels;
using EJournal.Models;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EJournal.Models.ViewModels.StudentViewModels;
using Microsoft.AspNetCore.Authorization;

namespace EJournal.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _dbContext;
        public StudentsController(IStudentRepository dbContext)
        {
            this._dbContext = dbContext;
        }
        [Authorize(Policy =WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Student> students = await _dbContext.GetAllAsync(
                include: student =>
                student
                .Include(s => s.Account)
                .ThenInclude(a => a.PersonalData)
                .Include(s => s.Class)
                );
            return View(students);
        }


        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Upsert(int? id, int? classId)
        {
            UpsertStudentViewModel studentVM = new UpsertStudentViewModel();
            IEnumerable<SelectListItem> classList = _dbContext.GetAllClassesList();
            studentVM = new UpsertStudentViewModel
            {
                ClassList = classList
            };
            if (id == null)
            {
                studentVM.Id = 0;
                studentVM.ClassKey = classId != null ? (int)classId : -1;
                return View(studentVM);
            }
            else
            {
                Student student = await _dbContext.FirstOrDefaultAsync(isDetail:true, s => s.Id == id);
                if (student == null)
                {
                    return NotFound();
                }
                studentVM.SetStudent(student, classList);
                return View(studentVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> Upsert(UpsertStudentViewModel inputAccountStudent)
        {
            bool isValid = true;
            ModelState.Remove("ClassList");
            if (ModelState.IsValid)
            {
                if (_dbContext.Any(e => e.Account.EMail == inputAccountStudent.EMail && e.Id != inputAccountStudent.Id) && inputAccountStudent.EMail != null)
                {
                    ViewData["EmailDuplicate"] = true;
                    isValid = false;
                }
                if (_dbContext.Any(e => e.Account.PhoneNumber == inputAccountStudent.PhoneNumber && e.Id != inputAccountStudent.Id) && inputAccountStudent.PhoneNumber != null)
                {
                    ViewData["PhoneDuplicate"] = true;
                    isValid = false;
                }
                if (_dbContext.Any(e => e.Account.PersonalData.PassId == inputAccountStudent.PassId && e.Id != inputAccountStudent.Id) && inputAccountStudent.PassId != null)
                {
                    ViewData["PassDuplicate"] = true;
                    isValid = false;
                }
                if (_dbContext.Any(e => e.Account.PersonalData.SNILS == inputAccountStudent.SNILS && e.Id != inputAccountStudent.Id) && inputAccountStudent.SNILS != null)
                {
                    ViewData["SNILSDuplicate"] = true;
                    isValid = false;
                }

                if (isValid)
                {
                    if (inputAccountStudent.Id == 0)
                    {
                        Student newStudent = inputAccountStudent.CreateStudent();
                        await _dbContext.AddAsync(newStudent);
                    }
                    else
                    {
                        Student student = await _dbContext.FirstOrDefaultAsync(filter: e => e.Id == inputAccountStudent.Id);
                        if (student == null)
                        {
                            return NotFound();
                        }
                        inputAccountStudent.GetCopy(student);
                        student.Account.isChanged = true;
                        await _dbContext.UpdateAsync(student);
                    }
                    await _dbContext.SaveAsync();

                    return RedirectToAction("Index");
                }
            }
            IEnumerable<SelectListItem> classList = _dbContext.GetAllClassesList();
            inputAccountStudent.ClassList = classList;
            return View(inputAccountStudent);
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
                Student student = await _dbContext.FirstOrDefaultAsync(isDetail: true, e => e.Id == id);
                if (student == null)
                {
                    return NotFound();
                }
                DetailsStudentViewModel detailsEmloyee = new DetailsStudentViewModel();
                detailsEmloyee.SetStudent(student);
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
                Student student = await _dbContext.FirstOrDefaultAsync(filter: s => s.Id == id);
                if (student == null)
                {
                    return NotFound();
                }
                _dbContext.Remove(student);
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
