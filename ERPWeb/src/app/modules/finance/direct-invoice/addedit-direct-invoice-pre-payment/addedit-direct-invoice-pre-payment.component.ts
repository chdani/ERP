import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { WebApiService } from 'app/shared/webApiService';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { FinanceService } from '../../finance.service';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AddEditBudgetAllocationComponent } from '../../budget-allocation/addEdit-budget/addEdit-budget.component';
import { DatePipe } from '@angular/common';
import { takeUntil } from 'rxjs/operators';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';


@Component({
  selector: 'app-addedit-direct-invoice-pre-payment',
  templateUrl: './addedit-direct-invoice-pre-payment.component.html',
  styleUrls: ['./addedit-direct-invoice-pre-payment.component.scss'],
  providers: [TranslatePipe, DialogService, DatePipe, AddEditBudgetAllocationComponent]
})
export class AddeditDirectInvoicePrePaymentComponent extends AppComponentBase implements OnInit, OnDestroy {
  lang;
  constructor(
    injector: Injector,
    public router: Router,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private dialogService: DialogService,
    private _confirmService: ConfirmationService,
    private _primengConfig: PrimeNGConfig,
    private formBuilder: FormBuilder,
    private _financeService: FinanceService,

  ) {
    super(injector,'SCR_DIRECT_INVOICE_PRE','allowWrite', _commonService );
    this._primengConfig.ripple = true;
    _commonService.fileObserver.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((file: any) => {
        this.selectedUploadData.appDocuments = file;
      });
  }

  id: any;
  public createOrEditFrom: FormGroup;
  public ledgerCodes: any = [];
  public filteredLedgers: any = [];
  public costCenters: any = [];
  public vendorMaster: any = [];
  public filteredVendor: any = [];
  uploadConfig: FileUploadConfig;
  public submitted = false;
  selectedUploadData: any = {};
  public selectedLedger: any;
  AppDocuments: any = [];
  public selectedVendor: any;
  prviousInvoiceInfo: any;
  Filecontent: any = [];
  fileUpload = [];
  directInvoice: any;
  dialogRef: DynamicDialogRef;
  header: any = [];
  ledgerBalance:any=[];
  addoredit: boolean = false;
  allowDeletePermission: boolean = true;

  public activeTabIndex: any;

  vendorCode: '';
  ledgerCode: '';

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
      appDocuments: []
    };

    this.getAll();
    this.getHistoryData()
  }
  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != '') {
      //getting transaction id
      let getTransactionId = history.state.transactionId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("DIRECT_INVOICE_EDIT_TITLE"));
        this.getDirectInvoice();
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
  }

  async getAll() {
    this.ledgerCodes = await this._financeService.getLedgerAccounts("", false, false, this._translate, this.orgId);
    this.costCenters = await this._financeService.getCostCenters(false, false, this._translate);
    this.vendorMaster = await this._financeService.getVendorMaster();
    var search = {
      orgId: this.orgId,
      finYear: this.financialYear
    };
    var result = await this._webApiService.post("getLedgerAccWiseCurrentBalance", search);
    if (result) {
      this.ledgerBalance = result as any;
      this.ledgerCodes.forEach(element => {
        var balanceAcc = this.ledgerBalance.find(a => a.ledgerCode == element.ledgerCode)
        if (balanceAcc)
          element.balance = balanceAcc.balance;
        else
          element.balance = 0;
      });
    }
  }

  async getDirectInvoice() {
    let result = await this._webApiService.get('getDirectInvPrePayById/' + this.id);
    if (result) {
      this.Filecontent = result.appDocuments,
        this.prviousInvoiceInfo = result as any;
      if (this.prviousInvoiceInfo.validations == null) {
        this.directInvoice = {
          id: this.prviousInvoiceInfo.id,
          FinYear: this.prviousInvoiceInfo.finYear,
          OrgId: this.prviousInvoiceInfo.orgId,
          VendorMasterId: this.prviousInvoiceInfo.vendorMasterId,
          InvoiceNo: this.prviousInvoiceInfo.invoiceNo,
          LedgerCode: this.prviousInvoiceInfo.ledgerCode,
          CostCenterCode: this.prviousInvoiceInfo.costCenterCode,
          Amount: this.prviousInvoiceInfo.amount,
          Remarks: this.prviousInvoiceInfo.remarks,
          InvoiceDate: new Date(this.prviousInvoiceInfo.invoiceDate),
          DocumentDate: new Date(this.prviousInvoiceInfo.documentDate),

        }
        var ledger = this.ledgerCodes.filter(a => a.ledgerCode == this.prviousInvoiceInfo.ledgerCode)[0];
        if (ledger)
          this.selectedLedger = ledger;

        var vendor = this.vendorMaster.filter(a => a.id == this.prviousInvoiceInfo.vendorMasterId)[0];
        if (vendor)
          this.selectedVendor = vendor;
      }
    }
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

  filterLedgerAccounts(event: any) {    
    this.filteredLedgers = this.ledgerCodes.filter(a => a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase())
    && a.balance > 0);
  }

  filterVendorMaster(event: any) {
    this.filteredVendor = this.vendorMaster.filter(a => a.vendorName.toUpperCase().includes(event.query.toUpperCase()));
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

    if (!this.selectedLedger) {
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

    //addLedgeer and Vendor
    this.directInvoice.LedgerCode = this.selectedLedger.ledgerCode;
    this.directInvoice.VendorMasterId = this.selectedVendor.id;

    this.directInvoice.OrgId = this.orgId;
    this.directInvoice.FinYear = this.financialYear;

    if (this.directInvoice.id && this.directInvoice.id != "") {
      this.directInvoice.action = 'M';
      this.directInvoice.modifiedDate = this.prviousInvoiceInfo.modifiedDate;
      this.directInvoice.active = this.prviousInvoiceInfo.active;
    }
    else {
      this.directInvoice.action = 'N';
      this.directInvoice.finYear = this.financialYear;
    }
    this.directInvoice.active = "Y";
    this.directInvoice.status = "SUBMITTED";

    var result = await this._webApiService.post("saveDirectInvPrePay", this.directInvoice);

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
  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

}