using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static EJournal.Models.PersonalData;
using System.ComponentModel;

namespace EJournal.Models.ViewModels.EmployeeViewModels
{
    public class UpSertEmployeeViewModel
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
        [DisplayName("Полномочия")]
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
            EMail = employee.Account.EMail;
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
            employee.Account.isActivate = isActivate;
            employee.Account.PhoneNumber = PhoneNumber;
            employee.Account.EMail = EMail;
            employee.Account.PersonalData.FullName = FullName;
            employee.Account.PersonalData.PassId = PassId;
            employee.Account.PersonalData.SNILS = SNILS;
            employee.Account.PersonalData.DateBirth = DateBirth;
            employee.Account.PersonalData.gender = gender;
            employee.Roles = selectedRoles.ToList();
        }


    }
}
