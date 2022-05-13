import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { FinanceService } from 'app/modules/finance/finance.service';
import { takeUntil } from 'rxjs/operators';
import { getLocaleDateTimeFormat } from '@angular/common';

@Component({
  selector: 'addEdit-cash-transaction',
  templateUrl: './addEdit-cash-transaction.component.html',
  styleUrls: ['./addEdit-cash-transaction.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe]
})

export class AddEditCashTransactionComponent extends AppComponentBase implements OnInit, OnDestroy {
  cashForm: any;
  budgetAmount: any;
  ledgerCode: any;
  remarks: any;
  lang;

  public transTypes: any = [];
  public processTypes: any = [];
  public ledgerCodes: any = [];
  public filteredLedgers: any = [];
  public costCenters: any = [];

  public selectedLedger: any;
  public selectedtransType: any;
  public selectedProcessType: any;
  public selectedCostCenter: any;
  public prevCashInfo: any;
  public selectedTransId: string;
  public cashAccounts: any = [];
  public selectedAccount: any = {}
  public activeTabIndex: any;
  defaultOrgId: any;
  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _toastrService: ToastrService,
    private _primengConfig: PrimeNGConfig,
    private _codeMasterService: CodesMasterService,
    private _financeService: FinanceService,
    private _confirmService: ConfirmationService,
  ) {
    super(injector, 'SCR_CASH_MGMT', 'allowAdd', _commonService);
    this._primengConfig.ripple = true;
  }

  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.cashForm = {
      id: '',
      accountId: '-1',
      tellerId: '',
      tellerUserId: '',
      transType: 'E',
      finYear: '',
      processType: '',
      costCenter: '',
      ledgerCode: '',
      amount: '',
      transDate: '',
      recipient: '',
      referenceNo: '',
      debit: '',
      credit: '',
      remarks: ''
    };

    this._commonService.finYearInfo.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((finYear: string) => {
        this.financialYear = finYear;
      });

    // if (history.state) {
    //   this.cashForm.transType = history.state.mode;
    //   if (history.state.transactionId && history.state.transactionId != '') {
    //     this.cashForm.id = history.state.transactionId;
    //     this._commonService.updatePageName(this._translate.transform("CASHMGMT_EDIT"));
    //   }
    // }
    // else {
    //   this._commonService.updatePageName(this._translate.transform("CASHMGMT_ADD"));
    // }
    this.selectedAccount.opening = 0;
    this.selectedAccount.closing = 0;
    this.selectedAccount.credit = 0;
    this.selectedAccount.debit = 0;
    this.getAll()
    this.getHistoryData()
  }
  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != '') {
      this.cashForm.transType = history.state.mode;
      //getting transaction id

      let getTransactionId = history.state.transactionId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.cashForm.id = history.state.transactionId;
        if (this.cashForm.transType == 'E')
          this._commonService.updatePageName(this._translate.transform("CASHMGMT_EDIT_EXP"));
        else
          this._commonService.updatePageName(this._translate.transform("CASHMGMT_EDIT_REC"));
      }
      else {
        if (this.cashForm.transType == 'E')
          this._commonService.updatePageName(this._translate.transform("CASHMGMT_ADD_EXP"));
        else
          this._commonService.updatePageName(this._translate.transform("CASHMGMT_ADD_REC"));
      }
      //getting active tab index value
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("CASHMGMT_ADD_EXP"));
    }
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  async getAll() {
    this.defaultOrgId = await this._webApiService.get("getDefaultOrgId");
    this.transTypes = await this._financeService.getCashTransactionTypes(this._translate);
    this.processTypes = await this._codeMasterService.getCodesDetailByGroupCode("CASHPROCESSTYPE", false, false, this._translate);
    this.ledgerCodes = await this._financeService.getLedgerAccounts("", false, false, this._translate, this.defaultOrgId.id);
    this.costCenters = await this._financeService.getCostCenters(false, false, this._translate);
    var searchBalance = {
      userId: this.userContext.id,
      finYear: this.financialYear,
      orgId: this.defaultOrgId.id,
      balanceDate: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate())
    }

    var tellers = await this._webApiService.get("getPettyCashTellers")
    if (!tellers || tellers.length == 0) {
      this._toastrService.error(this._translate.transform("CASHMGMT_NO_TELLER"));
      this.router.navigateByUrl("finance/cash-transaction");
      return;
    }
    var teller = tellers.find(a => a.userId == this.userContext.id);

    if (!teller || (this.cashForm.transType == "R" && !teller.isHeadTeller)) {
      this._toastrService.error(this._translate.transform("CASHMGMT_NO_TELLER"));
      this.router.navigateByUrl("finance/cash-transaction");
      return;
    }

    var accounts = await this._webApiService.get("getPettyCashAccounts");
    if (!accounts || accounts.length == 0) {
      this._toastrService.error(this._translate.transform("CASHMGMT_NO_TELLER"));
      this.router.navigateByUrl("finance/cash-transaction");
      return;
    }

    var cashAccRes = await this._webApiService.post("getPettyCashBalance", searchBalance);
    var cashBalance = [];
    if (cashAccRes && cashAccRes.length >= 0) {
      cashBalance = cashAccRes as any;
    }
    // var headAcccount = accounts.filter(a => a.isHeadAccount)
    // if (!headAcccount || headAcccount.length == 0 ) {
    //   this._toastrService.error(this._translate.transform("CASHMGMT_NO_TELLER"));
    //   this.router.navigateByUrl("finance/cash-transaction");
    //   return;
    // }

    accounts.forEach(account => {
      var balance = cashBalance.find(a => a.accountId == account.id && a.tellerId == teller.id);
      this.cashAccounts.push({
        accountId: account.id,
        accountName: account.accountName,
        balanceDate: new Date(),
        closingBalance: balance ? balance.closingBalance : 0,
        credit: balance ? balance.credit : 0,
        debit: balance ? balance.debit : 0,
        finYear: this.financialYear,
        openingBalance: balance ? balance.openingBalance : 0,
        tellerId: teller.id,
        tellerName: teller.tellerName,
        tellerUserId: teller.userId,
        userName: teller.userName,
        // isHeadAccount: account.isHeadAccount,
        isHeadTeller: teller.isHeadTeller
      })
    });


    if (this.cashForm.id && this.cashForm.id != '') {
      var result = await this._webApiService.get("getCashTransactionById/" + this.cashForm.id);
      if (result) {
        this.prevCashInfo = result as any;
        if (this.prevCashInfo.validations == null) {
          this.cashForm = {
            id: this.prevCashInfo.id,
            tellerId: this.prevCashInfo.tellerId,
            accountId: this.prevCashInfo.accountId,
            tellerUserId: this.prevCashInfo.tellerUserId,
            transType: this.prevCashInfo.transType,
            finYear: this.prevCashInfo.finYear,
            processType: this.prevCashInfo.processType,
            costCenter: this.prevCashInfo.costCenter,
            ledgerCode: this.prevCashInfo.ledgerCode,
            amount: this.prevCashInfo.transType == 'E' ? this.prevCashInfo.debit : this.prevCashInfo.credit,
            recipient: this.prevCashInfo.recipient,
            referenceNo: this.prevCashInfo.referenceNo,
            remarks: this.prevCashInfo.remarks,
            transDate: new Date(this.prevCashInfo.transDate),
          };

          var ledger = this.ledgerCodes.filter(a => a.ledgerCode == this.prevCashInfo.ledgerCode)[0];
          if (ledger)
            this.selectedLedger = ledger;
        }
      }
    }
  }


  async createOredit() {
    if (!this.cashForm.accountId || this.cashForm.accountId == "-1") {
      this._toastrService.error(this._translate.transform("CASHMGMT_CASHACC_REQ"));
      return;
    }

    var account = this.cashAccounts.filter(a => a.accountId == this.cashForm.accountId);
    if (account && account.length > 0) {
      if (this.cashForm.transType == "R") {
        // if (!account[0].isHeadAccount) {
        //   this._toastrService.error(this._translate.transform("CASHMGMT_CASHACC_HEADACC_REQ"));
        //   return;
        // }
        if (!account[0].isHeadTeller) {
          this._toastrService.error(this._translate.transform("CASHMGMT_CASHACC_HEADTELLER_REG"));
          return;
        }
      }
      this.cashForm.tellerId = account[0].tellerId;
      this.cashForm.tellerUserId = account[0].tellerUserId;
    }
    else {
      this._toastrService.error(this._translate.transform("CASHMGMT_CASHACC_REQ"));
      return;
    }

    if (!this.cashForm.processType) {
      this._toastrService.error(this._translate.transform("CASHMGMT_PROCESS_TYPE_REQ"));
      return;
    }

    // if (!this.cashForm.transType) {
    //   this._toastrService.error(this._translate.transform("CASHMGMT_TRANSTYPE_REQ"));
    //   return;
    // }

    if (!this.selectedLedger) {
      this._toastrService.error(this._translate.transform("CASHMGMT_LEDGER_REQ"));
      return;
    }

    if (!this.cashForm.costCenter) {
      this._toastrService.error(this._translate.transform("CASHMGMT_COST_CENTER_REQ"));
      return;
    }

    if (this.cashForm.amount == "" || Number(this.cashForm.transamountate) == 0) {
      this._toastrService.error(this._translate.transform("CASHMGMT_AMOUNT_REQ"));
      return;
    }

    if (this.cashForm.transDate == "") {
      this._toastrService.error(this._translate.transform("CASHMGMT_TRANS_DATE_REQ"));
      return;
    }

    if (this.cashForm.recipient == "") {
      this._toastrService.error(this._translate.transform("CASHMGMT_RECIPIENT_REQ"));
      return;
    }

    if (this.cashForm.id && this.cashForm.id != "") {
      this.cashForm.action = 'M';
      this.cashForm.modifiedDate = this.prevCashInfo.modifiedDate;
    }
    else {
      this.cashForm.action = 'N';
      this.cashForm.finYear = this.financialYear;
    }
    this.cashForm.active = 'Y';
    this.cashForm.ledgerCode = this.selectedLedger.ledgerCode;
    if (this.cashForm.transType == "E")
      this.cashForm.debit = Number(this.cashForm.amount);
    else
      this.cashForm.credit = Number(this.cashForm.amount);

    this.cashForm.orgId = this.defaultOrgId.id;

    var result = await this._webApiService.post("saveCashTransaction", this.cashForm)
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        // this._confirmService.confirm({
        //   message: this._translate.transform("APP_PRINT_CONFIRM_MSG"),
        //   accept: async () => {
        //     window.open('print/cash-invoice-print?id=' + output.referenceId);
        //     this.router.navigate(["finance/cash-transaction"], {
        //       state: { setActiveTabIndex: this.activeTabIndex },
        //     });
        //   },
        //   reject: async () => {
        //     this.router.navigate(["finance/cash-transaction"], {
        //       state: { setActiveTabIndex: this.activeTabIndex },
        //     });
        //   }
        // });
        this.router.navigate(["finance/cash-transaction"], {
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
    this.router.navigate(["finance/cash-transaction"], {
      state: { setActiveTabIndex: this.activeTabIndex },
    });
  }

  filterLedgerAccounts(event: any) {
    this.filteredLedgers = this.ledgerCodes.filter(a => a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase()));
  }

  accountChange() {
    if (this.cashForm.accountId && this.cashForm.accountId != "-1") {
      var account = this.cashAccounts.filter(a => a.accountId == this.cashForm.accountId);
      if (account && account.length > 0) {
        this.selectedAccount.opening = account[0].openingBalance;
        this.selectedAccount.closing = account[0].closingBalance;
        this.selectedAccount.credit = account[0].credit;
        this.selectedAccount.debit = account[0].debit;
      }
    }
    else {

      this.selectedAccount.opening = 0;
      this.selectedAccount.closing = 0;
      this.selectedAccount.credit = 0;
      this.selectedAccount.debit = 0;
    }
  }
}
