using EJournal.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Linq;

namespace EJournal.Models.ViewModels.TimetableViewModels
{
    public class TimetableViewModel
    {
        public int Id { get; set; }
        //public Dictionary<Class, IEnumerable<Lesson>> timetable { get; set; } = new();
        public Dictionary<Class, IEnumerable<SelectListItem>> listSubjects { get; set; } = new();
        public Dictionary<int, List<int>> classLessons { get; set; } = new();
        //public List<Lesson> lessons { get; set; } = new();
        [DisplayName("День")]
        public DateTime date { get; set; }

        public void SetTimetable(IEnumerable<Class> classes, DateTime date)
        {
            this.date = date;
            classLessons = new Dictionary<int, List<int>>(classes.Count());
            listSubjects = new Dictionary<Class, IEnumerable<SelectListItem>>(classes.Count());
            //timetable = new Dictionary<Class, IEnumerable<Lesson>>(classes.Count());
            listSubjects = new Dictionary<Class, IEnumerable<SelectListItem>>(classes.Count());
            Id = ITimetableRepository.GetDayId(date);
            foreach (var classItem in classes)
            {
                List<int> dicsiplineKeysClass = new List<int>(8);
                for (int i = 0; i < 8; i++)
                {
                    dicsiplineKeysClass.Add(-1);
                }
                classItem.Disciplines.ForEach(d =>
                {
                    d.Lessons.Where(l => l.DateTime == date).ToList().ForEach(l =>
                    {
                        dicsiplineKeysClass[l.Index] = l.DisciplineKey;
                    });
                });
                //timetable.Add(classItem, ClassLesson);
                classLessons.Add(classItem.Id, dicsiplineKeysClass);
                listSubjects.Add(classItem, classItem.Disciplines.Select(d => new SelectListItem { Text = d.Subject.Name + " | " + d.Employee.Account.PersonalData.FullName, Value = d.Id.ToString() }));
            }
        }
        public void GetCopy(IEnumerable<Class> classes)
        {
            foreach (var classItem in classes)
            {
                for (int i = 0; i < 8; i++)
                {
                    Discipline discipline = classItem.Disciplines.FirstOrDefault(d => d.Lessons.Any(l => l.DateTime == date && l.Index == i));
                    if (discipline != null)
                    {
                        discipline.Lessons.Remove(discipline.Lessons.FirstOrDefault(l => l.DateTime == date && l.Index == i));
                    }
                    Lesson newLesson = new Lesson
                    {
                        DisciplineKey = classLessons[classItem.Id][i],
                        Index = i,
                        DateTime = date
                    };
                    classItem.Disciplines.FirstOrDefault(d => d.Id == classLessons[classItem.Id][i])?.Lessons.Add(newLesson);
                }
            }
        }
    }
}
