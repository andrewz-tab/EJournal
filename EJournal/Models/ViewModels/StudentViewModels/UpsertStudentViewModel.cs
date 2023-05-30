using Microsoft.AspNetCore.Mvc.Rendering;
using static EJournal.Models.PersonalData;
using System.ComponentModel;

namespace EJournal.Models.ViewModels.StudentViewModels
{
    public class UpsertStudentViewModel
    {
        public int Id { get; set; } = 0;
        [DisplayName("ФИО")]
        public string FullName { get; set; }
        [DisplayName("Дата рождения")]
        public DateTime DateBirth { get; set; } = DateTime.Now;
        [DisplayName("Пол")]
        public Gender gender { get; set; } = Gender.Men;
        public string? PassId { get; set; }
        [DisplayName("СНИЛС")]
        public string SNILS { get; set; }
        [DisplayName("Описание")]
        public string? Description { get; set; }
        [DisplayName("Электронная почта")]
        public string EMail { get; set; }
        [DisplayName("Номер телефона")]
        public string? PhoneNumber { get; set; }
        public bool isActivate { get; set; } = false;
        [DisplayName("Класс")]
        public int ClassKey { get; set; }
        public IEnumerable<SelectListItem> ClassList { get; set; }
        public UpsertStudentViewModel() { }
        public Student CreateStudent()
        {
            Student newStudent = new Student();
            newStudent.Description = Description;
            newStudent.Account = new Account();
            newStudent.Account.isActivate = false;
            newStudent.Account.PhoneNumber = PhoneNumber;
            newStudent.Account.EMail = EMail;
            newStudent.Account.Student = newStudent;
            newStudent.Account.TypeUser = new TypeUser();
            newStudent.Account.PersonalData = new PersonalData();
            newStudent.Account.PersonalData.FullName = FullName;
            newStudent.Account.PersonalData.PassId = PassId;
            newStudent.Account.PersonalData.SNILS = SNILS;
            newStudent.Account.PersonalData.DateBirth = DateBirth;
            newStudent.Account.PersonalData.gender = gender;
            newStudent.Account.PersonalData.Account = newStudent.Account;
            newStudent.Account.Student = newStudent;
            newStudent.ClassKey = ClassKey;
            return newStudent;
        }
        public void SetStudent(Student student, IEnumerable<SelectListItem> classList)
        {
            Id = student.Id;
            Description = student.Description;
            PhoneNumber = student.Account.PhoneNumber;
            EMail = student.Account.EMail;
            SNILS = student.Account.PersonalData.SNILS;
            FullName = student.Account.PersonalData.FullName;
            PassId = student.Account.PersonalData.PassId;
            DateBirth = student.Account.PersonalData.DateBirth;
            gender = student.Account.PersonalData.gender;
            isActivate = student.Account.isActivate;
            ClassList = classList;
            ClassKey = student.ClassKey;
        }
        public void GetCopy(Student student)
        {
            student.Description = Description;
            student.Account.isActivate = isActivate;
            student.Account.PhoneNumber = PhoneNumber;
            student.Account.EMail = EMail;
            student.Account.PersonalData.FullName = FullName;
            student.Account.PersonalData.PassId = PassId;
            student.Account.PersonalData.SNILS = SNILS;
            student.Account.PersonalData.DateBirth = DateBirth;
            student.Account.PersonalData.gender = gender;
            student.ClassKey = ClassKey;
        }
    }
}
