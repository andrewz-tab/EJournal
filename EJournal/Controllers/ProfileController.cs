using EJournal.Models;
using EJournal.Models.ViewModels.ProfileViewModels;
using EJournal.Response;
using EJournal.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EJournal.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAccountService _accountService;
        public ProfileController(IAccountService accountService) 
        {
            _accountService = accountService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            ProfileViewModel model = new ProfileViewModel();
            var user = HttpContext.User;
            var response = await _accountService.GetFullAccountAsync(new ClaimsIdentity(user.Claims));
            if (response.StatusCode != StatusCodeEnum.OK)
            {
                ViewData["Error"] = ((BaseResponse<Account>)response).Description;
                return View(model);
            }
            Account account = response.Data;
            if (account.TypeUser.Name == WC.StudentUser)
            {
                model.SetStudent(account.Student);
            }
            else if (account.TypeUser.Name == WC.EmployeeUser)
            {
                model.SetEmployee(account.Employee);
            }
            else
            {
                ViewData["Error"] = "Ошибка аккаунта, обратитесь к администрации школы";
                return View(model);
            }
            return View(model);
        }
    }
}
