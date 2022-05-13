using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class DirInvPrePayHistoryCommentsBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public DirInvPrePayHistoryCommentsBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        //DirInvPrePayStatusHistFunction(GetById,Save,Inactive)

        public List<DirInvPrePayStatusHist> GetDirInvPrePayStatusHistListById(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var dirInvPrePayHdrs = _repository.GetQuery<DirInvPrePayStatusHist>().Where(a => a.DirectInvPrePaymentId == Id).OrderByDescending(a => a.CreatedDate).ToList();
            dirInvPrePayHdrs.ForEach(Status =>
            {
                var userName = usemaster.Where(a => a.Id == Status.CreatedBy).FirstOrDefault();
                Status.Id = Status.Id;
                Status.Comments = Status.Comments;
                Status.Status = Status.Status;
                Status.CreatedDate = Status.CreatedDate;
                if (userName != null)
                    Status.UserName = userName.FirstName + " " + userName.LastName;
            });
            return dirInvPrePayHdrs;
        }

        public void SaveDirInvPrePayStatusHist(DirInvPrePayStatusHist dirInvPrePayStatusHist, bool saveChanges)
        {
            DirInvPrePayStatusHist dirInvPrePayStatusHist1 = new DirInvPrePayStatusHist();

            dirInvPrePayStatusHist1.Active = "Y";
            dirInvPrePayStatusHist1.DirectInvPrePaymentId = dirInvPrePayStatusHist.Id;
            dirInvPrePayStatusHist1.Status = dirInvPrePayStatusHist.Status;
            dirInvPrePayStatusHist1.Comments = dirInvPrePayStatusHist.Comments;

            InsertDirInvPrePayStatusHist(dirInvPrePayStatusHist1, saveChanges);
        }
        private void InsertDirInvPrePayStatusHist(DirInvPrePayStatusHist AdddirInvPrePayStatusHist, bool saveChanges)
        {
            if (AdddirInvPrePayStatusHist.Id == Guid.Empty)
            {
                AdddirInvPrePayStatusHist.Id = Guid.NewGuid();
                _repository.Add(AdddirInvPrePayStatusHist, false);
            }
            else
                _repository.Update(AdddirInvPrePayStatusHist, false);

            if (saveChanges)
                _repository.SaveChanges();
        }


        //DirInvPrePayHistFunctions(GetById,Save,Inactive)
        public List<DirInvPrePayHist> GetDirInvPrePayHistListById(Guid directInvPrePaymentId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var dirInvPrePayHdrsHistory = _repository.GetQuery<DirInvPrePayHist>().Where(a => a.DirectInvPrePaymentId == directInvPrePaymentId).OrderByDescending(a => a.CreatedDate).ToList();
            dirInvPrePayHdrsHistory.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return dirInvPrePayHdrsHistory;
        }

        public void SaveDirInvPrePayHist(DirInvPrePayHist dirInvPrePayHist, bool saveChanges)
        {
            DirInvPrePayHist dirInvPrePayHist1 = new DirInvPrePayHist();

            dirInvPrePayHist1.Active = "Y";
            dirInvPrePayHist1.DirectInvPrePaymentId = dirInvPrePayHist.DirectInvPrePaymentId;
            dirInvPrePayHist1.FieldName = dirInvPrePayHist.FieldName;
            dirInvPrePayHist1.PrevValue = dirInvPrePayHist.PrevValue;
            dirInvPrePayHist1.CurrentValue = dirInvPrePayHist.CurrentValue;

            InsertDirInvPrePayHist(dirInvPrePayHist1, saveChanges);
        }
        private void InsertDirInvPrePayHist(DirInvPrePayHist AdddirInvPrePayHist, bool saveChanges)
        {
            if (AdddirInvPrePayHist.Id == Guid.Empty)
            {
                AdddirInvPrePayHist.Id = Guid.NewGuid();
                _repository.Add(AdddirInvPrePayHist, false);
            }
            else
                _repository.Update(AdddirInvPrePayHist, false);
            if (saveChanges)
                _repository.SaveChanges();
        }


        //DirInvPrePayCommentFunctions(GetById,Save,Inactive)
        public List<DirInvPrePayComment> GetDirInvPrePayCommentListById(Guid directInvPrePaymentId)
        {
            var Comments = _repository.GetQuery<DirInvPrePayComment>().Where(a => a.DirectInvPrePaymentId == directInvPrePaymentId).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "DIRINVPRECMTATT" && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.DirectInvPrePaymentId = ele.DirectInvPrePaymentId;
                ele.Comments = ele.Comments;
                ele.CreatedDate = ele.CreatedDate;
                if (userName != null)
                    ele.UserName = userName.FirstName + " " + userName.LastName;
                if (appDoucument != null)
                {
                    foreach (var doc in appDoucument)
                        doc.FileContent = null;

                    ele.appDocuments = appDoucument;
                }

            });

            return Comments.OrderByDescending(a => a.CreatedDate).ToList();
        }
        public List<AppDocument> GetDirInvPrePayAttachmentsById(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<DirInvPrePayComment>().Where(a => a.DirectInvPrePaymentId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "DIRINVPRECMTATT") && fileAttachmentIds.Contains(a.TransactionId))
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

        public List<DirInvPrePayComment> SaveDirInvPrePayComment(DirInvPrePayComment dirInvPrePayComment)
        {
            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            dirInvPrePayComment.Active = "Y";

            InsertDirInvPrePayComment(dirInvPrePayComment);
            var id = dirInvPrePayComment.Id;
            if (dirInvPrePayComment.appDocuments != null && dirInvPrePayComment.appDocuments.Count > 0)
            {
                foreach (var item in dirInvPrePayComment.appDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = "DIRINVPRECMTATT",
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


            return GetDirInvPrePayCommentListById(dirInvPrePayComment.DirectInvPrePaymentId);
        }

        private void InsertDirInvPrePayComment(DirInvPrePayComment addDirInvPrePayComment)
        {
            if (addDirInvPrePayComment.Id == Guid.Empty)
            {
                addDirInvPrePayComment.Id = Guid.NewGuid();
                _repository.Add(addDirInvPrePayComment, false);
            }
            else
                _repository.Update(addDirInvPrePayComment, false);
            _repository.SaveChanges();
        }
    }

}