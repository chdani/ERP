import { JsonpClientBackend } from '@angular/common/http';
import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppExportExcelConfig, AppExportExcelHeaderConfig, AppExportExcelHorizantolAlign, AppExportExcelVerticalAlign, AppExportService } from 'app/shared/services/app-export.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'primeng/api';
import { of, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FinanceService } from '../../finance.service';
import { DatePipe, CurrencyPipe } from '@angular/common';

@Component({
  selector: 'petty-cash-balance',
  templateUrl: './petty-cash-balance.component.html',
  styleUrls: ['./petty-cash-balance.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DatePipe, CurrencyPipe]
})
export class PettCashBalanceComponent extends AppComponentBase implements OnInit, OnDestroy {

  @ViewChild('printPreview', { static: false }) printPrview;

  public cashExpenses: any = [];
  public selectedCashExpense: any = {}
  public cashReceipts: any = [];
  public selectedReceipt: any = {}
  public cashForm: any;
  displayModal: boolean = false;

  public selectedLedger: any;
  public processTypes: any = [];
  public costCenters: any = [];
  public ledgers: any = [];
  public transTypes: any = [];
  public filteredLedgers: any = [];
  private activeTabIndex: number = 0;
  public selectedTransId: string;
  public displayPreview: boolean = false;

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    private _codeMasterService: CodesMasterService,
    private _confirmService: ConfirmationService,
    private _financeService: FinanceService,
    private _export: AppExportService,
    private _datePipe: DatePipe,
    private _currencyPipe: CurrencyPipe

  ) {
    super(injector, 'SCR_CASH_MGMT', 'allowAdd', _commonService)


  }

  ngOnInit(): void {
    this._commonService.updatePageName(this._translate.transform("CASHMGMT_TITLE"));
    this.cashExpenses = [];

    this.cashForm = {
      finYear: '',
      processType: '',
      costCenter: '',
      ledgerCode: '',
      amount: '',
      fromTransDate: '',
      toTransDate: '',
      recipient: '',
      referenceNo: '',
      debit: '',
      credit: '',
    };
    this.getDefaultsAndLoadSearch();
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  async getDefaultsAndLoadSearch() {
    this.processTypes = await this._codeMasterService.getCodesDetailByGroupCode("CASHPROCESSTYPE", true, false, this._translate);
    this.ledgers = await this._financeService.getLedgerAccounts("", false, false, this._translate, this.orgId);
    this.costCenters = await this._financeService.getCostCenters(true, false, this._translate);
    this.transTypes = await this._financeService.getCashTransactionTypes(this._translate);
    this.searchCashTransactions();
  }

  async searchCashTransactions() {
    this.cashForm.finYear = this.financialYear;
    if (this.activeTabIndex == 0) {
      this.cashForm.transType = "E";
      this.cashForm.debit = Number(this.cashForm.amount);
      this.cashForm.credit = 0;
    }
    else {
      this.cashForm.transType = "R";
      this.cashForm.credit = Number(this.cashForm.amount);
      this.cashForm.debit = 0;
    }

    if (this.selectedLedger)
      this.cashForm.ledgerCode = this.selectedLedger.ledgerCode;
    else
      this.cashForm.ledgerCode = 0;

    var result = await this._webApiService.post("getCashTransactions", this.cashForm);
    if (result) {
      var output = result as any;
      if (output.validations == null) {
        var transactions = output ?? of([]);
        if (this.processTypes && this.processTypes.length > 0) {
          transactions.forEach(element => {
            element.amount = element.transType == "E" ? element.debit : element.credit;
            var processType = this.processTypes.find(a => a.code == element.processType);
            if (processType)
              element.processTypeDesc = processType.description;

            var costCenter = this.costCenters.find(a => a.code == element.costCenter);
            if (costCenter)
              element.costCenterDesc = costCenter.codeDescription;

            var ledger = this.ledgers.find(a => a.ledgerCode == element.ledgerCode)
            if (ledger)
              element.ledger = ledger.ledgerDescCode;
          });
        }

        if (this.cashForm.transType == "E")
          this.cashExpenses = transactions;
        else
          this.cashReceipts = transactions;
      }
    }
  }

  createCashTransaction() {
    this.router.navigate(["finance/addEdit-cash-transaction"], {
      state: { transactionId: "" },
    });
  }

  editCashTransaction(selectedItem) {
    this.router.navigate(["finance/addEdit-cash-transaction"], {
      state: { transactionId: selectedItem.id },
    });
  }

  exportExcel() {

    let exportConfig = new AppExportExcelConfig();
    exportConfig.HeaderConfig = [];
    var header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("CASHMGMT_PROCESS");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 12;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("CASHMGMT_TRANSDATE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 12;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("CASHMGMT_AMOUNT");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Right;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    header.ColumnFormat = "QAR #,##0";
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("CASHMGMT_LEDGER");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 30;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("CASHMGMT_COSTCENTER");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 30;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("CASHMGMT_RECIPIENT");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 25;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("CASHMGMT_REFERENCE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 15;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("CASHMGMT_REMARKS");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 50;
    exportConfig.HeaderConfig.push(header);


    var exportData = [];
    var data = [];
    if (this.activeTabIndex == 0) {
      data = JSON.parse(JSON.stringify(this.cashExpenses));
      exportConfig.SheetName = this._translate.transform("CASHMGMT_EXPENSES");
    }
    else {
      data = JSON.parse(JSON.stringify(this.cashReceipts));
      exportConfig.SheetName = this._translate.transform("CASHMGMT_RECEIPTS");
    }
    data.forEach(element => {
      var object = {
        processTitle: element.processTypeDesc,
        transDateTitle: this._datePipe.transform(element.transDate, 'dd-MM-yyyy'),
        amountTitle: Number(element.amount),
        ledgerTitle: element.ledger,
        costCenterTitle: element.costCenterDesc,
        recipientTitle: element.recipient,
        referenceTitle: element.referenceNo,
        remarksTitle: element.remarks,
      }
      exportData.push(object);

    });
    exportConfig.ExcelData = exportData;
    exportConfig.FileName = "Test Excel.xlsx";
    this._export.excelConfig = exportConfig;
    this._export.exportExcel()
  }

  async deleteCashTransaction(item) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("saveCashTransaction", item)
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.cashForm.transType = item.transType;
          }
          else {
            console.log(output.messages[0]);
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });

  }

  filterLedgerAccounts(event: any) {
    this.filteredLedgers = this.ledgers.filter(a => a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase()));
  }

  clearSearchCriteria() {
    this.cashForm = {
      finYear: '',
      processType: '',
      costCenter: '',
      ledgerCode: '',
      amount: '',
      fromTransDate: '',
      toTransDate: '',
      recipient: '',
      referenceNo: '',
      debit: '',
      credit: '',
    };
    this.selectedLedger = null;
  }

  getTransactions(event) {
    this.activeTabIndex = event.index;
    if (event.index == 0) {
      this.cashForm.transType = "E";
      this.searchCashTransactions();
    }
    else {
      this.cashForm.transType = "R";
      this.searchCashTransactions();
    }
  }

  printCashTransaction(item) {
    this.selectedTransId = item.id;
    this.displayPreview = true;
  }

}