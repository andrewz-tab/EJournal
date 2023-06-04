using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EJournal.Models.ViewModels.TimetableViewModels
{
    public class TimetableForTeacherViewModel
    {
        public int teacherId { get; set; }
        public string teacherName { get; set; }
        [BindProperty, DataType("week")]
        public string weekId { get; set; }
        public DateTime dateBegin { get; set; }
        public Dictionary<DateTime, List<Lesson>> timetable { get; set; }
        public void SetTimeTable(Employee teacher , IEnumerable<Lesson> lessons, DateTime dateBegin)
        {
            teacherId = teacher.Id;
            weekId = $"{dateBegin.Year}-W{ISOWeek.GetWeekOfYear(dateBegin)}";
            teacherName = teacher.Account.PersonalData.FullName;
            this.dateBegin = dateBegin;
            timetable = new Dictionary<DateTime, List<Lesson>>(6);
            for (int i = 0; i < 6; i++)
            {
                IEnumerable<Lesson> temp = lessons.Where(l => l.DateTime == dateBegin.AddDays(i));

                timetable.Add(dateBegin.AddDays(i), new List<Lesson>());
                timetable[dateBegin.AddDays(i)] = temp.ToList();
            }

        }
    }
}
