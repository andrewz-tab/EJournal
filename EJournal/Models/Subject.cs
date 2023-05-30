using System.ComponentModel;

namespace EJournal.Models
{
    public class Subject
    {
        public int Id { get; set; }
        [DisplayName("Название предмета")]
        public string Name { get; set; }

        public List<Discipline> Disciplines { get; set; } = new();
    }
}
