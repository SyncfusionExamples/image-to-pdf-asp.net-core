using ImageToPDF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ImageToPDF.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
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
                    if (formFile.ContentType == "image/tiff")
                    {
                        //Get the image stream and draw frame by frame
                        using (var tiffImage = new PdfBitmap(file))
                        {
                            int frameCount = tiffImage.FrameCount;
                            for (int i = 0; i < frameCount; i++)
                            {
                                //Add pages to the document
                                var page = document.Pages.Add();
                                //Getting page size to fit the image within the page
                                SizeF pageSize = page.GetClientSize();
                                //Selecting frame in TIFF
                                tiffImage.ActiveFrame = i;
                                //Draw TIFF frame
                                page.Graphics.DrawImage(tiffImage, 0, 0, pageSize.Width, pageSize.Height);
                            }
                        }

                    }
                    else
                    {
                        //Loading the image                       
                        PdfImage image = PdfImage.FromStream(file);
                        //Adding new page
                        PdfPage page = page = document.Pages.Add();
                        //Drawing image to the PDF page
                        page.Graphics.DrawImage(image, new PointF(0, 0));
                    }
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
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
