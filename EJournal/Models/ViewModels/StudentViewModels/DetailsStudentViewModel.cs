using EJournal.Models;
using System.ComponentModel;
using static EJournal.Models.PersonalData;

namespace EJournal.Models.ViewModels.StudentViewModels
{
    public class DetailsStudentViewModel
    {

        public int Id { get; set; } = 0;
        public int? EmployeeClassManagerId { get; set; }
        public int accountId { get; set; }
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
        public string ClassName { get; set; }
        public int ClassKey { get; set; }
        public IEnumerable<Discipline> disciplines { get; set; }

        public void SetStudent(Student student)
        {
            EmployeeClassManagerId = student.Class.EmployeeKey;
            accountId = student.AccountKey;
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
            disciplines = student.Class.Disciplines;
            ClassKey = student.ClassKey;
            ClassName = student.Class.Number.ToString() + student.Class.Liter;
        }
    }
}