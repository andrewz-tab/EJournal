namespace EJournal.Models
{
    public class Class
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public char Liter { get; set; }
        public int? EmployeeKey { get; set; }
        public Employee? Employee { get; set; }
        public List<Student> Students { get; set; } = new();
        public List<Discipline> Disciplines { get; set; } = new();

        public string Name { get
            {
                return Number.ToString() + Liter;
            }
        }
    }
}
