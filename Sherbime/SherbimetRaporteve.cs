using ClosedXML.Excel;
using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Sherbime
{
    public class SherbimetRaporteve
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<SherbimetRaporteve> _logger;

        public SherbimetRaporteve(IWebHostEnvironment environment, ILogger<SherbimetRaporteve> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public byte[] GjeneroExcelVleratProdukteve(IEnumerable<VleraProduktit> vlerat, string? kategoria, string? origjina)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Dosja e vlerave");

            // Shto logon dhe titullin
            var titleRow = worksheet.Row(1);
            titleRow.Height = 40;
            var titleCell = worksheet.Cell(1, 1);
            titleCell.Value = "DOGANA E KOSOVËS";
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 18;
            titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(1, 1, 1, 10).Merge();

            var subtitleCell = worksheet.Cell(2, 1);
            subtitleCell.Value = "DOSJA E VLERAVE TË PRODUKTEVE";
            subtitleCell.Style.Font.Bold = true;
            subtitleCell.Style.Font.FontSize = 14;
            subtitleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(2, 1, 2, 10).Merge();

            // Shto informacionin e filtrave
            int currentRow = 4;
            if (!string.IsNullOrEmpty(kategoria))
            {
                worksheet.Cell(currentRow, 1).Value = $"Kategoria: {kategoria}";
                worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                currentRow++;
            }
            if (!string.IsNullOrEmpty(origjina))
            {
                worksheet.Cell(currentRow, 1).Value = $"Origjina: {origjina}";
                worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                currentRow++;
            }

            worksheet.Cell(currentRow, 1).Value = $"Data e gjenerimit: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
            worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
            currentRow += 2;

            // Shto header-at e tabelës
            var headerRow = currentRow;
            var headers = new[] { "Nr.", "Kodi", "Kodi i produktit", "Përshkrimi", "Njësia", "Vlera (€)", "Kategoria", "Origjina", "Përditësuar", "Aktiv" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Shto të dhënat
            int rowNumber = 1;
            currentRow++;
            foreach (var vlera in vlerat)
            {
                worksheet.Cell(currentRow, 1).Value = rowNumber;
                worksheet.Cell(currentRow, 2).Value = vlera.Kodi;
                worksheet.Cell(currentRow, 3).Value = vlera.KodiProduktit;
                worksheet.Cell(currentRow, 4).Value = vlera.Pershkrimi;
                worksheet.Cell(currentRow, 5).Value = vlera.Njesia;
                worksheet.Cell(currentRow, 6).Value = vlera.VleraDoganore;
                worksheet.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00 €";
                worksheet.Cell(currentRow, 7).Value = vlera.Kategoria;
                worksheet.Cell(currentRow, 8).Value = vlera.Origjina;
                worksheet.Cell(currentRow, 9).Value = vlera.Modifikuar_Me?.ToString("dd.MM.yyyy");
                worksheet.Cell(currentRow, 10).Value = vlera.Eshte_Aktiv ? "Po" : "Jo";

                // Vendos border për të gjitha qelizat
                for (int i = 1; i <= headers.Length; i++)
                {
                    worksheet.Cell(currentRow, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                currentRow++;
                rowNumber++;
            }

            // Auto-fit kolonat
            worksheet.Columns().AdjustToContents();

            // Ruaj në MemoryStream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GjeneroExcelShpenzimetTransportit(IEnumerable<ShpenzimiTransportit> shpenzimet, string? llojiTransportit)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Shpenzimet e transportit");

            // Shto logon dhe titullin
            var titleRow = worksheet.Row(1);
            titleRow.Height = 40;
            var titleCell = worksheet.Cell(1, 1);
            titleCell.Value = "DOGANA E KOSOVËS";
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 18;
            titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(1, 1, 1, 8).Merge();

            var subtitleCell = worksheet.Cell(2, 1);
            subtitleCell.Value = "RAPORTI I SHPENZIMEVE TË TRANSPORTIT";
            subtitleCell.Style.Font.Bold = true;
            subtitleCell.Style.Font.FontSize = 14;
            subtitleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(2, 1, 2, 8).Merge();

            int currentRow = 4;
            if (!string.IsNullOrEmpty(llojiTransportit))
            {
                worksheet.Cell(currentRow, 1).Value = $"Lloji i transportit: {llojiTransportit}";
                worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                currentRow++;
            }

            worksheet.Cell(currentRow, 1).Value = $"Data e gjenerimit: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
            worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
            currentRow += 2;

            // Header
            var headerRow = currentRow;
            var headers = new[] { "Nr.", "Rruga", "Lloji i transportit", "Shënime", "Cmimi për njësi (€)", "Data e krijimit", "Krijuar nga", "Aktiv" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Data
            int rowNumber = 1;
            currentRow++;
            foreach (var shpenzim in shpenzimet)
            {
                worksheet.Cell(currentRow, 1).Value = rowNumber;
                worksheet.Cell(currentRow, 2).Value = $"{shpenzim.VendiOrigjines}-{shpenzim.VendiDestinacionit}";
                worksheet.Cell(currentRow, 3).Value = shpenzim.LlojiTransportit;
                worksheet.Cell(currentRow, 4).Value = shpenzim.Shenime ?? "-";
                worksheet.Cell(currentRow, 5).Value = shpenzim.CmimiPerNjesi;
                worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00 €";
                worksheet.Cell(currentRow, 6).Value = shpenzim.DataKrijimit.ToString("dd.MM.yyyy");
                worksheet.Cell(currentRow, 7).Value = shpenzim.KrijuarNga ?? "-";
                worksheet.Cell(currentRow, 8).Value = shpenzim.Aktiv ? "Po" : "Jo";

                for (int i = 1; i <= headers.Length; i++)
                {
                    worksheet.Cell(currentRow, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                currentRow++;
                rowNumber++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GjeneroExcelKomentetDegeve(IEnumerable<KomentiDeges> komentet, string? dega)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Komentet e degëve");

            // Shto logon dhe titullin
            var titleRow = worksheet.Row(1);
            titleRow.Height = 40;
            var titleCell = worksheet.Cell(1, 1);
            titleCell.Value = "DOGANA E KOSOVËS";
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 18;
            titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(1, 1, 1, 8).Merge();

            var subtitleCell = worksheet.Cell(2, 1);
            subtitleCell.Value = "RAPORTI I KOMENTEVE TË DEGËVE";
            subtitleCell.Style.Font.Bold = true;
            subtitleCell.Style.Font.FontSize = 14;
            subtitleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(2, 1, 2, 8).Merge();

            int currentRow = 4;
            if (!string.IsNullOrEmpty(dega))
            {
                worksheet.Cell(currentRow, 1).Value = $"Dega: {dega}";
                worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                currentRow++;
            }

            worksheet.Cell(currentRow, 1).Value = $"Data e gjenerimit: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
            worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
            currentRow += 2;

            // Header
            var headerRow = currentRow;
            var headers = new[] { "Nr.", "ID", "Dega", "Kodi tarifar", "Mesazhi", "Data e dërgimit", "Statusi", "Dërguar nga" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightYellow;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Data
            int rowNumber = 1;
            currentRow++;
            foreach (var koment in komentet)
            {
                worksheet.Cell(currentRow, 1).Value = rowNumber;
                worksheet.Cell(currentRow, 2).Value = koment.Id.ToString();
                worksheet.Cell(currentRow, 3).Value = koment.EmriDeges;
                worksheet.Cell(currentRow, 4).Value = koment.KodiTarifar;
                worksheet.Cell(currentRow, 5).Value = koment.Mesazhi;
                worksheet.Cell(currentRow, 6).Value = koment.DataDergimit.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(currentRow, 7).Value = koment.EshteZgjidhur ? "Zgjidhur" : "Në pritje";
                worksheet.Cell(currentRow, 8).Value = koment.DergoPrejNga ?? "-";

                for (int i = 1; i <= headers.Length; i++)
                {
                    worksheet.Cell(currentRow, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                currentRow++;
                rowNumber++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
