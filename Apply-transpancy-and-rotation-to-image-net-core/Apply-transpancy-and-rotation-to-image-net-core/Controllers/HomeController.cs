using Apply_transpancy_and_rotation_to_image_net_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Diagnostics;

namespace Apply_transpancy_and_rotation_to_image_net_core.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ConvertToPDF(IList<IFormFile> files)
        {
            //Creating the new PDF document
            PdfDocument document = new PdfDocument();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    MemoryStream file = new MemoryStream();
                    formFile.CopyTo(file);

                    //Loading the image
                    PdfImage image = PdfImage.FromStream(file);

                    //Adding new page
                    PdfPage page = document.Pages.Add();

                    //Apply transparency
                    page.Graphics.SetTransparency(0.5f);

                    //Rotate the coordinate system
                    page.Graphics.RotateTransform(-45);

                    //Drawing image to the PDF page
                    page.Graphics.DrawImage(image, new PointF(0, 0));
                }
            }

            //Saving the PDF to the MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream);

            //Set the position as '0'
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