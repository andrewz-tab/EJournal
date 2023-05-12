using Microsoft.AspNetCore.Mvc.Rendering;

namespace EJournal.Models.ViewModels
{
    public class UpSertClassViewModel
    {
        public int Number { get; set; }
        public char Liter { get; set; }
        public int EmployeeKey { get; set; }
        public IEnumerable<SelectListItem> EmployeeDropDown { get; set; }

    }
}
