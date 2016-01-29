using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using TuesPechkin;

namespace PDFWebkitSample.Controllers
{
    public class PdfController : Controller
    {
        //
        // GET: /Pdf/

        public ActionResult PdfGenerator()
        {
            return View();
        }

        public ActionResult GeneratePDF()
        {
            var url = "http://www.google.com";
            byte[] pdfBuf = PDFHelper.ConvertToPdf(PDFHelper.DataType.URL, url, "DocumentTitle");

            var cd = new ContentDisposition
            {
                FileName = "DocName.pdf",
                Inline = false // always prompt the user for downloading, set to true if you want the browser to try to show the file inline
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(pdfBuf, "application/pdf");
        }       

    }

    public class PDFHelper
    {
        //private static IDeployment deployment = new StaticDeployment(@"C:\Alex\PDF");
        //private static IConverter converter = new ThreadSafeConverter(new RemotingToolset<PdfToolset>(deployment));

        private static IConverter converter = new ThreadSafeConverter(
                                    new RemotingToolset<PdfToolset>(
                                        new Win32EmbeddedDeployment(
                                            new TempFolderDeployment())));

        public enum DataType
        {
            HTML,
            URL
        }      

        public static byte[] ConvertToPdf(PDFHelper.DataType type, string data, string documentName)
        {
            var webSettings = new WebSettings
            {
                EnableJavascript = true,
                LoadImages = true,
                EnableIntelligentShrinking = true
            };

            ObjectSettings objSettings = null;

            if (type == DataType.HTML)
                objSettings = new ObjectSettings { HtmlText = data, WebSettings = webSettings };
            else
                objSettings = new ObjectSettings { PageUrl = data, WebSettings = webSettings };

            var document = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    ProduceOutline = true,
                    DocumentTitle = documentName,
                    PaperSize = PaperKind.Letter,
                    UseCompression = true,
                    ImageDPI = 600,
                    ImageQuality = 100,


                    //DPI = 1200,
                    Margins =
                    {
                        Top = 1,
                        Right = 0.5,
                        Bottom = 1,
                        Left = 0.5,
                        Unit = Unit.Centimeters
                    }
                },
                Objects = {
                   objSettings
                }
            };

            return converter.Convert(document);
        }
    }
}
