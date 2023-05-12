using System.ComponentModel;

namespace EJournal.Models
{
    public class Role
    {
        public int Id { get; set; }
        [DisplayName("Название роли")]
        public string Name { get; set; }

        public List<Employee> Employees { get; set; } = new();
    }
}
