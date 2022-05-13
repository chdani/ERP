import { Component, OnInit, ViewEncapsulation, Injector } from '@angular/core';
import { WebApiService } from 'app/shared/webApiService';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { FinanceService } from '../../finance.service';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MenuItem } from 'primeng/api';
import { ExportModel } from 'app/shared/model/export-model';
import { ExportService } from 'app/shared/services/export-service';



@Component({
  selector: 'ledger-histroy',
  templateUrl: './ledger-histroy.component.html',
  styleUrls: ['./ledger-histroy.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DatePipe, CurrencyPipe]
})

export class LedgerHistroyRepComponent extends AppComponentBase implements OnInit {
  lang;
  public ledgerBals: any = [];
  ledgers: any = [];
  ledgerSearch: any = {};
  drpTransTypes: any = [];
  transsactionTypes: any = [];
  public showOrHideOrgFinyear: boolean = false;
  openingBalance: any;
  closingBalance: any;
  pdfdisabled: boolean = true;
  item: MenuItem[];
  activeTabIndex: number = 0;
  budgetTypes: any = [];

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _finService: FinanceService,
    private _codeMasterService: CodesMasterService,
    private _datePipe: DatePipe,
    private _currencyPipe: CurrencyPipe,
    private _exportService: ExportService
  ) {
    super(injector, 'SCR_LEDGER_HISTROY', 'allowView', _commonService)
    this._commonService.updatePageName(this._translate.transform("LEDGER_HISTROY"));
  }

  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.getDefaults();
    this.showHideOrgFinyear('MNU_LEDGER_HISTROY');
    this.item = [
      {
        label: this._translate.transform("APP_PDF"),
        icon: 'pi pi-file-pdf',
        command: (event) => { this.export('PDF'); }
      },
      {
        label: this._translate.transform("APP_EXCEL"),
        icon: 'pi pi-file-excel',
        command: (event) => { this.export('EXCEL'); }
      },
    ];
  }
  export(exportType) {
    this.ledgerSearch.exportType = exportType;
    this.ledgerSearch.exportHeaderText = this._translate.transform("LEDGER_HISTROY");
    var _url = "downloadLedgerHistroy";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: this._translate.transform("LEDGER_HISTROY"),
      _request: this.ledgerSearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }
  async AllledgerCodes() {
    this.ledgers = await this._finService.getLedgerAccounts("", false, false, this._translate, this.orgId);
    var search = {
      orgId: this.orgId,
      finYear: this.financialYear
    };
    var result = await this._webApiService.post("getLedgerAccWiseCurrentBalance", search);
    if (result) {
      this.ledgers.forEach(element => {
        var balanceAcc = result.find(a => a.ledgerCode == element.ledgerCode)
        if (balanceAcc)
          element.balance = balanceAcc.balance;
        else
          element.balance = 0;
      });
      this.ledgers = this.ledgers.filter(a => a.balance > 0);
    }
  }
  async getDefaults() {
    await this.AllledgerCodes();
    this.budgetTypes = await this._codeMasterService.getCodesDetailByGroupCode("BUDGTDOCTYPE", false, false, this._translate);
    this.transsactionTypes = await this._codeMasterService.getCodesDetailByGroupCode("LEDGERTRANTYPES", false, false, this._translate);
    this.transsactionTypes.forEach(element => {
      if (element.code != "TRNOPENINGBALANCE" && element.code != "TRNCLOSINGBALANCE" && element.code != "TRNDIRECTINVOICE")
        this.drpTransTypes.push(element);
    });

    this.clearFilterSerach();
    this.getLedgerBalances();
  }

  async getLedgerBalances() {
    this.ledgerSearch.finYear = this.financialYear;
    this.ledgerSearch.orgId = this.orgId;
    this.ledgerBals = [];
    this.ledgerSearch.userId = JSON.parse(localStorage.getItem('LUser')).userContext.id;
    var result = await this._webApiService.post("getLedgerHistroy", this.ledgerSearch);
    if (result.length == 2)
      this.pdfdisabled = false;
    else
      this.pdfdisabled = true;
    if (result) {
      result.forEach(element => {

        if (element.transactionType == "TRNOPENINGBALANCE") {
          this.openingBalance = element.credit > 0 ? element.credit : (element.debit > 0 ? (-1 * element.debit) : 0)
          this.openingBalance = this._currencyPipe.transform(this.openingBalance, ' ', 'symbol', '0.0-0')
        }
        else if (element.transactionType == "TRNCLOSINGBALANCE") {
          this.closingBalance = element.credit > 0 ? element.credit : (element.debit > 0 ? (-1 * element.debit) : 0)
          this.closingBalance = this._currencyPipe.transform(this.closingBalance, ' ', 'symbol', '0.0-0')
        }
        else {
          element.credit = element.credit > 0 ? element.credit : "";
          element.debit = element.debit > 0 ? element.debit : "";
          var ledger = this.ledgers.find(a => a.ledgerCode == element.ledgerCode);
          if (ledger) {
            if (ledger.ledgerCode != "")
              element.ledger = ledger.ledgerDescCode;
            else
              element.ledger = "";
          }
          var transType = this.transsactionTypes.filter(a => a.code == element.transactionType);
          if (transType && transType.length > 0)
            element.transaction = transType[0].description
          this.ledgerBals.push(element);
        }
      });
    }
  }

  clearFilterSerach() {
    this.ledgerSearch = {
      fromDate: this.plusMinusMonthToCurrentDate(-1),
      toDate: new Date()
    };
  }
}
