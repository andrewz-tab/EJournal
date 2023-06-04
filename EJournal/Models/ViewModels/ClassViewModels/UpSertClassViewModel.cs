using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EJournal.Models.ViewModels.ClassViewModels
{
    public class UpSertClassViewModel
    {
        public int Id { get; set; } = 0;
        [DisplayName("Номер")]
        [Required(ErrorMessage = "Введите номер класса от 1 до 11")]
        [Range(1, 11, ErrorMessage = "Номер класса может быть от 1 до 11")]
        public int Number { get; set; }

        [DisplayName("Буква")]
        [Required(ErrorMessage = "Введите букву класса")]
        [RegularExpression(@"[А-Я]{1}", ErrorMessage = "Введите заглавную букву класса на русском языке")]
        public char Liter { get; set; }
        [DisplayName("Классный руководитель")]
        public int? EmployeeKey { get; set; }
        public IEnumerable<SelectListItem> EmployeeDropDown { get; set; }

        public void SetClass(Class Class)
        {
            Id = Class.Id;
            Number = Class.Number;
            EmployeeKey = Class.EmployeeKey;
            Liter = Class.Liter;
        }
        public Class CreateClass()
        {
            Class newClass = new Class();
            newClass.Id = Id;
            newClass.Number = Number;
            newClass.EmployeeKey = EmployeeKey;
            newClass.Liter = Liter;
            return newClass;
        }
        public void GetCopy(Class updateClass)
        {
            updateClass.EmployeeKey = EmployeeKey;
            updateClass.Number = Number;
            updateClass.Liter = Liter;
        }
    }
}
