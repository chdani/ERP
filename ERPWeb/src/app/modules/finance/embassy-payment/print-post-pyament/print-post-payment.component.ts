import { Component, Injector, Input, OnInit, SimpleChanges } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { WebApiService } from 'app/shared/webApiService';
import { FinanceService } from '../../finance.service';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { AppExportService } from 'app/shared/services/app-export.service';
import { ActivatedRoute } from '@angular/router';
//import { AppCommonService } from 'app/shared/services/app-common.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppPrintService } from 'app/shared/services/app-print.service';

@Component({
  selector: 'print-post-payment',
  templateUrl: './print-post-payment.component.html',
  styleUrls: ['./print-post-payment.component.scss'],
  providers: [TranslatePipe, DatePipe, CurrencyPipe]
})

export class EmbPostPaymentPrintComponent extends AppComponentBase implements OnInit {
  postPayment: any;
  embassies: any = [];
  ledgers: any = [];
  loaded: boolean = false;
  public _transactionId: string;

  public getActivateTabIndex: string;
  public activeTabIndex: any;
  public backUrlForPrint: any;
  constructor(
    injector: Injector,
    private _webApiService: WebApiService,
    private _translate: TranslatePipe,
    private _codeMasterService: CodesMasterService,
    private _financeService: FinanceService,
    private activatedRoute: ActivatedRoute,
    public _commonService: AppCommonService,
    private spinner: NgxSpinnerService,
    private _printService: AppPrintService
  ) {
    super(injector, 'SCR_EMBASSY_PAY', 'allowAdd', _commonService);
  }

  ngOnInit(): void {
    this.spinner.show();
    this.getHistoryData();
  }
  // get state-history values 
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
    this.spinner.show();
    this.embassies = await this._financeService.getEmbassyList(false, this._translate);
    this.ledgers = await this._financeService.getLedgerAccounts("", false, false, this._translate, this.orgId);

    var result = await this._webApiService.get("getEmbpostPaymentById/" + this._transactionId);
    if (result) {
      var prevPostPayment = result as any;
      if (prevPostPayment.validations == null) {
        this.postPayment = {
          bookNo: prevPostPayment.bookNo,
          paymentDate: new Date(prevPostPayment.paymentDate),
          embassyId: prevPostPayment.embassyId,
          finYear: prevPostPayment.finYear,
          currencyCode: prevPostPayment.currencyCode,
          currencyRate: prevPostPayment.currencyRate,
          amount: prevPostPayment.amount,
          currencyAmount: prevPostPayment.currencyAmount
        };
        var embassy = this.embassies.filter(a => a.id == prevPostPayment.embassyId)[0];
        if (embassy)
          this.postPayment.embassyName = embassy.embassyName;

        var ledger = this.ledgers.filter(a => a.ledgerCode == prevPostPayment.ledgerCode)[0];
        if (ledger)
          this.postPayment.ledger = ledger.ledgerDescCode;
        this.passDataToService();
        this.loaded = true;
      }
    }
  }
}