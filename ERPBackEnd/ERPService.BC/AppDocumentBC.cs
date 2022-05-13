using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ERPService.BC
{
    public class AppDocumentBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public AppDocumentBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public AppResponse saveAppDocument(List<AppDocument> appDocument, bool saveChanges = true)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();
            
            foreach (var item in appDocument)
                InsertUpdateAppDocument(item, false);

            if (saveChanges)
                _repository.SaveChanges();

            appResponse.Status = APPMessageKey.DATASAVESUCSS;
            return appResponse;
        }
        public AppResponse saveAppDocumentNew(List<AppDocument> appDocumentsFromUI, bool saveChanges = true)
        {

            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();
            var id = appDocumentsFromUI[0].TransactionId;
            var appDocuments = new List<AppDocument>();
            appDocuments = _repository.GetQuery<AppDocument>().Where(a => a.TransactionId == id).ToList();

            foreach (var document in appDocumentsFromUI)
            {
                if (document.Action == 'D')
                {
                    var deletedDoc = appDocuments.FirstOrDefault(dc => dc.Id == document.Id);
                    if (deletedDoc != null)
                        _repository.Delete(deletedDoc);
                }
                else
                {
                    if (document.FileContent != null && document.DocumentType != "" && document.FileName != "")
                    {
                        var insertedDoc = new AppDocument
                        {
                            Id = Guid.NewGuid(),
                            TransactionId = document.TransactionId,
                            TransactionType = document.TransactionType,
                            ExpiryDate = DateTime.Now,
                            FileContent = document.FileContent,
                            UniqueNumber = document.UniqueNumber,
                            DocumentType = document.DocumentType,
                            FileName = document.FileName,
                            CreatedBy = document.CreatedBy,
                            CreatedDate = document.CreatedDate,
                            ModifiedBy = document.ModifiedBy,
                            ModifiedDate = document.ModifiedDate,
                            Active = "Y"
                        };
                        _repository.Add(insertedDoc);
                    }
                }

                try
                {
                    _repository.SaveChanges();
                }

                catch (Exception ex)
                {
                    appResponse.Status = ERPExceptions.APP_MESSAGES[APPMessageKey.FAILED];
                }
            }
            if (saveChanges)
                _repository.SaveChanges();

            return appResponse;
        }
        public List<AppDocument> getFileAttachment(Guid id)
        {

            var document = _repository.GetQuery<AppDocument>().Where(a => a.TransactionId == id && a.Active == "Y").ToList();
            document.ForEach(element =>
                {
                    element.FileContent = null;
                });
            return document;
        }
        public AppDocument getFileDowenload(Guid id)
        {
            var document = _repository.GetQuery<AppDocument>().Where(a => a.Id == id && a.Active == "Y").FirstOrDefault();
            return document;
        }
        private void InsertUpdateAppDocument(AppDocument appDocument1, bool saveChanges)
        {
            if (appDocument1.Id == Guid.Empty)
            {

                appDocument1.Id = Guid.NewGuid();
                _repository.Add(appDocument1, false);
            }
            else
                _repository.Update(appDocument1, false);
            if (saveChanges)
                _repository.SaveChanges();
        }

    }
}