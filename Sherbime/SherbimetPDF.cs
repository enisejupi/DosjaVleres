using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using System.Text;

namespace KosovaDoganaModerne.Sherbime
{
    /// <summary>
    /// Shërbim i dedikuar për gjenerimin e PDF-ve nga HTML
    /// Përdor iText7 për konvertim të avancuar HTML në PDF
    /// </summary>
    public class SherbimetPDF
    {
        private readonly ILogger<SherbimetPDF> _logger;

        public SherbimetPDF(ILogger<SherbimetPDF> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Konverton HTML në PDF me konfigurim të plotë
        /// </summary>
        /// <param name="htmlContent">Përmbajtja HTML për tu konvertuar</param>
        /// <param name="paperSize">Madhësia e letrës (A4, Letter, etj.)</param>
        /// <returns>PDF si byte array</returns>
        public byte[] KonvertoHTMLNePDF(string htmlContent, string paperSize = "A4")
        {
            try
            {
                using var memoryStream = new MemoryStream();
                
                // Krijo writer dhe PdfDocument
                var writer = new PdfWriter(memoryStream);
                var pdfDocument = new PdfDocument(writer);
                
                // Vendos madhësinë e letrës
                var pageSize = paperSize.ToUpper() switch
                {
                    "A4" => iText.Kernel.Geom.PageSize.A4,
                    "A3" => iText.Kernel.Geom.PageSize.A3,
                    "LETTER" => iText.Kernel.Geom.PageSize.LETTER,
                    "LEGAL" => iText.Kernel.Geom.PageSize.LEGAL,
                    _ => iText.Kernel.Geom.PageSize.A4
                };
                pdfDocument.SetDefaultPageSize(pageSize);

                // Properties të dokumentit
                var properties = new ConverterProperties();
                properties.SetBaseUri(Environment.CurrentDirectory);

                // Konverto HTML në PDF
                HtmlConverter.ConvertToPdf(htmlContent, pdfDocument, properties);

                pdfDocument.Close();

                var pdfBytes = memoryStream.ToArray();
                _logger.LogInformation("PDF gjeneruar me sukses: {Size} bytes", pdfBytes.Length);
                
                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gabim gjatë gjenerimit të PDF");
                throw new InvalidOperationException("Nuk mund të gjenerohet PDF: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Konverton HTML në PDF me metadata të plotë
        /// </summary>
        public byte[] KonvertoHTMLNePDFMeMetadata(
            string htmlContent, 
            string titulli, 
            string autori, 
            string subjekti,
            string paperSize = "A4")
        {
            try
            {
                using var memoryStream = new MemoryStream();
                
                var writer = new PdfWriter(memoryStream);
                var pdfDocument = new PdfDocument(writer);

                // Vendos metadata
                var info = pdfDocument.GetDocumentInfo();
                info.SetTitle(titulli);
                info.SetAuthor(autori);
                info.SetSubject(subjekti);
                info.SetCreator("Dogana e Kosovës - Sistema e Menaxhimit të Dosjeve");
                info.SetKeywords("Dogana, Kosovo, Raporti");

                // Vendos madhësinë e letrës
                var pageSize = paperSize.ToUpper() switch
                {
                    "A4" => iText.Kernel.Geom.PageSize.A4,
                    "A3" => iText.Kernel.Geom.PageSize.A3,
                    "LETTER" => iText.Kernel.Geom.PageSize.LETTER,
                    "LEGAL" => iText.Kernel.Geom.PageSize.LEGAL,
                    _ => iText.Kernel.Geom.PageSize.A4
                };
                pdfDocument.SetDefaultPageSize(pageSize);

                var properties = new ConverterProperties();
                properties.SetBaseUri(Environment.CurrentDirectory);

                HtmlConverter.ConvertToPdf(htmlContent, pdfDocument, properties);

                pdfDocument.Close();

                var pdfBytes = memoryStream.ToArray();
                _logger.LogInformation("PDF me metadata gjeneruar me sukses: {Size} bytes", pdfBytes.Length);
                
                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gabim gjatë gjenerimit të PDF me metadata");
                throw new InvalidOperationException("Nuk mund të gjenerohet PDF: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Gjeneron PDF thjeshtë nga HTML pa stil të komplikuar
        /// </summary>
        public byte[] GjeneroPDFThjeshte(string htmlContent)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                HtmlConverter.ConvertToPdf(htmlContent, memoryStream);
                
                var pdfBytes = memoryStream.ToArray();
                _logger.LogInformation("PDF i thjeshtë gjeneruar: {Size} bytes", pdfBytes.Length);
                
                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gabim gjatë gjenerimit të PDF të thjeshtë");
                throw;
            }
        }
    }
}
