 using ERPService.DataModel.DTO;
using System.Data.Entity;


namespace ERPService.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=DBConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new UserMasterConfig());
            //Master tables
            modelBuilder.Entity<AppDocument>().ToTable("AppDocument");
            modelBuilder.Entity<UserMaster>().ToTable("UserMaster");
            modelBuilder.Entity<AppAccessRoleMapping>().ToTable("AppAccessRoleMap");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<UserRoleMapping>().ToTable("UserRoleMap");
            modelBuilder.Entity<LangMaster>().ToTable("LangMaster");
            modelBuilder.Entity<CodesMaster>().ToTable("CodesMaster");
            modelBuilder.Entity<CodesDetails>().ToTable("CodesDetail");
            modelBuilder.Entity<UserSetting>().ToTable("UserSetting");
            modelBuilder.Entity<SystemSetting>().ToTable("SystemSettings");
            modelBuilder.Entity<Organization>().ToTable("Organization");
            modelBuilder.Entity<EmbassyMaster>().ToTable("EmbassyMaster");
            modelBuilder.Entity<CurrencyMaster>().ToTable("CurrencyMaster");

            //Finance Tables
            modelBuilder.Entity<UserOrganizationMap>().ToTable("UserOrganization");
            modelBuilder.Entity<UserLedgerAccnt>().ToTable("UserLedgerAccnt");
            modelBuilder.Entity<LedgerAccount>().ToTable("LedgerAccounts");
            modelBuilder.Entity<BudgAllocHdr>().ToTable("BudgAllocHdr");
            modelBuilder.Entity<BudgAllocDet>().ToTable("BudgAllocDet");
            modelBuilder.Entity<LedgerBalance>().ToTable("LedgerBalance");
            modelBuilder.Entity<LedgerBalanceDraft>().ToTable("LedgerBalDraft");
            modelBuilder.Entity<LedgerAccountGrp>().ToTable("LedgerAccountGrps");
            modelBuilder.Entity<CashTransaction>().ToTable("CashTransacion");
            modelBuilder.Entity<CashTransacionHist>().ToTable("CashTransacionHist");
            modelBuilder.Entity<CashTransacionComment>().ToTable("CashTransacionComment");
            modelBuilder.Entity<CostCenter>().ToTable("CostCenters");
            modelBuilder.Entity<PettyCashAccount>().ToTable("PettyCashAccount");
            modelBuilder.Entity<PettyCashTeller>().ToTable("PettyCashTeller");
            modelBuilder.Entity<PettyCashTransferComment>().ToTable("PettyCashTransferComment");
            modelBuilder.Entity<PettyCashTransferHist>().ToTable("PettyCashTransferHist");
            modelBuilder.Entity<PettyCashTransfer>().ToTable("PettyCashTransfer");
            modelBuilder.Entity<PettyCashBalance>().ToTable("PettyCashBalance");
            modelBuilder.Entity<EmbPrePaymentHdr>().ToTable("EmbPrePaymentHdr");
            modelBuilder.Entity<EmbPrePaymentEmbDet>().ToTable("EmbPrePaymentEmbDet");
            modelBuilder.Entity<EmbPrePaymentInvDet>().ToTable("EmbPrePaymentInvDet");
            modelBuilder.Entity<EmbPrePaymentHdrComment>().ToTable("EmbPrePaymentHdrComment");
            modelBuilder.Entity<EmbPrePaymentHdrHist>().ToTable("EmbPrePaymentHdrHist");
            modelBuilder.Entity<EmbPrePaymentHdrStatusHist>().ToTable("EmbPrePaymentHdrStatusHist");
            modelBuilder.Entity<EmbPrePaymentEmbDetComment>().ToTable("EmbPrePaymentEmbDetComment");
            modelBuilder.Entity<EmbPrePaymentEmbDetHist>().ToTable("EmbPrePaymentEmbDetHist");
            modelBuilder.Entity<EmbPrePaymentInvDetComment>().ToTable("EmbPrePaymentInvDetComment");
            modelBuilder.Entity<EmbPrePaymentInvDetHist>().ToTable("EmbPrePaymentInvDetHist");
            modelBuilder.Entity<EmbPostPayment>().ToTable("EmbPostPayment");
            modelBuilder.Entity<EmbPostPaymentInvDet>().ToTable("EmbPostPaymentInvDet");
            modelBuilder.Entity<EmbPostPaymentComment>().ToTable("EmbPostPaymentComment");
            modelBuilder.Entity<EmbPostPaymentHist>().ToTable("EmbPostPaymentHist");
            modelBuilder.Entity<EmbPostPaymentStatusHist>().ToTable("EmbPostPaymentStatusHist");
            modelBuilder.Entity<VendorMaster>().ToTable("VendorMaster");
            modelBuilder.Entity<DirectInvPostPayment>().ToTable("DirectInvPostPayment");
            modelBuilder.Entity<DirInvPostPayHist>().ToTable("DirInvPostPayHist");
            modelBuilder.Entity<DirInvPostPayStatusHist>().ToTable("DirInvPostPayStatusHist ");
            modelBuilder.Entity<DirInvPostPayComment>().ToTable("DirInvPostPayComment");
            modelBuilder.Entity<DirectInvPrePayment>().ToTable("DirectInvPrePayment");
            modelBuilder.Entity<DirInvPrePayStatusHist>().ToTable("DirInvPrePayStatusHist");
            modelBuilder.Entity<DirInvPrePayComment>().ToTable("DirInvPrePayComment");
            modelBuilder.Entity<DirInvPrePayHist>().ToTable("DirInvPrePayHist");
            modelBuilder.Entity<BudgAllocHdrComment>().ToTable("BudgAllocHdrComment");
            modelBuilder.Entity<BudgAllocHdrHist>().ToTable("BudgAllocHdrHist");
            modelBuilder.Entity<BudgAllocHdrStatusHist>().ToTable("BudgAllocHdrStatusHist");
            modelBuilder.Entity<BudgAllocDetHist>().ToTable("BudgAllocDetHist");
            modelBuilder.Entity<BudgAllocDetComment>().ToTable("BudgAllocDetComment");
            //Application Tabels
            modelBuilder.Entity<ActivityLog>().ToTable("ActivityLog");
            modelBuilder.Entity<ExceptionLog>().ToTable("ExceptionLog");
            modelBuilder.Entity<AppAccess>().ToTable("AppAccess");
            modelBuilder.Entity<AppMenuMaster>().ToTable("AppMenuMaster");
            modelBuilder.Entity<AppMessage>().ToTable("AppMessage");
            //HR
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<JobPosition>().ToTable("JobPosition");
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<EmpEducation>().ToTable("EmpEducation");
            modelBuilder.Entity<EmpDependent>().ToTable("EmpDependent");
            
            //Purchase and Inventory
            modelBuilder.Entity<ProdUnitMaster>().ToTable("ProdUnitMaster");
            modelBuilder.Entity<ProdCategoryWorkFlow>().ToTable("ProdCatWorkFlow");
            modelBuilder.Entity<ProductCategory>().ToTable("ProdCategory");
            modelBuilder.Entity<ProdSubCategory>().ToTable("ProdSubCategory");
            modelBuilder.Entity<ServiceRequest>().ToTable("ServiceRequest");
            modelBuilder.Entity<ServiceReqApproval>().ToTable("ServiceReqApproval");
            modelBuilder.Entity<ProductMaster>().ToTable("ProductMaster");
            modelBuilder.Entity<VendorQuotation>().ToTable("VendorQuotation");
            modelBuilder.Entity<VendorQuotationComment>().ToTable("VendorQuotationComment");
            modelBuilder.Entity<VendorQuotationDetComment>().ToTable("VendorQuotationDetComment");
            modelBuilder.Entity<VendorQuotationDetHist>().ToTable("VendorQuotationDetHist");
            modelBuilder.Entity<VendorQuotationHist>().ToTable("VendorQuotationHist");
            modelBuilder.Entity<VendorQuotationStatusHist>().ToTable("VendorQuotationStatusHist");
            modelBuilder.Entity<VendorQuotationDet>().ToTable("VendorQuotationDet");
            modelBuilder.Entity<QuotationRequest>().ToTable("QuotationRequest");
            modelBuilder.Entity<QuotationReqDet>().ToTable("QuotationReqDet");
            modelBuilder.Entity<VendorProduct>().ToTable("VendorProduct");
            modelBuilder.Entity<VendorContact>().ToTable("VendorContact");
            modelBuilder.Entity<VendorContract>().ToTable("VendorContract");
            modelBuilder.Entity<ServiceReqHist>().ToTable("ServiceReqHist");
            modelBuilder.Entity<QuotaReqVendorDet>().ToTable("QuotReqVendorDet");
            modelBuilder.Entity<ServiceReqComment>().ToTable("ServiceReqComment");
            modelBuilder.Entity<GoodsRecNote>().ToTable("GoodsReceiptNote");
            modelBuilder.Entity<GoodsRecNoteComment>().ToTable("GoodsRecNoteComment");
            modelBuilder.Entity<GoodsRecNoteDet>().ToTable("GoodReceiptNoteDet");
            modelBuilder.Entity<GoodsRecNoteDetComment>().ToTable("GoodsRecNoteDetComment");
            modelBuilder.Entity<GoodsRecNoteDetHist>().ToTable("GoodsRecNoteDetHist");
            modelBuilder.Entity<GoodsRecNoteHist>().ToTable("GoodsRecNoteHist");
            modelBuilder.Entity<GoodsRecNoteStatusHist>().ToTable("GoodsRecNoteStatusHist");
            modelBuilder.Entity<WareHouse>().ToTable("WareHouse");
            modelBuilder.Entity<WareHouseLocation>().ToTable("WareHouseLocation");
            modelBuilder.Entity<ProductInventory>().ToTable("ProdInventory");
            modelBuilder.Entity<PurchaseOrder>().ToTable("PurchaseOrder");
            modelBuilder.Entity<PurchaseOrderDet>().ToTable("PurchaseOrderDet");
            modelBuilder.Entity<ProdInvIssue>().ToTable("ProdInvIssue");
            modelBuilder.Entity<ProdInvIssueComment>().ToTable("ProdInvIssueComment");
            modelBuilder.Entity<ProdInvIssueDet>().ToTable("ProdInvIssueDet");
            modelBuilder.Entity<ProdInvIssueDetComment>().ToTable("ProdInvIssueDetComment");
            modelBuilder.Entity<ProdInvIssueHist>().ToTable("ProdInvIssueHist");
            modelBuilder.Entity<ProdInvIssueDetHist>().ToTable("ProdInvIssueDetHist");
            modelBuilder.Entity<ProdInvIssueStatusHist>().ToTable("ProdInvIssueStatusHist");
            modelBuilder.Entity<ProdInventoryBalance>().ToTable("ProdInventoryBalance");
            modelBuilder.Entity<PurchaseRequestDet>().ToTable("PurchaseRequestDet");
            modelBuilder.Entity<PurchaseRequest>().ToTable("PurchaseRequest");
            modelBuilder.Entity<PurchaseReqHist>().ToTable("PurchaseReqHist");
            modelBuilder.Entity<PurchaseReqDetHist>().ToTable("PurchaseReqDetHist");
            modelBuilder.Entity<PurchaseReqDetComment>().ToTable("PurchaseReqDetComment");
            modelBuilder.Entity<PurchaseReqComment>().ToTable("PurchaseReqComment");
            modelBuilder.Entity<PurchaseRequestStatusHist>().ToTable("PurchaseRequestStatusHist");
            modelBuilder.Entity<PurchaseOrdComment>().ToTable("PurchaseOrdComment");
            modelBuilder.Entity<PurchaseOrdDetComment>().ToTable("PurchaseOrdDetComment");
            modelBuilder.Entity<PurchaseOrdDetHist>().ToTable("PurchaseOrdDetHist");
            modelBuilder.Entity<PurchaseOrdHist>().ToTable("PurchaseOrdHist");
            modelBuilder.Entity<PurchaseOrdStatusHist>().ToTable("PurchaseOrdStatusHist");
            modelBuilder.Entity<InventoryTransfer>().ToTable("InventoryTransfer");
            modelBuilder.Entity<InventoryTransferDet>().ToTable("InventoryTransferDet");
            modelBuilder.Entity<InvTransferComment>().ToTable("InvTransferComment");
            modelBuilder.Entity<InvTransferDetComment>().ToTable("InvTransferDetComment");
            modelBuilder.Entity<InvTransferDetHist>().ToTable("InvTransferDetHist");
            modelBuilder.Entity<InvTransferHist>().ToTable("InvTransferHist");
            modelBuilder.Entity<InvTransferStatusHist>().ToTable("InvTransferStatusHist");
            modelBuilder.Entity<QuotationReqDetComment>().ToTable("QuotationReqDetComment");
            modelBuilder.Entity<QuotationReqDetHist>().ToTable("QuotationReqDetHist");
            modelBuilder.Entity<QuotationRequestComment>().ToTable("QuotationRequestComment");
            modelBuilder.Entity<QuotationReqStatusHist>().ToTable("QuotationReqStatusHist");
            modelBuilder.Entity<QuotationRequestHist>().ToTable("QuotationRequestHist");
            modelBuilder.Entity<ProdSubCategory>().ToTable("ProdSubCategory");
        }

        public override int SaveChanges()
        {
            int result = base.SaveChanges();

            return result;
        }
    }
}