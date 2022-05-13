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
    public class BudgetHistroyCommentsBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public BudgetHistroyCommentsBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public List<BudgAllocHdrComment> SaveBudgtAllocHdrComment(BudgAllocHdrComment budgAllocHdrComment)
        {

            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            budgAllocHdrComment.Active = "Y";

            InsertUpdateComment(budgAllocHdrComment);
            var id = budgAllocHdrComment.Id;
            if (budgAllocHdrComment.AppDocuments != null && budgAllocHdrComment.AppDocuments.Count > 0)
            {
                foreach (var item in budgAllocHdrComment.AppDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = "BUDGTALLOCHDRCMTS",
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


            return GetBudgtAllocHdrComments(budgAllocHdrComment.BudgAllocHdrId);
        }
        public List<BudgAllocDetHist> SaveBudgAllocDetHist(BudgAllocDetHist budgAllocDetComment)
        {
            BudgAllocDetHist budgAllocComment1 = new BudgAllocDetHist();

            budgAllocComment1.Active = "Y";
            budgAllocComment1.Id = budgAllocDetComment.Id;
            budgAllocComment1.BudgAllocDetId = budgAllocDetComment.BudgAllocDetId;
            budgAllocComment1.FieldName = budgAllocDetComment.FieldName;
            budgAllocComment1.PrevValue = budgAllocDetComment.PrevValue;
            budgAllocComment1.CurrentValue = budgAllocDetComment.CurrentValue;

            InsertUpdateSaveBudgAllocDetHist(budgAllocComment1);
            return _repository.GetQuery<BudgAllocDetHist>().Where(a => a.BudgAllocDetId == budgAllocComment1.BudgAllocDetId).ToList();
        }
        public void SaveBudgAllocHdrHist(List<BudgAllocHdrHist> budgAllocHdrHist, bool saveChanges)
        {
            foreach (var hist in budgAllocHdrHist)
                InsertUpdateBudAllocHdrHist(hist, saveChanges);
        }
        private void InsertUpdateSaveBudgAllocDetHist(BudgAllocDetHist budgAllocHdrComment1, bool saveChanges = true)
        {

            if (budgAllocHdrComment1.Id == Guid.Empty)
            {
                budgAllocHdrComment1.Id = Guid.NewGuid();
                _repository.Add(budgAllocHdrComment1, true);
            }
            else
            {
                _repository.Update(budgAllocHdrComment1, true);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }
        private void InsertUpdateBudAllocHdrHist(BudgAllocHdrHist budgAllocHdrComment1, bool saveChanges = true)
        {
            if (budgAllocHdrComment1.Id == Guid.Empty)
            {
                budgAllocHdrComment1.Id = Guid.NewGuid();
                _repository.Add(budgAllocHdrComment1, true);
            }
            else
            {
                _repository.Update(budgAllocHdrComment1, true);
            }
            if (saveChanges)
                _repository.SaveChanges();
        }
        public void SaveStatusHistory(BudgAllocHdrStatusHist statushistroy, bool saveChanges = true)
        {
            BudgAllocHdrStatusHist statushistroy1 = new BudgAllocHdrStatusHist();

            statushistroy1.Active = "Y";
            statushistroy1.Id = statushistroy.Id;
            statushistroy1.Status = statushistroy.Status;
            statushistroy1.Comments = statushistroy.Comments;
            statushistroy1.BudgAllocHdrId = statushistroy.BudgAllocHdrId;

            InsertUpdateStatus(statushistroy1);
            if (saveChanges)
                _repository.SaveChanges();
        }
        public List<BudgAllocHdrStatusHist> getHistoryStatus(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var budgAllocHdrs = _repository.GetQuery<BudgAllocHdrStatusHist>().Where(a => a.BudgAllocHdrId == id).OrderByDescending(a => a.CreatedDate).ToList();
            budgAllocHdrs.ForEach(Status =>
            {
                var userName = usemaster.Where(a => a.Id == Status.CreatedBy).FirstOrDefault();
                Status.Id = Status.Id;
                Status.BudgAllocHdrId = Status.BudgAllocHdrId;
                Status.Comments = Status.Comments;
                Status.Status = Status.Status;
                Status.CreatedDate = Status.CreatedDate;
                if (userName != null)
                    Status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return budgAllocHdrs;
        }
        public List<BudgAllocHdrHist> GetBudgAlocHdrHist(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var budgAllocHdrsHistory = _repository.GetQuery<BudgAllocHdrHist>().Where(a => a.BudgAllocHdrId == id).OrderByDescending(a => a.CreatedDate).ToList();
            budgAllocHdrsHistory.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.BudgAllocHdrId = Hsty.BudgAllocHdrId;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return budgAllocHdrsHistory;
        }

        public List<AppDocument> GetBudgAlocHdrAttachments(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<BudgAllocHdrComment>().Where(a => a.BudgAllocHdrId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "BUDGTALLOCHDRCMTS" || a.TransactionType == "BUDGETALLOCATION") && fileAttachmentIds.Contains(a.TransactionId))
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
        public List<BudgAllocHdrComment> GetBudgtAllocHdrComments(Guid Id)
        {
            var Comments = _repository.GetQuery<BudgAllocHdrComment>().Where(a => a.BudgAllocHdrId == Id).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "BUDGTALLOCHDRCMTS" && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.BudgAllocHdrId = ele.BudgAllocHdrId;
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
        public List<BudgAllocDetHist> getBudgAlocDetHist(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var budgAllocDetailsHistory = _repository.GetQuery<BudgAllocDetHist>().Where(a => a.BudgAllocDetId == Id).ToList();
            budgAllocDetailsHistory.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.BudgAllocDetId = Hsty.BudgAllocDetId;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;

            });


            return budgAllocDetailsHistory;
        }
        private void InsertUpdateStatus(BudgAllocHdrStatusHist statushistroy1)
        {

            if (statushistroy1.Id == Guid.Empty)
            {
                statushistroy1.Id = Guid.NewGuid();
                _repository.Add(statushistroy1, true);
            }
            else
            {
                _repository.Update(statushistroy1, true);
            }

        }
        private void InsertUpdateComment(BudgAllocHdrComment budgAllocHdrComment1)
        {

            if (budgAllocHdrComment1.Id == Guid.Empty)
            {
                budgAllocHdrComment1.Id = Guid.NewGuid();
                _repository.Add(budgAllocHdrComment1, true);

            }
            else
            {

                _repository.Update(budgAllocHdrComment1, true);
            }

        }

    }

}