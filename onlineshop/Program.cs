using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using onlineshop.Context;
using onlineshop.Helper;
using onlineshop.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<OnlineShopDb>(option =>
{
    option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});
builder.Services.AddIdentity<T_User, IdentityRole>().AddEntityFrameworkStores<OnlineShopDb>();
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = "/Login";
    option.AccessDeniedPath = "/AccessDenied";
});
builder.Services.AddCustomService();
builder.Services.AddHttpClient();
builder.Services.AddSession(option=>
{
    option.IdleTimeout = TimeSpan.FromMinutes(5);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    using (var scope = app.Services.CreateScope())  // The 'using' ensures proper disposal
    {
        var services = scope.ServiceProvider;
        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.AdminSeedAsync();  // Seed data
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
