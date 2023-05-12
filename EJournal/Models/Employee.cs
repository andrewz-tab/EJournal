using System.ComponentModel;

namespace EJournal.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public String? Description { get; set; }

        public List<Class> Classes { get; set; } = new();
        public List<Discipline> Disciplines { get; set; } = new();
        [DisplayName("Роли")]
        public List<Role> Roles { get; set; } = new();
        public int AccountKey { get; set; }
        public Account Account { get; set; }
    }
}
