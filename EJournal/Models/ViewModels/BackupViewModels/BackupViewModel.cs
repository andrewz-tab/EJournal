namespace EJournal.Models.ViewModels.BackupViewModels
{
    public class BackupViewModel
    {
        public string statusMessage { get; set; }
        public bool isError { get; set; } = false;
        public BackupViewModel(string statusMessage, bool isError)
        {
            this.statusMessage = statusMessage;
            this.isError = isError;
        }
    }
}
