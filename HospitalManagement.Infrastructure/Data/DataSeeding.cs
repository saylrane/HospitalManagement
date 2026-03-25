using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.Infrastructure.Data
{
    public class DataSeeding
    {
        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            // Define roles
            string[] roles = { "Admin", "Doctor", "Patient", "Pharmacist", "Receptionist" };

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

            // Create default Pharmacist user
            const string pharmacistEmail = "pharmacist@hospital.com";
            const string pharmacistPassword = "Pharmacy123!";

            var pharmacistUser = await userManager.FindByEmailAsync(pharmacistEmail);
            if (pharmacistUser == null)
            {
                var newPharmacistUser = new IdentityUser
                {
                    UserName = pharmacistEmail,
                    Email = pharmacistEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newPharmacistUser, pharmacistPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newPharmacistUser, "Pharmacist");
                }
            }

            // Create default Receptionist user
            const string receptionistEmail = "receptionist@hospital.com";
            const string receptionistPassword = "Reception123!";

            var receptionistUser = await userManager.FindByEmailAsync(receptionistEmail);
            if (receptionistUser == null)
            {
                var newReceptionistUser = new IdentityUser
                {
                    UserName = receptionistEmail,
                    Email = receptionistEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newReceptionistUser, receptionistPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newReceptionistUser, "Receptionist");
                }
            }
        }
    }
}
