using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using KosovaDoganaModerne.Te_dhenat;
using KosovaDoganaModerne.Depo;
using KosovaDoganaModerne.Sherbime;
using KosovaDoganaModerne.Modelet.Entitetet;
using System.Text.Json.Serialization;
using Serilog;
using Microsoft.AspNetCore.HttpOverrides;

// Configure Serilog FIRST before anything else to capture all startup logs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/startup-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

Log.Information("🚀 Starting Kosova Dogana Moderne application...");

try
{
var builder = WebApplication.CreateBuilder(args);

// Use Serilog for all logging
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/application-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 52428800,
        rollOnFileSizeLimit: true)
    .WriteTo.File(
        path: "logs/errors-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 90,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
        fileSizeLimitBytes: 52428800,
        rollOnFileSizeLimit: true));

Log.Information($"Environment: {builder.Environment.EnvironmentName}");
Log.Information($"ContentRootPath: {builder.Environment.ContentRootPath}");

// Configure Kestrel ONLY for Development environment
// In Production, IIS handles all hosting (no ports needed)
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(5000); // HTTP for development
        serverOptions.ListenAnyIP(5001, listenOptions =>
        {
            listenOptions.UseHttps(); // HTTPS with development certificate
        });
    });
    
    // Configure HTTPS redirection only for development
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
        options.HttpsPort = 5001;
    });
    
    Log.Information("🔧 Development mode: Kestrel listening on ports 5000 (HTTP) and 5001 (HTTPS)");
}
else
{
    // Production: IIS handles everything, no Kestrel configuration needed
    // Configure Forwarded Headers for IIS proxy scenarios
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });
    
    // HTTPS redirection handled by IIS bindings
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
        // Don't specify HttpsPort - let IIS handle it via bindings
    });
    
    Log.Information("🚀 Production mode: IIS hosting (no Kestrel ports)");
}

// Configure HSTS for production
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

// Shto shërbimet për MVC dhe Razor Pages
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Fix JSON circular reference errors
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddRazorPages(options =>
{
    // Configure Identity Area pages
    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
});

// Konfiguro bazën e të dhënave
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Log.Information($"Database connection string: {connectionString}");

builder.Services.AddDbContext<AplikacioniDbKonteksti>(options =>
{
    options.UseSqlite(connectionString);
    
    // Suppress PendingModelChangesWarning in Production
    // This allows the application to start even if migrations are pending
    // In Production, migrations should be applied separately before deployment
    if (!builder.Environment.IsDevelopment())
    {
        options.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        
        Log.Warning("⚠️ PendingModelChangesWarning suppressed in Production - ensure migrations are applied");
    }
});

// Shto Identity për autentifikim
builder.Services.AddDefaultIdentity<Perdoruesi>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    // Enable 2FA
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AplikacioniDbKonteksti>()
.AddDefaultTokenProviders(); // Required for 2FA

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
builder.Services.AddScoped<SherbimetPrintimit>();
builder.Services.AddScoped<SherbimetPDF>();

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

Log.Information("Application built successfully, configuring middleware pipeline...");

// Configure path base for IIS virtual directory deployment BEFORE any middleware
// This is critical for IIS virtual directory hosting
var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
    Log.Information($"✓ PathBase configured: {pathBase}");
    
    if (app.Environment.IsDevelopment())
    {
        Log.Information($"  Application available at:");
        Log.Information($"    HTTP:  http://localhost:5000{pathBase}");
        Log.Information($"    HTTPS: https://localhost:5001{pathBase}");
        Log.Information($"    Network: http://10.10.173.154:5000{pathBase}");
    }
    else
    {
        Log.Information($"  IIS Virtual Directory: {pathBase}");
        Log.Information($"  Application available at:");
        Log.Information($"    HTTP:  http://10.10.173.154{pathBase}");
        Log.Information($"    HTTPS: https://10.10.173.154{pathBase} (if SSL configured in IIS)");
    }
}
else
{
    Log.Information("ℹ PathBase not configured - application will run on root path");
}

// Configure Forwarded Headers for IIS (must be early in pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    Log.Information("✓ Forwarded Headers middleware enabled for IIS");
}

// Use detailed error pages in Production when configured
var showDetailedErrors = builder.Configuration.GetValue<bool>("ApplicationSettings:ShowDetailedErrors", false);
if (!app.Environment.IsDevelopment())
{
    if (showDetailedErrors)
    {
        Log.Warning("⚠️ DETAILED ERROR PAGES ENABLED IN PRODUCTION - This should only be temporary for debugging!");
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }
    app.UseHsts(); // Use HTTP Strict Transport Security in production
}
else
{
    app.UseDeveloperExceptionPage();
}

// Redirect HTTP to HTTPS only if configured
if (!app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("UseHttpsRedirection", true))
{
    app.UseHttpsRedirection();
}

// Use Serilog request logging
app.UseSerilogRequestLogging();

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

Log.Information("Middleware pipeline configured, initializing database...");

// Krijo bazën e të dhënave nëse nuk ekziston
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AplikacioniDbKonteksti>();
        var userManager = services.GetRequiredService<UserManager<Perdoruesi>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        Log.Information("Applying database migrations...");
        
        // Apply any pending migrations - this will create the database if it doesn't exist
        // and update it if needed, WITHOUT deleting existing data
        await context.Database.MigrateAsync();
        Log.Information("✓ Database created/updated successfully");
        
        // Krijo rolet nëse nuk ekzistojnë
        foreach (var role in RoletSistemit.TeGjithaRolet)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Log.Information($"✓ Role '{role}' created");
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
                Log.Information($"✓ Admin user created: {adminEmail}");
                Log.Information($"   Username: {adminEmail}");
                Log.Information($"   Password: Admin@123");
            }
            else
            {
                Log.Error($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // Seed të dhënat fillestare vetëm nëse tabela është bosh
        await DbSeeder.SeedAsync(context);
        
        Log.Information("🎉 System is ready for use! 🎉");
        
        if (app.Environment.IsDevelopment())
        {
            Log.Information($"\n📍 Access application (Development):");
            if (!string.IsNullOrEmpty(pathBase))
            {
                Log.Information($"   - Local: http://localhost:5000{pathBase}");
                Log.Information($"   - Network: http://10.10.173.154:5000{pathBase}");
            }
            else
            {
                Log.Information($"   - Local: http://localhost:5000");
                Log.Information($"   - Network: http://10.10.173.154:5000");
            }
        }
        else
        {
            Log.Information($"\n📍 Access application (IIS Production):");
            if (!string.IsNullOrEmpty(pathBase))
            {
                Log.Information($"   - HTTP:  http://10.10.173.154{pathBase}");
                Log.Information($"   - HTTPS: https://10.10.173.154{pathBase}");
            }
            else
            {
                Log.Information($"   - HTTP:  http://10.10.173.154");
                Log.Information($"   - HTTPS: https://10.10.173.154");
            }
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ CRITICAL ERROR during database initialization");
        Log.Error($"Connection String: {connectionString}");
        Log.Error($"Content Root: {app.Environment.ContentRootPath}");
        
        // Try to get more specific database error information
        if (ex.InnerException != null)
        {
            Log.Error(ex.InnerException, "Inner exception details");
        }
        
        // Don't crash the app, but make sure error is visible
        throw;
    }
}

Log.Information("Starting application...");
app.Run();
Log.Information("Application stopped.");

}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
