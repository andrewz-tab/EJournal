using System.ComponentModel;

namespace EJournal.Models
{
    public class Mark
    {
        public int Id { get; set; }
        [DisplayName("Значение")]
        public int? Value { get; set; }
        [DisplayName("Описание")]
        public String? Decription { get; set; }
        public int LessonKey { get; set; }
        public Lesson Lesson { get; set; }
        [DisplayName("Ученик")]
        public int StudentKey { get; set; }
        public Student Student { get; set; }
    }
}
