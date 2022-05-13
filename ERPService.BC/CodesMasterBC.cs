using ERPService.BC.Utility;
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
    public class CodesMasterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public CodesMasterBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        private void InsertUpdateCodeDetails(CodesDetails codesDetails, bool saveChanges)
        {

            if (codesDetails.Id == Guid.Empty)
            {
                codesDetails.Id = Guid.NewGuid();
                _repository.Add(codesDetails);
            }
            else
                _repository.Update(codesDetails);
            if (saveChanges)
                _repository.SaveChanges();

        }
        private void InsertUpdateCodeMaster(CodesMaster codesMaster, bool saveChanges)
        {
            if (codesMaster.Id == Guid.Empty)
            {
                codesMaster.Id = Guid.NewGuid();
                _repository.Add(codesMaster, false);
                if (codesMaster.CodesDetail != null && codesMaster.CodesDetail.Count > 0)
                {
                    foreach (var codes in codesMaster.CodesDetail)
                    {
                      
                        codes.CodesMasterId = codesMaster.Id;
                        InsertUpdateCodeDetails(codes, false);
                    }
                }

            }
            else
            {
                if (codesMaster.CodesDetail != null && codesMaster.CodesDetail.Count > 0)
                {
                    foreach (var codes in codesMaster.CodesDetail)
                    {
                     
                        codes.CodesMasterId = codesMaster.Id;
                        InsertUpdateCodeDetails(codes, false);
                    }
                }
                _repository.Update(codesMaster, false);
            }
            if (saveChanges)
            {
                _repository.SaveChanges();
            }

        }
        public AppResponse SaveCodeMaster(CodesMaster codesMasters)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;

            if (string.IsNullOrEmpty(codesMasters.Code) || string.IsNullOrEmpty(codesMasters.Description))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var codeMaster = _repository.GetQuery<CodesMaster>().Where(a => a.Id != codesMasters.Id && (a.Code == codesMasters.Code || a.Description == codesMasters.Description && a.Active == "Y")).FirstOrDefault();
            if (codeMaster != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICODEDES], codesMasters.Code, codesMasters.Description));
                validation = false;
            }

            if (codesMasters.Action != 'N' || codesMasters.Id != Guid.Empty)
            {
                codeMaster = _repository.GetById<CodesMaster>(codesMasters.Id);
                if (codeMaster != null)
                {
                    if (codeMaster.ModifiedDate != null && codeMaster.ModifiedDate != codesMasters.ModifiedDate)
                    {
                        validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.RECMODIOTERUSERCODEC], codesMasters.Code, codesMasters.Description));
                        validation = false;
                    }
                }
                else
                {
                    validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTEXITCODEDEC], codesMasters.Code, codesMasters.Description));
                    validation = false;
                }
            }
            var childMessages = new List<string>();
            var result = validateCodeDetails(codesMasters.CodesDetail, out childMessages);
            if (!result)
            {
                validation = result;
                validationMessages.AddRange(childMessages);
            }
            if (validation)
            {
                InsertUpdateCodeMaster(codesMasters, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

                //remove cache values from cache.
                AppGeneralMethods.RemoveCache(ERPCacheKey.CODES, _repository);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse SaveCodeMasterList(List<CodesMaster> codesMastersList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var codesMaster in codesMastersList)
            {
                InsertUpdateCodeMaster(codesMaster, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.CODES, _repository);
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                    appResponse.Messages = new List<string>();
                }
                catch (Exception ex)
                {
                    appResponse.Status = APPMessageKey.ONEORMOREERR;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ex.Message);
                }
            }
            return appResponse;
        }
        public AppResponse SaveCodesDetailsList(List<CodesDetails> codesDetailsList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var codesDetails in codesDetailsList)
            {
                InsertUpdateCodeDetails(codesDetails, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.CODES, _repository);
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                    appResponse.Messages = new List<string>();
                }
                catch (Exception ex)
                {
                    appResponse.Status = APPMessageKey.ONEORMOREERR;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ex.Message);
                }
            }
            return appResponse;
        }

        private bool validateCodeDetails(ICollection<CodesDetails> codesDetail, out List<string> validationMessages)
        {
            var validation = true;
            validationMessages = new List<string>();
            foreach (var det in codesDetail)
            {
                if (string.IsNullOrEmpty(det.Code) || string.IsNullOrEmpty(det.Description))
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CODEORDECMISS]);
                    validation = false;
                }

                var codeMaster = _repository.GetQuery<CodesMaster>().Where(a => a.Id != det.Id && (a.Code == det.Code || a.Description == det.Description)).FirstOrDefault();
                if (codeMaster != null)
                {
                    validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPCODEDETCODE_DEC], det.Code, det.Description));
                    validation = false;
                }


            }

            return validation;
        }

        public CodesMaster GetCodesMasterById(Guid codesMasterId)
        {
            var codemaster = _repository.Get<CodesMaster>(a => a.Id == codesMasterId);
            codemaster.CodesDetail = _repository.GetQuery<CodesDetails>().Where(a => a.CodesMasterId == codemaster.Id).OrderBy(a => a.DisplayOrder).ToList();
            return codemaster;
        }
        public List<CodesMaster> GetCodesMasterSerachFilter(CodesMaster codesMaster)
        {
            var codesMasters = _repository.GetQuery<CodesMaster>().Where(a =>
                 (string.IsNullOrEmpty(codesMaster.Description) || a.Description.Contains(codesMaster.Description))
                && (string.IsNullOrEmpty(codesMaster.Code) || a.Code.Contains(codesMaster.Code)) &&
                a.CodeType == "U" ).OrderByDescending(a => a.CreatedDate).ToList();
            if (codesMaster != null & codesMasters.Count > 0)
            {
                foreach (var master in codesMasters)
                    master.CodesDetail = _repository.GetQuery<CodesDetails>().Where(a => a.CodesMasterId == master.Id).Distinct().ToList();
            }
            return codesMasters;

        }
    }

}