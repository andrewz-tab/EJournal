using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EJournal.Models.ViewModels.DisciplineViewModels
{
    public class UpsertDisciplineViewModel
    {
        public int Id { get; set; } = 0;
        [DisplayName("Учитель")]
        public int EmployeeKey { get; set; }
        public IEnumerable<SelectListItem> EmployeeDropDown { get; set; }
        [DisplayName("Класс")]
        public int ClassKey { get; set; }
        public IEnumerable<SelectListItem> ClassDropDown { get; set; }
        [DisplayName("Предмет")]
        public int SubjectKey { get; set; }
        public IEnumerable<SelectListItem> SubjectDropDown { get; set; }
        [DisplayName("Классы")]
        public List<int> ClassesIds { get; set; }
        public MultiSelectList ClassMultpleSelectList { get; set; }

        public void SetDiscipline(Discipline discipline)
        {
            Id = discipline.Id;
            EmployeeKey = discipline.EmployeeKey;
            ClassKey = discipline.ClassKey;
            SubjectKey = discipline.SubjectKey;
        }
        public Discipline CreateDiscipline()
        {
            Discipline newDiscipline = new Discipline();
            newDiscipline.Id = Id;
            newDiscipline.EmployeeKey = EmployeeKey;
            newDiscipline.SubjectKey = SubjectKey;
            newDiscipline.ClassKey = ClassKey;
            return newDiscipline;
        }
        public IEnumerable<Discipline> CreateDisciplines()
        {
            List<Discipline> disciplines = new List<Discipline>(ClassesIds.Count());
            foreach(var classId in ClassesIds)
            {
                disciplines.Add(new Discipline { Id = Id, EmployeeKey = EmployeeKey, SubjectKey =  SubjectKey, ClassKey = classId });
            }
            return disciplines;
        }
        public void GetCopy(Discipline updateDiscipline)
        {
            updateDiscipline.EmployeeKey = EmployeeKey;
            updateDiscipline.SubjectKey = SubjectKey;
            updateDiscipline.ClassKey = ClassKey;
        }
    }
}
