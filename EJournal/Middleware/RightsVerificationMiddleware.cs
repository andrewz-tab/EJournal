using EJournal.Response;
using EJournal.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Security.Claims;
using System.Text.Json;
namespace EJournal.Utility
{
	public class RightsVerificationMiddleware
	{
		private readonly RequestDelegate next;
		public RightsVerificationMiddleware(RequestDelegate next)
		{
			this.next = next;
		}
		public async Task InvokeAsync(HttpContext context, IAccountService _accountService, EndpointDataSource endpointDataSource)
		{
            var user = context.User;
			if (user == null)
			{
				await next.Invoke(context);
			}
			else if (user.Identity?.IsAuthenticated ?? false)
			{
				var response = await _accountService.CheckChanged(new ClaimsIdentity(user.Claims));
				if (response.StatusCode == StatusCodeEnum.OK)
				{
					if (response.Data)
					{
						var responseUpdate = await _accountService.Update(new ClaimsIdentity(user.Claims));
						if (responseUpdate.StatusCode == StatusCodeEnum.OK)
						{
							await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
							var claimsPrincipal = new ClaimsPrincipal(responseUpdate.Data);
							await context.SignInAsync(claimsPrincipal);
						}
						else
						{
							await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
						}
					}
					if (user.HasClaim(claim => claim.Type == WC.RequiredChangePassword) && context.Request.Path != "/Login/ChangePassword" && context.Request.Path != "/Login/Logout")
					{
						context.Response.Redirect("/Login/ChangePassword");
                    }
					await next.Invoke(context);
				}
				else
				{
					await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
					await next.Invoke(context);
				}
			}
			else
			{
				await next.Invoke(context);
			}
		}
	}
	public static class RightsVerificationExtensions
	{
		public static IApplicationBuilder UseCheckAccount(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<RightsVerificationMiddleware>();
		}
	}

}
