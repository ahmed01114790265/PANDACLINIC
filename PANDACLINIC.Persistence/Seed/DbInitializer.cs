using Microsoft.AspNetCore.Identity;
using PANDACLINIC.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Persistence.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAdminUser(RoleManager<IdentityRole<Guid>> roleManager, UserManager<ApplicationUser> userManager)
        {
          
            string[] roleNames = { "Admin", "Staff", "Customer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }

           
            var adminEmail = "admin@pandaclinic.com";
            var adminPassword = "PandaAdmin123!";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    fullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword); 

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
            else
            {
                var changed = false;

                if (adminUser.UserName != adminEmail)
                {
                    adminUser.UserName = adminEmail;
                    changed = true;
                }

                if (adminUser.Email != adminEmail)
                {
                    adminUser.Email = adminEmail;
                    changed = true;
                }

                if (!adminUser.EmailConfirmed)
                {
                    adminUser.EmailConfirmed = true;
                    changed = true;
                }

                if (changed)
                {
                    await userManager.UpdateAsync(adminUser);
                }

                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                if (!await userManager.CheckPasswordAsync(adminUser, adminPassword))
                {
                    var resetToken = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                    await userManager.ResetPasswordAsync(adminUser, resetToken, adminPassword);
                }
            }
        }
    }
}
