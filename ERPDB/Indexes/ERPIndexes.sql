CREATE INDEX IDX_ServiceRequest_ID ON ServiceRequest (ID) ;
CREATE INDEX IDX_ServiceRequest_TRANS_DATE ON ServiceRequest (TransDate) ;
CREATE INDEX IDX_ServiceRequest_TRANS_NO ON ServiceRequest (TransNo) ;
CREATE INDEX IDX_ServiceRequest_STATUS ON ServiceRequest (CurApprovalLevel,NextApprovalLevel) ;
CREATE INDEX IDX_ServiceRequest_EMP_ID ON ServiceRequest (EmployeeId) ;
CREATE INDEX IDX_ServiceRequest_ACTIVE ON ServiceRequest (Active) ;

CREATE INDEX IDX_ServiceRequestDet_ID ON ServiceRequestDet (ID) ;
CREATE INDEX IDX_ServiceRequestDet_ID_HDRID ON ServiceRequestDet (ID, ServiceRequestId) ;
CREATE INDEX IDX_ServiceRequestDet_HDRID ON ServiceRequestDet (ServiceRequestId) ;
CREATE INDEX IDX_ServiceRequestDet_PROD_ID ON ServiceRequestDet (ProductMasterId) ;
CREATE INDEX IDX_ServiceRequestDet_ACTIVE ON ServiceRequestDet (Active) ;

CREATE INDEX IDX_ServiceReqApproval_ID ON ServiceReqApproval (ID) ;
CREATE INDEX IDX_ServiceReqApproval_ID_HDRID ON ServiceReqApproval (ID, ServiceRequestId) ;
CREATE INDEX IDX_ServiceReqApproval_HDRID ON ServiceReqApproval (ServiceRequestId) ;
CREATE INDEX IDX_ServiceReqApproval_ACTIVE ON ServiceReqApproval (Active) ;

CREATE INDEX IDX_ProductMaster_ID ON ProductMaster (ID) ;
CREATE INDEX IDX_ProductMaster_CATE_ID ON ProductMaster (ProdCategoryId) ;
CREATE INDEX IDX_ProductMaster_ACTIVE ON ProductMaster (Active) ;

CREATE INDEX IDX_ProdCategory_ID ON ProdCategory (ID) ;
CREATE INDEX IDX_ProdCategory_ACTIVE ON ProdCategory (Active) ;

CREATE INDEX IDX_ProdCatWorkflow_ID ON ProdCatWorkflow (ID) ;
CREATE INDEX IDX_ProdCatWorkflow_CATE_ID ON ProdCatWorkflow (ProdCategoryId) ;
CREATE INDEX IDX_ProdCatWorkflow_ACTIVE ON ProdCatWorkflow (Active) ;

CREATE INDEX IDX_UserMastert_ID ON UserMaster (ID) ;
CREATE INDEX IDX_UserMastert_Emp_ID ON UserMaster (EmployeeId) ;
CREATE INDEX IDX_UserMaster_ACTIVE ON UserMaster (Active) ;

CREATE INDEX IDX_Employee_ID ON Employee (ID);
CREATE INDEX IDX_Employee_ACTIVE ON Employee (Active) ;

CREATE INDEX IDX_ActivityLog_RequestedOn ON ActivityLog (RequestedOn);
CREATE INDEX IDX_ExceptionLog_CREATEDAT ON ExceptionLog (ExceptionOccurredAt);
