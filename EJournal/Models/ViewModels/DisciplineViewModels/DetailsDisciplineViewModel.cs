using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace EJournal.Models.ViewModels.DisciplineViewModels
{
    public class DetailsDisciplineViewModel
    {
        public int Id { get; set; } = 0;
        public int classId { get; set; }
        public int EmployeeId { get; set; }
        public int? EmployeeClassManagerId { get; set; }
        [DisplayName("Учитель")]
        public string EmployeeName { get; set; }
        [DisplayName("Класс")]
        public string ClassName { get; set; }
        [DisplayName("Предмет")]
        public string SubjectName { get; set; }

        public IEnumerable<SelectListItem> lessons { get; set; }
        public Dictionary<SelectListItem, List<int>> studentMarks { get; set; }
        public void SetDiscipline(Discipline discipline)
        {
            EmployeeClassManagerId = discipline.Class.EmployeeKey;
            classId = discipline.ClassKey;
            EmployeeId = discipline.EmployeeKey;
            Id = discipline.Id;
            EmployeeName = discipline.Employee.Account.PersonalData.FullName;
            ClassName = discipline.Class.Number.ToString() + discipline.Class.Liter;
            SubjectName = discipline.Subject.Name;
            List<Lesson> lessonsTemp = discipline.Lessons.OrderByDescending(l => l.DateTime).ThenByDescending(l => l.Index).ToList();
            lessons = lessonsTemp.Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.DateTime.ToString("dd, MMM") });
            studentMarks = new Dictionary<SelectListItem, List<int>>(discipline.Class.Students.Count());
            discipline.Class.Students.ForEach(s =>
            {
                List<int> marks = new List<int>(lessonsTemp.Count());
                lessonsTemp.ForEach(l => marks.Add(l.Marks.FirstOrDefault(m => m.StudentKey == s.Id)?.Value ?? 0));
                studentMarks.TryAdd(new SelectListItem { Text = s.Account.PersonalData.FullName, Value = s.Id.ToString() }, marks);
            });

            
        }
    }
    
}
