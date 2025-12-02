using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Te_dhenat;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using ClosedXML.Excel;

namespace KosovaDoganaModerne.Sherbime
{
    /// <summary>
    /// Shërbim i centralizuar për menaxhimin e printimit, PDF dhe Excel gjenerimit.
    /// Të gjitha operacionet logohen automatikisht për auditim të sigurisë.
    /// </summary>
    public class SherbimetPrintimit
    {
        private readonly AplikacioniDbKonteksti _konteksti;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SherbimetPrintimit> _logger;
        private readonly SherbimetPDF _sherbimetPDF;

        public SherbimetPrintimit(
            AplikacioniDbKonteksti konteksti,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SherbimetPrintimit> logger,
            SherbimetPDF sherbimetPDF)
        {
            _konteksti = konteksti;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _sherbimetPDF = sherbimetPDF;
        }

        #region Merr formatin global të printimit

        /// <summary>
        /// Merr formatin global të printimit për një modul specifik
        /// </summary>
        /// <param name="llojiModulit">Lloji i modulit (VleratProdukteve, KomentetDegeve, etj.)</param>
        /// <returns>Formati i printimit ose null nëse nuk ekziston</returns>
        public async Task<FormatPrintimi?> MerrFormatinGlobal(string llojiModulit)
        {
            try
            {
                var formatGlobal = await _konteksti.GlobalPrintFormats
                    .Include(g => g.FormatPrintimi)
                    .Where(g => g.LlojiModulit == llojiModulit && g.EshteAktiv)
                    .FirstOrDefaultAsync();

                return formatGlobal?.FormatPrintimi;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gabim gjatë marrjes së formatit global për modulin {Moduli}", llojiModulit);
                return null;
            }
        }

        /// <summary>
        /// Kontrollon nëse ekziston një format global për një modul
        /// </summary>
        public async Task<bool> EkzistonFormatGlobal(string llojiModulit)
        {
            return await _konteksti.GlobalPrintFormats
                .AnyAsync(g => g.LlojiModulit == llojiModulit && g.EshteAktiv);
        }

        #endregion

        #region Gjenerimi i përmbajtjes HTML për printim

        /// <summary>
        /// Gjeneron përmbajtjen HTML për printim bazuar në formatin global
        /// </summary>
        public async Task<string> GjeneroPërmbajtjeHTML(string llojiModulit, object data, Dictionary<string, string>? parametra = null)
        {
            try
            {
                var formatGlobal = await MerrFormatinGlobal(llojiModulit);
                
                if (formatGlobal == null)
                {
                    // Përdor format default nëse nuk ka format global
                    return GjeneroHTMLDefault(llojiModulit, data, parametra);
                }

                // Merr template HTML dhe CSS
                var htmlTemplate = formatGlobal.HtmlTemplate;
                var cssStyle = formatGlobal.CssStyle;
                var logoUrl = formatGlobal.LogoUrl ?? "/images/dogana-logo.png";
                var logoPosition = formatGlobal.LogoPosition ?? "center";

                // Ndërto HTML-në finale
                var html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html>");
                html.AppendLine("<head>");
                html.AppendLine("<meta charset='utf-8' />");
                html.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0' />");
                html.AppendLine($"<title>Raport - {llojiModulit}</title>");
                html.AppendLine("<style>");
                html.AppendLine(@"
                    body { font-family: Arial, sans-serif; margin: 20px; }
                    table { width: 100%; border-collapse: collapse; margin-top: 20px; }
                    th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                    th { background-color: #f2f2f2; font-weight: bold; }
                    .header { text-align: center; margin-bottom: 30px; }
                    .logo { max-height: 80px; margin-bottom: 10px; }
                    .footer { margin-top: 40px; padding-top: 20px; border-top: 1px solid #ddd; text-align: center; font-size: 10pt; color: #666; }
                    @media print { 
                        .no-print { display: none; }
                        body { margin: 0; }
                    }
                ");
                if (!string.IsNullOrEmpty(cssStyle))
                {
                    html.AppendLine(cssStyle);
                }
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");

                // Header me logo
                html.AppendLine($"<div class='header' style='text-align: {logoPosition};'>");
                html.AppendLine($"<img src='{logoUrl}' alt='Logo' class='logo' />");
                html.AppendLine("<h1>REPUBLIKA E KOSOVËS</h1>");
                html.AppendLine("<h2>DOGANA E KOSOVËS</h2>");
                html.AppendLine($"<h3>RAPORTI - {llojiModulit}</h3>");
                html.AppendLine($"<p>Data e gjenerimit: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
                html.AppendLine("</div>");

                // Përmbajtja nga data
                var content = await GjeneroTabelenHTML(llojiModulit, data, parametra);
                html.AppendLine(content);

                // Footer
                html.AppendLine("<div class='footer'>");
                html.AppendLine("<p><strong>Dogana e Republikës së Kosovës</strong></p>");
                html.AppendLine("<p>Rruga \"Bill Clinton\" p.n., 10000 Prishtinë, Kosovë</p>");
                html.AppendLine("<p>Tel: +383 38 200 33 000 | Email: info@dogana.rks-gov.net</p>");
                html.AppendLine("</div>");

                html.AppendLine("</body>");
                html.AppendLine("</html>");

                return html.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gabim gjatë gjenerimit të HTML për modulin {Moduli}", llojiModulit);
                throw;
            }
        }

        /// <summary>
        /// Gjeneron tabelën HTML bazuar në llojin e modulit
        /// </summary>
        private async Task<string> GjeneroTabelenHTML(string llojiModulit, object data, Dictionary<string, string>? parametra)
        {
            return llojiModulit switch
            {
                "VleratProdukteve" or "DosjaTeDisponueshme" or "DosjaVlerave" => 
                    GjeneroTabelenVleratProdukteve(data, parametra),
                "KomentetDegeve" or "RaportiKomentetDegeve" => 
                    GjeneroTabelenKomentetDegeve(data, parametra),
                "ShpenzimiTransportit" or "ShpenzimetTransportit" => 
                    GjeneroTabelenShpenzimetTransportit(data, parametra),
                _ => GjeneroHTMLDefault(llojiModulit, data, parametra)
            };
        }

        private string GjeneroTabelenVleratProdukteve(object data, Dictionary<string, string>? parametra)
        {
            var vlerat = data as IEnumerable<VleraProduktit> ?? new List<VleraProduktit>();
            var html = new StringBuilder();

            // Info parameters
            if (parametra != null)
            {
                html.AppendLine("<div class='info-section' style='margin-bottom: 20px;'>");
                foreach (var param in parametra)
                {
                    html.AppendLine($"<p><strong>{param.Key}:</strong> {param.Value}</p>");
                }
                html.AppendLine($"<p><strong>Totali i produkteve:</strong> {vlerat.Count()}</p>");
                html.AppendLine("</div>");
            }

            html.AppendLine("<table>");
            html.AppendLine("<thead><tr>");
            html.AppendLine("<th>Nr.</th><th>Kodi</th><th>Emri</th><th>Përshkrimi</th><th>Vlera</th><th>Njësia</th><th>Origjina</th><th>Kategoria</th>");
            html.AppendLine("</tr></thead>");
            html.AppendLine("<tbody>");

            int nr = 1;
            foreach (var vlera in vlerat)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{nr}</td>");
                html.AppendLine($"<td><strong>{vlera.KodiProduktit}</strong></td>");
                html.AppendLine($"<td>{vlera.EmriProduktit}</td>");
                html.AppendLine($"<td>{(vlera.Pershkrimi.Length > 50 ? vlera.Pershkrimi.Substring(0, 50) + "..." : vlera.Pershkrimi)}</td>");
                html.AppendLine($"<td>{vlera.VleraDoganore:N2} {vlera.Valuta}</td>");
                html.AppendLine($"<td>{vlera.Njesia}</td>");
                html.AppendLine($"<td>{vlera.Origjina}</td>");
                html.AppendLine($"<td>{vlera.Kategoria}</td>");
                html.AppendLine("</tr>");
                nr++;
            }

            html.AppendLine("</tbody>");
            html.AppendLine("</table>");
            return html.ToString();
        }

        private string GjeneroTabelenKomentetDegeve(object data, Dictionary<string, string>? parametra)
        {
            var komentet = data as IEnumerable<KomentiDeges> ?? new List<KomentiDeges>();
            var html = new StringBuilder();

            if (parametra != null)
            {
                html.AppendLine("<div class='info-section' style='margin-bottom: 20px;'>");
                foreach (var param in parametra)
                {
                    html.AppendLine($"<p><strong>{param.Key}:</strong> {param.Value}</p>");
                }
                html.AppendLine($"<p><strong>Totali i komenteve:</strong> {komentet.Count()}</p>");
                html.AppendLine("</div>");
            }

            html.AppendLine("<table>");
            html.AppendLine("<thead><tr>");
            html.AppendLine("<th>Nr.</th><th>Dega</th><th>Kodi Tarifar</th><th>Mesazhi</th><th>Data</th><th>Statusi</th>");
            html.AppendLine("</tr></thead>");
            html.AppendLine("<tbody>");

            int nr = 1;
            foreach (var koment in komentet)
            {
                var statusi = koment.EshteZgjidhur ? "Zgjidhur" : "Në pritje";
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{nr}</td>");
                html.AppendLine($"<td>{koment.EmriDeges}</td>");
                html.AppendLine($"<td><strong>{koment.KodiTarifar}</strong></td>");
                html.AppendLine($"<td>{(koment.Mesazhi.Length > 100 ? koment.Mesazhi.Substring(0, 100) + "..." : koment.Mesazhi)}</td>");
                html.AppendLine($"<td>{koment.DataDergimit:dd/MM/yyyy}</td>");
                html.AppendLine($"<td>{statusi}</td>");
                html.AppendLine("</tr>");
                nr++;
            }

            html.AppendLine("</tbody>");
            html.AppendLine("</table>");
            return html.ToString();
        }

        private string GjeneroTabelenShpenzimetTransportit(object data, Dictionary<string, string>? parametra)
        {
            var shpenzimet = data as IEnumerable<ShpenzimiTransportit> ?? new List<ShpenzimiTransportit>();
            var html = new StringBuilder();

            if (parametra != null)
            {
                html.AppendLine("<div class='info-section' style='margin-bottom: 20px;'>");
                foreach (var param in parametra)
                {
                    html.AppendLine($"<p><strong>{param.Key}:</strong> {param.Value}</p>");
                }
                html.AppendLine($"<p><strong>Totali i shpenzimeve:</strong> {shpenzimet.Count()}</p>");
                html.AppendLine("</div>");
            }

            html.AppendLine("<table>");
            html.AppendLine("<thead><tr>");
            html.AppendLine("<th>Nr.</th><th>Origjina</th><th>Destinacioni</th><th>Lloji</th><th>Çmimi/Njësi</th><th>Njësia</th>");
            html.AppendLine("</tr></thead>");
            html.AppendLine("<tbody>");

            int nr = 1;
            foreach (var shpenzim in shpenzimet)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{nr}</td>");
                html.AppendLine($"<td>{shpenzim.VendiOrigjines}</td>");
                html.AppendLine($"<td>{shpenzim.VendiDestinacionit}</td>");
                html.AppendLine($"<td>{shpenzim.LlojiTransportit}</td>");
                html.AppendLine($"<td>{shpenzim.CmimiPerNjesi:N2} {shpenzim.Valuta}</td>");
                html.AppendLine($"<td>{shpenzim.NjesiaMatese}</td>");
                html.AppendLine("</tr>");
                nr++;
            }

            html.AppendLine("</tbody>");
            html.AppendLine("</table>");
            return html.ToString();
        }

        private string GjeneroHTMLDefault(string llojiModulit, object data, Dictionary<string, string>? parametra)
        {
            var html = new StringBuilder();
            html.AppendLine($"<div class='alert alert-warning'>");
            html.AppendLine($"<p>Formati default për modulin: <strong>{llojiModulit}</strong></p>");
            html.AppendLine($"<p>Data: {JsonSerializer.Serialize(data)}</p>");
            html.AppendLine("</div>");
            return html.ToString();
        }

        #endregion

        #region Gjenerimi i PDF

        /// <summary>
        /// Gjeneron PDF bazuar në formatin global të modulit
        /// </summary>
        public async Task<byte[]> GjeneroPDF(string llojiModulit, object data, Dictionary<string, string>? parametra = null)
        {
            try
            {
                var formatGlobal = await MerrFormatinGlobal(llojiModulit);
                var paperSize = formatGlobal?.PaperSize ?? "A4";
                
                // Gjenero HTML content
                var htmlContent = await GjeneroPërmbajtjeHTML(llojiModulit, data, parametra);
                
                // Konverto në PDF me metadata
                var titulli = $"Raport - {llojiModulit}";
                var autori = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Dogana e Kosovës";
                var subjekti = $"Raporti i gjeneruar nga sistemi i doganës për modulin {llojiModulit}";
                
                var pdfBytes = _sherbimetPDF.KonvertoHTMLNePDFMeMetadata(
                    htmlContent, 
                    titulli, 
                    autori, 
                    subjekti, 
                    paperSize);
                
                _logger.LogInformation("PDF generated successfully for module {Module} with {Size} bytes", 
                    llojiModulit, pdfBytes.Length);
                
                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for module {Module}", llojiModulit);
                throw;
            }
        }

        #endregion

        #region Gjenerimi i Excel

        /// <summary>
        /// Gjeneron Excel bazuar në formatin global të modulit
        /// </summary>
        public async Task<byte[]> GjeneroExcel(string llojiModulit, object data, Dictionary<string, string>? parametra = null)
        {
            try
            {
                using var workbook = new XLWorkbook();
                
                var worksheet = llojiModulit switch
                {
                    "VleratProdukteve" or "DosjaTeDisponueshme" or "DosjaVlerave" => 
                        KrijoExcelVleratProdukteve(workbook, data, parametra),
                    "KomentetDegeve" or "RaportiKomentetDegeve" => 
                        KrijoExcelKomentetDegeve(workbook, data, parametra),
                    "ShpenzimiTransportit" or "ShpenzimetTransportit" => 
                        KrijoExcelShpenzimetTransportit(workbook, data, parametra),
                    _ => KrijoExcelDefault(workbook, llojiModulit, data, parametra)
                };

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var excelBytes = stream.ToArray();
                
                _logger.LogInformation("Excel generated successfully for module {Module} with {Size} bytes", 
                    llojiModulit, excelBytes.Length);
                
                return excelBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel for module {Module}", llojiModulit);
                throw;
            }
        }

        private IXLWorksheet KrijoExcelVleratProdukteve(XLWorkbook workbook, object data, Dictionary<string, string>? parametra)
        {
            var vlerat = data as IEnumerable<VleraProduktit> ?? new List<VleraProduktit>();
            var worksheet = workbook.Worksheets.Add("Vlerat e Produkteve");

            // Header information
            int row = 1;
            worksheet.Cell(row, 1).Value = "REPUBLIKA E KOSOVËS";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 16;
            row++;
            
            worksheet.Cell(row, 1).Value = "DOGANA E KOSOVËS";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 14;
            row++;
            
            worksheet.Cell(row, 1).Value = "DOSJA E VLERAVE DOGANORE";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            row += 2;

            // Parameters
            if (parametra != null)
            {
                foreach (var param in parametra)
                {
                    worksheet.Cell(row, 1).Value = param.Key;
                    worksheet.Cell(row, 2).Value = param.Value;
                    row++;
                }
            }
            
            worksheet.Cell(row, 1).Value = "Data e gjenerimit";
            worksheet.Cell(row, 2).Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            row++;
            
            worksheet.Cell(row, 1).Value = "Totali i produkteve";
            worksheet.Cell(row, 2).Value = vlerat.Count();
            row += 2;

            // Column headers
            var headers = new[] { "Nr.", "Kodi", "Emri produktit", "Përshkrimi", "Vlera", "Njësia", "Origjina", "Kategoria" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(row, i + 1).Value = headers[i];
                worksheet.Cell(row, i + 1).Style.Font.Bold = true;
                worksheet.Cell(row, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }
            row++;

            // Data
            int nr = 1;
            foreach (var vlera in vlerat)
            {
                worksheet.Cell(row, 1).Value = nr;
                worksheet.Cell(row, 2).Value = vlera.KodiProduktit;
                worksheet.Cell(row, 3).Value = vlera.EmriProduktit;
                worksheet.Cell(row, 4).Value = vlera.Pershkrimi;
                worksheet.Cell(row, 5).Value = $"{vlera.VleraDoganore:N2} {vlera.Valuta}";
                worksheet.Cell(row, 6).Value = vlera.Njesia;
                worksheet.Cell(row, 7).Value = vlera.Origjina;
                worksheet.Cell(row, 8).Value = vlera.Kategoria;
                row++;
                nr++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();
            
            return worksheet;
        }

        private IXLWorksheet KrijoExcelKomentetDegeve(XLWorkbook workbook, object data, Dictionary<string, string>? parametra)
        {
            var komentet = data as IEnumerable<KomentiDeges> ?? new List<KomentiDeges>();
            var worksheet = workbook.Worksheets.Add("Komentet e Degeve");

            int row = 1;
            worksheet.Cell(row, 1).Value = "REPUBLIKA E KOSOVËS";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 16;
            row++;
            
            worksheet.Cell(row, 1).Value = "DOGANA E KOSOVËS";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 14;
            row++;
            
            worksheet.Cell(row, 1).Value = "RAPORTI I KOMENTEVE TË DEGËVE";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            row += 2;

            if (parametra != null)
            {
                foreach (var param in parametra)
                {
                    worksheet.Cell(row, 1).Value = param.Key;
                    worksheet.Cell(row, 2).Value = param.Value;
                    row++;
                }
            }
            
            worksheet.Cell(row, 1).Value = "Data e gjenerimit";
            worksheet.Cell(row, 2).Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            row++;
            
            worksheet.Cell(row, 1).Value = "Totali i komenteve";
            worksheet.Cell(row, 2).Value = komentet.Count();
            row += 2;

            var headers = new[] { "Nr.", "Dega", "Kodi Tarifar", "Mesazhi", "Data", "Statusi" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(row, i + 1).Value = headers[i];
                worksheet.Cell(row, i + 1).Style.Font.Bold = true;
                worksheet.Cell(row, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }
            row++;

            int nr = 1;
            foreach (var koment in komentet)
            {
                var statusi = koment.EshteZgjidhur ? "Zgjidhur" : "Në pritje";
                worksheet.Cell(row, 1).Value = nr;
                worksheet.Cell(row, 2).Value = koment.EmriDeges;
                worksheet.Cell(row, 3).Value = koment.KodiTarifar;
                worksheet.Cell(row, 4).Value = koment.Mesazhi;
                worksheet.Cell(row, 5).Value = koment.DataDergimit.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 6).Value = statusi;
                row++;
                nr++;
            }

            worksheet.Columns().AdjustToContents();
            return worksheet;
        }

        private IXLWorksheet KrijoExcelShpenzimetTransportit(XLWorkbook workbook, object data, Dictionary<string, string>? parametra)
        {
            var shpenzimet = data as IEnumerable<ShpenzimiTransportit> ?? new List<ShpenzimiTransportit>();
            var worksheet = workbook.Worksheets.Add("Shpenzimet e Transportit");

            int row = 1;
            worksheet.Cell(row, 1).Value = "REPUBLIKA E KOSOVËS";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 16;
            row++;
            
            worksheet.Cell(row, 1).Value = "DOGANA E KOSOVËS";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 14;
            row++;
            
            worksheet.Cell(row, 1).Value = "RAPORTI I SHPENZIMEVE TË TRANSPORTIT";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            row += 2;

            if (parametra != null)
            {
                foreach (var param in parametra)
                {
                    worksheet.Cell(row, 1).Value = param.Key;
                    worksheet.Cell(row, 2).Value = param.Value;
                    row++;
                }
            }
            
            worksheet.Cell(row, 1).Value = "Data e gjenerimit";
            worksheet.Cell(row, 2).Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            row++;
            
            worksheet.Cell(row, 1).Value = "Totali i shpenzimeve";
            worksheet.Cell(row, 2).Value = shpenzimet.Count();
            row += 2;

            var headers = new[] { "Nr.", "Origjina", "Destinacioni", "Lloji", "Çmimi/Njësi", "Njësia" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(row, i + 1).Value = headers[i];
                worksheet.Cell(row, i + 1).Style.Font.Bold = true;
                worksheet.Cell(row, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }
            row++;

            int nr = 1;
            foreach (var shpenzim in shpenzimet)
            {
                worksheet.Cell(row, 1).Value = nr;
                worksheet.Cell(row, 2).Value = shpenzim.VendiOrigjines;
                worksheet.Cell(row, 3).Value = shpenzim.VendiDestinacionit;
                worksheet.Cell(row, 4).Value = shpenzim.LlojiTransportit;
                worksheet.Cell(row, 5).Value = $"{shpenzim.CmimiPerNjesi:N2} {shpenzim.Valuta}";
                worksheet.Cell(row, 6).Value = shpenzim.NjesiaMatese;
                row++;
                nr++;
            }

            worksheet.Columns().AdjustToContents();
            return worksheet;
        }

        private IXLWorksheet KrijoExcelDefault(XLWorkbook workbook, string llojiModulit, object data, Dictionary<string, string>? parametra)
        {
            var worksheet = workbook.Worksheets.Add(llojiModulit);
            worksheet.Cell(1, 1).Value = $"Formati default për modulin: {llojiModulit}";
            return worksheet;
        }

        #endregion

        #region Auditimi i printimit

        /// <summary>
        /// Regjistron një operacion printimi në auditim
        /// </summary>
        public async Task<PrintAuditLog> RegjistroAuditimPrintimi(
            string llojiRaportit,
            string formatiEksportimit,
            int numriRekordeve,
            object? filtrat = null,
            int? formatPrintimiId = null,
            string? shenime = null,
            bool eshteSuksesshem = true,
            string? mesazhiGabimit = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var perdoruesi = httpContext?.User?.Identity?.Name ?? "Anonim";
                var adresaIP = MerrAdresenIP();
                var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
                var sesioniId = httpContext?.Session.Id;

                var auditLog = new PrintAuditLog
                {
                    Perdoruesi = perdoruesi,
                    LlojiRaportit = llojiRaportit,
                    FormatiEksportimit = formatiEksportimit,
                    NumriRekordeve = numriRekordeve,
                    Filtrat = filtrat != null ? JsonSerializer.Serialize(filtrat) : null,
                    FormatPrintimiId = formatPrintimiId,
                    DataPrintimit = DateTime.UtcNow,
                    AdresaIP = adresaIP,
                    UserAgent = userAgent,
                    SesioniId = sesioniId,
                    Shenime = shenime,
                    EshteSuksesshem = eshteSuksesshem,
                    MesazhiGabimit = mesazhiGabimit
                };

                _konteksti.PrintAuditLogs.Add(auditLog);
                await _konteksti.SaveChangesAsync();

                _logger.LogInformation(
                    "Print audit: User={User}, Report={Report}, Format={Format}, Records={Records}, Success={Success}",
                    perdoruesi, llojiRaportit, formatiEksportimit, numriRekordeve, eshteSuksesshem);

                return auditLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gabim gjatë regjistrimittë auditimit të printimit");
                throw;
            }
        }

        /// <summary>
        /// Merr log-et e printimit sipas filtrave
        /// </summary>
        public async Task<IEnumerable<PrintAuditLog>> MerrLogetPrintimit(
            string? perdoruesi = null,
            string? llojiRaportit = null,
            DateTime? ngaData = null,
            DateTime? deriData = null,
            int? numriRreshtave = null)
        {
            var query = _konteksti.PrintAuditLogs
                .Include(p => p.FormatPrintimi)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(perdoruesi))
                query = query.Where(p => p.Perdoruesi.Contains(perdoruesi));

            if (!string.IsNullOrWhiteSpace(llojiRaportit))
                query = query.Where(p => p.LlojiRaportit == llojiRaportit);

            if (ngaData.HasValue)
                query = query.Where(p => p.DataPrintimit >= ngaData.Value);

            if (deriData.HasValue)
                query = query.Where(p => p.DataPrintimit <= deriData.Value.AddDays(1));

            query = query.OrderByDescending(p => p.DataPrintimit);

            if (numriRreshtave.HasValue)
                query = query.Take(numriRreshtave.Value);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Merr statistikat e printimit për një përdorues
        /// </summary>
        public async Task<PrintStatistics> MerrStatistikatPerdoruesit(string perdoruesi, DateTime? ngaData = null)
        {
            var query = _konteksti.PrintAuditLogs
                .Where(p => p.Perdoruesi == perdoruesi);

            if (ngaData.HasValue)
                query = query.Where(p => p.DataPrintimit >= ngaData.Value);

            var logs = await query.ToListAsync();

            return new PrintStatistics
            {
                TotaliPrintimeve = logs.Count,
                TotaliPrint = logs.Count(l => l.FormatiEksportimit == "Print"),
                TotaliPDF = logs.Count(l => l.FormatiEksportimit == "PDF"),
                TotaliExcel = logs.Count(l => l.FormatiEksportimit == "Excel"),
                TotaliSuksesshem = logs.Count(l => l.EshteSuksesshem),
                TotaliGabim = logs.Count(l => !l.EshteSuksesshem),
                TotaliRekordeve = logs.Sum(l => l.NumriRekordeve),
                DataEParesPrintimit = logs.Min(l => (DateTime?)l.DataPrintimit),
                DataEFunditPrintimit = logs.Max(l => (DateTime?)l.DataPrintimit)
            };
        }

        #endregion

        #region Metoda ndihmëse

        private string? MerrAdresenIP()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            // Kontrollo për proxy
            var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            }

            return ipAddress;
        }

        #endregion

        #region Validimi i sigurisë

        /// <summary>
        /// Kontrollon nëse një përdorues ka tejkaluar limitin e printimeve për një periudhë
        /// </summary>
        public async Task<bool> KaTekaluarLimitinPrintimeve(string perdoruesi, int limiti = 100, int oret = 24)
        {
            var kohaNga = DateTime.UtcNow.AddHours(-oret);
            var numriPrintimeve = await _konteksti.PrintAuditLogs
                .Where(p => p.Perdoruesi == perdoruesi && p.DataPrintimit >= kohaNga)
                .CountAsync();

            return numriPrintimeve >= limiti;
        }

        /// <summary>
        /// Merr përdoruesit më aktiv për printim
        /// </summary>
        public async Task<IEnumerable<UserPrintActivity>> MerrPerdoruesitMeAktiv(int numriPerdoruesve = 10, DateTime? ngaData = null)
        {
            var query = _konteksti.PrintAuditLogs.AsQueryable();

            if (ngaData.HasValue)
                query = query.Where(p => p.DataPrintimit >= ngaData.Value);

            var result = await query
                .GroupBy(p => p.Perdoruesi)
                .Select(g => new UserPrintActivity
                {
                    Perdoruesi = g.Key,
                    NumriPrintimeve = g.Count(),
                    NumriRekordeve = g.Sum(p => p.NumriRekordeve),
                    DataEFunditPrintimit = g.Max(p => p.DataPrintimit)
                })
                .OrderByDescending(u => u.NumriPrintimeve)
                .Take(numriPerdoruesve)
                .ToListAsync();

            return result;
        }

        #endregion
    }

    #region Klasat ndihmëse

    /// <summary>
    /// Statistikat e printimit për një përdorues
    /// </summary>
    public class PrintStatistics
    {
        public int TotaliPrintimeve { get; set; }
        public int TotaliPrint { get; set; }
        public int TotaliPDF { get; set; }
        public int TotaliExcel { get; set; }
        public int TotaliSuksesshem { get; set; }
        public int TotaliGabim { get; set; }
        public int TotaliRekordeve { get; set; }
        public DateTime? DataEParesPrintimit { get; set; }
        public DateTime? DataEFunditPrintimit { get; set; }
    }

    /// <summary>
    /// Aktiviteti i printimit të një përdoruesi
    /// </summary>
    public class UserPrintActivity
    {
        public string Perdoruesi { get; set; } = string.Empty;
        public int NumriPrintimeve { get; set; }
        public int NumriRekordeve { get; set; }
        public DateTime DataEFunditPrintimit { get; set; }
    }

    #endregion
}
