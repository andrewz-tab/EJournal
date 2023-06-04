using EJournal.Models;
using EJournal.Models.ViewModels.DisciplineViewModels;
using EJournal.Models.ViewModels.LessonViewModel;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using System.Security.Claims;

namespace EJournal.Controllers
{
    public class LessonsController : Controller
    {
        private readonly ILessonRepository _dbContext;
        public LessonsController(ILessonRepository dbContext)
        {
            this._dbContext = dbContext;
        }
        /*public async Task<IActionResult> Index()
        {
            IEnumerable<Lesson> lessons = await _dbContext.GetAllAsync(
                include: lesson =>
                lesson 
                .Include(l => l.Discipline)
                .ThenInclude(d => d.Class)
                .Include(l => l.Discipline)
                .ThenInclude(d => d.Subject)
                .Include(l => l.Discipline)
                .ThenInclude(d => d.Employee)
                    .ThenInclude(e => e.Account)
                    .ThenInclude(a => a.PersonalData)
                ); 
            return View(lessons);
        }*/
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                Lesson lesson = await _dbContext.FirstOrDefaultAsync(isDetail: true, l => l.Id == id);
                if (lesson == null)
                {
                    return NotFound();
                }
                var user = User;
                var roles = user.FindAll(ClaimTypes.Role);
                if (roles.Count() == 1 && roles.FirstOrDefault()?.Value == WC.TeacherRole)
                {
                    if (!(user.FindFirstValue(WC.EmployeeId) == lesson.Discipline.EmployeeKey.ToString() || user.FindFirstValue(WC.EmployeeId) == lesson.Discipline.Class.EmployeeKey.ToString()))
                    {
                        return NotFound();
                    }
                }
                if (user.FindFirstValue(WC.TypeUser) == WC.StudentUser && !(user.FindFirstValue(WC.ClassId) == lesson.Discipline.ClassKey.ToString()))
                {
                    return NotFound();
                }
                DetailsLessonViewModel detailsLesson = new DetailsLessonViewModel();
                detailsLesson.SetLesson(lesson);
                return View(detailsLesson);
            }
        }
        [Authorize]
        [Authorize(Policy = WC.PolicyOnlyForEmployee)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                Lesson lesson = await _dbContext.FirstOrDefaultAsync(isDetail: true, l => l.Id == id);
                if (lesson == null)
                {
                    return NotFound();
                }

                ////////////////
                var user = User;
                var roles = user.FindAll(ClaimTypes.Role);
                if (roles != null)
                {
                    if (!user.HasClaim(claim => (claim.Type == WC.EmployeeId && claim.Value == lesson.Discipline.EmployeeKey.ToString()) || (claim.Value == WC.AdminRole)))
                    {
                        return NotFound();
                    }
                    if (roles.Count() == 1 && roles.FirstOrDefault()?.Value == WC.TeacherRole)
                    {
                        if (!(user.FindFirstValue(WC.EmployeeId) == lesson.Discipline.EmployeeKey.ToString()))
                        {
                            return NotFound();
                        }
                    }
                }
                ///////////////
                DetailsLessonViewModel detailsLesson = new DetailsLessonViewModel();
                detailsLesson.SetLesson(lesson);
                return View(detailsLesson);
            }
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Policy = WC.PolicyOnlyForEmployee)]
        public async Task<IActionResult> Edit(DetailsLessonViewModel detailsLesson)
        {
            int id = detailsLesson.Id;
            bool isValid = true;
            ModelState.Remove("marks");
            ModelState.Remove("index");
            ModelState.Remove("date");
            ModelState.Remove("SubjectName");
            ModelState.Remove("ClassName");
            ModelState.Remove("EmployeeName");
            ModelState.Remove("studentIDName");
            ModelState.Remove("EmployeeClassManagerId");
            if (ModelState.IsValid)
            {
                if (!(isValid = detailsLesson.studentIDMark.ToList().TrueForAll(mark => mark.Value >= -1 && mark.Value <= 5)));
                {
                    ViewData["MarkNotValid"] = true;
                }
                if(isValid)
                {
                    Lesson lesson = await _dbContext.FirstOrDefaultAsync(isDetail: true, l => l.Id == detailsLesson.Id);
                    if (lesson == null)
                    {
                        return NotFound();
                    }
                    /////////////////
                    var userup = User;
                    var rolesup = userup.FindAll(ClaimTypes.Role);
                    if (rolesup != null)
                    {
                        if (!userup.HasClaim(claim => (claim.Type == WC.EmployeeId && claim.Value == lesson.Discipline.EmployeeKey.ToString()) || (claim.Value == WC.AdminRole)))
                        {
                            return NotFound();
                        }
                        if (rolesup.Count() == 1 && rolesup.FirstOrDefault()?.Value == WC.TeacherRole)
                        {
                            if (!(userup.FindFirstValue(WC.EmployeeId) == lesson.Discipline.EmployeeKey.ToString()))
                            {
                                return NotFound();
                            }
                        }
                    }
                    ////////////////
                    detailsLesson.GetCopy(lesson);
                    await _dbContext.UpdateAsync(lesson);
                    await _dbContext.SaveAsync();
                    return RedirectToAction("Details", new { id });
                }
            }
            Lesson lessontemp = await _dbContext.FirstOrDefaultAsync(isDetail: true, l => l.Id == detailsLesson.Id);
            if (lessontemp == null)
            {
                return NotFound();
            }
            /////////////////
            var user = User;
            var roles = user.FindAll(ClaimTypes.Role);
            if (roles != null)
            {
                if (!user.HasClaim(claim => (claim.Type == WC.EmployeeId && claim.Value == lessontemp.Discipline.EmployeeKey.ToString()) || (claim.Value == WC.AdminRole)))
                {
                    return NotFound();
                }
                if (roles.Count() == 1 && roles.FirstOrDefault()?.Value == WC.TeacherRole)
                {
                    if (!(user.FindFirstValue(WC.EmployeeId) == lessontemp.Discipline.EmployeeKey.ToString()))
                    {
                        return NotFound();
                    }
                }
            }
            ////////////////
            detailsLesson.SetLesson(lessontemp);
            return View(detailsLesson);

        }
        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id <= 0 || id == null)
            {
                return NotFound();
            }
            else
            {
                Lesson lesson = await _dbContext.FirstOrDefaultAsync(filter: l => l.Id == id);
                if (lesson == null)
                {
                    return NotFound();
                }
                _dbContext.Remove(lesson);
                await _dbContext.SaveAsync();
            }
            return RedirectToAction("Index");
        }
*/
    }
}
