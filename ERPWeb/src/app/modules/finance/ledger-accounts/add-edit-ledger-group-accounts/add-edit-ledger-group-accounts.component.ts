import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-edit-ledger-group-accounts',
  templateUrl: './add-edit-ledger-group-accounts.component.html',
  styleUrls: ['./add-edit-ledger-group-accounts.component.scss']
})
export class AddEditLedgerGroupAccountsComponent extends AppComponentBase implements OnInit {
  lang;

  public addEditLedgerGroupAccount: any;
  public preLedgerGroupAccountInfo: any;
  public ledgerAccountId: any;
  public ledgerGroupAccountData: any = [];

  public activeTabIndex: any;
  constructor(
    injector: Injector,
    public router: Router,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
  ) {
    super(injector,'SCR_LEDGER_ACCOUNT_MST','allowAdd', _commonService );
  }

  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    // get ledgerAccountId history
    // if (history.state && history.state.ledgerGroupAccountId && history.state.ledgerGroupAccountId != '') {
    //   this.ledgerAccountId = history.state.ledgerGroupAccountId;
    //   this._commonService.updatePageName(this._translate.transform("LEDGER_ACCOUNTS_EDIT_TITLE"));
    //   this.getEditLedgerGroupAccount();
    // }
    // else {
    //   this._commonService.updatePageName(this._translate.transform("LEDGER_ACCOUNTS_ADD_TITLE"));
    // }

    this.addEditLedgerGroupAccount = {
      id: '',
      AccountCode: '',
      AccountDesc: '',
      LedgerCodeFrom: '',
      LedgerCodeTo: '',
    };

    this.getHistoryData();

  }

  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != '') {
      //getting transaction id
      let getTransactionId = history.state.transactionId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.ledgerAccountId = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("LEDGER_GROUP_ACCOUNTS_EDIT_TITLE"));
        this.getEditLedgerGroupAccount();
      }
      else {
        this._commonService.updatePageName(this._translate.transform("LEDGER_GROUP_ACCOUNTS_ADD_TITLE"));
      }
      //getting active tab index value
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("LEDGER_GROUP_ACCOUNTS_ADD_TITLE"));
    }
  }
  // get Ledger Account Details by ID ---------------------------------------------------------------------------
  validateFromChar(event) {
    const separator = '^([0-9])';
    const maskSeparator = new RegExp(separator, 'g');
    let result = maskSeparator.test(event.key);
    if (!result) {
      this._toastrService.error(this._translate.transform("LEDGER_GROUP_ACCOUNT_CODE_FROM_NUMERIC_VAL_REQ"));
      return result;
    }
  }
  validateToChar(event) {
    const separator = '^([0-9])';
    const maskSeparator = new RegExp(separator, 'g');
    let result = maskSeparator.test(event.key);
    if (!result) {
      this._toastrService.error(this._translate.transform("LEDGER_GROUP_ACCOUNT_CODE_TO_NUMERIC_VAL_REQ"));
      return result;
    }
  }

  async saveLedgerGroupAccount() {

    if (this.addEditLedgerGroupAccount.AccountCode == "") {
      this._toastrService.error(this._translate.transform("LEDGER_GROUP_ACCOUNT_CODE_REQ"));
      return;
    }

    if (this.addEditLedgerGroupAccount.AccountDesc == "") {
      this._toastrService.error(this._translate.transform("LEDGER_GROUP_ACCOUNT_DESC_REQ"));
      return;
    }

    if (this.addEditLedgerGroupAccount.LedgerCodeFrom == "") {
      this._toastrService.error(this._translate.transform("LEDGER_GROUP_ACCOUNT_CODE_FROM_REQ"));
      return;
    }

    if (this.addEditLedgerGroupAccount.LedgerCodeTo == "") {
      this._toastrService.error(this._translate.transform("LEDGER_GROUP_ACCOUNT_CODE_TO_REQ"));
      return;
    }

    const ledgerFromCode = parseInt(this.addEditLedgerGroupAccount.LedgerCodeFrom);
    const ledgerToCode = parseInt(this.addEditLedgerGroupAccount.LedgerCodeTo);

    if (ledgerFromCode >= ledgerToCode) {
      this._toastrService.error(this._translate.transform("LEDGER_GROUP_ACCOUNT_CODE_FROM_TO_REQ"));
      return;
    }



    if (this.ledgerAccountId && this.ledgerAccountId != "") {
      this.addEditLedgerGroupAccount.Action = 'M';
      this.addEditLedgerGroupAccount.modifiedDate = this.preLedgerGroupAccountInfo.modifiedDate;
      this.addEditLedgerGroupAccount.active = this.preLedgerGroupAccountInfo.active;
    }
    else {
      this.addEditLedgerGroupAccount.Action = 'N';
      this.addEditLedgerGroupAccount.Active = 'Y';
    }
  
    var result = await this._webApiService.post("saveLedgerAccountGroup", this.addEditLedgerGroupAccount);

    if (result) {
      var output = result as any;
      if (output.validations == null) {
        if (output.status == "DATASAVESUCSS") {
          this._toastrService.success(this._translate.transform("APP_SUCCESS"));
          // this.router.navigateByUrl("finance/ledger-account");
          this.router.navigate(["finance/ledger-account"], {
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


  // get Ledger Account Details by ID ---------------------------------------------------------------------------

  async getEditLedgerGroupAccount() {
    if (this.ledgerAccountId && this.ledgerAccountId != '') {
      let result = await this._webApiService.get('getLedgerAccountGroupById/' + this.ledgerAccountId);
      if (result) {
        this.preLedgerGroupAccountInfo = result as any;
        if (this.preLedgerGroupAccountInfo.validations == null) {
          this.addEditLedgerGroupAccount = {
            id: this.preLedgerGroupAccountInfo.id,
            AccountCode: this.preLedgerGroupAccountInfo.accountCode,
            AccountDesc: this.preLedgerGroupAccountInfo.accountDesc,
            LedgerCodeFrom: this.preLedgerGroupAccountInfo.ledgerCodeFrom,
            LedgerCodeTo: this.preLedgerGroupAccountInfo.ledgerCodeTo,
            Action: this.preLedgerGroupAccountInfo.action,
            Active: this.preLedgerGroupAccountInfo.active,
          }
        }
      }
    }
  }

  // Go back to previous page --------------------------------------------------------------------------------------
  goToPreviousScreen() {
    this.router.navigate(["finance/ledger-account"], {
      state: { setActiveTabIndex: this.activeTabIndex },
    });
  }

}
