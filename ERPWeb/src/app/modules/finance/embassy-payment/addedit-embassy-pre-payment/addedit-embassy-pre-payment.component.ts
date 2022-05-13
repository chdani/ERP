import { Component, Injector, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { takeUntil } from 'rxjs/operators';
import { FinanceService } from '../../finance.service';

@Component({
  selector: 'addedit-embassy-pre-payment',
  templateUrl: './addedit-embassy-pre-payment.component.html',
  styleUrls: ['./addedit-embassy-pre-payment.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService]
})

export class AddEditEmbassyPrePaymentComponent extends AppComponentBase implements OnInit, OnDestroy {
  prePayment: any;
  invDetails: any = [];
  embDetails: any = [];

  public embassies: any = [];
  public currencies: any = [];

  public filteredEmbassies: any = [];
  public filteredCurrencies: any = [];
  public filteredLedger: any = [];

  public selectedEmbassy: any;
  public selectedCurrency: any;

  public prevPrePayment: any;
  public selectedTransId: string;

  public activeTabIndex: any;
  public invDetTitle: string = "";
  embPrePayDetTotelAmount: any;
  embPrePayInvDetTotelAmount: any;
  embPrePayInvDetCurrencyAmount: any;
  public showInvoices: boolean = false;
  allowDeletePermission: boolean = true;
  lang: any;
  public selectedEmbassyDet: any;
  dialogRef: DynamicDialogRef;
  selectedUploadData: any = {};
  header: any = [];
  uploadConfig: FileUploadConfig;
  ledgerCodes: any = [];
  transactionTypes: any = [];

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _toastrService: ToastrService,
    private _primengConfig: PrimeNGConfig,
    private _finService: FinanceService,
    private _confirmService: ConfirmationService,
    private dialogService: DialogService,
    private _codeMasterService: CodesMasterService,
  ) {
    super(injector, 'SCR_EMB_PRE_PAYMENTS', 'allowWrite', _commonService);
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this._primengConfig.ripple = true;
    this._commonService.fileObserver.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((file: any) => {
        this.selectedUploadData.appDocuments = file;
      });
  }

  ngOnInit(): void {
    this.prePayment = {
      id: '',
      bookNo: '',
      bookDate: '',
      embassyId: '',
      finYear: this.financialYear,
      orgId: this.orgId,
      currencyCode: '',
      amount: '',
      dueAmount: '',
      currencyRate: '',
      appDocuments: [],
    };

    this.loadDefaults();
    this.embPrePayDetTotelAmount = 0;
    this.embPrePayInvDetTotelAmount = 0;
    this.embPrePayInvDetCurrencyAmount = 0;

  }
  async loadDefaults() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let getTransactionId = history.state.transactionId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.prePayment.id = history.state.transactionId;
        this._commonService.updatePageName(this._translate.transform("EMB_PRE_PAYMENT_EDIT"));
      }
      else {
        this._commonService.updatePageName(this._translate.transform("EMB_PRE_PAYMENT_ADD"));
      }
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("EMB_PRE_PAYMENT_ADD"));
    }
    await this.getAll();
  }
  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
  async Allledgercode() {
    var search = {
      orgId: this.orgId,
      finYear: this.financialYear
    };
    var result = await this._webApiService.post("getLedgerAccWiseCurrentBalance", search);
    if (result) {
      this.ledgerCodes.forEach(element => {
        var balanceAcc = result.find(a => a.ledgerCode == element.ledgerCode)
        if (balanceAcc)
          element.balance = balanceAcc.balance;
        else
          element.balance = 0;
      });
    }
  }
  async getAll() {
    this.ledgerCodes = await this._finService.getLedgerAccounts("", false, false, this._translate, this.orgId);
    await this.Allledgercode();
    this.transactionTypes = await this._codeMasterService.getCodesDetailByGroupCode("LEDGERACCUSEDFOR", false, true, this._translate);
    this.embassies = await this._finService.getEmbassyList(false, this._translate);
    this.currencies = await this._finService.getCurrencies(false, this._translate);

    if (this.prePayment.id && this.prePayment.id != '') {
      var result = await this._webApiService.get("getEmbPrePaymentById/" + this.prePayment.id);
      if (result) {
        this.prevPrePayment = result as any;
        var embassy = this.embassies.find(a => a.id == this.prevPrePayment.embassyId);
        var currency = this.currencies.find(a => a.code == this.prevPrePayment.currencyCode);
        if (this.prevPrePayment.validations == null) {
          this.prePayment = {
            id: this.prevPrePayment.id,
            selectedEmbassy: embassy,
            selectedCurrency: currency,
            bookNo: this.prevPrePayment.bookNo,
            bookDate: new Date(this.prevPrePayment.bookDate),
            remarks: this.prevPrePayment.remarks,
            embassyId: this.prevPrePayment.embassyId,
            finYear: this.prevPrePayment.finYear,
            currencyCode: this.prevPrePayment.currencyCode,
            currencyRate: this.prevPrePayment.currencyRate,
            orgId: this.prevPrePayment.orgId,
            appDocuments: this.prevPrePayment.appDocuments,
          };
          this.embPrePayDetTotelAmount = 0;
          this.embDetails = this.prevPrePayment.embPrePaymentEmbDet;
          this.embDetails.forEach(a => {
            a.embPrePaymentInvDet.forEach(q => {
              q.SelectedLedger = this.ledgerCodes.find(w => w.ledgerCode == q.ledgerCode);
              q.invDate = a.clearanceOrdDate;
            })
          });
          this.totelAmountEmbPrePayDet();
        }
      }
    }
  }


  async createOredit() {
    if (this.prePayment.selectedEmbassy == "") {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_EMB_NAME_REQ"));
      return;
    }
    if (this.prePayment.selectedCurrency == "") {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_CURRENCY_REQ"));
      return;
    }
    if (this.prePayment.currencyRate == "") {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_CURRENCYRATE_REQ"));
      return;
    }
    if (this.prePayment.bookDate == "") {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_BOOK_DATE_REQ"));
      return;
    }

    if (this.prePayment.bookNo == "") {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_BOOK_NO_REQ"));
      return;
    }

    if (!this.embDetails || this.embDetails.length == 0) {
      this._toastrService.error(this._translate.transform("EMB_PAYMENT_INVDET_REQ"));
      return;
    }
    var validattion = true;
    this.embDetails.forEach((element, eleIdx) => {
      if (!element.clearanceOrdNo) {
        this._toastrService.error(this._translate.transform("EMB_PAYMENT_CLEATANNO_REQ"));
        validattion = false;;
      }
      if (!element.clearanceOrdDate) {
        this._toastrService.error(this._translate.transform("EMB_PAYMENT_CLEARORDDATE_REQ"));
        validattion = false;;
      }
      if (element.amount == "" || Number(element.amount) == 0) {
        this._toastrService.error(this._translate.transform("EMB_PAYMENT_AMOUNT_REQ"));
        validattion = false;;
      }
      if (!element.embPrePaymentInvDet || element.embPrePaymentInvDet.length == 0) {
        this._toastrService.error(this._translate.transform("EMB_PAYMENT_INVDET_REQ"));
        validattion = false;
      }
      this.embDetails.forEach((duplicate, dupIdx) => {
        if (dupIdx != eleIdx && duplicate.clearanceOrdNo == element.clearanceOrdNo) {
          this._toastrService.error(this._translate.transform("EMB_PAYMENT_EMBASSY_DUP"));
          validattion = false;
        }
      });
      element.finYear = this.financialYear;
      element.orgId = this.orgId;
    });
    this.prePayment.EmbassyId = this.prePayment.selectedEmbassy.id;
    this.prePayment.CurrencyCode = this.prePayment.selectedCurrency.code;
    if (!validattion)
      return;

    if (this.prePayment.id && this.prePayment.id != "") {
      this.prePayment.action = 'M';
      this.prePayment.modifiedDate = this.prevPrePayment.modifiedDate;
      this.prevPrePayment.embPrePaymentEmbDet.forEach(element => {
        var embDet = this.embDetails.find(a => a.id == element.id);
        if (embDet) {
          embDet.modifiedDate = element.modifiedDate;
          embDet.action = "M"
        }
        else {
          element.active = "N";
          element.action = "M";
          this.embDetails.push(element);
        }

      });

    }
    else {
      this.prePayment.action = 'N';
      this.prePayment.finYear = this.financialYear;
    }
    this.embDetails.forEach(a => {
      a.embPrePaymentInvDet.forEach(q => {
        q.ledgerCode = q.SelectedLedger.ledgerCode;
        q.invDate = a.clearanceOrdDate;
      })
    });
    this.prePayment.embPrePaymentEmbDet = this.embDetails;
    this.prePayment.active = 'Y';
    this.prePayment.orgId = this.orgId;
    this.prePayment.status = "SUBMITTED";
    this.prePayment.userConsent = false;

    await this.saveEmbPaymentData();

  }
  attachmentLink(data, type) {
    if (this.isCompGranted('SCR_DIRECT_INVOICE_POST', this.actionType.allowDelete)) {
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

  async saveEmbPaymentData() {
    var result = await this._webApiService.post("saveEmbPrePayment", this.prePayment)
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.router.navigateByUrl("finance/embassy-payment");
      }
      else if (output.status == "DUPLICATETELEX") {
        this.confirmTelexDuplicate(output);
      }
      else {
        console.log(output.messages[0]);
        this._toastrService.error(output.messages[0])
      }
    }
  }
  confirmTelexDuplicate(result: any) {
    this._confirmService.confirm({
      message: result.messages[0] + " : " + this._translate.transform("EMB_TELEX_ALREADY_EXISTS_CONFIRM_MSG"),
      accept: async () => {
        this.prePayment.userConsent = true;
        this.saveEmbPaymentData()
      }
    });
  }
  addInvoicesToEmbassy() {
    var validattion = true;
    this.invDetails.forEach((element, eleIdx) => {
      if (element.SelectedLedger == "") {
        this._toastrService.error(this._translate.transform("EMB_PAYMENT_LEDGER_REQ"));
        validattion = false;
      }
      if (element.amount == "" || Number(element.amount) == 0) {
        this._toastrService.error(this._translate.transform("EMB_PAYMENT_INVAMT_REQ"));
        validattion = false;
      }
      if (element.currencyAmount == "" || Number(element.currencyAmount) == 0) {
        this._toastrService.error(this._translate.transform("EMB_PAYMENT_CUR_AMOUNT_REQ"));
        validattion = false;;
      }
      if (element.SelectedLedger.usedFor != this._translate.transform("EMBASSYCOMM")) {
        if (element.invNo == "") {
          this._toastrService.error(this._translate.transform("EMB_PAYMENT_INVNO_REQ"));
          validattion = false;
        }

        if (element.telexRef == "") {
          this._toastrService.error(this._translate.transform("EMB_PAYMENT_TELEX_REFNO_REQ"));
          validattion = false;
        }
      }
      element.finYear = this.prePayment.finYear;
      element.orgId = this.prePayment.orgId;
      element.dueAmount = element.amount;
    });
    this.totelAmountEmbPrePayDet();
    if (!validattion)
      return;
    else {
      this.selectedEmbassyDet.embPrePaymentInvDet = JSON.parse(JSON.stringify(this.invDetails))
      this.showInvoices = false;
    }

  }

  cancelAddEdit() {
    this.router.navigateByUrl("finance/embassy-payment");
  }

  async calculateCurrAmount(event, item) {
    item.currencyAmount = item.amount * this.prePayment.currencyRate;
    await this.totelAmountEmbPrePayInvDet();
  }
  async calculateInvCurrAmount(currencyRate) {
    this.embDetails.forEach(a => {
      a.embPrePaymentInvDet.forEach(q => {
        if (q.amount != "") {
          q.currencyAmount = q.amount * currencyRate
        }
      })
    });
    await this.totelAmountEmbPrePayInvDet();
  }
  async totelAmountEmbPrePayDet() {
    this.embPrePayDetTotelAmount = 0;
    this.embDetails.forEach(element => {
      if (element.amount != "") {
        this.embPrePayDetTotelAmount = (this.embPrePayDetTotelAmount) + (element.amount);
      }
    });
  }
  addNewEmbassy() {
    var newEmbassy =
    {
      amount: '',
      remarks: '',
      id: '',
      appDocuments: [],
      embPrePaymentInvDet: [],
      active: "Y"
    }

    this.embDetails.unshift(newEmbassy);
    this.totelAmountEmbPrePayDet();
  }


  deleteEmbassy(emb) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.embDetails.indexOf(emb);
        if (index >= 0)
          this.embDetails.splice(index, 1);
        this.totelAmountEmbPrePayDet();
      }

    });
  }

  async addEditInvoices(emb) {
    this.showInvoices = true;
    this.invDetails = [];
    this.invDetails = JSON.parse(JSON.stringify(emb.embPrePaymentInvDet));
    this.selectedEmbassyDet = emb;
    await this.totelAmountEmbPrePayInvDet()
    this.invDetTitle = this._translate.transform("EMB_ADD_INVOICE_TITLE") + (emb.selectedEmbassy.clearanceOrdNo ? emb.selectedEmbassy.clearanceOrdNo : "");
  }
  async totelAmountEmbPrePayInvDet() {
    this.embPrePayInvDetTotelAmount = 0;
    this.embPrePayInvDetCurrencyAmount = 0;
    this.selectedEmbassyDet.amount = 0;
    this.invDetails.forEach(element => {
      if (element.amount != "") {
        this.embPrePayInvDetTotelAmount += element.amount;
        this.embPrePayInvDetCurrencyAmount += element.currencyAmount;
        this.selectedEmbassyDet.amount += element.amount;
      }

    });
  }

  addNewInvoice() {
    var newInvoice =
    {
      LedgerCode: '',
      SelectedLedger: '',
      invNo: '',
      telexRef: '',
      amount: '',
      currencyAmount: '',
      remarks: '',
      id: '',
      active: "Y",
      appDocuments: [],
    }

    this.invDetails.unshift(newInvoice);
    this.totelAmountEmbPrePayInvDet();
  }

  deleteInvoice(inv) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.invDetails.indexOf(inv);
        if (index >= 0)
          this.invDetails.splice(index, 1);
        this.totelAmountEmbPrePayInvDet();
      }
    });
  }

  loadCurrency(emb) {
    var selected = this.currencies.filter(a => a.code == emb.selectedEmbassy.currencyCode);
    if (selected && selected.length > 0)
      emb.selectedCurrency = selected[0];
    else
      emb.selectedCurrency = {};
  }
  filterLedeger(event: any) {
    this.filteredLedger = this.ledgerCodes.filter(a => a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase())
      && a.balance > 0);
  }

  filterEmbasy(event: any) {
    this.filteredEmbassies = this.embassies.filter(a => a.embassyName.toUpperCase().includes(event.query.toUpperCase()));
  }

  filterCurrency(event: any) {
    this.filteredCurrencies = this.currencies.filter(a => a.currencyName.toUpperCase().includes(event.query.toUpperCase()));
  }
}
