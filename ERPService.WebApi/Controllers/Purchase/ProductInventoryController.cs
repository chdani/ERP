using ERPService.BC;
using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Microsoft.Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace ERPService.WebApi.Controllers
{
    public class ProductInventoryController : AppApiBaseController
    {
        public ProductInventoryController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpPost]
        [Route("getProdInventoryBalance")]
        [Authorize]
        public List<ProdInventoryBalance> GetProdInventoryBalance(ProdInventoryBalance input, bool isExport = false)
        {
            ProductInventoryBC purchaseOrderBC = new ProductInventoryBC(_logger, _repository);
            return purchaseOrderBC.GetProdInventoryBalance(input, isExport);
        }

        [HttpPost]
        [Route("getProductInvTransactions")]
        [Authorize]
        public List<ProductInventory> GetProductInvTransactions(ProdInventorySearch input, bool isExport = false)
        {
            ProductInventoryBC purchaseOrderBC = new ProductInventoryBC(_logger, _repository);
            return purchaseOrderBC.GetProductInvTransactions(input, isExport);
        }
        [HttpPost]
        [Route("downloadStockTransReport")]
        [Authorize]
        public HttpResponseMessage DownloadStockTransReport(ProdInventorySearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(ProdInventorySearch search, HttpResponseMessage httpResponse)
        {
            List<ProductInventory> transfers = GetProductInvTransactions(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new PDFGenerator<ProductInventory>
                    {
                        gridData = transfers,
                        _headerText = search.ExportHeaderText,
                        _repository = _repository,
                        _userContext = _userContext,
                        _logger = _logger,

                    };
                    buff = file.getByte();
                }
                else if (search.ExportType == ExportType.EXCEL)
                {
                    var file = new ExcelGenerator<ProductInventory>
                    {
                        data = transfers,
                        _headerText = search.ExportHeaderText,
                        _repository = _repository,
                        _userContext = _userContext,
                        _logger = _logger,

                    };
                    buff = file.getByte();
                }
            }
            base.CreateFileResponse(httpResponse, buff);
        }
        [HttpPost]
        [Route("downloadProdInventoryBalance")]
        [Authorize]
        public HttpResponseMessage DownloadProdInventoryBalance(ProdInventoryBalance search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(ProdInventoryBalance search, HttpResponseMessage httpResponse)
        {
            List<ProdInventoryBalance> transfers = GetProdInventoryBalance(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new PDFGenerator<ProdInventoryBalance>
                    {
                        gridData = transfers,
                        _headerText = search.ExportHeaderText,
                        _repository = _repository,
                        _userContext = _userContext,
                        _logger = _logger,

                    };
                    buff = file.getByte();
                }
                else if (search.ExportType == ExportType.EXCEL)
                {
                    var file = new ExcelGenerator<ProdInventoryBalance>
                    {
                        data = transfers,
                        _headerText = search.ExportHeaderText,
                        _repository = _repository,
                        _userContext = _userContext,
                        _logger = _logger,

                    };
                    buff = file.getByte();
                }
            }
            base.CreateFileResponse(httpResponse, buff);
        }
    }
}