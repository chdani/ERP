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
    public class BudgetDetHistoryCommentsBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public BudgetDetHistoryCommentsBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public List<BudgAllocDetComment> SaveBudgtAllocDetComment(BudgAllocDetComment budgAllocDetComment)
        {

            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            budgAllocDetComment.Active = "Y";

            InsertUpdateDetComment(budgAllocDetComment);
            var id = budgAllocDetComment.Id;
            if (budgAllocDetComment.AppDocuments != null && budgAllocDetComment.AppDocuments.Count > 0)
            {
                foreach (var item in budgAllocDetComment.AppDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = "BUDGETALLODETCMTS",
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


            return GetBudgtAllocDetComments(budgAllocDetComment.BudgAllocDetId);
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
        private void InsertUpdateSaveBudgAllocDetHist(BudgAllocDetHist budgAllocHdrComment1)
        {
            try
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
            catch (Exception e)
            {
                throw e;
            }
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

        public List<AppDocument> GetBudgAlocDetAttachments(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<BudgAllocDetComment>().Where(a => a.BudgAllocDetId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "BUDGTALLOCDETATT") && fileAttachmentIds.Contains(a.TransactionId))
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
        public List<BudgAllocDetComment> GetBudgtAllocDetComments(Guid Id)
        {
            var Comments = _repository.GetQuery<BudgAllocDetComment>().Where(a => a.BudgAllocDetId == Id).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "BUDGETALLODETCMTS" && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.BudgAllocDetId = ele.BudgAllocDetId;
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
        private void InsertUpdateDetComment(BudgAllocDetComment budgAllocDetComment1)
        {
            try
            {
                if (budgAllocDetComment1.Id == Guid.Empty)
                {
                    budgAllocDetComment1.Id = Guid.NewGuid();
                    _repository.Add(budgAllocDetComment1, true);

                }
                else
                {

                    _repository.Update(budgAllocDetComment1, true);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

}