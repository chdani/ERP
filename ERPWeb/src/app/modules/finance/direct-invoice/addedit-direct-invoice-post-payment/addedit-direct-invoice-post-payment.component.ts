import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { WebApiService } from 'app/shared/webApiService';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { FinanceService } from '../../finance.service';
import { DatePipe } from '@angular/common';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AddEditBudgetAllocationComponent } from '../../budget-allocation/addEdit-budget/addEdit-budget.component';
import { takeUntil } from 'rxjs/operators';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
@Component({
  selector: 'app-addedit-direct-invoice-post-payment',
  templateUrl: './addedit-direct-invoice-post-payment.component.html',
  styleUrls: ['./addedit-direct-invoice-post-payment.component.scss'],
  providers: [TranslatePipe, DialogService, DatePipe, AddEditBudgetAllocationComponent]
})
export class AddeditDirectInvoicePostPaymentComponent extends AppComponentBase implements OnInit {
  lang;

  // Load Invoice Details
  public pendingInvoices: any = []
  public selectedDirInv: any;
  public prevPostPayment: any;
  public filteredInvoice: any = [];

  public dirInvPrePaymentId: any;

  constructor(
    injector: Injector,
    public router: Router,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _confirmService: ConfirmationService,
    private _primengConfig: PrimeNGConfig,
    private formBuilder: FormBuilder,
    private _financeService: FinanceService,
    private datepipe: DatePipe,
    private dialogService: DialogService
  ) {
    super(injector,'SCR_DIRECT_INVOICE_POST','allowWrite', _commonService );
    this._primengConfig.ripple = true;
    _commonService.fileObserver.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((file: any) => {
        this.selectedUploadData.appDocuments = file;
      });
  }
  dialogRef: DynamicDialogRef;
  public id: any;
  public postPayment: any;
  public createOrEditFrom: FormGroup;
  uploadConfig: FileUploadConfig;
  addoredit: boolean = false;
  header: any = [];
  public ledgerCodes: any = [];
  public filteredLedgers: any = [];
  public costCenters: any = [];
  selectedUploadData: any = {};
  fileUpload = [];
  public vendorMaster: any = [];
  public filteredVendor: any = [];
  public submitted = false;
  public selectedLedger: any;
  public selectedVendor: any;
  public Filecontent: any = [];
  public previousInvoiceInfo: any;
  public directInvoice: any;
  allowDeletePermission: boolean = true;


  public vendorCode: '';
  public ledgerCode: '';

  public activeTabIndex: any;
  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.directInvoice = {
      id: '',
      FinYear: '',
      OrgId: '',
      VendorMasterId: '',
      InvoiceNo: '',
      InvoiceDate: '',
      DocumentDate: '',
      LedgerCode: '',
      CostCenterCode: '',
      Amount: '',
      Remarks: '',
      DueAmount: '',
      DirInvPrePaymentId: '',
      modifiedDate: '',
      appDocuments: []
    };

    this.getHistoryData();
  }
  attachmentLink(data, type) {
    if (this.isGranted('SCR_DIRECT_INVOICE', this.actionType.allowDelete)) {
      this.allowDeletePermission = false;
    }
    this.header = this._translate.transform("FILE_ATTACHMENT");
    this.selectedUploadData = data;
    this.uploadConfig =
    {
      TransactionId: data.id,
      TransactionType: type,
      AllowedExtns: ".png,.jpg,.gif,.jpeg,.bmp,.docx,.doc,.pdf,.msg",
      DocTypeRequired: false,
      DocumentReference: "",
      ReadOnly: this.allowDeletePermission,
      ScanEnabled: true,
      ShowSaveButton: false,
      FileContent: data.appDocuments
    };

    this.dialogRef = this.dialogService.open(FileUploadComponent, {
      header: this.header,
      width: "700px",
      closable: false,
      contentStyle: { "height": "500px", overflow: "auto" },
      baseZIndex: 500,
      dismissableMask: true,
      data: this.uploadConfig
    });
  }


  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != '') {
      //getting transaction id
      let getTransactionId = history.state.transactionId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.directInvoice.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("DIRECT_INVOICE_EDIT_TITLE"));
      }
      else {
        this._commonService.updatePageName(this._translate.transform("DIRECT_INVOICE_ADD_TITLE"));
      }
      //getting active tab index value
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("DIRECT_INVOICE_ADD_TITLE"));
    }
    this.getAll();
  }

  async getAll() {
    this.ledgerCodes = await this._financeService.getLedgerAccounts("", false, false, this._translate, this.orgId);
    this.costCenters = await this._financeService.getCostCenters(false, false, this._translate);
    this.vendorMaster = await this._financeService.getVendorMaster();
    console.log(this.vendorMaster);

    if (this.directInvoice.id && this.directInvoice.id != '') {
      let result = await this._webApiService.get('getDirectInvPostPayById/' + this.directInvoice.id);
      if (result) {
        this.Filecontent = result.appDocuments;
        this.previousInvoiceInfo = result as any;
        if (this.previousInvoiceInfo.validations == null) {
          var ledger =this.ledgerCodes.find(a=>a.ledgerCode ==this.previousInvoiceInfo.ledgerCode);
          this.directInvoice = {
            id: this.previousInvoiceInfo.id,
            FinYear: this.previousInvoiceInfo.finYear,
            OrgId: this.previousInvoiceInfo.orgId,
            VendorMasterId: this.previousInvoiceInfo.vendorMasterId,
            InvoiceNo: this.previousInvoiceInfo.invoiceNo,
            LedgerCode:ledger?.ledgerDescCode,
            CostCenterCode: this.previousInvoiceInfo.costCenterCode,
            Amount: this.previousInvoiceInfo.amount,
            Remarks: this.previousInvoiceInfo.remarks,
            InvoiceDate: new Date(this.previousInvoiceInfo.invoiceDate),
            DocumentDate: new Date(this.previousInvoiceInfo.documentDate),
            DueAmount: this.previousInvoiceInfo.dueAmount,
          }

          var ledger = this.ledgerCodes.filter(a => a.ledgerCode == this.previousInvoiceInfo.ledgerCode)[0];
          if (ledger)
            this.selectedLedger = ledger;

          var vendor = this.vendorMaster.filter(a => a.id == this.previousInvoiceInfo.vendorMasterId)[0];
          if (vendor) {
            this.selectedVendor = vendor;
          }

          var selectInv = this.pendingInvoices.filter(a => a.dueAmount == this.previousInvoiceInfo.dueAmount);
          if (selectInv) {
            this.selectedDirInv = selectInv;
          }
        }
        this.loadInvoices(this.selectedVendor, true);
      }
    }
  }

  filterVendorMaster(event: any) {
    this.filteredVendor = this.vendorMaster.filter(a => a.vendorName.toUpperCase().includes(event.query.toUpperCase()));
  }

  filterInvoice(event: any) {
    this.filteredInvoice = this.pendingInvoices.filter(a => a.dueAmount.toString().includes(event.query));
  }
  onInvoiceSelection() {
    if(this.selectedDirInv.invoiceDate !="0001-01-01T00:00:00"){
      this.directInvoice.InvoiceDate = new Date(this.selectedDirInv.invoiceDate);
    }    
    this.directInvoice.InvoiceNo = this.selectedDirInv.invoiceNo;
    var ledger =this.ledgerCodes.find(a=>a.ledgerCode ==this.selectedDirInv.LedgerCode);
    this.directInvoice.LedgerCode=ledger?.ledgerDescCode;
  }

  async saveDirectInvoice() {
    if (!this.directInvoice.InvoiceNo) {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_INVOICE_NO_REQ"));
      return;
    }

    if (this.directInvoice.InvoiceDate == "") {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_INVOICE_DATE_REQ"));
      return;
    }

    if (this.directInvoice.DocumentDate == "") {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_DOC_DATE_REQ"));
      return;
    }

    if (!this.selectedVendor) {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_VENDOR_REQ"));
      return;
    }

    if (this.directInvoice.LedgerCode == "") {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_LEDGER_REQ"));
      return;
    }

    if (this.directInvoice.CostCenterCode == "") {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_CC_REQ"));
      return;
    }

    if (this.directInvoice.Amount == "" || this.directInvoice.Amount <= 0) {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_AMOUNT_REQ"));
      return;
    }

    if ((this.selectedDirInv.dueAmount == "" || this.selectedDirInv.dueAmount <= 0)) {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_DUE_AMOUNT_REQ"));
      return;
    }
    var prevVAlue = 0;
    if (this.directInvoice.id && this.directInvoice.id != "")
      prevVAlue = this.previousInvoiceInfo.Amount
    if ((this.directInvoice.Amount > (this.selectedDirInv.dueAmount + prevVAlue))) {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_DUE_AMOUNT_EQUAL_REQ"));
      return;
    }

    var ledger =this.ledgerCodes.find(a=>a.ledgerDescCode ==this.directInvoice.LedgerCode);
    this.directInvoice.LedgerCode=ledger.ledgerCode;
    this.directInvoice.VendorMasterId = this.selectedVendor.id;

    this.directInvoice.OrgId = this.orgId;
    this.directInvoice.FinYear = this.financialYear;

    this.directInvoice.DueAmount = this.selectedDirInv.dueAmount;
    this.directInvoice.DirInvPrePaymentId = this.selectedDirInv.dirInvPrePaymentId;

    if (this.directInvoice.id && this.directInvoice.id != "") {
      this.directInvoice.action = 'M';
      this.directInvoice.modifiedDate = this.previousInvoiceInfo.modifiedDate;
      this.directInvoice.active = this.previousInvoiceInfo.active;
    }
    else {
      this.directInvoice.action = 'N';
      this.directInvoice.finYear = this.financialYear;
    }

    this.directInvoice.status = "SUBMITTED";
    var result = await this._webApiService.post("saveDirectInvPostPay", this.directInvoice);

    if (result) {
      var output = result as any;
      if (output.validations == null) {
        if (output.status == "DATASAVESUCSS") {
          this._toastrService.success(this._translate.transform("APP_SUCCESS"));
          this.router.navigate(["finance/direct-invoice"], {
            state: { setActiveTabIndex: this.activeTabIndex },
          });
        }
        else {
          this._toastrService.error(output.messages[0])
        }
      }
      else {
        this._toastrService.error(output.messages[0])
      }
    }
  }

  cancelAddEdit() {
    this.router.navigate(["finance/direct-invoice"], {
      state: { setActiveTabIndex: this.activeTabIndex },
    });
  }

  async loadInvoices(item: any, isEdit: boolean = false) {
    var data = {
      finYear: this.financialYear,
      orgId: this.orgId,
      vendorId: item.id
    };
    this.selectedDirInv = null;
    // Get Direct Invoice Pre Payments Due

    this.pendingInvoices = [];

    let result = await this._webApiService.post('getDirInvPrePaymentDues', data);
    if (result) {
      if (result.validations == null) {
        result.forEach(element => {
          this.pendingInvoices.push({
            dueAmount: element.dueAmount,
            dirInvPrePaymentId: element.dirInvPrePaymentId,
            invoiceDate: element.invoiceDate,
            invoiceNo: element.invoiceNo,
            LedgerCode:element.ledgerCode
          });
        });
      }
    }

    if (isEdit) {
      var invoice = this.pendingInvoices.filter(a => a.dirInvPrePaymentId = this.previousInvoiceInfo.dirInvPrePaymentId)[0];
      if (invoice)
        this.selectedDirInv = invoice;
    }
  }




}
