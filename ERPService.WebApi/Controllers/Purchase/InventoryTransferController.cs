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
namespace ERPService.WebApi.Controllers.Purchase
{
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryTransferController : AppApiBaseController
    {

        public InventoryTransferController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }



        [HttpPost]
        [Route("approveinventoryTransfer")]
        [Authorize]
        public AppResponse ApproveinventoryTransfer(InventoryTransfer inventoryTransfer)
        {
            InventoryTransferBC inventoryTransferBC = new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.ApproveInventoryTransfer(inventoryTransfer);
        }


        [HttpGet]
        [Route("getInventoryTransferById/{inventoryTransfId}")]
        [Authorize]
        public InventoryTransfer GetInventoryTransferById(Guid inventoryTransfId)
        {
            InventoryTransferBC inventoryTransferBC = new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.GetInventoryTransferById(inventoryTransfId);
        }

        [HttpGet]
        [Route("getInventoryTransferDetByHdrId/{inventoryTransfId}")]
        [Authorize]
        public List<InventoryTransferDet> GetInventoryTransferDetByHdrId(Guid inventoryTransfId)
        {
            InventoryTransferBC inventoryTransferBC = new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.GetInventoryTransferDetByHdrId(inventoryTransfId);
        }

        [HttpPost]
        [Route("getInventoryTransferList")]
        [Authorize]
        public List<InventoryTransfer> getInventoryTransferList(InventoryTransfSearchCTO input, bool isExport = false)
        {
            InventoryTransferBC inventoryTransferBC = new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.GetinventoryTransferList(input,isExport);
        }

        [HttpGet]
        [Route("getInventoryTransferDetList/{transferId}")]
        [Authorize]
        public List<InventoryTransferDet> getInventoryTransferDetList(Guid transferId)
        {
            InventoryTransferBC inventoryTransfer = new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransfer.GetInventoryTransferDetList(transferId);
        }

        [HttpPost]
        [Route("saveInventoryTransfer")]
        [Authorize]
        public AppResponse SaveInventoryTransfer(InventoryTransfer inventoryTransfer)
        {
           InventoryTransferBC inventoryTransferBC  =new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.SaveInventoryTransfer(inventoryTransfer);
        }
 
        [HttpPost]
        [Route("saveInventoryTransferComment")]
        [Authorize]
        public AppResponse SaveInventoryTransferComment(InvTransferComment inventoryTransferComment)
        {
           InventoryTransferBC inventoryTransferBC  =new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.SaveInventoryTransferComment(inventoryTransferComment);
        }

        [HttpPost]
        [Route("saveInventoryTransferDetComment")]
        [Authorize]
        public AppResponse SaveInventoryTransferDetComment(InvTransferDetComment inventoryTransferDetComment)
        {
           InventoryTransferBC inventoryTransferBC  =new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.SaveInventoryTransferDetComment(inventoryTransferDetComment);
        }

        [HttpGet]
        [Route("getInventoryTransferDetHistory/{InventoryTransferDetId}")]
        [Authorize]
        public List<InvTransferDetHist> GetInvTransferDetHistory(Guid InventoryTransferDetId)
        {
           InventoryTransferBC inventoryTransferBC  =new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.GetInvTransferDetHistory(InventoryTransferDetId);
        }

        [HttpGet]
        [Route("getInventoryTransferComment/{InventoryTransferId}")]
        [Authorize]
        public List<InvTransferComment> GetInventoryTransferComment(Guid InventoryTransferId)
        {
           InventoryTransferBC inventoryTransferBC  =new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.GetInvTransferComment(InventoryTransferId);
        }

        [HttpGet]
        [Route("getInventoryTransferDetComment/{InventoryTransferDetId}")]
        [Authorize]
        public List<InvTransferDetComment> GetInventoryTransferDetComment(Guid InventoryTransferDetId)
        {
           InventoryTransferBC inventoryTransferBC  =new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.GetInvTransferDetComment(InventoryTransferDetId);
        }

        [HttpGet]
        [Route("getInventoryTransferStatusHistory/{InventoryTransferId}")]
        [Authorize]
        public List<InvTransferStatusHist> GetInventoryTransferStatusHistory(Guid InventoryTransferId)
        {
            InventoryTransferBC prodInvBC =new InventoryTransferBC(_logger, _repository, _userContext);
            return prodInvBC.GetInvTransferStatusHistory(InventoryTransferId);

        }

        [HttpGet]
        [Route("getInventoryTransfersAttachments/{InventoryTransferId}")]
        [Authorize]
        public List<AppDocument> GetInventoryTransfersAttachments(Guid InventoryTransferId)
        {
           InventoryTransferBC inventoryTransferBC  =new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.GeInvTransferAttachments(InventoryTransferId);
        }
        [HttpPost]
        [Route("downloadInvtransfer")]
        [Authorize]
        public HttpResponseMessage DownloadInvtransfer(InventoryTransfSearchCTO search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(InventoryTransfSearchCTO search, HttpResponseMessage httpResponse)
        {
            List<InventoryTransfer> transfers = getInventoryTransferList(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new BudgetAllocationPDFGenerator<InventoryTransfer>
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
                    var file = new BudgetAllocationExcelGeneratore<InventoryTransfer>
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
        [Route("getShelfs")]
        [Authorize]
        public List<ShelfsData>  getShelfs(ShelfCodeReq Req)
        {
            InventoryTransferBC inventoryTransferBC = new InventoryTransferBC(_logger, _repository, _userContext);
            return inventoryTransferBC.getShelfs(Req);
        }
    }
}
