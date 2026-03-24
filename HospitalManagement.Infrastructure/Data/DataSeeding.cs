using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.Infrastructure.Data
{
    public class DataSeeding
    {
        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            // Define roles
            string[] roles = { "Admin", "Doctor", "Patient" };

            // Create roles if they don't exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create admin user if it doesn't exist
            const string adminEmail = "hospital@gmail.com";
            const string adminPassword = "Password1!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }
        }
    }
}
