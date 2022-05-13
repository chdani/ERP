using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class CodesMasterController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }
        public CodesMasterController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }
        [HttpGet]
        [Route("getCodesMasterById/{codeMasterId}")]
        [Authorize]
        public CodesMaster GetCodesMasterById(Guid codeMasterId)
        {
            CodesMasterBC codesMasterBC = new CodesMasterBC(_logger, _repository);
            return codesMasterBC.GetCodesMasterById(codeMasterId);
        }


        [HttpPost]
        [Route("saveCodesMaster")]
        [Authorize]
        public AppResponse SaveCodesMaster(CodesMaster codesMasterList)
        {
            var codeMasterBC = new CodesMasterBC(_logger, _repository);
            return codeMasterBC.SaveCodeMaster(codesMasterList);
        }

        [HttpPost]
        [Route("fetchAllCodesMasterDataLangBased")]
        [Authorize]
        public List<CodesMaster> FetchAllCodesMasterDataLangBased(UserLanguage input)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetLangBasedDataForCodeMaster(input.Language);
        }
        [HttpPost]
        [Route("getCodesMasterSerachFilter")]
        [Authorize]
        public List<CodesMaster> GetCodesMasterSerachFilter(CodesMaster codesMaster)
        {
            var codeMasterBC = new CodesMasterBC(_logger, _repository);
            return codeMasterBC.GetCodesMasterSerachFilter(codesMaster);
        }
        [HttpPost]
        [Route("getAllCodesMasterListByCode")]
        [Authorize]
        public List<CodesMaster> GetAllCodesMasterListByCode(CodesMaster codesMaster)
        {
            var codeMasterBC = new CodesMasterBC(_logger, _repository);
            return codeMasterBC.GetCodesMasterSerachFilter(codesMaster);
        }

    }
}