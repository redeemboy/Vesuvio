using Vesuvio.DatabaseMigration;
using Vesuvio.Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WeddingNepal.WebAPI.Helpers
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDatabaseContext context, IServiceProvider serviceProvider)
        {
            if (!context.Roles.Any() && !context.Users.Any())
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                IdentityResult roleResult;

                var role = new ApplicationRole()
                {
                    Name = "superadmin",
                    NormalizedName = "SUPERADMIN"
                };
                roleResult = await roleManager.CreateAsync(role);

                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var userName = new ApplicationUser
                {
                    UserName = "superadmin",
                    FirstName = "SuperAdmin"
                };
                string userPassword = "SUPERadmin@123!$";
                var createUser = await userManager.CreateAsync(userName, userPassword);
                if (createUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(userName, "superadmin");
                }
            }
        }
    }
}