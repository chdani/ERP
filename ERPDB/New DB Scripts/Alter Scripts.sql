ALTER table DirectInvPrePayment add PurchaseOrderId uniqueidentifier null;
ALTER table DirectInvPrePayment add DocumentNo bigint null;
Alter table DirectInvPrePayment alter column InvoiceDate date NULL;
ALTER table purchaseorder alter column ledgercode int not null;
ALTER table PurchaseOrder add CostCenterCode varchar(20) null;
ALTER table PurchaseOrder add FinYear varchar(20) null;
ALTER table PurchaseOrder add OrgId  uniqueidentifier null;
ALTER table PurchaseOrder add TotalAmount numeric(18,6) null;
ALTER table SystemSettings add [Description] nvarchar(200) NULL;
ALTER table purchaseorder drop column accountcode;
Alter table embprepaymentHdr add EmbassyId uniqueidentifier not null;
Alter table embprepaymentHdr add CurrencyCode nvarchar(10) null;
Alter table embprepaymentHdr add CurrencyRate  numeric(20, 6) null;
Alter table embprepaymentEmbDet drop column EmbassyId;
Alter table embprepaymentEmbDet drop column CurrencyCode;
Alter table embprepaymentEmbDet add ClearanceOrdNo nvarchar(25) not null;
Alter table embprepaymentEmbDet add ClearanceOrdDate date not null;
Alter table EmbPrePaymentInvDet  drop column CurrencyRate;
Alter table EmbPrePaymentInvDet alter column TelexRef nvarchar(50) null;
Alter table EmbPrePaymentInvDet alter column InvNo nvarchar(50) null;
Alter table EmbPrePaymentInvDet alter column InvDate date null;
Alter table EmbPrePaymentInvDet add LedgerCode int not null;
ALTER table ProductMaster add  ProdSubCategoryId uniqueidentifier null;
ALTER table ServiceRequest add  ProdSubCategoryId uniqueidentifier null;
ALTER table ServiceRequest add  RequiredQty numeric(18, 6) null;
ALTER table ServiceRequest alter column ProductMasterId uniqueidentifier null; 
ALTER table ServiceRequest alter column UnitMasterId uniqueidentifier null; 
drop table ServiceRequestDet;
alter table budgallochdr add FinBookNo nvarchar(20) null;
alter table Organization add DefaultOrg bit default 0;
--new script
alter table ServiceRequest add SeqNo bigint not null default 0;
alter table ServiceRequest drop column TransNo;
alter table ServiceRequest add TransNo varchar(20);

alter table QuotationRequest drop column TransNo;
alter table QuotationRequest add SeqNo bigint not null default 0;
alter table QuotationRequest add TransNo varchar(20);

alter table VendorQuotation drop column TransNo;
alter table VendorQuotation add SeqNo bigint not null default 0;
alter table VendorQuotation add TransNo varchar(20);

alter table PurchaseRequest drop column TransNo;
alter table PurchaseRequest add SeqNo bigint not null default 0;
alter table PurchaseRequest add TransNo varchar(20);

alter table PurchaseOrder drop column TransNo;
alter table PurchaseOrder add SeqNo bigint not null default 0;
alter table PurchaseOrder add TransNo varchar(20);

alter table GoodsReceiptNote drop column TransNo;
alter table GoodsReceiptNote add SeqNo bigint not null default 0;
alter table GoodsReceiptNote add TransNo varchar(20);

alter table ProdInvIssue drop column TransNo;
alter table ProdInvIssue add SeqNo bigint not null default 0;
alter table ProdInvIssue add TransNo varchar(20);

alter table InventoryTransfer drop column TransNo;
alter table InventoryTransfer add SeqNo bigint not null default 0;
alter table InventoryTransfer add TransNo varchar(20);