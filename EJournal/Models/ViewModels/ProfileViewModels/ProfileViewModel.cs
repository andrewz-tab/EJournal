using static EJournal.Models.PersonalData;
using System.ComponentModel;

namespace EJournal.Models.ViewModels.ProfileViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; } = 0;
        public int accountId { get; set; }
        [DisplayName("ФИО")]
        public string FullName { get; set; }
        [DisplayName("Дата рождения")]
        public DateTime DateBirth { get; set; } = DateTime.Now;
        [DisplayName("Пол")]
        public Gender gender { get; set; } = Gender.Men;
        [DisplayName("Серия и номер паспорта")]
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

        
        //Employee

        [DisplayName("Полномочия")]
        public List<Role> roles { get; set; }
        public IEnumerable<Class> classes { get; set; }

        public bool isStudent { get; set; } = true;

        public void SetEmployee(Employee employee)
        {
            isStudent = false;
            accountId = employee.AccountKey;
            Id = employee.Id;
            Description = employee.Description;
            PhoneNumber = employee.Account.PhoneNumber;
            EMail = employee.Account.EMail;
            SNILS = employee.Account.PersonalData.SNILS;
            FullName = employee.Account.PersonalData.FullName;
            PassId = employee.Account.PersonalData.PassId;
            DateBirth = employee.Account.PersonalData.DateBirth;
            gender = employee.Account.PersonalData.gender;
            isActivate = employee.Account.isActivate;
            roles = employee.Roles;
            classes = employee.Classes;
            disciplines = employee.Disciplines;
            ClassName = "";
        }
        public void SetStudent(Student student)
        {
            isStudent = true;
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
            roles = new List<Role>();
            classes = new List<Class>();
        }
    }
}
