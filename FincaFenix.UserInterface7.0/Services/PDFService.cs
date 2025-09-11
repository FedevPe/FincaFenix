using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace FincaFenix.UserInterface7._0.Services
{
    public class PDFService
    {
        private readonly IJSRuntime jSRuntime;

        public PDFService(IJSRuntime JSRuntime)
        {
            jSRuntime = JSRuntime;
        }
        public async Task DownloadPdfAsync(IDocument document, string fileName)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Genera el documento PDF en memoria
            byte[] pdfBytes = document.GeneratePdf();

            // Convierte los bytes a una cadena Base64
            string base64String = Convert.ToBase64String(pdfBytes);

            // Llama a la función de JavaScript para descargar el archivo
            string fullFileName = $"{fileName}.pdf";
            await jSRuntime.InvokeVoidAsync("downloadFileFromBase64", fullFileName, base64String);
        }
    }
}
