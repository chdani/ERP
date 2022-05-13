import { JsonpClientBackend } from '@angular/common/http';
import { Component, Injector, Input, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppExportExcelConfig, AppExportExcelHeaderConfig, AppExportExcelHorizantolAlign, AppExportExcelVerticalAlign, AppExportService } from 'app/shared/services/app-export.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { of, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FinanceService } from '../../finance.service';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { ThrowStmt } from '@angular/compiler';
import { NgxSpinnerService } from 'ngx-spinner';
import { saveAs } from 'file-saver/dist/FileSaver'
import { Byte } from '@angular/compiler/src/util';
import { ExportModel } from 'app/shared/model/export-model';
import { ExportService } from 'app/shared/services/export-service';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';

@Component({
  selector: 'cash-transaction',
  templateUrl: './cash-transaction.component.html',
  styleUrls: ['./cash-transaction.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DatePipe, DialogService, CurrencyPipe]
})
export class CashTransactionComponent extends AppComponentBase implements OnInit, OnDestroy {

  @ViewChild('printPreview', { static: false }) printPrview;
  items: MenuItem[];
  uploadConfig: FileUploadConfig;
  public cashExpenses: any = [];
  public selectedCashExpense: any = {}
  public cashReceipts: any = [];
  public selectedReceipt: any = {}
  public cashForm: any;
  public transferForm: any;
  displayModal: boolean = false;
  public enableTransferTab: boolean = false;
  public enableReceiptTab: boolean = false;
  public enableExpenseTab: boolean = false;
  public showTransFilter: boolean = false;

  public selectedLedger: any;
  public processTypes: any = [];
  public costCenters: any = [];
  public ledgers: any = [];
  public transTypes: any = [];
  public filteredLedgers: any = [];
  public activeTabIndex: any;
  public selectedTransId: string;
  public displayPreview: boolean = false;
  public accountBalances: any = [];
  pettyCashTransactionList: any = [];
  pdfdisabled: boolean = true;
  organization: any = [];
  pettyAccount: any = [];
  teller: any = [];
  dayClosedMsg = "";
  visibleSidebarPettyCashTransfer: boolean = false;
  visibleSidebarCashTransaction: boolean = false;
  addoredit: boolean = false;
  index: number = -1;
  lastIndex = -1;
  historyInfo: any;
  userComment: any = {};
  attachbtn: boolean = false;
  header: any = [];
  dialogRef: DynamicDialogRef;
  Filecontent: any = [];
  defaultOrgId: any;

  lang;
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
    private _currencyPipe: CurrencyPipe,
    private spinner: NgxSpinnerService,
    private _exportService: ExportService,
    private dialogService: DialogService
  ) {
    super(injector, 'SCR_CASH_MGMT', 'allowView', _commonService)
    this.userComment = {
      comments: ""
    }
  }

  ngOnInit(): void {
    this.items = [
      {
        label: 'PDF',
        icon: 'pi pi-file-pdf',
        command: (event) => { this.export('PDF', undefined); }
      },
      {
        label: 'Excel',
        icon: 'pi pi-file-excel',
        command: (event) => { this.export('EXCEL', undefined); }
      },
    ];

    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.defaultOrgId = this._webApiService.get("getDefaultOrgId");
    this.spinner.hide();
    this._commonService.updatePageName(this._translate.transform("CASHMGMT_TITLE"));
    this.cashExpenses = [];

    this.dayClosedMsg = this._translate.transform("APP_DAY_CLOSED");
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

    this.transferForm = {
      finYear: '',
      fromOrg: '',
      fromAccount: '',
      fromTeller: '',
      toOrg: '',
      toAccount: '',
      toTeller: '',
      fromTransDate: '',
      toTransDate: ''
    }

    this.getDefaultsAndLoadSearch();
    this.loadAccountBalance();
    this.getHistoryData();
    this.loadBalanceTransferSearchFields();
    this.showHideOrgFinyear('MNU_CASH_TRANSACTION');
    this.enableTransferTab = this.isCompGranted('SCR_CASH_TANSFER_TAB', "allowView");
    this.enableReceiptTab = this.isCompGranted('SCR_CASH_REC_TAB', "allowView");
    this.enableExpenseTab = this.isCompGranted('SCR_CASH_EXP_TAB', "allowView");

  }

  selectedRow: any;

  async onDataClick(row: any) {
    this.selectedRow = row;
    this.getGridContextMenu();
  }

  // get history data --------------------------------------------------------------------------------------------------
  async getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
        this.getTransactions(index_value, true);
        console.log(">>>>>>>====== 0", this.activeTabIndex);
      }
      else {
        this.activeTabIndex = 2;
        this.searchCashTransactions();
        console.log(">>>>>>>====== 1", this.activeTabIndex);
      }
    }
    else {
      this.activeTabIndex = 2;
      this.searchCashTransactions();
      console.log(">>>>>>>====== 2", this.activeTabIndex);
    }
  }
  //get default and load search ------------------------------------------------------------------------------------------
  async getDefaultsAndLoadSearch() {
    this.processTypes = await this._codeMasterService.getCodesDetailByGroupCode("CASHPROCESSTYPE", false, false, this._translate);
    this.ledgers = await this._financeService.getLedgerAccounts("", false, false, this._translate, this.defaultOrgId.id);
    this.costCenters = await this._financeService.getCostCenters(false, false, this._translate);
    this.transTypes = await this._financeService.getCashTransactionTypes(this._translate);
  }
  // load balance transfer search field ------------------------------------------------------------------------------------
  async loadBalanceTransferSearchFields() {
    this.organization = await this._financeService.getOrganizationList();
    this.pettyAccount = await this._financeService.getPettyCashAccountList();
    this.teller = await this._financeService.getTellerList();

    //Set Default Organization-----------
    await this.checkDayClosureStatus();
    //Set Default Organization
    let defaultOrg: any = {};
    let selectedOrg = this.organization.filter(a => a.code == this.defaultOrgId.id)[0];
    if (selectedOrg) {
      defaultOrg = { name: selectedOrg.name, code: selectedOrg.code }; //});
    }
    this.transferForm.organization = defaultOrg;
  }


  async clearTransferBalanceCriteria() {

    this.transferForm = {
      fromOrg: '',
      fromAccount: '',
      fromTeller: '',
      toOrg: '',
      toAccount: '',
      toTeller: '',
      fromTransDate: '',
      toTransDate: ''
    }
  }

  async loadAccountBalance() {
    var searchBalance = {
      userId: this.userContext.id,
      finYear: this.financialYear,
      orgId: this.defaultOrgId.id,
      balanceDate: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate())
    }

    this._webApiService.postObserver("getPettyCashBalance", searchBalance).pipe().subscribe(result => {
      this.accountBalances = result;
    });
  }



  async searchCashTransactions() {
    await this.getTransactionSearchCriteria();

    var result = await this._webApiService.post("getCashTransactions", this.cashForm);
    if (result) {
      if (result.length == 0)
        this.pdfdisabled = false;
      else
        this.pdfdisabled = true;
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

  gridContextMenu: MenuItem[] = [];

  private getTransactionSearchCriteria() {
    this.cashForm.finYear = this.financialYear;
    this.cashForm.orgId = this.defaultOrgId.id;

    if (this.activeTabIndex == 2) {
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
    this.cashForm.tellerUserId = this.userContext.id;

    return this.cashForm;
  }

  async getGridContextMenu() {
    var isDayClosed = this.selectedRow.isDayClosed;
    if (this.activeTabIndex == 1) {
      if (!isDayClosed) {
        this.gridContextMenu = [
          {
            label: 'Edit', icon: 'pi pi-pencil', command: (event) => { this.editCashTransfer(this.selectedRow); }
          },
          {
            label: 'Delete', icon: 'pi pi-trash', command: (event) => { this.deleteCashTransfer(this.selectedRow); }
          },
          {
            label: 'Print', icon: 'pi pi-print', command: (event) => { this.export('PDF', this.selectedRow.id); }
          },
          {
            label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showPettyCashTransferHistory(this.selectedRow) }
          },
        ];
      } else {
        this.gridContextMenu = [
          {
            label: 'Print', icon: 'pi pi-print', command: (event) => { this.export('PDF', this.selectedRow.id); }
          },
          {
            label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showPettyCashTransferHistory(this.selectedRow) }
          },
        ];
      }
    } else {
      var allowEdit = this.isGranted('SCR_CASH_MGMT', this.actionType.allowEdit);
      var allowDelete = this.isGranted('SCR_CASH_MGMT', this.actionType.allowDelete);

      this.gridContextMenu = [];
      if (!isDayClosed) {

        if (allowEdit) {
          let editItem: MenuItem = { label: 'Edit', icon: 'pi pi-pencil', command: (event) => { this.editCashTransaction(this.selectedRow); } };
          this.gridContextMenu.push(editItem);
        }

        if (allowDelete) {
          let deleteItem: MenuItem = { label: 'Delete', icon: 'pi pi-trash', command: (event) => { this.deleteCashTransaction(this.selectedRow); } }
          this.gridContextMenu.push(deleteItem);
        }
      }
      this.gridContextMenu.push({ label: 'Print', icon: 'pi pi-print', command: (event) => { this.export('PDF', this.selectedRow.id); } })
      this.gridContextMenu.push({ label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showCashTransactionHistory(this.selectedRow) } });
    }
  }
  async showPettyCashTransferHistory(selectedRow) {
    this.visibleSidebarPettyCashTransfer = true;
    this.historyInfo = {
      selectedItem: selectedRow,
      comments: [],
      history: [],
      appDocument: []
    }
    this.loadPettyCashTransComments(null);
    this.loadPettyCashTransAttachments();
    this.loadPettyCashTransHdrHistory();
  }
  async showCashTransactionHistory(selectedRow) {
    this.visibleSidebarCashTransaction = true;
    this.historyInfo = {
      selectedItem: selectedRow,
      comments: [],
      history: [],
      appDocument: []
    }
    this.loadCashTransactionComments(null);
    this.loadCashTransactionAttachments();
    this.loadCashTransactionHdrHistory();
  }
  attachmentLink(data, type) {
    this.header = this._translate.transform("FILE_ATTACHMENT_COMMENTS");
    this.addoredit = data.id ? false : true;
    this.uploadConfig =
    {
      TransactionId: data.id,
      TransactionType: type,
      AllowedExtns: ".png,.jpg,.gif,.jpeg,.bmp,.docx,.doc,.pdf,.msg",
      DocTypeRequired: false,
      DocumentReference: "",
      ReadOnly: this.addoredit,
      ScanEnabled: this.addoredit,
      ShowSaveButton: !this.addoredit,
      FileContent: []
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
  async savePettyCashTransComment() {
    if (this.userComment.comments == "")
      return;

    this.userComment.appDocuments = this.uploadConfig.FileContent
    this.userComment.pettyCashTransferId = this.historyInfo.selectedItem.id;

    var result = await this._webApiService.post("savePettyCashTransHdrComment", this.userComment);
    if (result) {
      this.loadPettyCashTransComments(result);
      this.loadPettyCashTransAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }
  async loadPettyCashTransComments(result) {
    this.historyInfo.comments = [];
    if (!result)
      result = await this._webApiService.get("getPettyCashTransComments/" + this.historyInfo.selectedItem.id);

    if (result) {
      result.forEach(element => {
        this.attachbtn = false;
        if (element.appDocuments.length != 0) {
          this.attachbtn = true;
        }
        this.historyInfo.comments.push({
          id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
          attachbtn: this.attachbtn,
        })
      });
    }
  }

  async loadPettyCashTransAttachments() {
    if (!this.historyInfo.appDocument || this.historyInfo.appDocument.length == 0) {
      var result = await this._webApiService.get("getPettyCashTransAttachments/" + this.historyInfo.selectedItem.id);
      this.historyInfo.appDocument = [];
      result.forEach(element => {
        this.historyInfo.appDocument.push({
          id: element.id,
          fileName: element.fileName,
          createdDate: element.createdDate,
          userName: element.userName,
        })
      });
    }
  }
  async saveCashTransactionComment() {
    if (this.userComment.comments == "")
      return;

    this.userComment.appDocuments = this.uploadConfig.FileContent
    this.userComment.cashTransacionId = this.historyInfo.selectedItem.id;

    var result = await this._webApiService.post("saveCashTransactionHdrComment", this.userComment);
    if (result) {
      this.loadCashTransactionComments(result);
      this.loadCashTransactionAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }
  async loadCashTransactionComments(result) {
    this.historyInfo.comments = [];
    if (!result)
      result = await this._webApiService.get("getCashTransAcionComments/" + this.historyInfo.selectedItem.id);

    if (result) {
      result.forEach(element => {
        this.attachbtn = false;
        if (element.appDocuments.length != 0) {
          this.attachbtn = true;
        }
        this.historyInfo.comments.push({
          id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
          attachbtn: this.attachbtn,
        })
      });
    }
  }

  async loadCashTransactionAttachments() {
    if (!this.historyInfo.appDocument || this.historyInfo.appDocument.length == 0) {
      var result = await this._webApiService.get("getCashTransacionAttachments/" + this.historyInfo.selectedItem.id);
      this.historyInfo.appDocument = [];
      result.forEach(element => {
        this.historyInfo.appDocument.push({
          id: element.id,
          fileName: element.fileName,
          createdDate: element.createdDate,
          userName: element.userName,
        })
      });
    }
  }
  async loadCashTransactionHdrHistory() {
    if (!this.historyInfo.history || this.historyInfo.history.length == 0) {
      var result = await this._webApiService.get("getCashTransAcionHist/" + this.historyInfo.selectedItem.id);
      this.historyInfo.history = [];
      result.forEach(element => {
        this.historyInfo.history.push({
          id: element.id,
          fieldName: this._translate.transform(element.fieldName),
          createdDate: element.createdDate,
          userName: element.userName,
          currentValue: element.currentValue,
          prevValue: element.prevValue
        })
      });
    }
  }
  async downloadFile(id) {
    var results = await this._webApiService.get("getFileDowenload/" + id);
    if (results) {
      var decodedString = atob(results.fileContent);
      saveAs.saveAs(decodedString, results.fileName);
    }
  }
  async loadPettyCashTransHdrHistory() {
    if (!this.historyInfo.history || this.historyInfo.history.length == 0) {
      var result = await this._webApiService.get("getpettycashtransHist/" + this.historyInfo.selectedItem.id);
      this.historyInfo.history = [];
      result.forEach(element => {
        this.historyInfo.history.push({
          id: element.id,
          fieldName: this._translate.transform(element.fieldName),
          createdDate: element.createdDate,
          userName: element.userName,
          currentValue: element.currentValue,
          prevValue: element.prevValue
        })
      });
    }
  }

  // create cash transaction -----------------------------------------------------------------------------------------
  createCashTransaction() {
    if (this.activeTabIndex == 1) {
      this.router.navigate(["finance/cash-transfer"], {
        state: { setActiveTabIndex: this.activeTabIndex },
      });
    }
    else if (this.activeTabIndex == 0) {
      this.router.navigate(["finance/addEdit-cash-transaction"], {
        state: { transactionId: "", mode: "R", setActiveTabIndex: this.activeTabIndex },
      });
    }
    else {
      this.router.navigate(["finance/addEdit-cash-transaction"], {
        state: { transactionId: "", mode: "E", setActiveTabIndex: this.activeTabIndex },
      });
    }
  }

  // edit cash transaction ---------------------------------------------------------------------------------------------
  async editCashTransaction(selectedItem) {
    let result = await this._webApiService.get('getPettyCashTransactionById/' + selectedItem.id);
    this.router.navigate(["finance/addEdit-cash-transaction"], {
      state: { transactionId: selectedItem.id, mode: selectedItem.transType, setActiveTabIndex: this.activeTabIndex },
    });
  }

  async editCashTransfer(item) {
    let result = await this._webApiService.get('getPettyCashTransactionById/' + item.id);
    this.router.navigate(["finance/cash-transfer"], {
      state: { transactionId: item.id, setActiveTabIndex: this.activeTabIndex },
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
    var sheetName = "";
    if (this.activeTabIndex == 2) {
      data = JSON.parse(JSON.stringify(this.cashExpenses));
      sheetName = this._translate.transform("CASHMGMT_EXPENSES");
    }
    else {
      data = JSON.parse(JSON.stringify(this.cashReceipts));
      sheetName = this._translate.transform("CASHMGMT_RECEIPTS");
    }

    exportConfig.SheetName = sheetName;

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
    exportConfig.FileName = sheetName + ".xlsx";
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
            this.searchCashTransactions();

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
  clearBalanceCriteria() {
    this.transferForm = {
      finYear: '',
      fromOrg: '',
      fromAccount: '',
      fromTeller: '',
      toOrg: '',
      toAccount: '',
      toTeller: '',
      fromTransDate: '',
      toTransDate: ''
    }
  }

  getTransactions(event, takeIndex: boolean = false) {
    let index = (takeIndex == true) ? event : event.index;
    this.activeTabIndex = index;

    if (index == 2) {
      this.cashForm.transType = "E";
      this.searchCashTransactions();
      this.showTransFilter = false;
    }
    else if (index == 1) {
      this.getPettyTransactionList();
      this.showTransFilter = true;
    }
    else {
      this.cashForm.transType = "R";
      this.searchCashTransactions();
      this.showTransFilter = false;
    }
  }



  async getPettyTransactionList() {
    this.transferForm.finYear = this.financialYear,
      this.transferForm.fromOrgId = this.defaultOrgId.id,
      this.transferForm.toOrgId = this.defaultOrgId.id

    var result = await this._webApiService.post("getPettyCashTransactions", this.transferForm);
    if (result) {
      if (result.length == 0)
        this.pdfdisabled = false;
      else
        this.pdfdisabled = true;
      this.pettyCashTransactionList = result;
    }

  }

  searchCashTransfer() {
    let search = {
      finYear: this.financialYear,
      fromOrgId: this.transferForm.fromOrg.code,
      toOrgId: this.transferForm.toOrg.code,
      fromAccountId: this.transferForm.fromAccount.code,
      toAccountId: this.transferForm.toAccount.code,
      fromTellerId: this.transferForm.fromTeller.code,
      toTellerId: this.transferForm.toTeller.code,
      fromTransDate: this.transferForm.fromTransDate,
      toTransDate: this.transferForm.toTransDate
    }

    this._webApiService.postObserver("getPettyCashTransactions", search).pipe().subscribe(result => {
      if (result)
        this.pettyCashTransactionList = result;
    });
  }

  async deleteCashTransfer(item) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("transferPettyCash", item)
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getPettyTransactionList();
          }
          else {
            console.log(output.messages[0]);
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });
  }


  // print cash transaction -----------------------------------------------------------------------------

  printCashTransaction(item) {
    this.spinner.show();
    let url = window.location.pathname;
    this.router.navigate(["print/cash-invoice-print"], {
      state: { transactionId: item.id, setActiveTabIndex: this.activeTabIndex, setBackUrlForPrint: url },
    });
  }
  sidenavClosed() {
    this.index = this.lastIndex;
    this.addoredit = false;
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  processdayclosure() {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DC_CONFIRM_MSG"),
      key: 'dayclosure',
      accept: async () => {
        let request = this.getRequestObject();
        var result = await this._webApiService.post("processDayClosure", request)
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS" && output.status != "MANDMISSING") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            var event = { index: this.activeTabIndex };
            this.getTransactions(event);
            this.checkDayClosureStatus();
          }
          else {
            // console.log(output.messages[0]);
            this._toastrService.error(output.messages[0])
          }
        }
      },
      reject: () => {
        this._confirmService.close();
      }
    });
  }

  isDayClosed: boolean = false;
  async checkDayClosureStatus() {
    let request = this.getRequestObject();
    var result = await this._webApiService.post("checkDayClosureStatus", request);
    if (result) {
      this.isDayClosed = result as boolean;
    }
  }

  private getRequestObject() {
    var fromTeller = this.teller.find(rt => rt.userId == this.userContext.id);
    let request = {
      FinYear: this.financialYear,
      FromTellerId: fromTeller ? fromTeller.code : "",
      FromOrgId: this.defaultOrgId.id,
      TransDate: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate())
    };
    return request;
  }



  export(exportType, id) {
    var request = this.getActiveTabSearchCriteria();
    request["ExportType"] = exportType;
    request["ExportHeaderText"] = "Cash Management - " + this.getActiveTabName();
    request["Id"] = id;
    var _url = "Download" + this.getActiveTabName();
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: this.getActiveTabName(),
      _request: request,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }

  getActiveTabName() {
    return this.activeTabIndex == 0 ? "Receipts" : (this.activeTabIndex == 1 ? "Transfers" : "Expenses");
  }

  getActiveTabSearchCriteria() {
    let req = {};
    if (this.activeTabIndex == 2) {
      this.cashForm.transType = "E";
      req = this.getTransactionSearchCriteria();
    }
    else if (this.activeTabIndex == 1) {
      req = {
        finYear: this.financialYear,
        fromOrgId: this.defaultOrgId.id,
        toOrgId: this.defaultOrgId.id,
      };
    }
    else {
      this.cashForm.transType = "R";
      req = this.getTransactionSearchCriteria();
    }
    return req;
  }
}