namespace EJournal.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Discipline> Disciplines { get; set; } = new();
    }
}
