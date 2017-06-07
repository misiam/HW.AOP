using System.Collections.Generic;
using WindowsServices.HW.ImgScanner.Interfaces;
using MigraDoc.DocumentObjectModel;
using System.IO;
using WindowsServices.HW.Logging;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;


namespace WindowsServices.HW.ImgScanner.Utils
{
    class PdfAggregatorFilesHandler : IFilesHandler
    {
        private ILogger _logger;

        public PdfAggregatorFilesHandler()
        {
            _logger = Logger.Current;
        }

        public void Handle(IList<string> filesToHandle, IStorageService storageService, string path)
        {
            var document = new Document();
            document.AddSection();

            foreach (var file in filesToHandle)
            {
                Handle(file, document);
            }
            PushChanges(document, storageService, path);
        }

        private void Handle(string inputFile, Document document)
        {
            _logger.LogInfo(" Handle: " + inputFile);
            var section = document.Sections[0];

            var img = section.AddImage(inputFile);
            img.RelativeHorizontal = RelativeHorizontal.Page;
            img.RelativeVertical = RelativeVertical.Page;

            img.Top = 0;
            img.Left = 0;

            img.Height = document.DefaultPageSetup.PageHeight;
            img.Width = document.DefaultPageSetup.PageWidth;

            section.AddPageBreak();
        }



        private void PushChanges(Document  document, IStorageService storageService, string path)
        {
            _logger.LogInfo(" PushChanges: " + path);
            var render = new PdfDocumentRenderer { Document = document };
            render.RenderDocument();

            using (var ms = new MemoryStream())
            {
                render.Save(ms, false);
                storageService.SaveToStorage(ms, path);
                ms.Close();
            }
        }

    }
}
