using Rotativa.AspNetCore;
using gestionpaises.Data;
using gestionpaises.Models;
using gestionpaises.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Conexión a MySQL ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// --- Identity con roles habilitados ---
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// --- Reemplaza el SignInManager default por el personalizado ---
builder.Services.AddScoped<SignInManager<ApplicationUser>, CustomSignInManager>();

builder.Services.AddScoped<ImageValidationService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnValidatePrincipal = async context =>
    {
        var userManager = context.HttpContext.RequestServices
            .GetRequiredService<UserManager<ApplicationUser>>();

        var sessionIdEnCookie = context.Principal?.FindFirst("SessionId")?.Value;
        var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            var usuario = await userManager.FindByIdAsync(userId);

            if (usuario == null || usuario.SessionId != sessionIdEnCookie)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            }
        }
    };
});

var app = builder.Build();

// --- Asegurar base de datos y sembrar roles/usuario administrador ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Consulta", "Editor", "Administrador" };
    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            await roleManager.CreateAsync(new IdentityRole(rol));
        }
    }

    string adminEmail = "admin@gestionpaises.com";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Nombre = "Administrador",
            Apellido1 = "Sistema",
            Apellido2 = ""
        };

        var resultado = await userManager.CreateAsync(adminUser, adminPassword);

        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Administrador");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "countryLanguage",
    pattern: "CountryLanguage/{action}/{countryCode}/{language}",
    defaults: new { controller = "CountryLanguage" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

// --- Configuración de Rotativa ---
RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

app.Run();