using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PhaseOne.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOne.Data
{
    public static class SeedData
    {
        private const string AdminEmail = "admin@feit.ukim.edu.mk";
        private const string AdminPassword = "Admin123!";

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1) Roles (ќе ги креира сите улоги, ама без да додава корисници за нив)
            string[] roles = { "Admin", "Professor", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                        throw new Exception($"Не успеа креирање улога '{role}'. Errors: {errors}");
                    }
                }
            }

            // 2) Admin user (само овој се креира автоматски)
            var adminUser = await EnsureUserAsync(
                userManager,
                email: AdminEmail,
                password: AdminPassword
            );
            await EnsureUserInRoleAsync(userManager, adminUser, "Admin");
        }

        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password
        )
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null) return user;

            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Не успеа креирање user '{email}'. Errors: {errors}");
            }

            return user;
        }

        private static async Task EnsureUserInRoleAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string role
        )
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                var result = await userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Не успеа додавање на '{user.Email}' во улога '{role}'. Errors: {errors}");
                }
            }
        }
    }
}
