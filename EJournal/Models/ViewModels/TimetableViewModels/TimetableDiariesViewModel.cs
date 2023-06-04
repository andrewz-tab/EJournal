using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace EJournal.Models.ViewModels.TimetableViewModels
{
	public class TimetableDiariesViewModel
	{
		public int classId { get; set; }
		public int? studentId { get; set; }
		public string ClassName { get; set; }
        [BindProperty, DataType("week")]
        public string weekId { get; set; }
		public DateTime dateBegin { get; set; }
		public Dictionary<DateTime, List<Lesson>> timetable { get; set; }
		public void SetTimeTable(Class cl, IEnumerable<Lesson> lessons, DateTime dateBegin, int? studentId)
		{
			this.studentId = studentId;
            classId = cl.Id;
			weekId = $"{dateBegin.Year}-W{ISOWeek.GetWeekOfYear(dateBegin)}";
            ClassName = cl.Number.ToString() + cl.Liter;
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
