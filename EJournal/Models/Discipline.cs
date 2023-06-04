using Aspose.Pdf;

namespace EJournal.Models
{
    public class Discipline
    {
        public int Id { get; set; }
        public int SubjectKey { get; set; }
        public Subject Subject { get; set; }
        public int EmployeeKey { get; set; }
        public Employee Employee { get; set; }
        public int ClassKey { get; set; }
        public Class Class { get; set; }
        public List<Lesson> Lessons { get; set; } = new();

    }
}
