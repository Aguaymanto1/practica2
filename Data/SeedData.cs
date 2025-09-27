using Microsoft.AspNetCore.Identity;

namespace practica2.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            // Crear rol Broker si no existe
            if (!await roleManager.RoleExistsAsync("Broker"))
            {
                await roleManager.CreateAsync(new IdentityRole("Broker"));
            }

            // Crear usuario broker de ejemplo
            var email = "broker@demo.com";
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                await userManager.CreateAsync(user, "Broker123!");
                await userManager.AddToRoleAsync(user, "Broker");
            }
        }
    }
}
