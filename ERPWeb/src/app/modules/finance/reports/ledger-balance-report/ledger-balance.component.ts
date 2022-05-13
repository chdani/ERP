import { Component, OnInit, ViewEncapsulation, Injector } from '@angular/core';
import { WebApiService } from 'app/shared/webApiService';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { FinanceService } from '../../finance.service';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MenuItem } from 'primeng/api';
import { ExportModel } from 'app/shared/model/export-model';
import { ExportService } from 'app/shared/services/export-service';

@Component({
  selector: 'ledger-balance',
  templateUrl: './ledger-balance.component.html',
  styleUrls: ['./ledger-balance.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DatePipe, CurrencyPipe]
})

export class LedgerBalanceRepComponent extends AppComponentBase implements OnInit {
  lang;
  public ledgerBals: any = [];
  ledgerSearch: any = {};
  public showOrHideOrgFinyear: boolean = false;
  pdfdisabled: boolean = true;
  item: MenuItem[];
  selectLedger: any;
  ledgerCodes: any = [];

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _finService: FinanceService,
    private _datePipe: DatePipe,
    private _exportService: ExportService
  ) {
    super(injector, 'SCR_LEDGER_BALANCE', 'allowView', _commonService)
    this._commonService.updatePageName(this._translate.transform("LEDGER_BALANCES"));
  }

  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.getDefaults();
    this.showHideOrgFinyear('MNU_LEDGER_BALANCE');
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
    this.ledgerSearch.exportHeaderText = "Ledger Balance";
    var _url = "downloadLedgerBalance";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: "Ledger Balance",
      _request: this.ledgerSearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }
  async AllledgerCodes() {
    this.ledgerCodes = await this._finService.getLedgerAccounts("", false, false, this._translate, this.orgId);
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
      this.ledgerCodes = this.ledgerCodes.filter(a => a.balance > 0);
    }
  }
  async getDefaults() {
    await this.AllledgerCodes();
    await this.clearFilterSerach();
    await this.getLedgerBalances();
  }

  async getLedgerBalances() {
    this.ledgerSearch.finYear = this.financialYear;
    this.ledgerSearch.orgId = this.orgId;
    this.ledgerBals = [];
    this.ledgerSearch.ledgerCodes = this.selectLedger
    this.ledgerSearch.userId = JSON.parse(localStorage.getItem('LUser')).userContext.id;
    this.ledgerBals = await this._webApiService.post("getLedgerBalances", this.ledgerSearch);
    if (this.ledgerBals.length == 0)
      this.pdfdisabled = false;
    else
      this.pdfdisabled = true;
  }
  clearFilterSerach() {
    this.ledgerSearch = {
      fromDate: this.plusMinusMonthToCurrentDate(-1),
      toDate: new Date()
    };
  }
}
