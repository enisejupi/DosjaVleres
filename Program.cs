using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using KosovaDoganaModerne.Te_dhenat;
using KosovaDoganaModerne.Depo;
using KosovaDoganaModerne.Sherbime;
using KosovaDoganaModerne.Modelet.Entitetet;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all network interfaces for domain access
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Listen on all network interfaces (0.0.0.0) instead of just localhost
    // This allows domain users to access the application
    serverOptions.ListenAnyIP(5000); // HTTP
    serverOptions.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS
    });
});

// Shto shërbimet për MVC dhe Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Konfiguro bazën e të dhënave
builder.Services.AddDbContext<AplikacioniDbKonteksti>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Shto Identity për autentifikim
builder.Services.AddDefaultIdentity<Perdoruesi>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AplikacioniDbKonteksti>();

// Shto shërbimet e sesionit
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Shto HttpContextAccessor për të marrë informacion për kërkesën
builder.Services.AddHttpContextAccessor();

// Regjistro shërbimet custom
builder.Services.AddScoped<IDepoja_VleraProduktit, Depoja_VleraProduktit>();
builder.Services.AddScoped<IDepoja_ShpenzimiTransportit, Depoja_ShpenzimiTransportit>();
builder.Services.AddScoped<IDepoja_KomentiDeges, Depoja_KomentiDeges>();
builder.Services.AddScoped<IDepoja_Dega, Depoja_Dega>();
builder.Services.AddScoped<IDepoja_KerkeseRegjistrim, Depoja_KerkeseRegjistrim>();
builder.Services.AddScoped<SherbimetAuditimit>();
builder.Services.AddScoped<SherbimetActiveDirectory>();
builder.Services.AddScoped<SherbimetRaporteve>();

// Konfiguro lokalizimin (Shqip/Anglisht)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var gjuhetMbeshtetese = new[]
    {
        new CultureInfo("sq-AL"),  // Shqip
        new CultureInfo("en-US")   // Anglisht
    };
    options.DefaultRequestCulture = new RequestCulture("sq-AL");
    options.SupportedCultures = gjuhetMbeshtetese;
    options.SupportedUICultures = gjuhetMbeshtetese;
});

var app = builder.Build();

// Përdor faqen e gabimeve në mënyrën e zhvillimit
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Përdor lokalizimin
app.UseRequestLocalization();

app.UseRouting();

// Përdor sesionet
app.UseSession();

// Përdor autentifikimin dhe autorizimin
app.UseAuthentication();
app.UseAuthorization();

// Konfiguro rrugët
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


// Krijo bazën e të dhënave nëse nuk ekziston
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AplikacioniDbKonteksti>();
        var userManager = services.GetRequiredService<UserManager<Perdoruesi>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine(" ? Baza e të dhënave u krijua me sukses");
        
        // Krijo rolet nëse nuk ekzistojnë
        foreach (var role in RoletSistemit.TeGjithaRolet)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($" ? Roli '{role}' u krijua");
            }
        }
        
        // Krijo përdoruesin admin nëse nuk ekziston
        var adminEmail = "shaban.ejupi@dogana-rks.org";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Perdoruesi
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmriPlote = "Administrator sistemi",
                Departamenti = "IT",
                Pozicioni = "Administrator",
                KodiZyrtarit = "CS-0001",
                EmailConfirmed = true,
                EshteAktiv = true,
                DataKrijimit = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, RoletSistemit.Admin);
                Console.WriteLine($" ? Përdoruesi admin u krijua: {adminEmail}");
                Console.WriteLine($"   Username: {adminEmail}");
                Console.WriteLine($"   Password: Admin@123");
            }
        }

        // Seed të dhënat fillestare
        await DbSeeder.SeedAsync(context);
        
        Console.WriteLine("\n ??? Sistemi është gati për përdorim! ???\n");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Gabim gjatë krijimit të bazës së të dhënave");
    }
}

app.Run();
