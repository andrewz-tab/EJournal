using Microsoft.AspNetCore.Mvc.Rendering;

namespace EJournal.Models.ViewModels
{
    public class AccountStudentVM : AccountBaseVM
    {

        public Student Student { get; set; }
        public IEnumerable<SelectListItem> ClassSelectList { get; set; }

    }
}
