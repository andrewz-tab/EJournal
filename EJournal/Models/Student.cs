using System.ComponentModel;

namespace EJournal.Models
{
    public class Student
    {
        public int Id { get; set; }
        [DisplayName("Класс")]
        public int ClassKey { get; set; }
        public Class Class { get; set; }
        [DisplayName("Описание")]
        public String? Description { get; set; }
        
        public List<Mark> Marks { get; set; } = new();
        public int AccountKey { get; set; }
        public Account Account { get; set; }
    }
}
