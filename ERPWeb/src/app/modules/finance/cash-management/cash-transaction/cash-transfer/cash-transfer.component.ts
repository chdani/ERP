import { Component, OnInit } from '@angular/core';
import { Injector, OnDestroy, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { NgxSpinnerService } from 'ngx-spinner';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { Router } from '@angular/router';
import { FinanceService } from 'app/modules/finance/finance.service';

@Component({
  selector: 'app-cash-transfer',
  templateUrl: './cash-transfer.component.html',
  styleUrls: ['./cash-transfer.component.scss'],
  providers: [TranslatePipe, DatePipe, CurrencyPipe]
})
export class CashTransferComponent extends AppComponentBase implements OnInit {

  transferForm: FormGroup;
  fromOrg: any = [];
  toOrg: any = [];
  fromAccount: any = [];
  toAccount: any = [];
  fromTeller: any = [];
  toTeller: any = [];
  closingBalance: any = 0;
  openingBalance: any = 0;
  creditAmount: any = 0;
  debitAmount: any = 0;
  transferId: any = '';
  prevTransaction: any = {};
  defaultOrgId: any;

  public activeTabIndex: any;
  constructor(
    injector: Injector,
    public _commonService: AppCommonService,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    private _datePipe: DatePipe,
    private _currencyPipe: CurrencyPipe,
    private formBuilder: FormBuilder,
    private spinner: NgxSpinnerService,
    private _financeService: FinanceService,
    private _router: Router) {
    super(injector, 'SCR_CASH_MGMT', 'allowAdd', _commonService)

  }

  ngOnInit(): void {
    this.transferForm = this.formBuilder.group({
      fromOrg: ['', Validators.required],
      toOrg: ['', Validators.required],
      fromAccount: ['', Validators.required],
      toAccount: ['', Validators.required],
      fromTeller: ['', Validators.required],
      toTeller: ['', [Validators.required]],
      amount: ['', Validators.required],
      remarks: ['']
    });

    this.GetAccountandTeller();
    this.getHistoryData();
  }
  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != "") {
      //getting transaction id
      let getTransactionId = history.state.transactionId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != "") {
        this.transferId = history.state.transactionId;
        this._commonService.updatePageName(this._translate.transform("CASH_TRANSFER_EDIT"));
      }
      //getting active tab index value
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("CASH_TRANSFER_ADD"));
    }
  }

  async getCashTransferDetails() {
    let result = await this._webApiService.get('getPettyCashTransactionById/' + this.transferId);
    console.log(result);
    if (result) {
      this.transferForm.patchValue({ fromOrg: this.getOrgSelect(result.fromOrgId) });
      this.transferForm.patchValue({ toOrg: this.getOrgSelect(result.toOrgId) });
      this.transferForm.patchValue({ fromAccount: this.getAccountSelect(result.fromAccountId) });
      this.transferForm.patchValue({ toAccount: this.getAccountSelect(result.toAccountId) });
      this.transferForm.patchValue({ fromTeller: this.getTellerSelect(result.fromTellerId) });
      this.transferForm.patchValue({ toTeller: this.getTellerSelect(result.toTellerId) });
      this.transferForm.patchValue({ amount: result.amount });
      this.transferForm.patchValue({ remarks: result.remarks });
      this.prevTransaction = result;
    }
    this.getClosingBalance();
  }

  getOrgSelect(orgId) {
    let selectedOrg = this.fromOrg.filter(a => a.code == orgId)[0];
    let defaultOrg: any = {};
    if (selectedOrg) {
      defaultOrg = { name: selectedOrg.name, code: selectedOrg.code }; //});
    }
    return defaultOrg;
  }

  getAccountSelect(accId) {
    let selectedAcc = this.fromAccount.filter(a => a.code == accId)[0];
    let defaultAcc: any = {};
    if (selectedAcc) {
      defaultAcc = { name: selectedAcc.name, code: selectedAcc.code }; //});
    }
    return defaultAcc;
  }

  getTellerSelect(tellerId) {
    let selectedTeller = this.fromTeller.filter(a => a.code == tellerId)[0];
    let defaultTeller: any = {};
    if (selectedTeller) {
      defaultTeller = { name: selectedTeller.name, code: selectedTeller.code }; //});
    }
    return defaultTeller;
  }

  async GetAccountandTeller() {
    //Get Petty Cash Account
    this.defaultOrgId = await this._webApiService.get("getDefaultOrgId");
    this.toAccount = this.fromAccount = await this._financeService.getPettyCashAccountList();

    //Get Teller Cash Account
    this.toTeller = this.fromTeller = await this._financeService.getTellerList();

    //Get Organization
    this.toOrg = this.fromOrg = await this._financeService.getOrganizationList();

    //Set Default Organization
    if (this.transferId == '') {
      let defaultOrg: any = {};
      let selectedOrg = this.fromOrg.filter(a => a.code == this.defaultOrgId.id)[0];
      if (selectedOrg) {
        defaultOrg = { name: selectedOrg.name, code: selectedOrg.code }; //});
      }
      this.transferForm.patchValue({ fromOrg: defaultOrg })
      this.transferForm.patchValue({ toOrg: defaultOrg })
    }
    else //Set Default Form value
    {
      this.getCashTransferDetails();
    }
  }

  getClosingBalance() {
    let fromAcc = this.transferForm.value.fromAccount.code;
    let fromTell = this.transferForm.value.fromTeller.code;
    if (!fromAcc || !fromTell) {
      return;
    }
    var searchBalance = {
      // userId: this.userContext.id,
      finYear: this.financialYear,
      balanceDate: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()),
      accountId: fromAcc,
      TellerId: fromTell,
      orgId: this.defaultOrgId.id
    }
    console.log(searchBalance);
    this._webApiService.postObserver("getPettyCashBalance", searchBalance).pipe().subscribe(result => {
      let balanceData = result[0];
      if (balanceData) {
        this.closingBalance = balanceData.closingBalance;
        this.openingBalance = balanceData.openingBalance;
        this.creditAmount = balanceData.credit;
        this.debitAmount = balanceData.debit;
      }
      else {
        this.closingBalance = 0;
        this.openingBalance = 0;
        this.creditAmount = 0;
        this.debitAmount = 0;
      }
    });
  }

  async onSubmit() {
    if (!this.transferForm.valid) {
      this._toastrService.error(this._translate.transform("CASHMGMT_TRANSFER_MAND"));
      return;
    }
    if (this.transferForm.value.fromOrg.code == this.transferForm.value.toOrg.code
      && this.transferForm.value.fromAccount.code == this.transferForm.value.toAccount.code
      && this.transferForm.value.fromTeller.code == this.transferForm.value.toTeller.code) {
      this._toastrService.error(this._translate.transform("CASHMGMT_TRANSFER_SAME_ACC"));
      return;
    }

    if (this.transferForm.value.amount == 0 || this.transferForm.value.amount < 0) {
      this._toastrService.error(this._translate.transform("CASHMGMT_TRANSFER_VALID_AMT"));
      return;
    }

    let transferFields: any = {};
    transferFields.fromAccountId = this.transferForm.value.fromAccount.code;
    transferFields.toAccountId = this.transferForm.value.toAccount.code;
    transferFields.fromTellerId = this.transferForm.value.fromTeller.code;
    transferFields.toTellerId = this.transferForm.value.toTeller.code;
    transferFields.fromOrgId = this.transferForm.value.fromOrg.code;
    transferFields.toOrgId = this.transferForm.value.toOrg.code;
    transferFields.amount = this.transferForm.value.amount;
    transferFields.finYear = this.financialYear;
    transferFields.remarks = this.transferForm.value.remarks;
    transferFields.transDate = new Date();
    transferFields.active = "Y";
    if (this.transferId) {
      transferFields.action = 'M';
      transferFields.id = this.prevTransaction.id;
      transferFields.modifiedDate = this.prevTransaction.modifiedDate;
    }
    else
      transferFields.action = 'N';
    let result = await this._webApiService.post('transferPettyCash', transferFields);
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        //this._router.navigateByUrl("finance/cash-transaction");
        this.router.navigate(["finance/cash-transaction"], {
          state: { setActiveTabIndex: this.activeTabIndex },
        });
      }
      else {
        this._toastrService.error(output.messages[0])
      }
    }

  }

  cancelTransfer() {
    this.router.navigate(["finance/cash-transaction"], {
      state: { setActiveTabIndex: this.activeTabIndex },
    });
  }
}