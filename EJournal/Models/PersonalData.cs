using System.ComponentModel;

namespace EJournal.Models
{
    public class PersonalData
    {
        public enum Gender
        {
            Men,
            Women
        }
        public int Id { get; set; }
        [DisplayName("ФИО")]
        public string FullName { get; set; }
        [DisplayName("Дата рождения")]
        public DateTime DateBirth { get; set; }
        public Gender gender { get; set; }
        public string? PassId { get; set; }
        [DisplayName("СНИЛС")]
        public string SNILS { get; set; }
        public int AccountKey { get; set; }
        public Account Account { get; set; }
    }
}
