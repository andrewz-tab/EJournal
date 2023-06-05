using Microsoft.AspNetCore.Mvc.Rendering;
using static EJournal.Models.PersonalData;
using System.ComponentModel;

namespace EJournal.Models.ViewModels.EmployeeViewModels
{
    public class DetailsEmloyeeViewModel
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
        [DisplayName("Полномочия")]
        public List<Role> roles { get; set; }
        public IEnumerable<Class> classes { get; set; }
        public IEnumerable<Discipline> disciplines { get; set; }
        public void SetEmployee(Employee employee)
        {
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
        }
    }
}
