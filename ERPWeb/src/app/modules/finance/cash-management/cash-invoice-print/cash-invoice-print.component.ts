import {  Component, Injector, Input,  OnInit, SimpleChanges } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { WebApiService } from 'app/shared/webApiService';
import { FinanceService } from '../../finance.service';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { AppExportService } from 'app/shared/services/app-export.service';
import { ActivatedRoute } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppPrintService } from 'app/shared/services/app-print.service';

@Component({
  selector: 'cash-invoice-print',
  templateUrl: './cash-invoice-print.component.html',
  styleUrls: [ './cash-invoice-print.component.scss'],
  providers: [TranslatePipe, DatePipe, CurrencyPipe]
})

export class CashInvoicePrintComponent extends AppComponentBase implements OnInit {
  cashForm: any;
  budgetAmount: any;
  ledgerCode: any;
  remarks: any;
  public _transactionId: string;

  get transactionId(): string { return this._transactionId; }

  public processTypes: any = [];
  public ledgerCodes: any = [];
  public filteredLedgers: any = [];
  public costCenters: any = [];

  public selectedLedger: any;
  loaded : boolean = false;
  public activeTabIndex: any;
  public backUrlForPrint: any;
  constructor(
    injector: Injector,
    private _webApiService: WebApiService,
    private _translate: TranslatePipe,
    private _codeMasterService: CodesMasterService,
    public _commonService: AppCommonService,    
    private _financeService: FinanceService,
    private _export: AppExportService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private _printService: AppPrintService
  ) {
    super(injector,'SCR_CASH_MGMT','allowAdd', _commonService );
  }

  ngOnInit(): void {
    this.spinner.show();
    this.getHistoryData()
  }

  // get history data --------------------------------------------------------------------------------------------------
  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != "") {
      //getting transaction id
      let getTransactionId = history.state.transactionId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != "") {
        this._transactionId = getTransactionId;
      this.getAll();
      }
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      this.backUrlForPrint = history.state.setBackUrlForPrint;
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
      }
    }
  }
  //pass data to service 
  passDataToService() {
    this._printService.setActiveTabIndex_value(this.activeTabIndex, this.backUrlForPrint)
  }
  async getAll() {
    this.processTypes = await this._codeMasterService.getCodesDetailByGroupCode("CASHPROCESSTYPE", false, false, this._translate);
    this.ledgerCodes = await this._financeService.getLedgerAccounts("", false, false, this._translate, this.orgId);
    this.costCenters = await this._financeService.getCostCenters(false, false, this._translate);

    var result = await this._webApiService.get("getCashTransactionById/" + this.transactionId);
      if (result) {
        this.cashForm = result as any;
        if (this.cashForm.validations == null) {
          this.cashForm.amount = this.cashForm.transType == 'E' ? this.cashForm.debit : this.cashForm.credit;
          this.cashForm.transType = this.cashForm.transType = "R" ? this._translate.transform("CASHMGMT_CASH_RECEIPT") : this._translate.transform("CASHMGMT_CASH_PAYMENT");
          var preocess = this.processTypes.filter(a => a.code == this.cashForm.processType)[0];
        if (preocess) {
            this.cashForm.processType = preocess.description;
        }

          var costCenter = this.costCenters.filter(a => a.code == this.cashForm.costCenter)[0];
        if (costCenter) {
            this.cashForm.costCenter = costCenter.codeDescription;
        }

          var ledger = this.ledgerCodes.filter(a => a.ledgerCode == this.cashForm.ledgerCode)[0];
        if (ledger) {
            this.cashForm.ledger = ledger.ledgerDescCode;
        }
          //this._export.exportAsPdf("cashInvoice.pdf", "printInvoice");

        this.passDataToService();
          this.loaded = true;
        }
      
    }
  }
}