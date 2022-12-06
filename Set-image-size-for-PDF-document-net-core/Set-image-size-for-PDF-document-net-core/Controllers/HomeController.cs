using Microsoft.AspNetCore.Mvc;
using Set_image_size_for_PDF_document_net_core.Models;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using System.Diagnostics;
using Syncfusion.Drawing;

namespace Set_image_size_for_PDF_document_net_core.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreatePDFDocument(IList<IFormFile> files)
        {
            //Creating the new PDF document
            PdfDocument document = new PdfDocument();

            //Setting page margin to 0
            document.PageSettings.Margins.All = 0;

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    MemoryStream file = new MemoryStream();
                    formFile.CopyTo(file);

                    //Loading the image
                    PdfImage image = PdfImage.FromStream(file);
                    //Setting same page size as image
                    PdfSection section = document.Sections.Add();
                    section.PageSettings.Width = image.PhysicalDimension.Width;
                    section.PageSettings.Height = image.PhysicalDimension.Height;
                    PdfPage page = section.Pages.Add();

                    //Drawing image to the PDF page
                    page.Graphics.DrawImage(image, new PointF(0, 0), new SizeF(page.Size.Width, page.Size.Height));

                    file.Dispose();
                }
            }
            //Saving the PDF to the MemoryStream
            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            //Set the position as '0'.
            stream.Position = 0;

            //Download the PDF document in the browser
            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");

            fileStreamResult.FileDownloadName = "ImageToPDF.pdf";

            return fileStreamResult;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}