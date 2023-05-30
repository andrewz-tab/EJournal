using System.ComponentModel;

namespace EJournal.Models
{
    public class Account
    {
        public int Id { get; set; }
        [DisplayName("Электронная почта")]
        public string EMail { get; set; }
        [DisplayName("Номер телефона")]
        public string? PhoneNumber { get; set; }
        public bool isActivate { get; set; } = false;
        public int TypeUserKey { get; set; }
        public TypeUser TypeUser { get; set; }
        public PersonalData PersonalData { get; set; }
        public Employee? Employee { get; set; }
        public Student? Student { get; set; }

    }
}
