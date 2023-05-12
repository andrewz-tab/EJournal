namespace EJournal.Models
{
    public class TypeUser
    {
        public int Id { get; set; }
        public String Name { get; set; }


        public List<Account> Accounts { get; set; } = new();
    }
}
