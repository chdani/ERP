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
    public class VendorMasterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;
        public VendorMasterBC(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger;
            _repository = repository;
            _userContext = userContext;
        }

        public VendorMaster GetVendorMasterById(Guid vendorId)
        {
            VendorMaster vendor = new VendorMaster();
            vendor = _repository.Get<VendorMaster>(a => a.Id == vendorId);
            vendor.VendorContacts = _repository.GetQuery<VendorContact>().Where(a => a.VendorMasterId == vendorId && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
            vendor.VendorContracts = _repository.GetQuery<VendorContract>().Where(a => a.VendorMasterId == vendorId && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();

            vendor.VendorProducts = _repository.GetQuery<VendorProduct>().Where(a => a.VendorMasterId == vendorId && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
            if (vendor.VendorContracts != null && vendor.VendorContracts.Count > 0)
            {
                var documents = _repository.List<AppDocument>(a => a.Active == "Y");
                foreach (var det in vendor.VendorContracts)
                {
                    det.AppDocuments = documents.Where(a => a.TransactionId == det.Id && a.TransactionType == "VENDORCONTRACT").ToList();
                }
            }
            if (vendor.VendorProducts != null)
            {
                var languageBC = new LangMasterBC(_logger, _repository);
                var products = languageBC.GetLangBasedDataForProductMaster(_userContext.Language);
                foreach (var vendorproduct in vendor.VendorProducts)
                {
                    var product = products.FirstOrDefault(a => a.Id == vendorproduct.ProductMasterId);
                    vendorproduct.ProductName = product != null ? product.ProdDescription : "";
                }

            }
            return vendor;
        }

        public List<VendorMaster> GetVendorMasterList()
        {
            List<VendorMaster> vendorMaster = null;
            vendorMaster = _repository.GetQuery<VendorMaster>().Where(a => a.Active == "Y").ToList();
            return vendorMaster;
        }
        public AppResponse SaveVendor(VendorMaster vendor)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;
            var Vendor1 = _repository.GetQuery<VendorMaster>().FirstOrDefault(a => a.Id != vendor.Id && a.Name == vendor.Name && a.Active == "Y");
            if (Vendor1 != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLINAMEALREADY]);
                validation = false;
            }
            if (vendor != null && validation)

            {
                if (vendor.Active == "Y")
                {
                    var oldVendorproduct = _repository.GetQuery<VendorProduct>().Where(a => a.VendorMasterId == vendor.Id).ToList();
                    foreach (var item in oldVendorproduct)
                    {
                        _repository.Delete(item);
                    }
                }
                InsertUpdateVendor(vendor);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        private void InsertUpdateVendor(VendorMaster vendor)
        {
            if (vendor.Id == Guid.Empty)
            {
                vendor.Id = Guid.NewGuid();
                _repository.Add(vendor, false);
            }
            else
                _repository.Update(vendor, false);
            if (vendor.VendorProducts != null)
            {
                foreach (var products in vendor.VendorProducts)
                {
                    products.VendorMasterId = vendor.Id;
                    InsertUpdateVendorproducts(products, false);
                }
            }
            if (vendor.VendorContracts != null)
            {
                foreach (var objvendorContracts in vendor.VendorContracts)
                {
                    objvendorContracts.VendorMasterId = vendor.Id;

                    InsertUpdateVendorContrats(objvendorContracts, false);

                    if (objvendorContracts.AppDocuments != null && objvendorContracts.AppDocuments.Count > 0)
                    {
                        foreach (var appDocument in objvendorContracts.AppDocuments)
                        {
                            appDocument.TransactionId = objvendorContracts.Id;
                        }
                        var appDocumentBC = new AppDocumentBC(_logger, _repository);
                        var AppDocument = appDocumentBC.saveAppDocument(objvendorContracts.AppDocuments, false);
                    }
                }

            }
            if (vendor.VendorContacts != null)
            {
                //vendor.VendorContacts.VendorMasterId = vendor.Id;
                //vendor.VendorContacts.Active = vendor.Active;
                //InsertUpdateVendorContact(vendor.VendorContacts, false);
                foreach (var objvendor in vendor.VendorContacts)
                {
                    objvendor.VendorMasterId = vendor.Id;
                    InsertUpdateVendorContact(objvendor, false);
                }
            }

            _repository.SaveChanges();
        }
        private void InsertUpdateVendorproducts(VendorProduct products, bool saveChanges)
        {

            if (products.Id == Guid.Empty)
            {
                products.Id = Guid.NewGuid();
                _repository.Add(products);
            }
            else
                _repository.Update(products);
            if (saveChanges)
                _repository.SaveChanges();
        }
        private void InsertUpdateVendorContrats(VendorContract contract, bool saveChanges)
        {

            if (contract.Id == Guid.Empty)
            {
                contract.Id = Guid.NewGuid();
                _repository.Add(contract);
            }
            else
                _repository.Update(contract);
            if (saveChanges)
                _repository.SaveChanges();
        }
        private void InsertUpdateVendorContact(VendorContact contact, bool saveChanges)
        {

            if (contact.Id == Guid.Empty)
            {
                contact.Id = Guid.NewGuid();
                _repository.Add(contact);
            }
            else
                _repository.Update(contact);
            if (saveChanges)
                _repository.SaveChanges();
        }
        public List<VendorMaster> GetVendorSerachFilter(VendorMaster vendor)
        {
            var vendorMaster = _repository.GetQuery<VendorMaster>().Where(a =>
                (string.IsNullOrEmpty(vendor.Name) || a.Name.Contains(vendor.Name))
               && (string.IsNullOrEmpty(vendor.Title) || a.Title.Contains(vendor.Title))
               && (string.IsNullOrEmpty(vendor.CountryName) || a.CountryName.Contains(vendor.CountryName))
                && (string.IsNullOrEmpty(vendor.BankCode) || a.BankCode.Contains(vendor.BankCode))
                && (vendor.LedgerCode == 0 || a.LedgerCode == vendor.LedgerCode) &&
               (string.IsNullOrEmpty(vendor.Others) || a.Address1.Contains(vendor.Others)
               || a.Address2.Contains(vendor.Others) || a.Address2.Contains(vendor.Others)
               || a.Email.Contains(vendor.Others) || a.Email.Contains(vendor.Others)
                   || a.Telephone.Contains(vendor.Others) || a.Telephone.Contains(vendor.Others)
               || a.POBox.Contains(vendor.Others) || a.POBox.Contains(vendor.Others)
                   || a.Mobile.Contains(vendor.Others) || a.Mobile.Contains(vendor.Others)
               || a.BankCountryCode.Contains(vendor.Others) || a.BankCountryCode.Contains(vendor.Others)
                   || a.IbanSwifT.Contains(vendor.Others) || a.IbanSwifT.Contains(vendor.Others)
               || a.BankAccName.Contains(vendor.Others) || a.BankAccName.Contains(vendor.Others)
                 || a.BankAccNo.Contains(vendor.Others) || a.BankAccNo.Contains(vendor.Others)
               || a.POBox.Contains(vendor.Others)) &&
                a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
            if (vendorMaster != null && vendorMaster.Count > 0)
            {
                foreach (var vendormaster in vendorMaster)
                {
                    vendormaster.VendorContacts = _repository.GetQuery<VendorContact>().Where(a => a.VendorMasterId == vendormaster.Id && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
                    vendormaster.VendorContracts = _repository.GetQuery<VendorContract>().Where(a => a.VendorMasterId == vendormaster.Id && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
                    vendormaster.VendorProducts = _repository.GetQuery<VendorProduct>().Where(a => a.VendorMasterId == vendormaster.Id && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
                }
            }
            return vendorMaster;
        }
    }
}