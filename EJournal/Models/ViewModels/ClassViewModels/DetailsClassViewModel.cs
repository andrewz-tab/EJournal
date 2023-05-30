using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace EJournal.Models.ViewModels.ClassViewModels
{
    public class DetailsClassViewModel
    {
        public int Id { get; set; } = 0;
        [DisplayName("Номер")]
        public int Number { get; set; }
        [DisplayName("Буква")]
        public char Liter { get; set; }
        [DisplayName("Класс")]
        public string Name { get { return Number.ToString() + " " + Liter; } }
        [DisplayName("Классный руководитель")]
        public int? EmployeeKey { get; set; }
        [DisplayName("Классный руководитель")]
        public String EmployeeName { get; set; }
        public IEnumerable<Student> students { get; set; }
        public IEnumerable<Discipline> disciplines { get; set; }

        public void SetClass(Class Class)
        {
            Id = Class.Id;
            Number = Class.Number;
            EmployeeKey = Class.EmployeeKey;
            Liter = Class.Liter;
            EmployeeName = Class.Employee.Account.PersonalData.FullName;
            students = Class.Students;
            disciplines = Class.Disciplines;
        }

    }
}
