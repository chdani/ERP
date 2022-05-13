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
    public class ServiceRequestController : AppApiBaseController
    {
        public ServiceRequestController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getServiceRequestById/{serviceRequestId}")]
        [Authorize]
        public ServiceRequest GetServiceRequestById(Guid serviceRequestId)
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServiceRequestById(serviceRequestId);
        }

        [HttpPost]
        [Route("saveServiceRequest")]
        [Authorize]
        public AppResponse SaveServiceRequest(ServiceRequest serviceRequest)
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.SaveServiceRequest(serviceRequest);
        }

        [HttpPost]
        [Route("saveServiceRequestComment")]
        [Authorize]
        public AppResponse SaveServiceRequestComment(ServiceReqComment serviceRequest)
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.SaveServiceRequestComment(serviceRequest);
        }

        [HttpPost]
        [Route("approveServiceRequest")]
        [Authorize]
        public AppResponse ApproveServiceRequest(ServiceRequest serviceRequest)
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.ApproveServiceRequest(serviceRequest, true);
        }


        [HttpPost]
        [Route("getServiceRequestList")]
        [Authorize]
        public List<ServiceRequest> GetServiceRequestList(ServiceRequestSearch input, bool isExport = false)
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServiceRequests(input, isExport);
        }

        [HttpGet]
        [Route("getServRequestsWithServiceDepartment")]
        [Authorize]
        public List<ServiceRequest> GetServRequestsWithServiceDepartment()
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServRequestsWithServiceDepartment();
        }

        [HttpGet]
        [Route("getServiceReqHistory/{servReqId}")]
        [Authorize]
        public List<ServiceReqHist> GetServiceReqHistory(Guid servReqId)
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServiceReqHistory(servReqId);
        }

        [HttpGet]
        [Route("getServiceReqComment/{servReqId}")]
        [Authorize]
        public List<ServiceReqComment> GetServiceReqComment(Guid servReqId)
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServiceReqComment(servReqId);
        }

        [HttpGet]
        [Route("getServiceReqStatusHistory/{servReqId}")]
        [Authorize]
        public List<ServiceReqApproval> GetServiceReqStatusHistory(Guid servReqId)
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServiceReqStatusHistory(servReqId);
        }

        [HttpGet]
        [Route("getServReqAttachments/{servReqId}")]
        [Authorize]
        public List<AppDocument> GetServReqAttachments(Guid servReqId)
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServReqAttachments(servReqId);
        }

        [HttpPost]
        [Route("downloadServiceReq")]
        [Authorize]
        public HttpResponseMessage DownloadServiceReq(ServiceRequestSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(ServiceRequestSearch search, HttpResponseMessage httpResponse)
        {
            List<ServiceRequest> transfers = GetServiceRequestList(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new BudgetAllocationPDFGenerator<ServiceRequest>
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
                    var file = new BudgetAllocationExcelGeneratore<ServiceRequest>
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
        [HttpGet]
        [Route("getServRequestsWithIdDepartment")]
        [Authorize]
        public List<ServiceRequest> GetServRequestsWithIdDepartment()
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServRequestsWithIdDepartment();
        }
        [HttpGet]
        [Route("getServRequestsWithProdConfiguration")]
        [Authorize]
        public List<ServiceRequest> GetServRequestsWithProdConfiguration()
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServRequestsWithProdConfiguration();
        }
        [HttpGet]
        [Route("updateTransNoSeqNo")]
        [Authorize]
        public AppResponse UpdateTransNoSeqNo()
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.UpdateTransNoSeqNo();
        }
        [HttpGet]
        [Route("getServRequestsWithApprovalServiceDepartment")]
        [Authorize]
        public List<ServiceRequest> GetServRequestsWithApprovalServiceDepartment()
        {
            ServiceRequestBC serviceReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
            return serviceReqBC.GetServRequestsWithApprovalServiceDepartment();
        }
    }
}