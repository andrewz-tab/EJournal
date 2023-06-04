using NuGet.Packaging.Signing;

namespace EJournal.Models.ViewModels.LoginViewModels
{
    public class ActivateAccountViewModel
    {
        public int accountId { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Login{ get; set; }
        public void SetActivateAccount(Account account, KeyValuePair<string, string> pasKey)
        {
            FullName = account.PersonalData.FullName;
            accountId = account.Id;
            Login = pasKey.Key;
            Password = pasKey.Value;
        }
    }
}
