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
    public class WareHouseBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public WareHouseBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public WareHouse GetWareHouseById(Guid wareHouseId)
        {
            var wareHouse = _repository.Get<WareHouse>(a => a.Id == wareHouseId);
            wareHouse.WareHouseLocation = _repository.GetQuery<WareHouseLocation>().Where(a => a.WarehouseId == wareHouseId && a.Active == "Y").ToList();
            return wareHouse;
        }

        public List<WareHouse> GetWareHouseList()
        {
            return _repository.GetQuery<WareHouse>().ToList(); ;
        }

        public List<WareHouseLocation> GetWareHouseLocationList()
        {
            return _repository.GetQuery<WareHouseLocation>().ToList();
        }

        public List<WareHouse> GetWareHouseListSearch(WareHouse wareHouse)
        {
            var warehouseList = _repository.GetQuery<WareHouse>().Where(a =>
                 (string.IsNullOrEmpty(wareHouse.Name) || a.Name.Contains(wareHouse.Name))
               && (string.IsNullOrEmpty(wareHouse.Email) || a.Email.Contains(wareHouse.Email)) &&
               (string.IsNullOrEmpty(wareHouse.ContactNo) || a.ContactNo.Contains(wareHouse.ContactNo)) &&
               (string.IsNullOrEmpty(wareHouse.Address) || a.Address.Contains(wareHouse.Address)) &&
               a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
            if (warehouseList != null || warehouseList.Count > 0)
            {
                foreach (var warehouses in warehouseList)
                {
                    warehouses.WareHouseLocation = _repository.GetQuery<WareHouseLocation>().Where(a => a.Active == "Y" && a.WarehouseId == warehouses.Id).ToList();
                }
            }
            return warehouseList;
        }
        public AppResponse SaveWareHouse(WareHouse wareHouse)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            var duplicateName = _repository.GetQuery<WareHouse>().FirstOrDefault(a => a.Active == "Y" && a.Name == wareHouse.Name && a.Id != wareHouse.Id);
            if (duplicateName != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLINAMEALREADY]);
                validation = false;
            }
            if (wareHouse != null && validation)
            {
                InsertUpdateWareHouse(wareHouse);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;

        }
        private void InsertUpdateWareHouse(WareHouse wareHouse)
        {
            if (wareHouse.Id == Guid.Empty)
            {
                wareHouse.Id = Guid.NewGuid();
                _repository.Add(wareHouse, false);
            }
            else
            {
                _repository.Update(wareHouse, false);
            }
            if (wareHouse.WareHouseLocation != null && wareHouse.WareHouseLocation.Count > 0)
            {
                foreach (var location in wareHouse.WareHouseLocation)
                {
                    location.WarehouseId = wareHouse.Id;
                    InsertUpdateWareHouseLocation(location, false);
                }
            }
            _repository.SaveChanges();
        }
        private void InsertUpdateWareHouseLocation(WareHouseLocation wareHouseLocation, bool location)
        {
            if (wareHouseLocation.Id == Guid.Empty)
            {
                wareHouseLocation.Id = Guid.NewGuid();
                _repository.Add(wareHouseLocation, false);
            }
            else
            {
                _repository.Update(wareHouseLocation, false);
            }
            if (location)
                _repository.SaveChanges();
        }
    }
}