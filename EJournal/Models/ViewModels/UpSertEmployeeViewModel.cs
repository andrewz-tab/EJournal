using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static EJournal.Models.PersonalData;
using System.ComponentModel;

namespace EJournal.Models.ViewModels
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
        public String? Description { get; set; }
        [DisplayName("Электронная почта")]
        public string EMail { get; set; }
        [DisplayName("Номер телефона")]
        public string? PhoneNumber { get; set; }
        public bool isActivate { get; set; } = false;
        public List<int> RoleIds { get; set; }
        public MultiSelectList RoleMultpleSelectList { get; set; }
        public UpSertEmployeeViewModel() { }
        public Employee CreateEmployee(TypeUser typeUser, IEnumerable<Role> selectedRoles)
        {
            Employee newEmployee = new Employee();
            newEmployee.Description = this.Description;
            newEmployee.Account = new Account();
            newEmployee.Account.isActivate = false;
            newEmployee.Account.PhoneNumber = this.PhoneNumber;
            newEmployee.Account.EMail = this.EMail;
            newEmployee.Account.Employee = newEmployee;
            newEmployee.Account.TypeUser = typeUser;
            newEmployee.Account.PersonalData = new PersonalData();
            newEmployee.Account.PersonalData.FullName = this.FullName;
            newEmployee.Account.PersonalData.PassId = this.PassId;
            newEmployee.Account.PersonalData.SNILS = this.SNILS;
            newEmployee.Account.PersonalData.DateBirth = this.DateBirth;
            newEmployee.Account.PersonalData.gender = this.gender;
            newEmployee.Account.PersonalData.Account = newEmployee.Account;
            newEmployee.Account.Employee = newEmployee;
            newEmployee.Roles = selectedRoles.ToList();
            return newEmployee;
        }
        public void SetEmployee(Employee employee, MultiSelectList roleMultpleSelectList)
        {
            this.Id = employee.Id;
            this.Description = employee.Description;
            this.PhoneNumber = employee.Account.PhoneNumber;
            this.EMail = employee.Account.EMail;
            this.SNILS = employee.Account.PersonalData.SNILS;
            this.FullName = employee.Account.PersonalData.FullName;
            this.PassId = employee.Account.PersonalData.PassId;
            this.DateBirth = employee.Account.PersonalData.DateBirth;
            this.gender = employee.Account.PersonalData.gender;
            this.isActivate = employee.Account.isActivate;
            this.RoleIds = new List<int>();
            employee.Roles.ForEach(r => this.RoleIds.Add(r.Id));
        }
        public void GetCopy(Employee employee, IEnumerable<Role> selectedRoles)
        {
            employee.Description = this.Description;
            employee.Account.isActivate = this.isActivate;
            employee.Account.PhoneNumber = this.PhoneNumber;
            employee.Account.EMail = this.EMail;
            employee.Account.PersonalData.FullName = this.FullName;
            employee.Account.PersonalData.PassId = this.PassId;
            employee.Account.PersonalData.SNILS = this.SNILS;
            employee.Account.PersonalData.DateBirth = this.DateBirth;
            employee.Account.PersonalData.gender = this.gender;
            employee.Roles = selectedRoles.ToList();
        }


    }
}
