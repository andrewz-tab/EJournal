using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static EJournal.Models.PersonalData;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EJournal.Models.ViewModels.EmployeeViewModels
{
    public class UpSertEmployeeViewModel
    {
        public int Id { get; set; } = 0;
        [DisplayName("ФИО")]
        [StringLength(100, ErrorMessage = "Максимальное значение символов - 100")]
        [Required(ErrorMessage = "Укажите ФИО сотрудника")]
        public string FullName { get; set; }
        [DisplayName("Дата рождения")]
        [Required(ErrorMessage = "Укажите дату рождения")]
        public DateTime DateBirth { get; set; } = DateTime.Now;
        [DisplayName("Пол")]
        [Required(ErrorMessage = "Выберите пол")]
        public Gender gender { get; set; } = Gender.Men;
        [DisplayName("Серия и номер паспорта")]
        [Required(ErrorMessage = "Укажите серию и номер пасспорта пасспорт")]
        [RegularExpression(@"[0-9]{10}", ErrorMessage = "Введите cерию и номер без пробела (использовать для этого только цифры)")]
        public string PassId { get; set; }
        [DisplayName("СНИЛС")]
        [Required(ErrorMessage = "Укажите значение СНИЛС")]
        [RegularExpression(@"[0-9]{11}", ErrorMessage = "Введите СНИЛС без пробелов и тире")]
        public string SNILS { get; set; }
        [DisplayName("Описание")]
        [MaxLength(100, ErrorMessage = "Максимальное значение символов - 100")]
        public string? Description { get; set; }
        [DisplayName("Электронная почта")]
        [EmailAddress(ErrorMessage = "Неверный формат написания электронной почты")]
        [Required(ErrorMessage = "Укажите электронную почту")]
        public string EMail { get; set; }
        [DisplayName("Номер телефона")]
        [Phone(ErrorMessage = "Неверный формат номера")]
        [Required(ErrorMessage = "Укажите номер мобильного телефона")]
        public string PhoneNumber { get; set; }
        public bool isActivate { get; set; } = false;
        [DisplayName("Полномочия")]
        [Required(ErrorMessage = "Укажите хотя бы одну роль для сотрдника")]
        public List<int> RoleIds { get; set; }
        public MultiSelectList RoleMultpleSelectList { get; set; }
        public UpSertEmployeeViewModel() { }
        public Employee CreateEmployee(IEnumerable<Role> selectedRoles)
        {
            Employee newEmployee = new Employee();
            newEmployee.Description = Description;
            newEmployee.Account = new Account();
            newEmployee.Account.isActivate = false;
            newEmployee.Account.PhoneNumber = PhoneNumber;
            newEmployee.Account.EMail = EMail;
            newEmployee.Account.Employee = newEmployee;
            newEmployee.Account.TypeUser = new TypeUser();
            newEmployee.Account.PersonalData = new PersonalData();
            newEmployee.Account.PersonalData.FullName = FullName;
            newEmployee.Account.PersonalData.PassId = PassId;
            newEmployee.Account.PersonalData.SNILS = SNILS;
            newEmployee.Account.PersonalData.DateBirth = DateBirth;
            newEmployee.Account.PersonalData.gender = gender;
            newEmployee.Account.PersonalData.Account = newEmployee.Account;
            newEmployee.Account.Employee = newEmployee;
            newEmployee.Roles = selectedRoles.ToList();
            return newEmployee;
        }
        public void SetEmployee(Employee employee, MultiSelectList roleMultpleSelectList)
        {
            Id = employee.Id;
            Description = employee.Description;
            PhoneNumber = employee.Account.PhoneNumber;
            EMail = employee.Account.EMail.ToLower();
            SNILS = employee.Account.PersonalData.SNILS;
            FullName = employee.Account.PersonalData.FullName;
            PassId = employee.Account.PersonalData.PassId;
            DateBirth = employee.Account.PersonalData.DateBirth;
            gender = employee.Account.PersonalData.gender;
            isActivate = employee.Account.isActivate;
            RoleMultpleSelectList = roleMultpleSelectList;
            RoleIds = new List<int>();
            employee.Roles.ForEach(r => RoleIds.Add(r.Id));
        }
        public void GetCopy(Employee employee, IEnumerable<Role> selectedRoles)
        {
            employee.Description = Description;
            employee.Account.PhoneNumber = PhoneNumber;
            employee.Account.EMail = EMail.ToLower();
            employee.Account.PersonalData.FullName = FullName;
            employee.Account.PersonalData.PassId = PassId;
            employee.Account.PersonalData.SNILS = SNILS;
            employee.Account.PersonalData.DateBirth = DateBirth;
            employee.Account.PersonalData.gender = gender;
            employee.Roles = selectedRoles.ToList();
        }


    }
}
