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
    public class DirInvPostPayHistCommentsBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public DirInvPostPayHistCommentsBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public List<DirInvPostPayComment> saveDirInvPostPayComment(DirInvPostPayComment comment)
        {

            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            comment.Active = "Y";

            inserUpdatesaveDirInvPostPayComment(comment);
            var id = comment.Id;
            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var item in comment.AppDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = "DIRINVPOSTPAYCOMMENT",
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


            return getDirInvPostPayComment(comment.DirectInvPostPaymentId);


        }
        public List<AppDocument> getDirInvPostPayAttachments(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<DirInvPostPayComment>().Where(a => a.DirectInvPostPaymentId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y" && a.TransactionType == "DIRINVPOSTPAYCOMMENT"
                  && fileAttachmentIds.Contains(a.TransactionId))
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
        private void inserUpdatesaveDirInvPostPayComment(DirInvPostPayComment dirInvPostPayComment)
        {
            try
            {
                if (dirInvPostPayComment.Id == Guid.Empty)
                {
                    dirInvPostPayComment.Id = Guid.NewGuid();
                    _repository.Add(dirInvPostPayComment, true);
                }
                else
                {
                    _repository.Update(dirInvPostPayComment, true);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<DirInvPostPayComment> getDirInvPostPayComment(Guid id)
        {
            var Comments = _repository.GetQuery<DirInvPostPayComment>().Where(a => a.DirectInvPostPaymentId == id).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y" && a.TransactionType == "DIRINVPOSTPAYCOMMENT"
                  && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.DirectInvPostPaymentId = ele.DirectInvPostPaymentId;
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
        public void saveDirInvPostPayHistory(DirInvPostPayHist History, bool saveChanges)
        {
            DirInvPostPayHist budgAllocHdrComment1 = new DirInvPostPayHist();

            budgAllocHdrComment1.Active = "Y";
            budgAllocHdrComment1.Id = History.Id;
            budgAllocHdrComment1.DirectInvPostPaymentId = History.DirectInvPostPaymentId;
            budgAllocHdrComment1.FieldName = History.FieldName;
            budgAllocHdrComment1.PrevValue = History.PrevValue;
            budgAllocHdrComment1.CurrentValue = History.CurrentValue;

            inserUpdatesaveDirInvPostPayHistroy(budgAllocHdrComment1, saveChanges);
        }

        private void inserUpdatesaveDirInvPostPayHistroy(DirInvPostPayHist dirInvPostPayHistroy, bool saveChanges)
        {
            if (dirInvPostPayHistroy.Id == Guid.Empty)
            {
                dirInvPostPayHistroy.Id = Guid.NewGuid();
                _repository.Add(dirInvPostPayHistroy, true);
            }
            else
            {
                _repository.Update(dirInvPostPayHistroy, true);
            }
            if (saveChanges)
                _repository.SaveChanges();
        }
        public List<DirInvPostPayHist> getDirInvPostPayHistroy(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var budgAllocHdrsHistory = _repository.GetQuery<DirInvPostPayHist>().Where(a => a.DirectInvPostPaymentId == id).OrderByDescending(a => a.CreatedDate).ToList();
            budgAllocHdrsHistory.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.DirectInvPostPaymentId = Hsty.DirectInvPostPaymentId;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return budgAllocHdrsHistory;
        }

        public void saveDirInvPostPayStatusHist(DirInvPostPayStatusHist comment, bool saveChanges)
        {
            DirInvPostPayStatusHist dirInvPostPayHistroyStatus = new DirInvPostPayStatusHist();

            dirInvPostPayHistroyStatus.Active = "Y";
            dirInvPostPayHistroyStatus.Comments = comment.Comments;
            dirInvPostPayHistroyStatus.Status = comment.Status;
            dirInvPostPayHistroyStatus.DirectInvPostPaymentId = comment.DirectInvPostPaymentId;

            inserUpdateDirInvPostPayHistroyStatus(dirInvPostPayHistroyStatus, saveChanges);
        }
        private void inserUpdateDirInvPostPayHistroyStatus(DirInvPostPayStatusHist dirInvPostPayHistroyStatus, bool saveChanges)
        {

            if (dirInvPostPayHistroyStatus.Id == Guid.Empty)
            {
                dirInvPostPayHistroyStatus.Id = Guid.NewGuid();
                _repository.Add(dirInvPostPayHistroyStatus, true);
            }
            else
            {
                _repository.Update(dirInvPostPayHistroyStatus, true);
            }
            if (saveChanges)
                _repository.SaveChanges();
        }
        public List<DirInvPostPayStatusHist> getDirInvPostPayStatusHist(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var budgAllocHdrs = _repository.GetQuery<DirInvPostPayStatusHist>().Where(a => a.DirectInvPostPaymentId == id).OrderByDescending(a => a.CreatedDate).ToList();
            budgAllocHdrs.ForEach(Status =>
            {
                var userName = usemaster.Where(a => a.Id == Status.CreatedBy).FirstOrDefault();
                Status.Id = Status.Id;
                Status.DirectInvPostPaymentId = Status.DirectInvPostPaymentId;
                Status.Comments = Status.Comments;
                Status.Status = Status.Status;
                Status.CreatedDate = Status.CreatedDate;
                if (userName != null)
                    Status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return budgAllocHdrs;
        }

    }
}
