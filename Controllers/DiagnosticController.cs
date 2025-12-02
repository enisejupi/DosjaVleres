using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KosovaDoganaModerne.Depo;
using KosovaDoganaModerne.Te_dhenat;
using System.Reflection;
using System.Security.Principal;
using System.IO;
using System.Text;

namespace KosovaDoganaModerne.Controllers
{
    /// <summary>
    /// Controller për diagnostikimin e problemeve me routing dhe konfigurimin
    /// </summary>
    [Route("Diagnostic")]
    public class DiagnosticController : Controller
    {
        private readonly IDepoja_VleraProduktit _depoja;
        private readonly IConfiguration _configuration;
        private readonly AplikacioniDbKonteksti _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DiagnosticController> _logger;

        public DiagnosticController(
            IDepoja_VleraProduktit depoja, 
            IConfiguration configuration,
            AplikacioniDbKonteksti context,
            IWebHostEnvironment environment,
            ILogger<DiagnosticController> logger)
        {
            _depoja = depoja;
            _configuration = configuration;
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Faqja e diagnostikimit që tregon informacionin e sistemit
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var diagnosticInfo = new Dictionary<string, object>
            {
                ["ApplicationName"] = "Kosova Dogana Moderne",
                ["Version"] = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown",
                ["Environment"] = _environment.EnvironmentName,
                ["PathBase"] = _configuration["PathBase"] ?? "(not set)",
                ["CurrentPath"] = Request.Path.ToString(),
                ["CurrentUrl"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}{Request.QueryString}",
                ["ServerTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["IsAuthenticated"] = User.Identity?.IsAuthenticated ?? false,
                ["UserName"] = User.Identity?.Name ?? "Anonymous",
                
                ["RouteInfo"] = new Dictionary<string, string>
                {
                    ["BaseUrl"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}",
                    ["ExpectedHistoriaUrl"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/VleratProdukteve/Historia/1",
                    ["ExpectedIndexUrl"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/VleratProdukteve/Index"
                },
                
                ["Configuration"] = new Dictionary<string, string>
                {
                    ["PathBase"] = _configuration["PathBase"] ?? "(not set)",
                    ["ConnectionString"] = _configuration.GetConnectionString("DefaultConnection")?.Substring(0, Math.Min(50, _configuration.GetConnectionString("DefaultConnection")?.Length ?? 0)) + "...",
                    ["MachineName"] = Environment.MachineName,
                    ["ProcessorCount"] = Environment.ProcessorCount.ToString(),
                    ["OSVersion"] = Environment.OSVersion.ToString(),
                    ["ContentRootPath"] = _environment.ContentRootPath,
                    ["WebRootPath"] = _environment.WebRootPath
                },

                ["DatabaseStatus"] = await GetDatabaseStatus(),
                ["FilePermissions"] = CheckFilePermissions(),
                ["MigrationStatus"] = await GetMigrationStatus()
            };
            
            return Json(diagnosticInfo);
        }

        /// <summary>
        /// Comprehensive system health check
        /// </summary>
        [HttpGet("Health")]
        public async Task<IActionResult> Health()
        {
            var health = new Dictionary<string, object>();
            var overallHealthy = true;

            try
            {
                // Database connectivity
                var canConnect = await _context.Database.CanConnectAsync();
                health["DatabaseConnectivity"] = new
                {
                    Status = canConnect ? "Healthy" : "Unhealthy",
                    CanConnect = canConnect
                };
                if (!canConnect) overallHealthy = false;

                // Database file exists
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var dbPath = GetDatabasePath(connectionString);
                var dbExists = System.IO.File.Exists(dbPath);
                health["DatabaseFile"] = new
                {
                    Status = dbExists ? "Healthy" : "Unhealthy",
                    Path = dbPath,
                    Exists = dbExists,
                    FullPath = Path.GetFullPath(dbPath),
                    Size = dbExists ? new FileInfo(dbPath).Length : 0
                };
                if (!dbExists) overallHealthy = false;

                // Pending migrations
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                var hasPendingMigrations = pendingMigrations.Any();
                health["Migrations"] = new
                {
                    Status = hasPendingMigrations ? "Warning" : "Healthy",
                    PendingCount = pendingMigrations.Count(),
                    PendingMigrations = pendingMigrations.ToList()
                };

                // Data accessibility
                var productCount = await _context.VleratProdukteve.CountAsync();
                health["DataAccess"] = new
                {
                    Status = "Healthy",
                    ProductCount = productCount
                };

                // File permissions
                var permissions = CheckFilePermissions();
                health["FilePermissions"] = permissions;
                
            }
            catch (Exception ex)
            {
                overallHealthy = false;
                health["Error"] = new
                {
                    Status = "Unhealthy",
                    Message = ex.Message,
                    Type = ex.GetType().Name,
                    StackTrace = ex.StackTrace
                };
                _logger.LogError(ex, "Health check failed");
            }

            Response.StatusCode = overallHealthy ? 200 : 503;
            health["OverallStatus"] = overallHealthy ? "Healthy" : "Unhealthy";
            health["CheckTime"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            return Json(health);
        }

        /// <summary>
        /// Teston nëse mund të qasemi në bazën e të dhënave
        /// </summary>
        [HttpGet("TestDatabase")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Cannot connect to database",
                        ConnectionString = _configuration.GetConnectionString("DefaultConnection")
                    });
                }

                var vlerat = await _depoja.MerrTeGjitha();
                var count = vlerat.Count();
                
                return Json(new
                {
                    Success = true,
                    Message = "Database connection is working",
                    ProductCount = count,
                    SampleProducts = vlerat.Take(3).Select(v => new
                    {
                        v.Id,
                        v.KodiProduktit,
                        v.EmriProduktit,
                        HistoriaUrl = Url.Action("Historia", "VleratProdukteve", new { id = v.Id })
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database test failed");
                return Json(new
                {
                    Success = false,
                    Message = "Error connecting to database",
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Liston të gjitha rrugët e regjistruara
        /// </summary>
        [HttpGet("Routes")]
        public IActionResult Routes()
        {
            var routes = new List<string>
            {
                "/",
                "/Home/Index",
                "/VleratProdukteve/Index",
                "/VleratProdukteve/Detajet/{id}",
                "/VleratProdukteve/Historia/{id}",
                "/VleratProdukteve/Krijo",
                "/VleratProdukteve/Perditeso/{id}",
                "/VleratProdukteve/Fshi/{id}",
                "/VleratProdukteve/Print/{id}",
                "/KomentetDegeve/Index",
                "/Diagnostic/Index",
                "/Diagnostic/Health",
                "/Diagnostic/TestDatabase",
                "/Diagnostic/Routes",
                "/Diagnostic/Logs"
            };

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var fullUrls = routes.Select(r => baseUrl + r).ToList();

            return Json(new
            {
                BaseUrl = baseUrl,
                PathBase = Request.PathBase.ToString(),
                RegisteredRoutes = routes,
                FullUrls = fullUrls,
                TestLinks = new
                {
                    Home = baseUrl,
                    VleratProdukteve = baseUrl + "/VleratProdukteve/Index",
                    Historia_Sample = baseUrl + "/VleratProdukteve/Historia/1",
                    Diagnostic = baseUrl + "/Diagnostic/Index",
                    Health = baseUrl + "/Diagnostic/Health"
                }
            });
        }

        /// <summary>
        /// Teston një ID specifik për Historia
        /// </summary>
        [HttpGet("TestHistoria/{id}")]
        public async Task<IActionResult> TestHistoria(int id)
        {
            try
            {
                var vlera = await _depoja.MerrSipasID(id);
                
                if (vlera == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = $"Product with ID {id} not found in database",
                        AvailableIds = (await _depoja.MerrTeGjitha()).Take(10).Select(v => v.Id).ToList()
                    });
                }

                var historia = await _depoja.MerrHistorine(id);
                
                return Json(new
                {
                    Success = true,
                    ProductId = id,
                    ProductName = vlera.EmriProduktit,
                    ProductCode = vlera.KodiProduktit,
                    HistoryCount = historia.Count(),
                    HistoriaUrl = Url.Action("Historia", "VleratProdukteve", new { id }),
                    FullHistoriaUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/VleratProdukteve/Historia/{id}",
                    Message = "Product exists and has history"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TestHistoria failed for ID {Id}", id);
                return Json(new
                {
                    Success = false,
                    Message = "Error checking product",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// View recent application logs
        /// </summary>
        [HttpGet("Logs")]
        public IActionResult Logs([FromQuery] int lines = 100, [FromQuery] string type = "application")
        {
            try
            {
                var logPath = Path.Combine(_environment.ContentRootPath, "logs");
                
                if (!Directory.Exists(logPath))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Logs directory does not exist",
                        Path = logPath
                    });
                }

                var pattern = type.ToLower() switch
                {
                    "error" => "errors-*.log",
                    "startup" => "startup-*.log",
                    _ => "application-*.log"
                };

                var logFiles = Directory.GetFiles(logPath, pattern)
                    .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                    .ToList();

                if (!logFiles.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = $"No log files found matching pattern: {pattern}",
                        LogPath = logPath,
                        AvailableFiles = Directory.GetFiles(logPath).Select(Path.GetFileName).ToList()
                    });
                }

                var latestLog = logFiles.First();
                var logLines = System.IO.File.ReadLines(latestLog)
                    .Reverse()
                    .Take(lines)
                    .Reverse()
                    .ToList();

                return Json(new
                {
                    Success = true,
                    LogFile = Path.GetFileName(latestLog),
                    LogPath = latestLog,
                    LastModified = new FileInfo(latestLog).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Lines = logLines.Count,
                    Content = string.Join(Environment.NewLine, logLines)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read logs");
                return Json(new
                {
                    Success = false,
                    Message = "Error reading logs",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Shfaq një HTML faqe ndihmëse me linqe për testim
        /// </summary>
        [HttpGet("Help")]
        public IActionResult Help()
        {
            return View();
        }

        // Helper methods
        private async Task<object> GetDatabaseStatus()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var dbPath = GetDatabasePath(connectionString);

                return new
                {
                    CanConnect = canConnect,
                    ConnectionString = connectionString,
                    DatabasePath = dbPath,
                    DatabaseExists = System.IO.File.Exists(dbPath),
                    FullPath = Path.GetFullPath(dbPath)
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                };
            }
        }

        private object CheckFilePermissions()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var dbPath = GetDatabasePath(connectionString);
                var dbDirectory = Path.GetDirectoryName(dbPath) ?? _environment.ContentRootPath;

                var checks = new Dictionary<string, object>
                {
                    ["ContentRoot"] = new
                    {
                        Path = _environment.ContentRootPath,
                        Exists = Directory.Exists(_environment.ContentRootPath),
                        CanWrite = CanWriteToDirectory(_environment.ContentRootPath)
                    },
                    ["DatabaseDirectory"] = new
                    {
                        Path = dbDirectory,
                        Exists = Directory.Exists(dbDirectory),
                        CanWrite = CanWriteToDirectory(dbDirectory)
                    }
                };

                if (System.IO.File.Exists(dbPath))
                {
                    checks["DatabaseFile"] = new
                    {
                        Path = dbPath,
                        Exists = true,
                        CanRead = CanReadFile(dbPath),
                        CanWrite = CanWriteFile(dbPath),
                        Size = new FileInfo(dbPath).Length
                    };
                }

                return checks;
            }
            catch (Exception ex)
            {
                return new { Error = ex.Message };
            }
        }

        private async Task<object> GetMigrationStatus()
        {
            try
            {
                var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

                return new
                {
                    AppliedCount = appliedMigrations.Count(),
                    PendingCount = pendingMigrations.Count(),
                    AppliedMigrations = appliedMigrations.ToList(),
                    PendingMigrations = pendingMigrations.ToList()
                };
            }
            catch (Exception ex)
            {
                return new { Error = ex.Message };
            }
        }

        private string GetDatabasePath(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return "Unknown";

            var dataSourceMatch = System.Text.RegularExpressions.Regex.Match(
                connectionString, 
                @"Data Source=([^;]+)", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (dataSourceMatch.Success)
            {
                var dbPath = dataSourceMatch.Groups[1].Value;
                if (!Path.IsPathRooted(dbPath))
                {
                    dbPath = Path.Combine(_environment.ContentRootPath, dbPath);
                }
                return dbPath;
            }

            return "Unknown";
        }

        private bool CanWriteToDirectory(string path)
        {
            try
            {
                var testFile = Path.Combine(path, $".permission_test_{Guid.NewGuid()}.tmp");
                System.IO.File.WriteAllText(testFile, "test");
                System.IO.File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CanReadFile(string path)
        {
            try
            {
                using var fs = System.IO.File.OpenRead(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CanWriteFile(string path)
        {
            try
            {
                using var fs = System.IO.File.OpenWrite(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
