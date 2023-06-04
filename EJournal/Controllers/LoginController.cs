using EJournal.Models;
using EJournal.Models.ViewModels.LoginViewModels;
using EJournal.Response;
using EJournal.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EJournal.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountService AccountService;
        public LoginController(IAccountService accountService)
        {
            this.AccountService = accountService;
        }
        public IActionResult Index(string? returnUrl)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return Redirect(returnUrl ?? "/Home");
            }
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }
        [Authorize(Policy = WC.PolicyOnlyForHeadTeacherOrAdmin)]
        public async Task<IActionResult> ActivateAccount(int accountId)
        {
            var logPass = await AccountService.ActivateAccountAsync(accountId);
            var account = await AccountService.LoadAccountAsync(accountId);
            if(logPass.StatusCode == StatusCodeEnum.AccountNotFound)
            {
                ViewData["Error"] = "Данный аккаунт был не найден!";
            }
            else if (logPass.StatusCode == StatusCodeEnum.ThisAccountHasBeenActivated)
            {
                ViewData["Error"] = "Данный аккаунт уже был активирован!";
            }
            else if (logPass.StatusCode == StatusCodeEnum.InternalServerError)
            {
                ViewData["Error"] = "Внутренная ошибка сервера, попробуйте еще раз!";
            }
            ActivateAccountViewModel model = new ActivateAccountViewModel();
            model.SetActivateAccount(account.Data, logPass.Data);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string? returnUrl, LoginViewModel model)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return Redirect(returnUrl ?? "/Home");
            }
            if (ModelState.IsValid)
            {
                var response = await AccountService.LoginAsync(model);
                if (response.StatusCode != StatusCodeEnum.OK)
                {
                    ViewData["Error"] = ((BaseResponse<ClaimsIdentity>)response).Description;
                }
                else
                {
                    try
                    {
                        var claimsPrincipal = new ClaimsPrincipal(response.Data);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        if (response.Data.HasClaim(claim => claim.Type == WC.RequiredChangePassword))
                        {
                            return RedirectToAction(nameof(ChangePassword));
                        }
                    }
                    catch
                    {
                        ViewData["Error"] = "Внутренняя ошибка сервера, попробуйте позже";
                        return View(new LoginViewModel());
                    }
                    return Redirect(returnUrl ?? "/Home");
                }
            }
            model.Password = "";
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            var user = HttpContext.User.Identity;
            if(user is not null && user.IsAuthenticated)
            {
                ChangePasswordViewModel model = new ChangePasswordViewModel();
                return View(model);
            }
            return Redirect("/Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = HttpContext.User.Identity;
                if (user is not null && user.IsAuthenticated)
                {
                    if(model.Password != model.PasswordConfirm)
                    {
                        ViewData["Error"] = "Пароли не совпадают";
                        model = new ChangePasswordViewModel();
                        return View(model);
                    }
                    var response = await AccountService.ChangePasswordAsync(new ClaimsIdentity(HttpContext.User.Claims), model);
                    if(response.StatusCode == StatusCodeEnum.OK)
                    {
                        ViewData["Good"] = "Пароль успешно заменен";
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        return View(new ChangePasswordViewModel());
                    }
                    else
                    {
                        ViewData["Error"] = response.Data;
                        model = new ChangePasswordViewModel();
                        return View(model);
                    }
                }
                return Redirect("/Login");
            }
            model = new ChangePasswordViewModel();
            return View(model);
        }
    }
}
