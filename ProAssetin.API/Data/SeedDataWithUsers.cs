using Microsoft.AspNetCore.Identity;
using ProAssetin.API.Models;

namespace ProAssetin.API.Data
{
    /// <summary>
    /// Extended seed data with test users
    /// Call this after initial database creation
    /// </summary>
    public static class SeedDataWithUsers
    {
        public static async Task SeedTestUsersAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure roles exist
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed 5 users for Infosys
            await SeedUserAsync(userManager, "admin@infosys.com", "Admin", "Infosys", "Admin", "infosys", "Bangalore", "Project Alpha", "Team A");
            await SeedUserAsync(userManager, "pavan@infosys.com", "Pavan", "Kumar", "User", "infosys", "Hyderabad", "Project Beta", "Team B");
            await SeedUserAsync(userManager, "ravi@infosys.com", "Ravi", "Sharma", "User", "infosys", "Pune", "Project Gamma", "Team C");
            await SeedUserAsync(userManager, "priya@infosys.com", "Priya", "Patel", "User", "infosys", "Mumbai", "Project Delta", "Team A");
            await SeedUserAsync(userManager, "amit@infosys.com", "Amit", "Singh", "User", "infosys", "Delhi", "Project Epsilon", "Team B");

            // Seed 5 users for Wipro
            await SeedUserAsync(userManager, "admin@wipro.com", "Admin", "Wipro", "Admin", "wipro", "Bangalore", "Project X", "Team 1");
            await SeedUserAsync(userManager, "sanjay@wipro.com", "Sanjay", "Mehta", "User", "wipro", "Chennai", "Project Y", "Team 2");
            await SeedUserAsync(userManager, "neha@wipro.com", "Neha", "Gupta", "User", "wipro", "Gurgaon", "Project Z", "Team 1");
            await SeedUserAsync(userManager, "vikram@wipro.com", "Vikram", "Reddy", "User", "wipro", "Bangalore", "Project A", "Team 3");
            await SeedUserAsync(userManager, "anjali@wipro.com", "Anjali", "Verma", "User", "wipro", "Pune", "Project B", "Team 2");

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("Test Users Created Successfully!");
            Console.WriteLine("===========================================");
            Console.WriteLine("Infosys Users (Password: Admin123):");
            Console.WriteLine("  1. admin@infosys.com (Admin)");
            Console.WriteLine("  2. pavan@infosys.com (User)");
            Console.WriteLine("  3. ravi@infosys.com (User)");
            Console.WriteLine("  4. priya@infosys.com (User)");
            Console.WriteLine("  5. amit@infosys.com (User)");
            Console.WriteLine("");
            Console.WriteLine("Wipro Users (Password: Admin123):");
            Console.WriteLine("  1. admin@wipro.com (Admin)");
            Console.WriteLine("  2. sanjay@wipro.com (User)");
            Console.WriteLine("  3. neha@wipro.com (User)");
            Console.WriteLine("  4. vikram@wipro.com (User)");
            Console.WriteLine("  5. anjali@wipro.com (User)");
            Console.WriteLine("===========================================");
        }

        private static async Task SeedUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string firstName,
            string lastName,
            string role,
            string tenantId,
            string? location = null,
            string? projectName = null,
            string? team = null)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                Console.WriteLine($"⚠ User already exists: {email}");
                return; // User already exists
            }

            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
                TenantId = tenantId,
                Location = location,
                ProjectName = projectName,
                Team = team,
                CreatedAt = DateTime.UtcNow,
                RegisterDate = DateTime.UtcNow,
                IsActive = true,
                EmailConfirmed = true,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };

            // Password: Admin123
            var result = await userManager.CreateAsync(user, "Admin123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                Console.WriteLine($"✓ Created user: {email} ({firstName} {lastName}) - Role: {role} - Tenant: {tenantId}");
            }
            else
            {
                Console.WriteLine($"✗ Failed to create user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}

