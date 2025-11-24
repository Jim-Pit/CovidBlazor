using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbDesign.Entities
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

    public class Role : IdentityRole<Guid>
	{
		public const string AdminRoleKey = "admin";
		public const string DoctorRoleKey = "doctor";
		public const string StaffRoleKey = "staff";

		public Role()
		{ }
		public Role(string roleName)
			: base(roleName)
		{ }
	}

	public class User : IdentityUser<Guid>
	{
		public const string DefaultAdminUsernameKey = "admin";

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Name => $"{FirstName} {LastName}";
		public ValueObjects.Address Address { get; set; }
		public UserRole UserRole { get; set; }

		public User()
		{
			EmailConfirmed = true;
			Address = new ValueObjects.Address();
		}

		public User(User user)
		{
			Id = user.Id;
			UserName = user.UserName;
			FirstName = user.FirstName;
			LastName = user.LastName;
			Email = user.Email;
			Address = user.Address;
			UserRole = user.UserRole;
		}
	}
}
