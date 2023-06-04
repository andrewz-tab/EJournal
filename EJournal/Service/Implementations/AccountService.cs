using Aspose.Foundation.UriResolver.RequestResponses;
using EJournal.Data;
using EJournal.Models;
using EJournal.Models.ViewModels.LoginViewModels;
using EJournal.Repository.IRepository;
using EJournal.Response;
using EJournal.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EJournal.Service.Implementations
{
    public class AccountService : IAccountService
    {
		private readonly JournalDbContext _dbContext;
        private readonly IStudentRepository _studentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public AccountService(JournalDbContext dbContext, IStudentRepository studentRepository, IEmployeeRepository employeeRepository)
        {
			_dbContext = dbContext;
            _studentRepository = studentRepository;
            _employeeRepository = employeeRepository;
        }

        private static string GeneratePass()
        {
            Random rand = new Random();
            const string validPass = "ab)cdefghijklm#nopqrstu%vwxyzAB$CD(EFG@HIJKLM*NOPQR!STUVWX&YZ1234^567890*";
            int sizePass = rand.Next(8, 11);
            string password = "";
            for (int i = 0; i < sizePass; i++)
            {
                password += validPass[rand.Next(validPass.Length)];
            }
            return password;
        }
        private static string GenerateLogin()
        {
            Random rand = new Random();
            const string validLogin = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int sizePass = rand.Next(8, 11);
            string login = "";
            for (int i = 0; i < sizePass; i++)
            {
                login += validLogin[rand.Next(validLogin.Length)];
            }
            return login;
        }
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var HashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = BitConverter.ToString(HashedBytes).Replace("-", "").ToLower();
                return hash;
            }
        }
        public async Task<IBaseResponse<KeyValuePair<string, string>>> ActivateAccountAsync(int accountId)
        {
            Random rand = new Random();
            BaseResponse<KeyValuePair<string, string>> response = new BaseResponse<KeyValuePair<string, string>>();
            Account account = _dbContext.Accounts.FirstOrDefault(a => a.Id == accountId);
            if (account == null)
            {
                response.StatusCode = StatusCodeEnum.AccountNotFound;
                return response;
            }
            else if (account.isActivate)
            {
                response.StatusCode = StatusCodeEnum.ThisAccountHasBeenActivated;
                return response;
            }
            else
            {
                string login;
                string pass;
                try
                {
                    do
                    {
                        login = GenerateLogin();
                        pass = GeneratePass();
                    } while (_dbContext.Accounts.Any(a => a.Login == login));
                    account.Login = login;
                    account.Password = HashPassword(pass);
                    account.isActivate = true;
                    account.isRequiredChangePassword = true;
                    _dbContext.Update(account);
                    await _dbContext.SaveChangesAsync();
                    response.StatusCode = StatusCodeEnum.OK;
                    response.Data = new KeyValuePair<string, string>(login, pass);
                }
                catch (Exception ex)
                {
                    response.Description = "Внутренняя ошибка сервера, обратитесь к администрации школы";
                    response.StatusCode = StatusCodeEnum.InternalServerError;
                }
                return response;
            }
        }

        public async Task<IBaseResponse<ClaimsIdentity>> LoginAsync(LoginViewModel model)
        {
            BaseResponse<ClaimsIdentity> response = new BaseResponse<ClaimsIdentity>();
            Account account = _dbContext.Accounts.FirstOrDefault(a => (a.Login == model.Login || a.PhoneNumber == model.Login || a.EMail == model.Login.ToLower())
            && a.Password == HashPassword(model.Password));
            if (account == null)
            {
                response.Description = "Неверно введен логин или пароль";
                response.StatusCode = StatusCodeEnum.Unauthorized;
                return response;
            }
            else if(account.isActivate == false)
            {
                response.Description = "Неверно введен логин или пароль";
                response.StatusCode = StatusCodeEnum.Unauthorized;
                return response;
            }
            await _dbContext.Entry(account).Reference(a => a.TypeUser).LoadAsync();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Login),
                new Claim(WC.TypeUser, account.TypeUser.Name)
            };
            if(account.isRequiredChangePassword)
            {
                claims.Add(new Claim(WC.RequiredChangePassword, "1"));
            }
            if (account.TypeUser.Name == WC.EmployeeUser)
            {
                Employee employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.AccountKey == account.Id);
                if (employee != null)
                {
                    await _dbContext.Entry(employee).Collection(e => e.Roles).LoadAsync();
                    employee.Roles.ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));
                    claims.Add(new Claim(WC.EmployeeId, employee.Id.ToString()));
                }
                else
                {
                    response.StatusCode = StatusCodeEnum.InternalServerError;
                    response.Description = "Ошибка аккаунта, обратитесь к администрации школы";
                    return response;
                }
            }
            else if(account.TypeUser.Name == WC.StudentUser)
            {
                Student student = await _dbContext.Students.FirstOrDefaultAsync(s => s.AccountKey == account.Id);
                if(student == null)
                {
                    response.StatusCode = StatusCodeEnum.InternalServerError;
                    response.Description = "Ошибка аккаунта, обратитесь к администрации школы";
                    return response;
                }
                claims.Add(new Claim(WC.StudentId, student.Id.ToString()));
                claims.Add(new Claim(WC.ClassId, student.ClassKey.ToString()));
            }
			account.isChanged = false;
			_dbContext.Accounts.Update(account);
			response.StatusCode = StatusCodeEnum.OK;
            response.Data = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return response;
        }

        public async Task<IBaseResponse<Account>> LoadAccountAsync(int accountId)
        {
            BaseResponse<Account> response = new BaseResponse<Account>();
            Account account = _dbContext.Accounts.FirstOrDefault(a => a.Id == accountId);
            if (account == null)
            {
                response.StatusCode = StatusCodeEnum.AccountNotFound;
                return response;
            }
            await _dbContext.Entry(account).Reference(a => a.PersonalData).LoadAsync();
            response.Data = account;
            response.StatusCode = StatusCodeEnum.OK;
            return response;
        }

        public async Task<IBaseResponse<Account>> GetFullAccountAsync(ClaimsIdentity claims)
        {
            BaseResponse<Account> response = new BaseResponse<Account>();
            string login = claims.FindFirst(ClaimTypes.Name)?.Value;
            Account account = _dbContext.Accounts.FirstOrDefault(a => a.Login == login);
            if (account == null)
            {
                response.Description = "Пользователь не найден";
                response.StatusCode = StatusCodeEnum.AccountNotFound;
                return response;
            }
            await _dbContext.Entry(account).Reference(a => a.TypeUser).LoadAsync();
            if (account.TypeUser?.Name == claims.FindFirst(WC.TypeUser)?.Value)
            {
                if (account.TypeUser.Name == WC.EmployeeUser)
                {
                    Employee employee = await _employeeRepository.FirstOrDefaultAsync(isDetail: true, filter: e => e.AccountKey == account.Id);
                    if (employee == null)
                    {
                        response.Description = "Пользователь не найден";
                        response.StatusCode = StatusCodeEnum.AccountNotFound;
                        return response;
                    }
                }
                else if (account.TypeUser.Name == WC.StudentUser)
                {
                    Student student = await _studentRepository.FirstOrDefaultAsync(isDetail: true, filter: s => s.AccountKey == account.Id);
                    if (student == null)
                    {
                        response.Description = "Пользователь не найден";
                        response.StatusCode = StatusCodeEnum.AccountNotFound;
                        return response;
                    }
                }
            }
            else
            {
                response.StatusCode = StatusCodeEnum.InternalServerError;
                response.Description = "Ошибка аккаунта, обратитесь к администрации школы";
                return response;
            }
            response.StatusCode = StatusCodeEnum.OK;
            response.Data = account;
            return response;
        }

        public async Task<IBaseResponse<String>> ChangePasswordAsync(ClaimsIdentity claims, ChangePasswordViewModel model)
        {
            BaseResponse<String> response = new BaseResponse<String>();
            string login = claims.FindFirst(ClaimTypes.Name)?.Value;
            Account account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Login == login && a.Password == HashPassword(model.CurrentPassword));
            if (account == null)
            {
                response.Data  = response.Description = "Неверно введен старый пароль";
                response.StatusCode = StatusCodeEnum.AccountNotFound;
                return response;
            }
            try
            {
                account.Password = HashPassword(model.Password);
                account.isRequiredChangePassword = false;
                _dbContext.Update(account);
                await _dbContext.SaveChangesAsync();
                response.StatusCode = StatusCodeEnum.OK;
            }
            catch
            {
                response.Data = response.Description = "Внутренняя ошибка сервера, обратитесь к администрации школы";
                response.StatusCode = StatusCodeEnum.InternalServerError;
            }
            return response;
        }
        public async Task<IBaseResponse<bool>> CheckChanged(ClaimsIdentity claims)
        {
            string login = claims.FindFirst(ClaimTypes.Name)?.Value;
            Account account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Login == login);
            if (account == null)
            {
                return new BaseResponse<bool>
                {
                    StatusCode = StatusCodeEnum.AccountNotFound,
                    Data = true
                };
            }
            else if (account.isChanged)
            {
                return new BaseResponse<bool>
                {
                    StatusCode = StatusCodeEnum.OK,
                    Data = true
                };
            }
            else
            {
                return new BaseResponse<bool>
                {
                    StatusCode = StatusCodeEnum.OK,
                    Data = false
                };
            }
        }

		public async Task<IBaseResponse<ClaimsIdentity>> Update(ClaimsIdentity claims)
        {
			BaseResponse<ClaimsIdentity> response = new BaseResponse<ClaimsIdentity>();
			string login = claims.FindFirst(ClaimTypes.Name)?.Value;
			Account account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Login == login);
			if (account == null)
			{
				response.Description = "Аккаунт не найден";
				response.StatusCode = StatusCodeEnum.AccountNotFound;
				return response;
			}
			else if (account.isActivate == false)
			{
				response.Description = "Неверно введен логин или пароль";
				response.StatusCode = StatusCodeEnum.Unauthorized;
				return response;
			}
			await _dbContext.Entry(account).Reference(a => a.TypeUser).LoadAsync();
			var NewClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, account.Login),
				new Claim(WC.TypeUser, account.TypeUser.Name)
			};
			if (account.isRequiredChangePassword)
			{
				NewClaims.Add(new Claim(WC.RequiredChangePassword, "1"));
			}
			if (account.TypeUser.Name == WC.EmployeeUser)
			{
				Employee employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.AccountKey == account.Id);
				if (employee != null)
				{
					await _dbContext.Entry(employee).Collection(e => e.Roles).LoadAsync();
					employee.Roles.ForEach(r => NewClaims.Add(new Claim(ClaimTypes.Role, r.Name)));
					NewClaims.Add(new Claim(WC.EmployeeId, employee.Id.ToString()));
				}
				else
				{
					response.StatusCode = StatusCodeEnum.InternalServerError;
					response.Description = "Ошибка аккаунта, обратитесь к администрации школы";
					return response;
				}
			}
			else if (account.TypeUser.Name == WC.StudentUser)
			{
				Student student = await _dbContext.Students.FirstOrDefaultAsync(s => s.AccountKey == account.Id);
				if (student == null)
				{
					response.StatusCode = StatusCodeEnum.InternalServerError;
					response.Description = "Ошибка аккаунта, обратитесь к администрации школы";
					return response;
				}
				NewClaims.Add(new Claim(WC.StudentId, student.Id.ToString()));
				NewClaims.Add(new Claim(WC.ClassId, student.ClassKey.ToString()));
			}
            account.isChanged = false;
			_dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
			response.StatusCode = StatusCodeEnum.OK;
			response.Data = new ClaimsIdentity(NewClaims, CookieAuthenticationDefaults.AuthenticationScheme);
			return response;
		}
	}
}
