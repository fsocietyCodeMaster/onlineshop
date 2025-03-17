using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using onlineshop.Context;
using onlineshop.Models;

namespace onlineshop.Helper
{
    public class DataSeeder
    {
        private readonly UserManager<T_User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DataSeeder> _logger;
        private readonly IConfiguration _configuration;
        public DataSeeder(
            OnlineShopDb context,
            UserManager<T_User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DataSeeder> logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task AdminSeedAsync()
        {
            if (!await _roleManager.RoleExistsAsync("admin"))
            {
                var adminRole = new IdentityRole { Name = "admin" };
                var roleResult = await _roleManager.CreateAsync(adminRole);

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Admin role created successfully.");
                }
                else
                {
                    _logger.LogError("Failed to create admin role: {Errors}", roleResult.Errors);
                    return;
                }
            }

            var adminFullname = _configuration["AdminUser:FullName"];
            var adminEmail = _configuration["AdminUser:Email"];
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new T_User
                {
                    UserName = adminEmail,
                    FullName = adminFullname,
                    PhoneNumber = "09999999999",
                    Email = adminEmail,
                };

                var adminnPassword = _configuration["AdminUser:Password"];

                var userResult = await _userManager.CreateAsync(adminUser, adminnPassword);

                if (userResult.Succeeded)
                {
                    _logger.LogInformation("Admin user created successfully with username {AdminEmail}.", adminFullname);
                    var addToRoleResult = await _userManager.AddToRoleAsync(adminUser,"admin");

                    if (addToRoleResult.Succeeded)
                    {
                        _logger.LogInformation("Admin user assigned to 'admin' role.");
                    }
                    else
                    {
                        _logger.LogError("Failed to assign admin role: {Errors}", addToRoleResult.Errors);
                    }
                }
                else
                {
                    _logger.LogError("Failed to create admin user: {Errors}", userResult.Errors);
                }
            }
            else
            {
                _logger.LogInformation("Admin user already exists.");
            }

        }
    }
}
