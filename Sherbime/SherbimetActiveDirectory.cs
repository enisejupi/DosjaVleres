using System.DirectoryServices;
using System.Globalization;

namespace KosovaDoganaModerne.Sherbime
{
    public class SherbimetActiveDirectory
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SherbimetActiveDirectory> _logger;

        public SherbimetActiveDirectory(IConfiguration configuration, ILogger<SherbimetActiveDirectory> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Autentifikon përdoruesin direkt në LDAP server
        /// </summary>
        public async Task<(bool Success, ADUserInfo? UserInfo)> AutontifikoNeActiveDirectory(string username, string password)
        {
            try
            {
                // Kontrollo nëse AD është aktivizuar
                var adEnabled = bool.Parse(_configuration["ActiveDirectory:Enabled"] ?? "false");
                if (!adEnabled)
                {
                    _logger.LogWarning("Active Directory është çaktivizuar. Duke lejuar autentifikimin pa verifikim AD për: {Username}", username);
                    return (true, new ADUserInfo
                    {
                        SamAccountName = username,
                        DisplayName = username,
                        Email = $"{username}@dogana.rks"
                    });
                }

                var ldapPath = _configuration["ActiveDirectory:LdapPath"];
                var domain = _configuration["ActiveDirectory:Domain"];

                if (string.IsNullOrEmpty(ldapPath) || string.IsNullOrEmpty(domain))
                {
                    _logger.LogError("Active Directory nuk është konfiguruar siç duhet.");
                    return (false, null);
                }

                // Nëse përdoruesi ka dhënë email, përdor vetëm pjesën para @
                string user = username.Trim();
                
                // Nëse është email format, merr vetëm username pjesën
                if (user.Contains("@"))
                {
                    user = user.Split('@')[0];
                }
                
                // Përdor Title Case për emrin e përdoruesit
                string newUser = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user);
                string domainUser = $"{domain}\\{newUser}";

                _logger.LogInformation("Duke tentuar autentifikimin për: {DomainUser}", domainUser);

                // Provo të autentifikosh direkt me kredencialet e përdoruesit
                bool authenticated = await Task.Run(() => AutentifikoPerdoruesin(ldapPath, domainUser, password.Trim()));

                if (authenticated)
                {
                    // Merr informacionin e përdoruesit nga LDAP
                    var userInfo = await Task.Run(() => MerrInformacioninPerdoruesit(ldapPath, domainUser, password.Trim(), newUser));
                    
                    _logger.LogInformation("Autentifikimi i suksesshëm për përdoruesin: {Username}", newUser);
                    return (true, userInfo);
                }

                _logger.LogWarning("Autentifikimi dështoi për përdoruesin: {Username}", username);
                return (false, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gabim gjatë autentifikimit në Active Directory për përdoruesin: {Username}", username);
                return (false, null);
            }
        }

        /// <summary>
        /// Autentifikon përdoruesin në LDAP duke përdorur DirectoryEntry (sikur te kolegu)
        /// </summary>
        private bool AutentifikoPerdoruesin(string path, string user, string pass)
        {
            DirectoryEntry de = new DirectoryEntry(path, user, pass, AuthenticationTypes.Secure);
            try
            {
                DirectorySearcher ds = new DirectorySearcher(de);
                ds.FindOne();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Autentifikimi dështoi për: {User} - {Message}", user, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Merr informacionin e përdoruesit nga LDAP
        /// </summary>
        private ADUserInfo MerrInformacioninPerdoruesit(string path, string user, string pass, string username)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry(path, user, pass, AuthenticationTypes.Secure);
                DirectorySearcher ds = new DirectorySearcher(de)
                {
                    Filter = $"(sAMAccountName={username})"
                };
                
                ds.PropertiesToLoad.Add("mail");
                ds.PropertiesToLoad.Add("displayName");
                ds.PropertiesToLoad.Add("department");
                ds.PropertiesToLoad.Add("title");
                ds.PropertiesToLoad.Add("sAMAccountName");
                ds.PropertiesToLoad.Add("givenName");
                ds.PropertiesToLoad.Add("sn");

                SearchResult result = ds.FindOne();
                
                if (result != null)
                {
                    return new ADUserInfo
                    {
                        Email = GetProperty(result, "mail"),
                        DisplayName = GetProperty(result, "displayName"),
                        Department = GetProperty(result, "department"),
                        Title = GetProperty(result, "title"),
                        SamAccountName = GetProperty(result, "sAMAccountName") ?? username,
                        FirstName = GetProperty(result, "givenName"),
                        LastName = GetProperty(result, "sn")
                    };
                }

                // Nëse nuk gjenden detaje, kthe informacion minimal
                return new ADUserInfo
                {
                    SamAccountName = username,
                    DisplayName = username,
                    Email = $"{username}@dogana.rks"
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Nuk mund të merrte informacionin për përdoruesin: {Username}", username);
                
                // Kthe informacion minimal në rast gabimi
                return new ADUserInfo
                {
                    SamAccountName = username,
                    DisplayName = username,
                    Email = $"{username}@dogana.rks"
                };
            }
        }

        /// <summary>
        /// Merr vlerën e një properti nga SearchResult
        /// </summary>
        private string? GetProperty(SearchResult result, string propertyName)
        {
            try
            {
                if (result.Properties.Contains(propertyName))
                {
                    var prop = result.Properties[propertyName];
                    if (prop.Count > 0)
                    {
                        return prop[0]?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Nuk mund të merrte property {PropertyName}: {Error}", propertyName, ex.Message);
            }

            return null;
        }
    }

    public class ADUserInfo
    {
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string? Department { get; set; }
        public string? Title { get; set; }
        public string? SamAccountName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
