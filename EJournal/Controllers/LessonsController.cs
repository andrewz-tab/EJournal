using EJournal.Models;
using EJournal.Models.ViewModels.DisciplineViewModels;
using EJournal.Models.ViewModels.LessonViewModel;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EJournal.Controllers
{
    public class LessonsController : Controller
    {
        private readonly ILessonRepository _dbContext;
        public LessonsController(ILessonRepository dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
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
        }
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
                DetailsLessonViewModel detailsLesson = new DetailsLessonViewModel();
                detailsLesson.SetLesson(lesson);
                return View(detailsLesson);
            }
        }
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
                DetailsLessonViewModel detailsLesson = new DetailsLessonViewModel();
                detailsLesson.SetLesson(lesson);
                return View(detailsLesson);
            }
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
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
            if(ModelState.IsValid)
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
            detailsLesson.SetLesson(lessontemp);
            return View(detailsLesson);

        }

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

    }
}
