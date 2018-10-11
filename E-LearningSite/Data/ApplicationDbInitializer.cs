using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace E_LearningSite.Data
{
    public class ApplicationDbInitializer
    {
        public static async Task InitializeIdentity(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager)
        {
            dbContext.Database.EnsureCreated();

            var role = await roleManager.FindByNameAsync(Helpers.EnumRole.Admin);

            if (role != null)
            {
                return;
            }

            string[] roles = {Helpers.EnumRole.Instructor, Helpers.EnumRole.Admin};
            foreach (var roleName in roles)
            {
                role = new IdentityRole(roleName);
                await roleManager.CreateAsync(role);
            }
        }
    }
}
