using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace EJournal.Models
{
    public class Subject
    {
        public int Id { get; set; }
        //[Remote(action: "CheckEmail", controller: "Home", ErrorMessage = "Email уже используется")]
        [DisplayName("Название предмета")]
        public string Name { get; set; }

        public List<Discipline> Disciplines { get; set; } = new();
    }
}
