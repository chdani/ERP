using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ERPService.BC
{
    public class PettyCashTransferHistCommentsBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public PettyCashTransferHistCommentsBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public List<PettyCashTransferComment> SavePettyCashTransHdrComment(PettyCashTransferComment pettyCashTransferComment)
        {

            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            pettyCashTransferComment.Active = "Y";

            InsertUpdateComment(pettyCashTransferComment);
            var id = pettyCashTransferComment.Id;
            if (pettyCashTransferComment.AppDocuments != null && pettyCashTransferComment.AppDocuments.Count > 0)
            {
                foreach (var item in pettyCashTransferComment.AppDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = "PETTYCASHTRANS",
                        ExpiryDate = DateTime.Now,
                        FileContent = item.FileContent,
                        UniqueNumber = item.DocumentType,
                        DocumentType = item.DocumentType,
                        FileName = item.FileName,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        Active = "Y",
                    };
                    appDocumentList.Add(appDocument1);
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(appDocumentList, false);
            }

            _repository.SaveChanges();


            return GetPettyCashTransComments(pettyCashTransferComment.PettyCashTransferId);
        }
        public void SavePettyCashTransferHist(PettyCashTransferHist pettyCashTransferHist, bool saveChanges)
        {
            PettyCashTransferHist pettyCashTransferHist1 = new PettyCashTransferHist();

            pettyCashTransferHist1.Active = "Y";
            pettyCashTransferHist1.PettyCashTransferId = pettyCashTransferHist.PettyCashTransferId;
            pettyCashTransferHist1.FieldName = pettyCashTransferHist.FieldName;
            pettyCashTransferHist1.PrevValue = pettyCashTransferHist.PrevValue;
            pettyCashTransferHist1.CurrentValue = pettyCashTransferHist.CurrentValue;

            InsertUpdateSavepettyCashTransferHist(pettyCashTransferHist1, saveChanges);
        }
        private void InsertUpdateSavepettyCashTransferHist(PettyCashTransferHist pettyCashTransferHist1, bool saveChanges)
        {
            if (pettyCashTransferHist1.Id == Guid.Empty)
            {
                pettyCashTransferHist1.Id = Guid.NewGuid();
                _repository.Add(pettyCashTransferHist1, false);
            }
            else
                _repository.Update(pettyCashTransferHist1, false);
            if (saveChanges)
                _repository.SaveChanges();
        }

        public List<PettyCashTransferHist> GetpettycashtransHist(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var pettyCashTransHdrsHistory = _repository.GetQuery<PettyCashTransferHist>().Where(a => a.PettyCashTransferId == id).OrderByDescending(a => a.CreatedDate).ToList();
            pettyCashTransHdrsHistory.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.PettyCashTransferId = Hsty.PettyCashTransferId;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return pettyCashTransHdrsHistory;
        }

        public List<AppDocument> GetPettyCashTransAttachments(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<PettyCashTransferComment>().Where(a => a.PettyCashTransferId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "PETTYCASHTRANS") && fileAttachmentIds.Contains(a.TransactionId))
                .OrderByDescending(a => a.CreatedDate).ToList();

            appDoucuments.ForEach(ele =>
            {
                var userName = usemaster.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                if (userName != null)
                    ele.UserName = userName.FirstName + " " + userName.LastName;

                ele.FileContent = null;
            });
            return appDoucuments;
        }
        public List<PettyCashTransferComment> GetPettyCashTransComments(Guid Id)
        {
            var Comments = _repository.GetQuery<PettyCashTransferComment>().Where(a => a.PettyCashTransferId == Id).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "PETTYCASHTRANS" && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.PettyCashTransferId = ele.PettyCashTransferId;
                ele.Comments = ele.Comments;
                ele.CreatedDate = ele.CreatedDate;
                if (userName != null)
                    ele.UserName = userName.FirstName + " " + userName.LastName;
                if (appDoucument != null)
                {
                    foreach (var doc in appDoucument)
                        doc.FileContent = null;

                    ele.AppDocuments = appDoucument;
                }

            });

            return Comments.OrderByDescending(a => a.CreatedDate).ToList();
        }


        private void InsertUpdateComment(PettyCashTransferComment pettyCashTransferComment)
        {

            if (pettyCashTransferComment.Id == Guid.Empty)
            {
                pettyCashTransferComment.Id = Guid.NewGuid();
                _repository.Add(pettyCashTransferComment, true);

            }
            else
            {

                _repository.Update(pettyCashTransferComment, true);
            }

        }

    }

}