using EJournal.Models;
using EJournal.Models.ViewModels.LoginViewModels;
using EJournal.Response;
using System.Security.Claims;

namespace EJournal.Service.Interfaces
{
    public interface IAccountService
    {
        public Task<IBaseResponse<Account>> LoadAccountAsync(int accountId);
        public Task<IBaseResponse<KeyValuePair<string, string>>> ActivateAccountAsync(int accountId);
        public Task<IBaseResponse<String>> ChangePasswordAsync(ClaimsIdentity claims, ChangePasswordViewModel model);
        public Task<IBaseResponse<ClaimsIdentity>> LoginAsync(LoginViewModel model);
        public Task<IBaseResponse<Account>> GetFullAccountAsync(ClaimsIdentity claims);
		public Task<IBaseResponse<bool>> CheckChanged(ClaimsIdentity claims);
		public Task<IBaseResponse<ClaimsIdentity>> Update(ClaimsIdentity claims);
	}
}
