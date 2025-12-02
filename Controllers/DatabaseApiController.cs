using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Text;

namespace KosovaDoganaModerne.Controllers
{
    [ApiController]
    [Route("api/database")]
    public class DatabaseApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DatabaseApiController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Data Source=KosovaDoganaModerne.db";
        }

        /// <summary>
        /// Merr listën e të gjitha tabelave në bazën e të dhënave
        /// </summary>
        [HttpGet("tables")]
        public async Task<IActionResult> GetTables()
        {
            try
            {
                var tables = new List<string>();
                
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        SELECT name FROM sqlite_master 
                        WHERE type='table' 
                        AND name NOT LIKE 'sqlite_%'
                        AND name NOT LIKE '__EFMigrationsHistory'
                        ORDER BY name;
                    ";
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                }
                
                return Ok(tables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Gabim gjatë ngarkimit të tabelave: {ex.Message}" });
            }
        }

        /// <summary>
        /// Merr informacion për një tabelë specifike
        /// </summary>
        [HttpGet("table-info/{tableName}")]
        public async Task<IActionResult> GetTableInfo(string tableName)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Merr numrin e rreshtave
                    var countCommand = connection.CreateCommand();
                    countCommand.CommandText = $"SELECT COUNT(*) FROM [{tableName}]";
                    var rowCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                    
                    // Merr informacion për kolonat
                    var columnsCommand = connection.CreateCommand();
                    columnsCommand.CommandText = $"PRAGMA table_info([{tableName}])";
                    
                    var columns = new List<string>();
                    using (var reader = await columnsCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            columns.Add(reader.GetString(1)); // Column name
                        }
                    }
                    
                    return Ok(new
                    {
                        tableName,
                        rowCount,
                        columnCount = columns.Count,
                        columns
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Merr të dhënat e një tabele me pagination
        /// </summary>
        [HttpGet("table-data/{tableName}")]
        public async Task<IActionResult> GetTableData(string tableName, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Merr numrin total të rreshtave
                    var countCommand = connection.CreateCommand();
                    countCommand.CommandText = $"SELECT COUNT(*) FROM [{tableName}]";
                    var totalRows = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                    
                    // Merr të dhënat me pagination
                    var dataCommand = connection.CreateCommand();
                    var offset = (page - 1) * pageSize;
                    dataCommand.CommandText = $"SELECT * FROM [{tableName}] LIMIT {pageSize} OFFSET {offset}";
                    
                    var rows = new List<Dictionary<string, object?>>();
                    var columns = new List<string>();
                    
                    using (var reader = await dataCommand.ExecuteReaderAsync())
                    {
                        // Merr emrat e kolonave
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns.Add(reader.GetName(i));
                        }
                        
                        // Merr të dhënat
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object?>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                row[columns[i]] = value;
                            }
                            rows.Add(row);
                        }
                    }
                    
                    return Ok(new
                    {
                        tableName,
                        columns,
                        rows,
                        totalRows,
                        page,
                        pageSize,
                        totalPages = (int)Math.Ceiling((double)totalRows / pageSize)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Ekzekuton një SQL query (vetëm SELECT)
        /// </summary>
        [HttpPost("execute-query")]
        public async Task<IActionResult> ExecuteQuery([FromBody] QueryRequest request)
        {
            try
            {
                // Kontrollo që query është vetëm SELECT
                var query = request.Query.Trim();
                if (!query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { error = "Vetëm query-të SELECT lejohen për siguri." });
                }
                
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    var command = connection.CreateCommand();
                    command.CommandText = query;
                    
                    var rows = new List<Dictionary<string, object?>>();
                    var columns = new List<string>();
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Merr emrat e kolonave
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns.Add(reader.GetName(i));
                        }
                        
                        // Merr të dhënat
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object?>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                row[columns[i]] = value;
                            }
                            rows.Add(row);
                        }
                    }
                    
                    return Ok(new
                    {
                        columns,
                        rows,
                        rowCount = rows.Count
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Eksporton një tabelë në CSV
        /// </summary>
        [HttpGet("export-table/{tableName}")]
        public async Task<IActionResult> ExportTable(string tableName)
        {
            try
            {
                var csv = new StringBuilder();
                
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT * FROM [{tableName}]";
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Headers
                        var columns = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns.Add(reader.GetName(i));
                        }
                        csv.AppendLine(string.Join(",", columns.Select(c => $"\"{c}\"")));
                        
                        // Rows
                        while (await reader.ReadAsync())
                        {
                            var values = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString();
                                values.Add($"\"{value?.Replace("\"", "\"\"")}\"");
                            }
                            csv.AppendLine(string.Join(",", values));
                        }
                    }
                }
                
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"{tableName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Eksporton të gjitha tabelat në një ZIP file
        /// </summary>
        [HttpGet("export-all")]
        public async Task<IActionResult> ExportAllTables()
        {
            try
            {
                var tables = new List<string>();
                
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        SELECT name FROM sqlite_master 
                        WHERE type='table' 
                        AND name NOT LIKE 'sqlite_%'
                        AND name NOT LIKE '__EFMigrationsHistory'
                        ORDER BY name;
                    ";
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                }
                
                // Create a ZIP file with all tables exported as CSV
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                    {
                        foreach (var tableName in tables)
                        {
                            var csv = new StringBuilder();
                            
                            using (var connection = new SqliteConnection(_connectionString))
                            {
                                await connection.OpenAsync();
                                
                                var command = connection.CreateCommand();
                                command.CommandText = $"SELECT * FROM [{tableName}]";
                                
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    // Headers
                                    var columns = new List<string>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        columns.Add(reader.GetName(i));
                                    }
                                    csv.AppendLine(string.Join(",", columns.Select(c => $"\"{c}\"")));
                                    
                                    // Rows
                                    while (await reader.ReadAsync())
                                    {
                                        var values = new List<string>();
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            var value = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString();
                                            values.Add($"\"{value?.Replace("\"", "\"\"")}\"");
                                        }
                                        csv.AppendLine(string.Join(",", values));
                                    }
                                }
                            }
                            
                            // Add CSV file to archive
                            var zipEntry = archive.CreateEntry($"{tableName}.csv", System.IO.Compression.CompressionLevel.Optimal);
                            using (var entryStream = zipEntry.Open())
                            using (var streamWriter = new StreamWriter(entryStream, Encoding.UTF8))
                            {
                                await streamWriter.WriteAsync(csv.ToString());
                            }
                        }
                    }
                    
                    memoryStream.Position = 0;
                    return File(memoryStream.ToArray(), "application/zip", $"DatabaseExport_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public class QueryRequest
        {
            public string Query { get; set; } = string.Empty;
        }
    }
}
