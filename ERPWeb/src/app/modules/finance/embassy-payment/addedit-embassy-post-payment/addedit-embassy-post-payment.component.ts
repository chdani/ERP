import { DatePipe, JsonPipe } from '@angular/common';
import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { takeUntil } from 'rxjs/operators';
import { FinanceService } from '../../finance.service';

@Component({
  selector: 'addedit-embassy-post-payment',
  templateUrl: './addedit-embassy-post-payment.component.html',
  styleUrls: ['./addedit-embassy-post-payment.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService, DatePipe]
})

export class AddEditEmbassyPostPaymentComponent extends AppComponentBase implements OnInit, OnDestroy {
  postPayment: any;
  invDetails: any = [];

  public embassies: any = [];
  public currencies: any = [];

  public filteredEmbassies: any = [];
  public filteredCurrencies: any = [];

  public selectedEmbassy: any;
  public selectedCurrency: any;

  public prevPostPayment: any;
  public selectedTransId: string;
  public pendingInvoices: any = []
  //selectedInvoice: any;
  //filteredInvoice: any = [];
  ledgers: any = [];
  filteredLedgers: any = [];
  ledgerBalance: any = [];
  selectedLedger: any;
  selectedUploadData: any = {};

  public lang: any;
  public activeTabIndex: any;
  //public selectedInvoices: any;
  public showInvoices: boolean = false;
  public enableInvButton = false;
  public disableAmount = true;
  public checkAllInvoices = false;
  dialogRef: DynamicDialogRef;
  header: any = [];
  uploadConfig: FileUploadConfig;
  allowDeletePermission: boolean = true;

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _toastrService: ToastrService,
    private _primengConfig: PrimeNGConfig,
    private _finService: FinanceService,
    private datepipe: DatePipe,
    private _confirmService: ConfirmationService,
    private dialogService: DialogService
  ) {
    super(injector, 'SCR_EMB_POST_PAYMENTS', 'allowWrite', _commonService);
    this._primengConfig.ripple = true;
    this._commonService.fileObserver.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((file: any) => {
        this.selectedUploadData.appDocuments = file;
      });
  }

  ngOnInit(): void {


    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.postPayment = {
      id: '',
      bookNo: '',
      paymentDate: '',
      currencyRate: '1',
      currencyAmount: '',
      ledgerCode: '',
      embassyId: '',
      finYear: this.financialYear,
      orgId: this.orgId,
      currencyCode: '',
      amount: '',
      appDocuments: [],
    };
    this.getHistoryData();
    this.getAll()
  }

  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != '') {
      //getting transaction id
      let getTransactionId = history.state.transactionId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.postPayment.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("EMB_POST_PAYMENT_EDIT"));
      }
      else {
        this._commonService.updatePageName(this._translate.transform("EMB_POST_PAYMENT_ADD"));
      }
      //getting active tab index value
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("EMB_POST_PAYMENT_ADD"));
    }
  }

  async getAll() {
    this.embassies = await this._finService.getEmbassyList(false, this._translate);
    this.currencies = await this._finService.getCurrencies(false, this._translate);
    this.ledgers = await this._finService.getLedgerAccounts("", false, false, this._translate, this.orgId);
    var search = {
      orgId: this.orgId,
      finYear: this.financialYear
    };
    var result = await this._webApiService.post("getLedgerAccWiseCurrentBalance", search);
    if (result) {
      this.ledgerBalance = result as any;
      this.ledgers.forEach(element => {
        var balanceAcc = this.ledgerBalance.find(a => a.ledgerCode == element.ledgerCode)
        if (balanceAcc)
          element.balance = balanceAcc.balance;
        else
          element.balance = 0;
      });
    }

    if (this.postPayment.id && this.postPayment.id != '') {
      var result = await this._webApiService.get("getEmbpostPaymentById/" + this.postPayment.id);
      if (result) {
        this.prevPostPayment = result as any;
        if (this.prevPostPayment.validations == null) {
          this.postPayment = {
            id: this.prevPostPayment.id,
            bookNo: this.prevPostPayment.bookNo,
            paymentDate: new Date(this.prevPostPayment.paymentDate),
            embassyId: this.prevPostPayment.embassyId,
            finYear: this.prevPostPayment.finYear,
            orgId: this.prevPostPayment.orgId,
            currencyCode: this.prevPostPayment.currencyCode,
            ledgerCode: this.prevPostPayment.ledgerCode,
            currencyRate: this.prevPostPayment.currencyRate,
            amount: this.prevPostPayment.amount,
            embPrePaymentHdrId: this.prevPostPayment.embPrePaymentHdrId,
            embPrePaymentDetId: this.prevPostPayment.embPrePaymentDetId,
            currencyAmount: this.prevPostPayment.currencyAmount,
            appDocuments: this.prevPostPayment.appDocuments
          };


          var embassy = this.embassies.filter(a => a.id == this.prevPostPayment.embassyId)[0];
          if (embassy)
            this.selectedEmbassy = embassy;

          var currency = this.currencies.filter(a => a.code == this.prevPostPayment.currencyCode)[0];
          if (currency)
            this.selectedCurrency = currency;

          var ledger = this.ledgers.filter(a => a.ledgerCode == this.prevPostPayment.ledgerCode)[0];
          if (ledger)
            this.selectedLedger = ledger;
        }
        this.loadInvoices(this.selectedEmbassy, true);
      }
    }
  }
  attachmentLink(data, type) {
    if (this.isCompGranted('SCR_BUDGT_ALLOCATION', this.actionType.allowDelete)) {
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
  async createOredit() {
    if (!this.selectedEmbassy) {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_EMB_NAME_REQ"));
      return;
    }
    if (!this.selectedCurrency) {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_CURRENCY_REQ"));
      return;
    }

    if (this.postPayment.amount == "" || Number(this.postPayment.amount) == 0) {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_AMOUNT_REQ"));
      return;
    }
    if (this.postPayment.currencyRate == "" || Number(this.postPayment.currencyRate) == 0) {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_CUR_RATE_REQ"));
      return
    }
    if (this.postPayment.currencyAmount == "" || Number(this.postPayment.currencyAmount) == 0) {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_CUR_AMOUNT_REQ"));
      return;
    }
    if (this.postPayment.paymentDate == "") {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_PAY_DATE_REQ"));
      return;
    }

    if (this.postPayment.bookNo == "") {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_BOOK_NO_REQ"));
      return;
    }
    if (!this.selectedLedger) {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_LEDGER_REQ"));
      return;
    }
    var selectedInvoices = this.pendingInvoices.filter(a => a.selected);
    if (!selectedInvoices || selectedInvoices.length == 0) {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_INVOICE_REQ"));
      return;
    }

    this.postPayment.embPostPaymentInvDet = []
    selectedInvoices.forEach(element => {
      this.postPayment.embPostPaymentInvDet.push({
        embPrePaymentInvDetId: element.detailId,
        active: "Y"
      });

    });

    if (this.postPayment.id && this.postPayment.id != "") {
      this.postPayment.action = 'M';
      this.postPayment.modifiedDate = this.prevPostPayment.modifiedDate;
      //mark as deleted for removed invoices
      this.prevPostPayment.embPostPaymentInvDet.forEach(element => {
        var det = this.postPayment.embPostPaymentInvDet.find(a => a.embPrePaymentInvDetId == element.embPrePaymentInvDetId)
        if (!det) {
          element.active = 'N'
          this.postPayment.embPostPaymentInvDet.push(JSON.parse(JSON.stringify(element)));
        }
      });
      //mark as modified for existing records.
      this.postPayment.embPostPaymentInvDet.forEach(element => {
        var det = this.prevPostPayment.embPostPaymentInvDet.find(a => a.embPrePaymentInvDetId == element.embPrePaymentInvDetId)
        if (det) {
          element.id = det.id;
          element.modifiedDate = det.modifiedDate;
        }
      });
    }
    else {
      this.postPayment.action = 'N';
      this.postPayment.finYear = this.financialYear;
    }

    this.postPayment.active = 'Y';
    this.postPayment.embassyId = this.selectedEmbassy.id;
    this.postPayment.currencyCode = this.selectedCurrency.code;
    this.postPayment.ledgerCode = this.selectedLedger.ledgerCode;
    this.postPayment.orgId = this.orgId;
    this.postPayment.status = "SUBMITTED";
    var result = await this._webApiService.post("saveEmbpostPayment", this.postPayment)
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));

        // this._confirmService.confirm({
        //   message: this._translate.transform("APP_PRINT_CONFIRM_MSG"),
        //   accept: async () => {
        //     let url = "finance/embassy-payment";
        //     this.router.navigate(["print/print-emb-post-payment"], {
        //       state: { transactionId: output.referenceId, setActiveTabIndex: this.activeTabIndex, setBackUrlForPrint: url },
        //     });
        //   },
        //   reject: async () => {
        //     this.router.navigate(["finance/embassy-payment"], {
        //       state: { setActiveTabIndex: this.activeTabIndex },
        //     });
        //   }
        // });
        this.router.navigate(["finance/embassy-payment"], {
          state: { setActiveTabIndex: this.activeTabIndex },
        });
      }
      else {
        console.log(output.messages[0]);
        this._toastrService.error(output.messages[0])
      }
    }
  }

  cancelAddEdit() {
    this.router.navigate(["finance/embassy-payment"], {
      state: { setActiveTabIndex: this.activeTabIndex },
    });
  }


  async loadInvoices(item: any, isEdit: boolean = false) {
    var input = {
      finYear: this.financialYear,
      orgId: this.orgId,
      embassyId: item.id
    };
    this.pendingInvoices = [];

    var invoices = [];
    if (isEdit) {
      this.prevPostPayment.embPostPaymentInvDet.forEach(element => {
        element.embPrePaymentDueDet.selected = true;
        invoices.push(JSON.parse(JSON.stringify(element.embPrePaymentDueDet)));
      });
    }
    var result = await this._webApiService.post("getEmbPrePaymentDues", input);
    if (result) {
      var totalDues = result as any;
      totalDues.forEach(emb => {
        emb.dueDetail.forEach(element => {
          element.selected = false;
          if (element.invNo != "")
            invoices.push(element);
        });
      });

    }
    this.pendingInvoices = invoices;
    this.enableInvButton = true;
  }

  filterEmbasy(event: any) {
    this.filteredEmbassies = this.embassies.filter(a => a.embassyName.toUpperCase().includes(event.query.toUpperCase()));
  }

  filterCurrency(event: any) {
    this.filteredCurrencies = this.currencies.filter(a => a.currencyName.toUpperCase().includes(event.query.toUpperCase()));
  }

  onInvoiceSelecDeselect(event, type) {
    if (type == "ALL") {
      this.pendingInvoices.forEach(element => {
        element.selected = event.checked;
      });
    }
    else {
      var checkedItems = this.pendingInvoices.filter(a => a.selected);
      this.checkAllInvoices = checkedItems.length == this.pendingInvoices.length;
    }
    this.postPayment.amount = 0;
    this.pendingInvoices.forEach(element => {
      if (element.selected)
        this.postPayment.amount += element.dueAmount;
    });
    this.postPayment.currencyAmount = this.postPayment.amount * this.postPayment.currencyRate;
  }

  filterLedger(event: any) {
    this.filteredLedgers = this.ledgers.filter(a => a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase())
      && a.balance > 0);
  }
  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }



  calculateCurrAmount(event) {
    this.postPayment.currencyAmount = this.postPayment.amount * this.postPayment.currencyRate;
  }
}
