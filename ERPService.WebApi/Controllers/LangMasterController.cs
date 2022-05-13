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
    public class LangMasterController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public LangMasterController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }

        [HttpPost]
        [Route("getLangMasterByKeyAndType")]
        [Authorize]
        public List<LangMaster> GetLangMasterByKeyAndType(LangMaster langMaster)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetLangMasterByKeyAndType(langMaster);
        }

        [HttpPost]
        [Route("saveLangMasters")]
        [Authorize]
        public AppResponse SaveLangMasters(List<LangMaster> langMasters)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.SaveLanguageMaster(langMasters);
        }
        [HttpPost]
        [Route("getTranslationDataByCodeType")]
        [Authorize]
        public AppTranslation GetTranslationDataByCodeType(AppTranslation appTranslation)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetTranslationDataByCodeType(appTranslation);
        }
        [HttpPost]
        [Route("saveTranslationData")]
        [Authorize]
        public AppResponse SaveTranslationData(AppTranslation appTranslation)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.SaveTranslationData(appTranslation);
        }
        [HttpPost]
        [Route("getLangBasedDataForSystemSettings")]
        [Authorize]
        public List<SystemSetting> GetLangBasedDataForSystemSettings(UserLanguage input)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetLangBasedDataForSystemSettings(input.Language);
        }
    }
}