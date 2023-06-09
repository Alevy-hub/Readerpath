using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Readerpath.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
})
    .AddRoles<IdentityRole>() // Dodaj us?ug? zarz?dzania rolami
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Index";
});

builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var adminRoleExists = await roleManager.RoleExistsAsync("Admin");
    var editorRoleExists = await roleManager.RoleExistsAsync("Editor");

    if (!adminRoleExists)
    {
        var adminRole = new IdentityRole("Admin");
        await roleManager.CreateAsync(adminRole);
    }

    if (!editorRoleExists)
    {
        var editorRole = new IdentityRole("Editor");
        await roleManager.CreateAsync(editorRole);
    }
}


app.Run();
