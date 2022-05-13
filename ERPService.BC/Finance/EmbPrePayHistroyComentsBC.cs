using ERPService.Common.Interfaces;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.BC.Finance
{
    public class EmbPrePayHistroyComentsBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public EmbPrePayHistroyComentsBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public List<EmbPrePaymentHdrComment> SaveEmbPrePaymentHdrHistComment(EmbPrePaymentHdrComment embPrePaymentHdrHist)
        {

            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            embPrePaymentHdrHist.Active = "Y";

            InsertUpdateComment(embPrePaymentHdrHist);
            var id = embPrePaymentHdrHist.Id;
            if (embPrePaymentHdrHist.AppDocuments != null && embPrePaymentHdrHist.AppDocuments.Count > 0)
            {
                foreach (var item in embPrePaymentHdrHist.AppDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = item.TransactionType,
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


            return GetEmbPrePaymentHdrHistComment(embPrePaymentHdrHist.EmbPrePaymentHdrId);
        }
        public List<EmbPrePaymentHdrComment> GetEmbPrePaymentHdrHistComment(Guid Id)
        {
            var Comments = _repository.GetQuery<EmbPrePaymentHdrComment>().Where(a => a.EmbPrePaymentHdrId == Id).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "EMBHDRCOMMENTS" && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.EmbPrePaymentHdrId = ele.EmbPrePaymentHdrId;
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
        private void InsertUpdateComment(EmbPrePaymentHdrComment budgAllocHdrComment1)
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

        public List<EmbPrePaymentHdrStatusHist> GetEmbPrePaymentHistoryStatus(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var EmbPrePaymentHdr = _repository.GetQuery<EmbPrePaymentHdrStatusHist>().Where(a => a.EmbPrePaymentHdrId == id).OrderByDescending(a => a.CreatedDate).ToList();
            EmbPrePaymentHdr.ForEach(Status =>
            {
                var userName = usemaster.Where(a => a.Id == Status.CreatedBy).FirstOrDefault();
                Status.Id = Status.Id;
                Status.EmbPrePaymentHdrId = Status.EmbPrePaymentHdrId;
                Status.Comments = Status.Comments;
                Status.Status = Status.Status;
                Status.CreatedDate = Status.CreatedDate;
                if (userName != null)
                    Status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return EmbPrePaymentHdr;
        }

        public List<EmbPrePaymentHdrHist> GetEmbPrePaymentHistory(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var EmbPrePaymentHdr = _repository.GetQuery<EmbPrePaymentHdrHist>().Where(a => a.EmbPrePaymentHdrId == id).OrderByDescending(a => a.CreatedDate).ToList();
            EmbPrePaymentHdr.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.EmbPrePaymentHdrId = Hsty.EmbPrePaymentHdrId;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return EmbPrePaymentHdr;
        }

        public List<AppDocument> GetEmbPrePaymentAttachments(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<EmbPrePaymentHdrComment>().Where(a => a.EmbPrePaymentHdrId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y" && a.TransactionType == "EMBHDRCOMMENTS"
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

        /////////////

        public List<EmbPrePaymentEmbDetComment> SaveEmbPrePaymentDetHistComment(EmbPrePaymentEmbDetComment prePaymentEmbDetComment)
        {

            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            prePaymentEmbDetComment.Active = "Y";

            InsertUpdateDetailsComment(prePaymentEmbDetComment);
            var id = prePaymentEmbDetComment.Id;
            if (prePaymentEmbDetComment.AppDocuments != null && prePaymentEmbDetComment.AppDocuments.Count > 0)
            {
                foreach (var item in prePaymentEmbDetComment.AppDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = item.TransactionType,
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


            return GetEmbPrePaymentDetHistComment(prePaymentEmbDetComment.EmbPrePaymentEmbDetId);
        }
        public List<EmbPrePaymentEmbDetComment> GetEmbPrePaymentDetHistComment(Guid Id)
        {
            var Comments = _repository.GetQuery<EmbPrePaymentEmbDetComment>().Where(a => a.EmbPrePaymentEmbDetId == Id).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "EMBDETCOMMENTS " && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.EmbPrePaymentEmbDetId = ele.EmbPrePaymentEmbDetId;
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
        private void InsertUpdateDetailsComment(EmbPrePaymentEmbDetComment budgAllocHdrComment1)
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



        public List<EmbPrePaymentEmbDetHist> GetEmbPrePaymentDetHistory(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var EmbPrePaymentHdr = _repository.GetQuery<EmbPrePaymentEmbDetHist>().Where(a => a.EmbPrePaymentEmbDetId == id).OrderByDescending(a => a.CreatedDate).ToList();
            EmbPrePaymentHdr.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.EmbPrePaymentEmbDetId = Hsty.EmbPrePaymentEmbDetId;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return EmbPrePaymentHdr;
        }
        public List<AppDocument> GetEmbPrePaymentdetAttachments(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<EmbPrePaymentEmbDetComment>().Where(a => a.EmbPrePaymentEmbDetId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y" && a.TransactionType == "EMBDETCOMMENTS"
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
        //////////////////////////////////////////////////////////////////////////
        public List<EmbPrePaymentInvDetComment> SaveEmbPreInvPaymentDetHistComment(EmbPrePaymentInvDetComment prePaymentInvDetComment)
        {

            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            prePaymentInvDetComment.Active = "Y";

            InsertUpdateInvDetailsComment(prePaymentInvDetComment);
            var id = prePaymentInvDetComment.Id;
            if (prePaymentInvDetComment.AppDocuments != null && prePaymentInvDetComment.AppDocuments.Count > 0)
            {
                foreach (var item in prePaymentInvDetComment.AppDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = item.TransactionType,
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


            return GetEmbPrePaymentInvDetHistComment(prePaymentInvDetComment.EmbPrePaymentInvDetId);
        }
        public List<EmbPrePaymentInvDetComment> GetEmbPrePaymentInvDetHistComment(Guid Id)
        {
            var Comments = _repository.GetQuery<EmbPrePaymentInvDetComment>().Where(a => a.EmbPrePaymentInvDetId == Id).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "EMBINVDETCOMMENTS " && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.EmbPrePaymentInvDetId = ele.EmbPrePaymentInvDetId;
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
        private void InsertUpdateInvDetailsComment(EmbPrePaymentInvDetComment prePaymentInvDetComment)
        {
            try
            {
                if (prePaymentInvDetComment.Id == Guid.Empty)
                {
                    prePaymentInvDetComment.Id = Guid.NewGuid();
                    _repository.Add(prePaymentInvDetComment, true);

                }
                else
                {

                    _repository.Update(prePaymentInvDetComment, true);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public List<EmbPrePaymentInvDetHist> GetEmbPrePaymentInvDetHistory(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var EmbPrePaymentHdr = _repository.GetQuery<EmbPrePaymentInvDetHist>().Where(a => a.EmbPrePaymentInvDetId == id).OrderByDescending(a => a.CreatedDate).ToList();
            EmbPrePaymentHdr.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.EmbPrePaymentInvDetId = Hsty.EmbPrePaymentInvDetId;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return EmbPrePaymentHdr;
        }
        public List<AppDocument> GetEmbPrePaymentInvdetAttachments(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<EmbPrePaymentInvDetComment>().Where(a => a.EmbPrePaymentInvDetId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y" && a.TransactionType == "EMBINVDETCOMMENTS"
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
    }

}






