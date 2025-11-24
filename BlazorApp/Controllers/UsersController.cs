using DbDesign;
using DbDesign.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoViDAccountant.Controllers
{
	[Route("api/users")]
	public class UsersController : ControllerBase
    {
		private readonly IDataProtector dataProtector;
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;
		private readonly CoViDAccountantDbContext dbContext;

		public UsersController(
			IDataProtectionProvider dataProtectionProvider,
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			CoViDAccountantDbContext dbContext)
		{
			dataProtector = dataProtectionProvider.CreateProtector("SignIn");
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.dbContext = dbContext;
		}

		[AllowAnonymous]
		[HttpGet("login")]
		public async Task<IActionResult> Login(string queryStringParam)
		{
			var data = dataProtector.Unprotect(queryStringParam);

			var parts = data.Split('|');

			var identityUser = await userManager.FindByIdAsync(parts[0]);

			var isTokenValid = await userManager.VerifyUserTokenAsync(identityUser, TokenOptions.DefaultProvider, "SignIn", parts[1]);

			if (isTokenValid)
			{
				await signInManager.SignInAsync(identityUser, true);
				if (parts.Length == 3 && Url.IsLocalUrl(parts[2]))
				{
					return Redirect(parts[2]);
				}
				var url = await RedirectTo(identityUser);
				if (!string.IsNullOrEmpty(url))
				{
					return Redirect(url);
				}
				return Redirect("/");
			}
			else
			{
				return Unauthorized("STOP!");
			}
		}

		//[Authorize]
		[HttpGet("logout")]
		public async Task<IActionResult> SignOut()
		{
			await signInManager.SignOutAsync();
			return Redirect("/account/login");
		}

		private async Task<string> RedirectTo(User user)
		{
			var userWithRole = await dbContext.Users
				//.Include(u => u.UserRole).ThenInclude(ur => ur.Role)
				.Include(u => u.UserRole.Role)
				.SingleOrDefaultAsync(u => u.Id == user.Id);

			if (new string[] { Role.StaffRoleKey, Role.DoctorRoleKey }.Contains(userWithRole.UserRole?.Role.Name) == true)
			{
				return "/personal";
			}
			return "";
		}
	}
}
