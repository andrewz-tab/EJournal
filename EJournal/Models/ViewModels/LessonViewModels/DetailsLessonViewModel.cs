using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace EJournal.Models.ViewModels.LessonViewModel
{
    public class DetailsLessonViewModel
    {
        public int Id { get; set; } = 0;
        public int DisciplineId { get; set; }
        public int ClassId { get; set; }
        public int? EmployeeClassManagerId { get; set; }
        public int EmployeeId { get; set; }
        [DisplayName("Учитель")]
        public string EmployeeName { get; set; }
        [DisplayName("Класс")]
        public string ClassName { get; set; }
        [DisplayName("Предмет")]
        public string SubjectName { get; set; }
        [DisplayName("Дата")]
        public DateTime date { get; set; }
        [DisplayName("Номер урока")]
        public int index { get; set; }
        [DisplayName("Домашнее задание")]
        public string? HomeWork { get; set; }
        public Dictionary<int, string> studentIDName { get; set; }
        public Dictionary<int, string> studentIDDecription { get; set; }
        public Dictionary<int, int> studentIDMark { get; set; }
        public readonly List<SelectListItem> marks;
        public DetailsLessonViewModel()
        {
            marks = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = " " },
                new SelectListItem { Value = "-1", Text = "Н" },
                new SelectListItem { Value = "1", Text = "1" },
                new SelectListItem { Value = "2", Text = "2" },
                new SelectListItem { Value = "3", Text = "3" },
                new SelectListItem { Value = "4", Text = "4" },
                new SelectListItem { Value = "5", Text = "5" }
            };
        }
        public void SetLesson(Lesson lesson)
        {
            EmployeeClassManagerId = lesson.Discipline.Class?.EmployeeKey;
            EmployeeId = lesson.Discipline.EmployeeKey;
            ClassId = lesson.Discipline.ClassKey;
            HomeWork = lesson.HomeWork;
            DisciplineId = lesson.DisciplineKey;
            Id = lesson.Id;
            date = lesson.DateTime;
            index = lesson.Index;
            EmployeeName = lesson.Discipline.Employee.Account.PersonalData.FullName;
            ClassName = lesson.Discipline.Class.Number.ToString() + lesson.Discipline.Class.Liter;
            SubjectName = lesson.Discipline.Subject.Name;
            studentIDName = new Dictionary<int, string>(lesson.Discipline.Class.Students.Count());
            studentIDDecription = new Dictionary<int, string>(lesson.Discipline.Class.Students.Count());
            studentIDMark = new Dictionary<int, int>(lesson.Discipline.Class.Students.Count());
            lesson.Discipline.Class.Students.ForEach(s =>
            {
                Mark tempMark = lesson.Marks.FirstOrDefault(m => s.Id == m.StudentKey);
                studentIDName.TryAdd(s.Id, s.Account.PersonalData.FullName );
                studentIDDecription.TryAdd(s.Id, tempMark?.Decription ?? "" );
                studentIDMark.TryAdd(s.Id, tempMark?.Value ?? 0);
            });
        }
        public void GetCopy(Lesson lesson)
        {
            lesson.HomeWork = HomeWork;
            lesson.Discipline.Class.Students.ForEach(s =>
            {
                Mark tempMark;
                bool isValidMark = false;
                tempMark = s.Marks.FirstOrDefault(m => m.LessonKey == lesson.Id && m.StudentKey == s.Id);
                if (tempMark == null)
                    tempMark = new Mark
                    {
                        LessonKey = lesson.Id,
                        StudentKey = s.Id
                    };
                if (tempMark != null)
                {
                    int mark = 0;
                    if (studentIDMark.TryGetValue(s.Id, out mark))
                    {
                        if (mark >= -1 && mark <= 5)
                            tempMark.Value = mark;
                    }
                    string description = "";
                    if (studentIDDecription.TryGetValue(s.Id, out description))
                    {
                        tempMark.Decription = description;
                    }
                }
                s.Marks.Add(tempMark);
                lesson.Marks.Add(tempMark);
            });
        }
    }
    
}
