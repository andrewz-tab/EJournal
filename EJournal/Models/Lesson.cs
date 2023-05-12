namespace EJournal.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public String? HomeWork { get; set; }
        public DateTime DateTime { get; set; }
        public int Index { get; set; }
        public int DisciplineKey { get; set; }
        public Discipline Discipline { get; set; }
        public List<Mark> Marks { get; set; } = new();

    }
}
