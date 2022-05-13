using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class LangMasterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private ObjectCache cache = MemoryCache.Default;
        public LangMasterBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public AppResponse SaveLanguageMaster(List<LangMaster> LangMaster)
        {
            AppResponse appResponse = new AppResponse();

            var langDB = _repository.GetQuery<LangMaster>().ToList();
            var validation = true;

            foreach (var lang in LangMaster)
            {
                if (langDB.Count(a => a.Id != lang.Id && a.Code == lang.Code) > 0)
                {
                    validation = false;
                    appResponse.Status = APPMessageKey.DUPLICATE;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                }

                if (string.IsNullOrEmpty(lang.Code) || string.IsNullOrEmpty(lang.Description) || string.IsNullOrEmpty(lang.CodeType) || string.IsNullOrEmpty(lang.Language))
                {
                    validation = false;
                    appResponse.Status = APPMessageKey.MANDMISSING;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                }
            }

            if (validation)
            {
                foreach (var lang in LangMaster)
                {
                    if (lang.Id != Guid.Empty)
                        _repository.Update(lang, false);
                    else
                    {
                        lang.Id = Guid.NewGuid();
                        _repository.Add(lang, false);
                    }
                }
                _repository.SaveChanges();
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
                appResponse.Status = APPMessageKey.ONEORMOREERR;
            return appResponse;
        }

        public List<LangMaster> GetLangMasterByKeyAndType(LangMaster langMaster)
        {
            List<LangMaster> languages = null;
            if (langMaster != null)
            {
                languages = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == langMaster.Language && langMaster.CodeType == a.CodeType && a.Active == "Y").ToList();
            }
            return languages;
        }
        public List<CodesMaster> GetLangBasedDataForCodeMaster(string language)
        {
            List<CodesMaster> codesMasters = new List<CodesMaster>();

            if (string.IsNullOrEmpty(language)) language = "en";

            var cacheKey = ERPCacheKey.CODES + language;
            if (cache.Contains(cacheKey))
                codesMasters = (List<CodesMaster>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.CODES && a.Active == "Y").ToList();

                var codesMaster = _repository.GetQuery<CodesMaster>();
                var codesDet = _repository.GetQuery<CodesDetails>();
                var codemasterList = from cm in codesMaster
                                     join cd in codesDet on cm.Id equals cd.CodesMasterId
                                     select new
                                     {
                                         Code = cm.Code,
                                         Description = cm.Description,
                                         Id = cm.Id,
                                         CodeDetail = cd
                                     };
                var masters = codemasterList.GroupBy(a => new { a.Id, a.Code, a.Description });
                foreach (var cml in masters)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == cml.Key.Code);
                    var lang = new CodesMaster();

                    if (langOther == null)
                    {
                        lang.Code = cml.Key.Code;
                        lang.Description = cml.Key.Description;
                    }
                    else
                    {
                        lang.Description = langOther.Description;
                        lang.Code = cml.Key.Code;
                    }
                    lang.Id = cml.Key.Id;

                    lang.CodesDetail = new List<CodesDetails>();
                    foreach (var det in cml.ToList())
                    {
                        langOther = langMastr.FirstOrDefault(a => a.Code == det.CodeDetail.Code);
                        if (langOther == null)
                        {
                            var codeDetail = new CodesDetails()
                            {
                                Code = det.CodeDetail.Code,
                                Description = det.CodeDetail.Description,
                                DisplayOrder = det.CodeDetail.DisplayOrder,
                                Id = det.CodeDetail.Id,
                                Active = det.CodeDetail.Active
                            };
                            lang.CodesDetail.Add(codeDetail);
                        }
                        else
                        {
                            var codeDetail = new CodesDetails()
                            {
                                Code = langOther.Code,
                                Description = langOther.Description,
                                DisplayOrder = det.CodeDetail.DisplayOrder,
                                Id = det.CodeDetail.Id,
                                Active = det.CodeDetail.Active
                            };
                            lang.CodesDetail.Add(codeDetail);
                        }
                    }
                    lang.CodesDetail = lang.CodesDetail.OrderBy(a => a.DisplayOrder).ToList();
                    codesMasters.Add(lang);
                }
                cache.Set(cacheKey, codesMasters, DateTime.Now.AddDays(1));
            }
            return codesMasters;
        }

        public List<AppMenuMaster> GetLangBasedDataForMenuMaster(string language)
        {
            List<AppMenuMaster> menuMasters = null;

            var cacheKey = ERPCacheKey.MENUS + language;
            if (cache.Contains(cacheKey))
                menuMasters = (List<AppMenuMaster>)cache.Get(cacheKey);
            else
            {
                var langMainmenuMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.MENUS && a.Active == "Y").ToList();

                var langSubmenuMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.MENUS && a.Active == "Y").ToList();

                var appMenuMasters = _repository.GetQuery<AppMenuMaster>().ToList();

                var masters = appMenuMasters.OrderBy(a => new { a.MainMenuDispOrd, a.DispOrder });
                foreach (var cml in masters)
                {
                    var langOther = langMainmenuMastr.FirstOrDefault(a => a.Code == cml.MainMenuCode);
                    var langSubMenuOther = langSubmenuMastr.FirstOrDefault(a => a.Code == cml.SubMenuCode);
                    var lang = new AppMenuMaster();

                    if (langOther == null)
                    {
                        lang.MainMenuCode = cml.MainMenuCode;
                        lang.MainMenuName = cml.MainMenuName;
                        lang.MainMenuDispOrd = cml.MainMenuDispOrd;
                        lang.MainMenuIcon = cml.MainMenuIcon;
                    }
                    else
                    {
                        lang.MainMenuCode = langOther.Code;
                        lang.MainMenuName = langOther.Description;
                        lang.MainMenuDispOrd = langOther.DisplayOrder;
                        lang.MainMenuIcon = cml.MainMenuIcon;
                    }

                    if (langSubMenuOther == null)
                    {
                        lang.SubMenuCode = cml.SubMenuCode;
                        lang.SubMenuName = cml.SubMenuName;
                        lang.DispOrder = cml.DispOrder;
                        lang.SubMenuIcon = cml.SubMenuIcon;
                    }
                    else
                    {
                        lang.SubMenuCode = langSubMenuOther.Code;
                        lang.SubMenuName = langSubMenuOther.Description;
                        lang.DispOrder = langSubMenuOther.DisplayOrder;
                        lang.SubMenuIcon = cml.SubMenuIcon;
                    }
                    lang.AppAccessId = cml.AppAccessId;
                    lang.Id = cml.Id;

                    menuMasters.Add(lang);
                }
                cache.Set(cacheKey, menuMasters, DateTime.Now.AddDays(1));
            }
            return menuMasters;
        }

        public List<UserRole> GetLangBasedDataForRoleMaster(string language)
        {
            List<UserRole> appRoles = null;

            var cacheKey = ERPCacheKey.ROLES + language;
            if (cache.Contains(cacheKey))
                appRoles = (List<UserRole>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.ROLES && a.Active == "Y").ToList();

                appRoles = _repository.GetQuery<UserRole>().ToList();

                foreach (var role in appRoles)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == role.RoleCode);

                    if (langOther != null)
                        role.RoleName = langOther.Description;
                }
                cache.Set(cacheKey, appRoles, DateTime.Now.AddDays(1));
            }
            return appRoles;
        }

        public List<AppAccess> GetLangBasedDataForAppAccess(string language)
        {
            List<AppAccess> appAccesses = null;

            var cacheKey = ERPCacheKey.ACCESS + language;
            if (cache.Contains(cacheKey))
                appAccesses = (List<AppAccess>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.ACCESS && a.Active == "Y").ToList();

                appAccesses = _repository.GetQuery<AppAccess>().ToList();

                foreach (var access in appAccesses)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == access.AccessCode);

                    if (langOther != null)
                        access.AccessName = langOther.Description;
                }
                cache.Set(cacheKey, appAccesses, DateTime.Now.AddDays(1));
            }
            return appAccesses;
        }

        public List<LedgerAccount> GetLangBasedDataForLedgerAccounts(string language)
        {
            List<LedgerAccount> ledgers = null;

            var cacheKey = ERPCacheKey.LEDGERACC + language;
            if (cache.Contains(cacheKey))
                ledgers = (List<LedgerAccount>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.LEDGERACC && a.Active == "Y").ToList();

                ledgers = _repository.GetQuery<LedgerAccount>().ToList();

                foreach (var ledger in ledgers)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == ledger.LedgerCode.ToString());

                    if (langOther != null)
                        ledger.LedgerDesc = langOther.Description;
                }
                cache.Set(cacheKey, ledgers, DateTime.Now.AddDays(1));
            }
            return ledgers;
        }

        public List<CostCenter> GetLangBasedDataForCostCenters(string language)
        {
            List<CostCenter> costCenters = null;

            var cacheKey = ERPCacheKey.COSTCENTERS + language;
            if (cache.Contains(cacheKey))
                costCenters = (List<CostCenter>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.LEDGERACC && a.Active == "Y").ToList();

                costCenters = _repository.GetQuery<CostCenter>().ToList();

                foreach (var cc in costCenters)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == cc.Code);

                    if (langOther != null)
                        cc.Description = langOther.Description;
                }
                cache.Set(cacheKey, costCenters, DateTime.Now.AddDays(1));
            }
            return costCenters;
        }

        public List<ProdUnitMaster> GetLangBasedDataForProdUnitMaster(string language)
        {
            List<ProdUnitMaster> languages = new List<ProdUnitMaster>();

            var cacheKey = ERPCacheKey.UNITS + language;
            if (cache.Contains(cacheKey))
                languages = (List<ProdUnitMaster>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.UNITS && a.Active == "Y").ToList();

                languages = _repository.GetQuery<ProdUnitMaster>().ToList();

                foreach (var cc in languages)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == cc.UnitCode);

                    if (langOther != null)
                        cc.UnitName = langOther.Description;
                }

                cache.Set(cacheKey, languages, DateTime.Now.AddDays(1));
            }
            return languages;
        }
        public List<VendorMaster> GetLangBasedDataForVendorMaster(string language)
        {
            List<VendorMaster> vendorMasters = new List<VendorMaster>();

            var cacheKey = ERPCacheKey.VENDORMASTER + language;
            if (cache.Contains(cacheKey))
                vendorMasters = (List<VendorMaster>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.VENDORMASTER && a.Active == "Y").ToList();

                vendorMasters = _repository.GetQuery<VendorMaster>().ToList();

                foreach (var vendors in vendorMasters)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == vendors.Title);

                    if (langOther != null)
                        vendors.Title = langOther.Description;
                }

                cache.Set(cacheKey, vendorMasters, DateTime.Now.AddDays(1));
            }
            return vendorMasters;
        }



        public List<ProductMaster> GetLangBasedDataForProductMaster(string language)
        {
            List<ProductMaster> products = new List<ProductMaster>();

            var cacheKey = ERPCacheKey.PRODUCTMASTER + language;
            if (cache.Contains(cacheKey))
                products = (List<ProductMaster>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.PRODUCTMASTER && a.Active == "Y").ToList();

                products = _repository.GetQuery<ProductMaster>().ToList();

                foreach (var cc in products)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == cc.ProdCode);

                    if (langOther != null)
                        cc.ProdDescription = langOther.Description;
                }

                cache.Set(cacheKey, products, DateTime.Now.AddDays(1));
            }
            return products;
        }


        public List<ProductCategory> GetLangBasedDataForProductCategory(string language)
        {
            List<ProductCategory> prodCats = new List<ProductCategory>();

            var cacheKey = ERPCacheKey.PRODUCTCATEGORY + language;
            if (cache.Contains(cacheKey))
                prodCats = (List<ProductCategory>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.PRODUCTCATEGORY && a.Active == "Y").ToList();

                prodCats = _repository.GetQuery<ProductCategory>().ToList();

                foreach (var cc in prodCats)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == cc.Code);

                    if (langOther != null)
                        cc.Name = langOther.Description;
                }

                cache.Set(cacheKey, prodCats, DateTime.Now.AddDays(1));
            }
            return prodCats;
        }
        public List<ProdSubCategory> FetchLangBasedDataForProductSubCategory(string language)
        {
            List<ProdSubCategory> prodSubCats = new List<ProdSubCategory>();

            var cacheKey = ERPCacheKey.PRODUCTSUBCATEGORY + language;
            if (cache.Contains(cacheKey))
                prodSubCats = (List<ProdSubCategory>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.PRODUCTSUBCATEGORY && a.Active == "Y").ToList();

                prodSubCats = _repository.GetQuery<ProdSubCategory>().ToList();

                foreach (var subCat in prodSubCats)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == subCat.Code);

                    if (langOther != null)
                        subCat.Name = langOther.Description;
                }

                cache.Set(cacheKey, prodSubCats, DateTime.Now.AddDays(1));
            }
            return prodSubCats;
        }

        public List<Department> GetLangBasedDataForDepartments(string language)
        {
            List<Department> departments = null;

            var cacheKey = ERPCacheKey.DEPARTMENT + language;
            if (cache.Contains(cacheKey))
                departments = (List<Department>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                    .Where(a => a.Language == language && a.CodeType == ERPCacheKey.DEPARTMENT && a.Active == "Y").ToList();

                departments = _repository.GetQuery<Department>().ToList();

                foreach (var department in departments)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == department.Code.ToString());

                    if (langOther != null)
                        department.Name = langOther.Description;
                }
                cache.Set(cacheKey, departments, DateTime.Now.AddDays(1));
            }
            return departments;
        }

        public List<SystemSetting> GetLangBasedDataForSystemSettings(string language)
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.SYSTEMSETTING + language;
            var systemSettings = new List<SystemSetting>();
            if (cache.Contains(cacheKey))
                systemSettings = (List<SystemSetting>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                        .Where(a => a.Language == language && a.CodeType == ERPCacheKey.SYSTEMSETTING && a.Active == "Y").ToList();

                systemSettings = _repository.List<SystemSetting>(q => q.Active == "Y");
                foreach (var setting in systemSettings)
                {
                    var langOther = langMastr.FirstOrDefault(a => a.Code == setting.ConfigKey.ToString());

                    if (langOther != null)
                        setting.LangDescription = langOther.Description;
                    else
                        setting.LangDescription = setting.Description;
                }
                cache.Set(cacheKey, systemSettings, DateTime.Now.AddDays(2));
            }
            return systemSettings;
        }

        public AppTranslation GetTranslationDataByCodeType(AppTranslation appTranslation)
        {
            AppTranslation appTranslationData = new AppTranslation();
            appTranslationData.TranslationData = new List<AppTranslationMaster>();
            appTranslationData.CodeType = appTranslation.CodeType;
            var langMaster = _repository.GetQuery<LangMaster>().Where(a => a.CodeType == appTranslation.CodeType).ToList();
            switch (appTranslation.CodeType)
            {
                case "CODESMASTER":
                    var codesMasterList = _repository.GetQuery<CodesMaster>().Where(a => a.Active == "Y");
                    foreach (var codesMaster in codesMasterList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == codesMaster.Code);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = codesMaster.Id,
                            Code = codesMaster.Code,
                            EnglishDescription = codesMaster.Description,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = codesMaster.ModifiedDate,
                            ModifiedBy = codesMaster.ModifiedBy

                        });

                    }


                    break;
                case "ACCESS":

                    var appAccessesList = _repository.GetQuery<AppAccess>().Where(a => a.Active == "Y");
                    foreach (var appAccesses in appAccessesList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == appAccesses.AccessCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = appAccesses.Id,
                            Code = appAccesses.AccessCode,
                            EnglishDescription = appAccesses.AccessName,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            CreatedBy = appAccesses.CreatedBy,
                            ModifiedDate = appAccesses.ModifiedDate,
                            ModifiedBy = appAccesses.ModifiedBy


                        });
                    }

                    break;
                case "CODESMASTERDETAILS":

                    var codesDetailsList = _repository.GetQuery<CodesDetails>().Where(a => a.Active == "Y");

                    foreach (var codesDetails in codesDetailsList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == codesDetails.Code);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = codesDetails.Id,
                            Code = codesDetails.Code,
                            EnglishDescription = codesDetails.Description,
                            EnglishDisplayOrder = codesDetails.DisplayOrder,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ArabicDisplayOrder = langMasterCodeBasedData == null ? 1 : langMasterCodeBasedData.DisplayOrder,
                            CreatedBy = codesDetails.CreatedBy,
                            CreatedDate = codesDetails.CreatedDate,
                            ModifiedDate = codesDetails.ModifiedDate,
                            ModifiedBy = codesDetails.ModifiedBy


                        });


                    }

                    break;
                case "UNITS":
                    var prodUnitMasterList = _repository.GetQuery<ProdUnitMaster>().Where(a => a.Active == "Y");

                    foreach (var prodUnitMaster in prodUnitMasterList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == prodUnitMaster.UnitCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = prodUnitMaster.Id,
                            Code = prodUnitMaster.UnitCode,
                            EnglishDescription = prodUnitMaster.UnitName,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = prodUnitMaster.ModifiedDate,
                            ModifiedBy = prodUnitMaster.ModifiedBy


                        });

                    }
                    break;
                case "MENUS":

                    var appMenuMasterList = _repository.GetQuery<AppMenuMaster>().Where(a => a.Active == "Y").ToList();

                    if (appMenuMasterList != null)
                    {
                        var appMenuMasterCodeDistinctList = appMenuMasterList.Select(s => s.MainMenuCode).Distinct().ToList();

                        if (appMenuMasterCodeDistinctList != null)
                        {
                            appMenuMasterCodeDistinctList.ForEach(appMenuMasterCodeDistinctData =>
                            {
                                if (appMenuMasterCodeDistinctData != null)
                                {
                                    var appMenuMastersData = appMenuMasterList.FirstOrDefault(a => a.MainMenuCode == appMenuMasterCodeDistinctData);
                                    var langMasterData = langMaster.FirstOrDefault(a => a.Code == appMenuMasterCodeDistinctData);

                                    appTranslationData.TranslationData.Add(new AppTranslationMaster
                                    {
                                        Id = appMenuMastersData.Id,
                                        Code = appMenuMastersData.MainMenuCode,
                                        EnglishDescription = appMenuMastersData.MainMenuName,
                                        EnglishDisplayOrder = appMenuMastersData.MainMenuDispOrd,
                                        ArabicDescription = langMasterData?.Description,
                                        ArabicDisplayOrder = langMasterData == null ? 1 : langMasterData.DisplayOrder,
                                        ModifiedDate = appMenuMastersData.ModifiedDate,
                                        ModifiedBy = appMenuMastersData.ModifiedBy
                                    });

                                }
                            });
                        }
                    }
                    break;
                case "SUBMENUS":
                    var appMenuMasterLists = _repository.GetQuery<AppMenuMaster>().Where(a => a.Active == "Y");

                    foreach (var appMenuMaster in appMenuMasterLists)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == appMenuMaster.SubMenuCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = appMenuMaster.Id,
                            Code = appMenuMaster.SubMenuCode,
                            EnglishDescription = appMenuMaster.SubMenuName,
                            EnglishDisplayOrder = appMenuMaster.DispOrder,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ArabicDisplayOrder = langMasterCodeBasedData == null ? 1 : langMasterCodeBasedData.DisplayOrder,
                            ModifiedDate = appMenuMaster.ModifiedDate,
                            ModifiedBy = appMenuMaster.ModifiedBy


                        });

                    }

                    break;
                case "ROLES":

                    var userRoleList = _repository.GetQuery<UserRole>().Where(a => a.Active == "Y");

                    foreach (var userRole in userRoleList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == userRole.RoleCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = userRole.Id,
                            Code = userRole.RoleCode,
                            EnglishDescription = userRole.RoleName,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = userRole.ModifiedDate,
                            ModifiedBy = userRole.ModifiedBy


                        });

                    }
                    break;
                case "LEDGERACC":
                    var ledgerAccountList = _repository.GetQuery<LedgerAccount>().Where(a => a.Active == "Y");
                    foreach (var ledgerAccount in ledgerAccountList)
                    {
                        var ledgerCode = Convert.ToString(ledgerAccount.LedgerCode);
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == ledgerCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = ledgerAccount.Id,
                            Code = ledgerCode,
                            EnglishDescription = ledgerAccount.LedgerDesc,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = ledgerAccount.ModifiedDate,
                            ModifiedBy = ledgerAccount.ModifiedBy


                        });

                    }
                    break;
                case "LEDGERACCGRP":
                    var ledgerAccountGrpList = _repository.GetQuery<LedgerAccountGrp>().Where(a => a.Active == "Y");

                    foreach (var ledgerAccountGrp in ledgerAccountGrpList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == ledgerAccountGrp.AccountCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = ledgerAccountGrp.Id,
                            Code = ledgerAccountGrp.AccountCode,
                            EnglishDescription = ledgerAccountGrp.AccountDesc,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = ledgerAccountGrp.ModifiedDate,
                            ModifiedBy = ledgerAccountGrp.ModifiedBy


                        });

                    }
                    break;
                case "COSTCENTERS":
                    var costCenterList = _repository.GetQuery<CostCenter>().Where(a => a.Active == "Y");

                    foreach (var costCenter in costCenterList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == costCenter.Code);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = costCenter.Id,
                            Code = costCenter.Code,
                            EnglishDescription = costCenter.Description,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = costCenter.ModifiedDate,
                            ModifiedBy = costCenter.ModifiedBy


                        });
                    }
                    break;
                case "PETTYCASHACCOUNT":
                    var pettyCashAccountList = _repository.GetQuery<PettyCashAccount>().Where(a => a.Active == "Y");

                    foreach (var pettyCashAccount in pettyCashAccountList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == pettyCashAccount.AccountCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = pettyCashAccount.Id,
                            Code = pettyCashAccount.AccountCode,
                            EnglishDescription = pettyCashAccount.AccountName,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = pettyCashAccount.ModifiedDate,
                            ModifiedBy = pettyCashAccount.ModifiedBy


                        });

                    }
                    break;
                case "PETTYCASHTELLER":

                    var pettyCashTellerList = _repository.GetQuery<PettyCashTeller>().Where(a => a.Active == "Y");
                    foreach (var pettyCashTeller in pettyCashTellerList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == pettyCashTeller.TellerCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = pettyCashTeller.Id,
                            Code = pettyCashTeller.TellerCode,
                            EnglishDescription = pettyCashTeller.TellerName,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = pettyCashTeller.ModifiedDate,
                            ModifiedBy = pettyCashTeller.ModifiedBy


                        });
                    }
                    break;
                case "ORGINFO":
                    var organizationList = _repository.GetQuery<Organization>().Where(a => a.Active == "Y");

                    foreach (var organization in organizationList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == organization.OrgCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = organization.Id,
                            Code = organization.OrgCode,
                            EnglishDescription = organization.OrgName,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = organization.ModifiedDate,
                            ModifiedBy = organization.ModifiedBy


                        });

                    }
                    break;
                case "EMBASSYMASTER":
                    var embassyMasterList = _repository.GetQuery<EmbassyMaster>().Where(a => a.Active == "Y");
                    foreach (var embassyMaster in embassyMasterList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == embassyMaster.CountryCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = embassyMaster.Id,
                            Code = embassyMaster.CountryCode,
                            EnglishDescription = embassyMaster.NameArabic,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = embassyMaster.ModifiedDate,
                            ModifiedBy = embassyMaster.ModifiedBy


                        });
                    }
                    break;
                case "CURRENCYMASTER":
                    var currencyMasterList = _repository.GetQuery<CurrencyMaster>().Where(a => a.Active == "Y");
                    foreach (var currencyMaster in currencyMasterList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == currencyMaster.Code);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = currencyMaster.Id,
                            Code = currencyMaster.Code,
                            EnglishDescription = currencyMaster.Name,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = currencyMaster.ModifiedDate,
                            ModifiedBy = currencyMaster.ModifiedBy


                        });

                    }
                    break;
                case "APPMESSAGE":
                    var appMessageList = _repository.GetQuery<AppMessage>().Where(a => a.Active == "Y");
                    foreach (var appMessage in appMessageList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == appMessage.Code);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = appMessage.Id,
                            Code = appMessage.Code,
                            EnglishDescription = appMessage.Description,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = appMessage.ModifiedDate,
                            ModifiedBy = appMessage.ModifiedBy


                        });
                    }
                    break;
                case "PRODUCTCATEGORY":
                    var productCategoryList = _repository.GetQuery<ProductCategory>().Where(a => a.Active == "Y");
                    foreach (var productCategory in productCategoryList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == productCategory.Code);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = productCategory.Id,
                            Code = productCategory.Code,
                            EnglishDescription = productCategory.Name,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = productCategory.ModifiedDate,
                            ModifiedBy = productCategory.ModifiedBy


                        });
                    }
                    break;
                case "PRODUCTMASTER":
                    var productMasterList = _repository.GetQuery<ProductMaster>().Where(a => a.Active == "Y");
                    foreach (var productMaster in productMasterList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == productMaster.ProdCode);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = productMaster.Id,
                            Code = productMaster.ProdCode,
                            EnglishDescription = productMaster.ProdDescription,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = productMaster.ModifiedDate,
                            ModifiedBy = productMaster.ModifiedBy


                        });
                    }
                    break;
                case "DEPARTMENT":

                    var departmentList = _repository.GetQuery<Department>().Where(a => a.Active == "Y");
                    foreach (var department in departmentList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == department.Code);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = department.Id,
                            Code = department.Code,
                            EnglishDescription = department.Name,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = department.ModifiedDate,
                            ModifiedBy = department.ModifiedBy


                        });

                    }
                    break;
                case "SYSTEMSETTING":
                    var systemSettingsList = _repository.GetQuery<SystemSetting>().Where(a => a.Active == "Y");

                    foreach (var systemSetting in systemSettingsList)
                    {
                        var langMasterCodeBasedData = langMaster.FirstOrDefault(a => a.Code == systemSetting.ConfigKey);

                        appTranslationData.TranslationData.Add(new AppTranslationMaster
                        {
                            Id = systemSetting.Id,
                            Code = systemSetting.ConfigKey,
                            EnglishDescription = systemSetting.Description,
                            ArabicDescription = langMasterCodeBasedData?.Description,
                            ModifiedDate = systemSetting.ModifiedDate,
                            ModifiedBy = systemSetting.ModifiedBy


                        });

                    }
                    break;
            }
            return appTranslationData;
        }
        public AppResponse SaveTranslationData(AppTranslation appTranslation)
        {
            AppResponse appResponse = new AppResponse();
            if (appTranslation.CodeType != null && appTranslation.TranslationData.Count > 0)
            {

                switch (appTranslation.CodeType)
                {
                    case "CODESMASTER":
                        List<CodesMaster> codesMaster = new List<CodesMaster>();
                        var codesMasterList = _repository.GetQuery<CodesMaster>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var codesMasterDataById = codesMasterList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            codesMaster.Add(new CodesMaster
                            {
                                Id = appTranslationMaster.Id,
                                Code = appTranslationMaster.Code,
                                Description = appTranslationMaster.EnglishDescription,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                CreatedBy = codesMasterDataById.CreatedBy,
                                CreatedDate = codesMasterDataById.CreatedDate,
                                CodeType = codesMasterDataById.CodeType,
                                Active = "Y"

                            });
                        }
                        var codesMasterBC = new CodesMasterBC(_logger, _repository);
                        codesMasterBC.SaveCodeMasterList(codesMaster);
                        break;
                    case "ACCESS":

                        List<AppAccess> appAccess = new List<AppAccess>();
                        var appAccessList = _repository.GetQuery<AppAccess>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var appAccessIdBasedData = appAccessList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            appAccess.Add(new AppAccess
                            {
                                Id = appTranslationMaster.Id,
                                AccessCode = appTranslationMaster.Code,
                                AccessName = appTranslationMaster.EnglishDescription,
                                CreatedBy = appTranslationMaster.CreatedBy,
                                CreatedDate = appTranslationMaster.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                ScreenUrl = appAccessIdBasedData?.ScreenUrl,
                                AccessType = appAccessIdBasedData.AccessType,
                                Active = "Y"

                            });
                        }
                        var appAccessBC = new AppAccessBC(_logger, _repository);
                        appAccessBC.SaveAppAccessList(appAccess);

                        break;
                    case "CODESMASTERDETAILS":
                        List<CodesDetails> codesDetails = new List<CodesDetails>();
                        var codesDetailsList = _repository.GetQuery<CodesDetails>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var codesDetailsDataById = codesDetailsList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            codesDetails.Add(new CodesDetails
                            {
                                Id = appTranslationMaster.Id,
                                Code = appTranslationMaster.Code,
                                Description = appTranslationMaster.EnglishDescription,
                                DisplayOrder = appTranslationMaster.EnglishDisplayOrder,
                                CreatedBy = appTranslationMaster.CreatedBy,
                                CreatedDate = appTranslationMaster.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                CodesMasterId = codesDetailsDataById.CodesMasterId,
                                Active = "Y"

                            });
                        }
                        var codesMasterBC1 = new CodesMasterBC(_logger, _repository);
                        codesMasterBC1.SaveCodesDetailsList(codesDetails);

                        break;
                    case "UNITS":
                        List<ProdUnitMaster> prodUnitMaster = new List<ProdUnitMaster>();
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            prodUnitMaster.Add(new ProdUnitMaster
                            {
                                Id = appTranslationMaster.Id,
                                UnitCode = appTranslationMaster.Code,
                                UnitName = appTranslationMaster.EnglishDescription,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                CreatedBy = appTranslationMaster.CreatedBy,
                                CreatedDate = appTranslationMaster.CreatedDate,
                                Active = "Y"

                            });
                        }
                        var prodUnitMasterBC = new ProdUnitMasterBC(_logger, _repository);
                        prodUnitMasterBC.SaveProdUnitMasterList(prodUnitMaster);
                        break;
                    case "MENUS":
                        List<AppMenuMaster> appMenuMaster = new List<AppMenuMaster>();
                        var appMenuMasterList = _repository.GetQuery<AppMenuMaster>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var appMenuMasterListByCode = appMenuMasterList.Where(a => a.MainMenuCode == appTranslationMaster.Code).ToList();
                            appMenuMasterListByCode.ForEach(appMenuMasterByCode =>
                            {
                                appMenuMaster.Add(new AppMenuMaster
                                {
                                    Id = appMenuMasterByCode.Id,
                                    ModuleCode = appMenuMasterByCode.ModuleCode,
                                    ModuleName = appMenuMasterByCode.ModuleName,
                                    ModuleDispOrder = appMenuMasterByCode.ModuleDispOrder,
                                    AppAccessId = appMenuMasterByCode.AppAccessId,
                                    MainMenuCode = appTranslationMaster.Code,
                                    SubMenuCode = appMenuMasterByCode.SubMenuCode,
                                    MainMenuName = appTranslationMaster.EnglishDescription,
                                    SubMenuName = appMenuMasterByCode.SubMenuName,
                                    DispOrder = appMenuMasterByCode.DispOrder,
                                    MainMenuDispOrd = appTranslationMaster.EnglishDisplayOrder,
                                    ShowFinYear = appMenuMasterByCode.ShowFinYear,
                                    ShowOrg = appMenuMasterByCode.ShowOrg,
                                    ModifiedBy = appMenuMasterByCode.ModifiedBy,
                                    ModifiedDate = appMenuMasterByCode.ModifiedDate,
                                    CreatedBy = appMenuMasterByCode.CreatedBy,
                                    CreatedDate = appMenuMasterByCode.CreatedDate,
                                    MainMenuIcon = appMenuMasterByCode.MainMenuIcon,
                                    SubMenuIcon = appMenuMasterByCode?.SubMenuIcon,
                                    ModuleIcon = appMenuMasterByCode?.ModuleIcon,
                                    Active = "Y"
                                });
                            });
                        }
                        var appMenuMasterBC = new AppMenuMasterBC(_logger, _repository);
                        appMenuMasterBC.SaveAppMenuMasterList(appMenuMaster);
                        break;
                    case "SUBMENUS":
                        List<AppMenuMaster> appSubManuMaster = new List<AppMenuMaster>();
                        var appMenuMasterList1 = _repository.GetQuery<AppMenuMaster>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var appMenuMasterDataById = appMenuMasterList1.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            appSubManuMaster.Add(new AppMenuMaster
                            {
                                Id = appTranslationMaster.Id,
                                ModuleCode = appMenuMasterDataById.ModuleCode,
                                ModuleName = appMenuMasterDataById.ModuleName,
                                ModuleDispOrder = appMenuMasterDataById.ModuleDispOrder,
                                AppAccessId = appMenuMasterDataById.AppAccessId,
                                MainMenuCode = appMenuMasterDataById.MainMenuCode,
                                SubMenuCode = appTranslationMaster.Code,
                                MainMenuName = appMenuMasterDataById.MainMenuName,
                                SubMenuName = appTranslationMaster.EnglishDescription,
                                DispOrder = appTranslationMaster.EnglishDisplayOrder,
                                MainMenuDispOrd = appMenuMasterDataById.MainMenuDispOrd,
                                ShowFinYear = appMenuMasterDataById.ShowFinYear,
                                ShowOrg = appMenuMasterDataById.ShowOrg,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                CreatedBy = appMenuMasterDataById.CreatedBy,
                                CreatedDate = appMenuMasterDataById.CreatedDate,
                                MainMenuIcon = appMenuMasterDataById.MainMenuIcon,
                                SubMenuIcon = appMenuMasterDataById?.SubMenuIcon,
                                ModuleIcon = appMenuMasterDataById?.ModuleIcon,
                                Active = "Y"

                            });
                        }
                        var appMenuMasterBC1 = new AppMenuMasterBC(_logger, _repository);
                        appMenuMasterBC1.SaveAppMenuMasterList(appSubManuMaster);
                        break;
                    case "ROLES":
                        List<UserRole> userRole = new List<UserRole>();
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            userRole.Add(new UserRole
                            {
                                Id = appTranslationMaster.Id,
                                RoleCode = appTranslationMaster.Code,
                                RoleName = appTranslationMaster.EnglishDescription,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                CreatedBy = appTranslationMaster.CreatedBy,
                                CreatedDate = appTranslationMaster.CreatedDate,
                                Active = "Y"

                            });
                        }
                        var userRoleBC = new UserRoleBC(_logger, _repository);
                        userRoleBC.SaveUserRoleList(userRole);
                        break;
                    case "LEDGERACC":
                        List<LedgerAccount> ledgerAccount = new List<LedgerAccount>();
                        var ledgerAccountList = _repository.GetQuery<LedgerAccount>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var ledgerAccountDataById = ledgerAccountList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            var ledgerCode = Convert.ToInt32(appTranslationMaster.Code);
                            ledgerAccount.Add(new LedgerAccount
                            {
                                Id = appTranslationMaster.Id,
                                LedgerCode = ledgerCode,
                                LedgerDesc = appTranslationMaster.EnglishDescription,
                                UsedFor = ledgerAccountDataById.UsedFor,
                                Remarks = ledgerAccountDataById.Remarks,
                                CreatedBy = ledgerAccountDataById.CreatedBy,
                                CreatedDate = ledgerAccountDataById.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                Active = "Y"

                            });
                        }
                        var ledgerAccountBC = new LedgerAccountBC(_logger, _repository);
                        ledgerAccountBC.SaveLedgerAccountList(ledgerAccount);
                        break;
                    case "LEDGERACCGRP":
                        List<LedgerAccountGrp> ledgerAccountGrp = new List<LedgerAccountGrp>();
                        var ledgerAccountGrpList = _repository.GetQuery<LedgerAccountGrp>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var ledgerAccountGrpDataById = ledgerAccountGrpList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            ledgerAccountGrp.Add(new LedgerAccountGrp
                            {
                                Id = appTranslationMaster.Id,
                                AccountCode = appTranslationMaster.Code,
                                AccountDesc = appTranslationMaster.EnglishDescription,
                                LedgerCodeFrom = ledgerAccountGrpDataById.LedgerCodeFrom,
                                LedgerCodeTo = ledgerAccountGrpDataById.LedgerCodeTo,
                                CreatedBy = ledgerAccountGrpDataById.CreatedBy,
                                CreatedDate = ledgerAccountGrpDataById.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                Active = "Y"

                            });
                        }
                        var ledgerAccountBC1 = new LedgerAccountBC(_logger, _repository);
                        ledgerAccountBC1.SaveLedgerAccountGrpList(ledgerAccountGrp);
                        break;
                    case "COSTCENTERS":
                        List<CostCenter> costCenter = new List<CostCenter>();
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            costCenter.Add(new CostCenter
                            {
                                Id = appTranslationMaster.Id,
                                Code = appTranslationMaster.Code,
                                Description = appTranslationMaster.EnglishDescription,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                CreatedBy = appTranslationMaster.CreatedBy,
                                CreatedDate = appTranslationMaster.CreatedDate,
                                Active = "Y"

                            });
                        }
                        var costCenterBC = new CostCenterBC(_logger, _repository);
                        costCenterBC.SaveCostCenters(costCenter);
                        break;
                    case "PETTYCASHACCOUNT":
                        List<PettyCashAccount> pettyCashAccount = new List<PettyCashAccount>();
                        var pettyCashAccountList = _repository.GetQuery<PettyCashAccount>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var pettyCashAccountDataById = pettyCashAccountList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            pettyCashAccount.Add(new PettyCashAccount
                            {
                                Id = appTranslationMaster.Id,
                                AccountCode = appTranslationMaster.Code,
                                AccountName = appTranslationMaster.EnglishDescription,
                                IsHeadAccount = pettyCashAccountDataById.IsHeadAccount,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                CreatedDate = pettyCashAccountDataById.CreatedDate,
                                CreatedBy = pettyCashAccountDataById.CreatedBy,
                                Active = "Y"

                            });
                        }
                        var pettyCashAccoutBC = new PettyCashAccoutBC(_logger, _repository);
                        pettyCashAccoutBC.SavePettyCashAccountList(pettyCashAccount);
                        break;
                    case "PETTYCASHTELLER":
                        List<PettyCashTeller> pettyCashTeller = new List<PettyCashTeller>();
                        var pettyCashTellerList = _repository.GetQuery<PettyCashTeller>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var pettyCashTellerData = pettyCashTellerList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            pettyCashTeller.Add(new PettyCashTeller
                            {
                                Id = appTranslationMaster.Id,
                                TellerCode = appTranslationMaster.Code,
                                TellerName = appTranslationMaster.EnglishDescription,
                                UserId = pettyCashTellerData.UserId,
                                IsHeadTeller = pettyCashTellerData.IsHeadTeller,
                                CreatedBy = pettyCashTellerData.CreatedBy,
                                CreatedDate = pettyCashTellerData.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                Active = "Y"

                            });
                        }
                        var pettyCashTellerBC = new PettyCashTellerBC(_logger, _repository);
                        pettyCashTellerBC.SavePettyCashTellerList(pettyCashTeller);
                        break;
                    case "ORGINFO":
                        List<Organization> organization = new List<Organization>();
                        var organizationList = _repository.GetQuery<Organization>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var organizationDataById = organizationList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            organization.Add(new Organization
                            {
                                Id = appTranslationMaster.Id,
                                OrgCode = appTranslationMaster.Code,
                                OrgName = appTranslationMaster.EnglishDescription,
                                Location = organizationDataById.Location,
                                CreatedBy = organizationDataById.CreatedBy,
                                CreatedDate = organizationDataById.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                Active = "Y"

                            });
                        }
                        var organizationBC = new OrganizationBC(_logger, _repository);
                        organizationBC.SaveOrganizationList(organization);
                        break;
                    //EMBASSYMASTER No!
                    case "EMBASSYMASTER":
                        List<EmbassyMaster> embassyMaster = new List<EmbassyMaster>();
                        var embassyMasterList = _repository.GetQuery<EmbassyMaster>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var embassyMasterDataById = embassyMasterList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            embassyMaster.Add(new EmbassyMaster
                            {
                                Id = appTranslationMaster.Id,
                                CountryCode = appTranslationMaster.Code,
                                NameEng = embassyMasterDataById.NameEng,
                                NameArabic = appTranslationMaster.EnglishDescription,
                                Number = embassyMasterDataById.Number,
                                Address = embassyMasterDataById.Address,
                                DefaultCurrency = embassyMasterDataById.DefaultCurrency,
                                CreatedBy = embassyMasterDataById.CreatedBy,
                                CreatedDate = embassyMasterDataById.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                Active = "Y"

                            });
                        }
                        var embassyMasterBC = new EmbassyMasterBC(_logger, _repository);
                        embassyMasterBC.SaveEmbassyMasterList(embassyMaster);
                        break;
                    case "CURRENCYMASTER":
                        List<CurrencyMaster> currencyMaster = new List<CurrencyMaster>();
                        var embassyMasterDataByIdList = _repository.GetQuery<CurrencyMaster>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var currencyMasterDataById = embassyMasterDataByIdList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            currencyMaster.Add(new CurrencyMaster
                            {
                                Id = appTranslationMaster.Id,
                                Code = appTranslationMaster.Code,
                                Name = appTranslationMaster.EnglishDescription,
                                CountryCode = currencyMasterDataById.CountryCode,
                                CountryName = currencyMasterDataById.CountryName,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                CreatedBy = currencyMasterDataById.CreatedBy,
                                CreatedDate = currencyMasterDataById.CreatedDate,
                                Active = "Y"

                            });
                        }
                        var currencyMasterBC = new CurrencyMasterBC(_logger, _repository);
                        currencyMasterBC.SaveCurrencyMasterList(currencyMaster);
                        break;
                    case "APPMESSAGE":
                        List<AppMessage> appMessage = new List<AppMessage>();
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            appMessage.Add(new AppMessage
                            {
                                Id = appTranslationMaster.Id,
                                Code = appTranslationMaster.Code,
                                Description = appTranslationMaster.EnglishDescription,
                                CreatedBy = appTranslationMaster.CreatedBy,
                                CreatedDate = appTranslationMaster.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                Active = "Y"

                            });
                        }
                        var appAccessBC1 = new AppAccessBC(_logger, _repository);
                        appAccessBC1.SaveAppMessageList(appMessage);
                        break;
                    case "PRODUCTCATEGORY":
                        List<ProductCategory> productCategory = new List<ProductCategory>();
                        var productCategoryList = _repository.GetQuery<ProductCategory>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var productCategoryDataById = productCategoryList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            productCategory.Add(new ProductCategory
                            {
                                Id = appTranslationMaster.Id,
                                Code = appTranslationMaster.Code,
                                Name = appTranslationMaster.EnglishDescription,
                                ApprovalLevels = productCategoryDataById.ApprovalLevels,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                CreatedBy = productCategoryDataById.CreatedBy,
                                CreatedDate = productCategoryDataById.CreatedDate,
                                Active = "Y"

                            });
                        }
                        var productCategoryBC = new ProductCategoryBC(_logger, _repository);
                        productCategoryBC.SaveProductCategoryList(productCategory);
                        break;
                    case "PRODUCTMASTER":
                        List<ProductMaster> productMaster = new List<ProductMaster>();
                        var productMasterList = _repository.GetQuery<ProductMaster>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var productMasterDataById = productMasterList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            productMaster.Add(new ProductMaster
                            {
                                Id = appTranslationMaster.Id,
                                ProdCategoryId = productMasterDataById.ProdCategoryId,
                                ProdCode = appTranslationMaster.Code,
                                ProdDescription = appTranslationMaster.EnglishDescription,
                                Barcode = productMasterDataById.Barcode,
                                ReOrderLevel = productMasterDataById.ReOrderLevel,
                                IsExpirable = productMasterDataById.IsExpirable,
                                IsStockable = productMasterDataById.IsStockable,
                                DefaultUnitId = productMasterDataById.DefaultUnitId,
                                CreatedBy = productMasterDataById.CreatedBy,
                                CreatedDate = productMasterDataById.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                Active = "Y"

                            });
                        }
                        var productMasterBC = new ProductMasterBC(_logger, _repository);
                        productMasterBC.SaveProductMasterList(productMaster);
                        break;
                    case "DEPARTMENT":
                        List<Department> department = new List<Department>();
                        var departmentList = _repository.GetQuery<Department>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var departmentDataById = departmentList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            department.Add(new Department
                            {
                                Id = appTranslationMaster.Id,
                                Code = appTranslationMaster.Code,
                                Name = appTranslationMaster.EnglishDescription,
                                ParentId = departmentDataById.ParentId,
                                Type = departmentDataById.Type,
                                CreatedBy = departmentDataById.CreatedBy,
                                CreatedDate = departmentDataById.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                Active = "Y"

                            });
                        }
                        var departmentBC = new DepartmentBC(_logger, _repository);
                        departmentBC.SaveDepartmentList(department);
                        break;
                    case "SYSTEMSETTING":
                        List<SystemSetting> systemSetting = new List<SystemSetting>();
                        var systemSettingList = _repository.GetQuery<SystemSetting>().Where(a => a.Active == "Y");
                        foreach (var appTranslationMaster in appTranslation.TranslationData)
                        {
                            var systemSettingById = systemSettingList.FirstOrDefault(a => a.Id == appTranslationMaster.Id);
                            systemSetting.Add(new SystemSetting
                            {
                                Id = appTranslationMaster.Id,
                                ConfigKey = appTranslationMaster.Code,
                                Description = appTranslationMaster.EnglishDescription,
                                ConfigValue = systemSettingById.ConfigValue,
                                Type = systemSettingById.Type,
                                CreatedBy = appTranslationMaster.CreatedBy,
                                CreatedDate = appTranslationMaster.CreatedDate,
                                ModifiedBy = appTranslationMaster.ModifiedBy,
                                ModifiedDate = appTranslationMaster.ModifiedDate,
                                Active = "Y"

                            });
                        }
                        var systemSettingsBC = new SystemSettingsBC(_logger, _repository);
                        systemSettingsBC.SaveSystemSettings(systemSetting);
                        break;
                }


                List<LangMaster> langMasters = new List<LangMaster>();

                appTranslation.TranslationData.ForEach(langMaster =>
                {

                    if (langMaster.ArabicDescription != null && langMaster.ArabicDescription != "")
                    {
                        langMasters.Add(new LangMaster
                        {
                            Id = Guid.NewGuid(),
                            Code = langMaster.Code,
                            Description = langMaster.ArabicDescription,
                            CodeType = appTranslation.CodeType,
                            Language = "ar",
                            DisplayOrder = langMaster.ArabicDisplayOrder,
                            Active = "Y"
                        });
                    }
                });
                UpdateLangMaster(langMasters, appTranslation.CodeType);
                _repository.SaveChanges();
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }

            else
                appResponse.Status = APPMessageKey.ONEORMOREERR;
            return appResponse;
        }

        private void UpdateLangMaster(List<LangMaster> langMasters, string codeType)
        {
            var langMasterList = _repository.GetQuery<LangMaster>().Where(a => a.CodeType == codeType).ToList();
            if (langMasterList.Count > 0 && langMasterList != null)
            {
                foreach (var lang in langMasterList)
                {
                    _repository.Delete(lang, false);
                }
                _repository.SaveChanges();
            }
            if (langMasters.Count > 0 && langMasters != null)
            {
                foreach (var langMaster in langMasters)
                {
                    _repository.Add(langMaster, false);
                }
            }


        }
    }

}
