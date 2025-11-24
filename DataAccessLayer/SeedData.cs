using DbDesign.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DbDesign
{
    public class SeedData
    {
		public static async Task<bool> SeedIdentityRoles(UserManager<User> userManager, RoleManager<Role> roleManager)
		{
			bool saveChanges = false, commit = true;
			if (!await roleManager.RoleExistsAsync(Role.AdminRoleKey))
			{
				await roleManager.CreateAsync(new Role(Role.AdminRoleKey));
				saveChanges = true;
			}
			if (!await roleManager.RoleExistsAsync(Role.DoctorRoleKey))
			{
				await roleManager.CreateAsync(new Role(Role.DoctorRoleKey));
				saveChanges = true;
			}
			if (!await roleManager.RoleExistsAsync(Role.StaffRoleKey))
			{
				await roleManager.CreateAsync(new Role(Role.StaffRoleKey));
				saveChanges = true;
			}

			if (await userManager.FindByNameAsync(User.DefaultAdminUsernameKey) == null)
			{
				var user = new User
				{
					UserName = User.DefaultAdminUsernameKey,
					//EmailConfirmed = true
				};
				var result = await userManager.CreateAsync(user, "A9m!n");
				result = await userManager.AddToRoleAsync(user, Role.AdminRoleKey);
				saveChanges = true;
			}
			return saveChanges;
		}
	}
}
 