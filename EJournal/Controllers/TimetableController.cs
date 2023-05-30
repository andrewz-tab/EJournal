using EJournal.Models;
using EJournal.Models.ViewModels.TimetableViewModels;
using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EJournal.Controllers
{
    public class TimetableController : Controller
    {
        private readonly ITimetableRepository _dbContext;
        public TimetableController(ITimetableRepository dbContext) 
        {
            this._dbContext = dbContext;
        }
        public async Task<IActionResult> Index(int? dayId)
        {
            DateTime currentDate = ITimetableRepository.GetDateTimeByDayId(dayId);
            IEnumerable<Class> classesTimetable = await _dbContext.GetClassesTimetableByDay(currentDate);
            TimetableViewModel upsertTimetableVM = new TimetableViewModel();
            upsertTimetableVM.SetTimetable(classesTimetable, currentDate);
            return View(upsertTimetableVM);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? dayId)
        {
            DateTime currentDate = ITimetableRepository.GetDateTimeByDayId(dayId);
            IEnumerable<Class> classesTimetable = await _dbContext.GetClassesTimetableByDay(currentDate);
            TimetableViewModel upsertTimetableVM = new TimetableViewModel();
            upsertTimetableVM.SetTimetable(classesTimetable, currentDate);
            return View(upsertTimetableVM);
        }
        [HttpGet]
        public async Task<IActionResult> ByDayId(DateTime? date, int? dayId)
        {
            if (dayId != null)
            {
                return
                await Task<IActionResult>.Run(() =>
                {
                    return RedirectToAction("Index", new { dayId });
                });
            }
            else if (date != null)
            {
                return
                await Task<IActionResult>.Run(() =>
                {
                    dayId = ITimetableRepository.GetDayId(date);
                    return RedirectToAction("Index", new { dayId });
                }
                );
            }
            else
            {
                return
                await Task<IActionResult>.Run(() =>
                {
                    return RedirectToAction("Index", new { dayId });
                }
                );
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditByDayId(DateTime? date, int? dayId)
        {
            if (dayId != null)
            {
                return
                await Task<IActionResult>.Run(() =>
                {
                    return RedirectToAction("Edit", new { dayId });
                });
            }
            else if(date != null)
            {
                return
                await Task<IActionResult>.Run(() =>
                {
                    dayId = ITimetableRepository.GetDayId(date);
                    return RedirectToAction("Edit", new { dayId });
                }
                );
            }
            else
            {
                return
                await Task<IActionResult>.Run(() =>
                {
                    return RedirectToAction("Edit", new { dayId });
                }
                );
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TimetableViewModel timetableViewModel)
        {
            bool isValid = true;
            ModelState.Remove("listSubjects");
            if(ModelState.IsValid)
            {
                IEnumerable<Class> classesTimetable = await _dbContext.GetClassesTimetableByDay(timetableViewModel.date);
                timetableViewModel.GetCopy(classesTimetable);
                HashSet<object> EmployeeKeyLessonIndex = new HashSet<object>(classesTimetable.Count() * 8);
                HashSet<string> ErrorMessages = new HashSet<string>();
                //int[,] EmployeeKeyLessonIndex = new int[classesTimetable.Count()*8, 2];
                ViewData["DuplicateLessonForEmployee"] = "";

                foreach (var classItem in classesTimetable)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Discipline discipline = classItem.Disciplines.FirstOrDefault(d => d.Lessons.Any(l => l.DateTime == timetableViewModel.date && l.Index == i));
                        if (discipline != null)
                        {
                            if (!EmployeeKeyLessonIndex.Add(new { discipline.EmployeeKey, i }))
                            {
                                ErrorMessages.Add("Невозможно установить 2 и более одновременно занятия для учителя " + discipline.Employee.Account.PersonalData.FullName);
                                isValid = false;
                            }
                        }
                    }
                }

                ViewData["errors"] = "";
                foreach (var error in ErrorMessages)
                {
                    ViewData["errors"] += error + "<br>";
                }
                if(!isValid)
                {
                    timetableViewModel.SetTimetable(classesTimetable, timetableViewModel.date);
                    return View(timetableViewModel);
                }
                await _dbContext.SaveAsync();
                int dayId = timetableViewModel.Id;
                return RedirectToAction("Index", new { dayId });
            }
            return View(timetableViewModel);
        }
    }
}
