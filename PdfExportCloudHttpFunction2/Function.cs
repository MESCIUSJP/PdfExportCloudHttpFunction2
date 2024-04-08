using Google.Cloud.Functions.Framework;
using Google.Cloud.Storage.V1;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace PdfExportCloudHttpFunction2
{
    public class Function : IHttpFunction
    {
        /// <summary>
        /// Logic for your function goes here.
        /// </summary>
        /// <param name="context">The HTTP context, containing the request and the response.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleAsync(HttpContext context)
        {
            HttpRequest request = context.Request;
            string name = request.Query["name"].ToString();

            string Message = string.IsNullOrEmpty(name)
                ? "Hello, World!!"
                : $"Hello, {name}!!";

            // トライアル版または製品版のライセンスキーを設定
            //GcPdfDocument.SetLicenseKey("");

            GcPdfDocument doc = new();
            GcPdfGraphics g = doc.NewPage().Graphics;

            g.DrawString(Message,
                new TextFormat() { Font = StandardFonts.Helvetica, FontSize = 12 },
                new PointF(72, 72));

            using MemoryStream outputstream = new();
            StorageClient sc = StorageClient.Create();
            doc.Save(outputstream, false);
            await sc.UploadObjectAsync("diodocs-export", "Result.pdf", "application/pdf", outputstream);
        }
    }
}
