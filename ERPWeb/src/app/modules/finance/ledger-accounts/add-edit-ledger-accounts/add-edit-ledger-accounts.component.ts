import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-edit-ledger-accounts',
  templateUrl: './add-edit-ledger-accounts.component.html',
  styleUrls: ['./add-edit-ledger-accounts.component.scss']
})
export class AddEditLedgerAccountsComponent extends AppComponentBase implements OnInit {


  lang;
  public addEditLedgerAccount: any;
  public previousLedgerAccountInfo: any;
  public ledgerAccountId: any;

  public selectedDescription: any;

  public transactionTypes: any = [];
  public filteredUsedFor: any = [];

  public activeTabIndex: any;
  constructor(
    injector: Injector,
    public router: Router,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _codeMasterService: CodesMasterService,
  ) {
    super(injector, 'SCR_LEDGER_ACCOUNT_MST', 'allowAdd', _commonService);
  }

  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;

    // if (history.state && history.state.ledgerAccountId && history.state.ledgerAccountId != '') {
    //   this.ledgerAccountId = history.state.ledgerAccountId;
    //   this._commonService.updatePageName(this._translate.transform("LEDGER_ACCOUNTS_EDIT_TITLE"));
    //   this.getEditLedgerAccount();
    // }
    // else {
    //   this._commonService.updatePageName(this._translate.transform("LEDGER_ACCOUNTS_ADD_TITLE"));
    // }


    //get code

    this.addEditLedgerAccount = {
      id: '',
      ledgerCode: '',
      ledgerDesc: '',
      usedFor: '',
      remarks: '',
      action: '',
      active: '',
    };

    this.getHistoryData();
  }


  //get default result
  async getDefaults() {
    this.transactionTypes = await this._codeMasterService.getCodesDetailByGroupCode("LEDGERACCUSEDFOR", false, true, this._translate);
  }
  // get history data --------------------------------------------------------------------------------------------
  async getHistoryData() {
    await this.getDefaults();
    if (history.state != null && history.state != undefined && history.state != '') {
      //getting transaction id
      let getTransactionId = history.state.transactionId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.ledgerAccountId = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("LEDGER_ACCOUNTS_EDIT_TITLE"));
        await this.getEditLedgerAccount();
      }
      else {
        this._commonService.updatePageName(this._translate.transform("LEDGER_ACCOUNTS_ADD_TITLE"));
      }
      //getting active tab index value
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("LEDGER_ACCOUNTS_ADD_TITLE"));
    }
  }


  //filtered used for

  filterUsedFor(event: any) {
    this.filteredUsedFor = this.transactionTypes.filter(a => a.description.toUpperCase().includes(event.query.toUpperCase()));
  }


  // Save Ledger Account -------------------------------------------------------------
  async saveLedgerAccount() {
    if (this.addEditLedgerAccount.ledgerCode == "") {
      this._toastrService.error(this._translate.transform("LEDGER_CODE_REQ"));
      return;
    }

    if (this.addEditLedgerAccount.ledgerDesc == "") {
      this._toastrService.error(this._translate.transform("LEDGER_NAME_REQ"));
      return;
    }

    if (this.selectedDescription == "") {
      this._toastrService.error(this._translate.transform("LEDGER_USED_FOR_REQ"));
      return;
    }


    if (this.ledgerAccountId && this.ledgerAccountId != "") {
      this.addEditLedgerAccount.action = 'M';
      this.addEditLedgerAccount.modifiedDate = this.previousLedgerAccountInfo.modifiedDate;
      this.addEditLedgerAccount.active = this.previousLedgerAccountInfo.active;
    }
    else {
      this.addEditLedgerAccount.action = 'N';
      this.addEditLedgerAccount.active = 'Y';
    }
    this.addEditLedgerAccount.usedFor = this.selectedDescription;
    var result = await this._webApiService.post("saveLedgerAccount", this.addEditLedgerAccount);

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

  async getEditLedgerAccount() {
    if (this.ledgerAccountId && this.ledgerAccountId != '') {
      let result = await this._webApiService.get('getLedgerAccountById/' + this.ledgerAccountId);
      if (result) {
        this.previousLedgerAccountInfo = result as any;
        if (this.previousLedgerAccountInfo.validations == null) {
          this.addEditLedgerAccount = {
            id: this.previousLedgerAccountInfo.id,
            ledgerCode: this.previousLedgerAccountInfo.ledgerCode,
            ledgerDesc: this.previousLedgerAccountInfo.ledgerDesc,
            usedFor: this.previousLedgerAccountInfo.usedFor,
            remarks: this.previousLedgerAccountInfo.remarks,
            action: this.previousLedgerAccountInfo.action,
            active: this.previousLedgerAccountInfo.active,
          }

          var getUsedFor = this.transactionTypes.filter(a => a.code == this.previousLedgerAccountInfo.usedFor)[0];
          if (getUsedFor) {
            this.selectedDescription = getUsedFor.code;
          }

        }
      }
    }
  }

  // Go back to previous page -----------------------------------------------------------------------------------
  goToPreviousScreen() {
    this.router.navigate(["finance/ledger-account"], {
      state: { setActiveTabIndex: this.activeTabIndex },
    });
  }
}
