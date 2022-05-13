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
    public class CashTransactionHistoryCommentsBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public CashTransactionHistoryCommentsBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public List<CashTransacionComment> SaveCashTransactionHdrComment(CashTransacionComment cashTransacionComment)
        {

            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            cashTransacionComment.Active = "Y";

            InsertUpdateComment(cashTransacionComment);
            var id = cashTransacionComment.Id;
            if (cashTransacionComment.AppDocuments != null && cashTransacionComment.AppDocuments.Count > 0)
            {
                foreach (var item in cashTransacionComment.AppDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = "CASHTRANSACION",
                        ExpiryDate = DateTime.Now,
                        FileContent = item.FileContent,
                        UniqueNumber = item.DocumentType,
                        DocumentType = item.DocumentType,
                        FileName = item.FileName,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        Active ="Y",
                    };
                    appDocumentList.Add(appDocument1);
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(appDocumentList, false);
            }

            _repository.SaveChanges();


            return GetCashTransAcionComments(cashTransacionComment.CashTransacionId);
        }
        public void SaveCashTransactionHist(CashTransacionHist cashTransacionHist, bool saveChanges)
        {
            CashTransacionHist cashTransacionHist1 = new CashTransacionHist();

            cashTransacionHist1.Active = "Y";
            cashTransacionHist1.CashTransacionId = cashTransacionHist.CashTransacionId;
            cashTransacionHist1.FieldName = cashTransacionHist.FieldName;
            cashTransacionHist1.PrevValue = cashTransacionHist.PrevValue;
            cashTransacionHist1.CurrentValue = cashTransacionHist.CurrentValue;

            InsertUpdateSaveCashTransacionHist(cashTransacionHist1, saveChanges);
        }
        private void InsertUpdateSaveCashTransacionHist(CashTransacionHist cashTransacionHist1, bool saveChanges)
        {
            if (cashTransacionHist1.Id == Guid.Empty)
            {
                cashTransacionHist1.Id = Guid.NewGuid();
                _repository.Add(cashTransacionHist1, false);
            }
            else
                _repository.Update(cashTransacionHist1, false);
            if (saveChanges)
                _repository.SaveChanges();
        }



        public List<CashTransacionHist> GetCashTransactionHist(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var cashTransAcionHistory = _repository.GetQuery<CashTransacionHist>().Where(a => a.CashTransacionId == id).OrderByDescending(a => a.CreatedDate).ToList();
            cashTransAcionHistory.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.CashTransacionId = Hsty.CashTransacionId;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return cashTransAcionHistory;
        }

        public List<AppDocument> GetCashTransacionAttachments(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<CashTransacionComment>().Where(a => a.CashTransacionId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "CASHTRANSACION") && fileAttachmentIds.Contains(a.TransactionId))
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
        public List<CashTransacionComment> GetCashTransAcionComments(Guid Id)
        {
            var Comments = _repository.GetQuery<CashTransacionComment>().Where(a => a.CashTransacionId == Id).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "CASHTRANSACION" && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.CashTransacionId = ele.CashTransacionId;
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


        private void InsertUpdateComment(CashTransacionComment cashTransacionComment)
        {

            if (cashTransacionComment.Id == Guid.Empty)
            {
                cashTransacionComment.Id = Guid.NewGuid();
                _repository.Add(cashTransacionComment, true);

            }
            else
            {

                _repository.Update(cashTransacionComment, true);
            }

        }

    }

}