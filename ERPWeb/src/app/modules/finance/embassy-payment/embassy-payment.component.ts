import { Component, OnInit, ViewEncapsulation, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { DialogService } from 'primeng/dynamicdialog';
import { FinanceService } from '../finance.service';
import { AppExportExcelConfig, AppExportExcelHeaderConfig, AppExportExcelHorizantolAlign, AppExportExcelVerticalAlign, AppExportService } from 'app/shared/services/app-export.service';
import { DatePipe } from '@angular/common';

import * as XLSX from 'xlsx';
import { NgxSpinnerService } from 'ngx-spinner';
import { WebApiService } from 'app/shared/webApiService';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { ExportService } from 'app/shared/services/export-service';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { ExportModel } from 'app/shared/model/export-model';
import { saveAs } from 'file-saver';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';

@Component({
  selector: 'app-embassy-payment',
  templateUrl: './embassy-payment.component.html',
  styleUrls: ['./embassy-payment.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService, DatePipe]
})

export class EmbassyPaymentComponent extends AppComponentBase implements OnInit {
  lang;
  uploadConfig: FileUploadConfig;
  embassies: any = [];
  ledgers: any = [];
  prePayments: any = [];
  statuses: any = [];
  selectedStatus: any;
  userComment: any = {};
  userComment2: any = {};
  userComment3: any = {};
  userComment4: any = {};
  expandedRows: any = {};
  invpreDetail: any = [];
  postPayments: any = [];
  prePaySearch: any = {};
  postPaySearch: any = {};
  activeTabIndex: number = 0;
  approveRetRemarks: string = "";
  approveReturnHdr: string = "";
  displayModal: boolean = false;
  gridContextMenu: MenuItem[];
  postgridDaetailsContextMenu: MenuItem[];
  gridDaetailsContextMenu: MenuItem[];
  showApproveReturn: boolean = false;
  selectedTrans: any;
  showApproveButton: boolean = false;
  showReturnButton: boolean = false;
  header: any = [];
  index: number = -1;
  lastIndex = -1;
  isImportExcelDialogVisible: boolean = false;
  isImportExcelButtonVisible: boolean = true;
  public uplPrePayment: any = {};
  dialogRef: any;
  addoredit: boolean;
  Filecontent: any = [];
  visibleSidebar1: boolean;
  prePayHdrHis: { selectedItem: any; comments: any[]; history: any[]; appDocument: any[]; historyStatus: any[]; };
  prePayEmbDetHis: { selectedItem: any; comments: any[]; history: any[]; appDocument: any[]; historyStatus: any[]; };
  prePayInvDetHis: { selectedItem: any; comments: any[]; history: any[]; appDocument: any[]; historyStatus: any[]; };
  postPayHistory: { selectedItem: any; comments: any[]; history: any[]; appDocument: any[]; historyStatus: any[]; };
  attachbtn: boolean;
  currencies: any;
  prevPrePayment: any = [];
  embDetails: any = [];
  invDetTitle: any;
  showInvoices: boolean;
  visibleSidebar2: boolean;
  visibleSidebar3: boolean;
  visibleSidebar4: boolean;
  public showOrHideOrgFinyear: boolean = false;
  allHighlight: boolean = false;
  pendHighlight: boolean = false;
  retHighlight: boolean = false;
  approvedHighlight:boolean=false;
  pdfdisabled: boolean = true;
  item: MenuItem[];
  PrePaymentform: any = {};


  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    private _confirmService: ConfirmationService,
    private _dialogService: DialogService,
    public _commonService: AppCommonService,
    private _finService: FinanceService,
    private _export: AppExportService,
    private _datePipe: DatePipe,
    private spinner: NgxSpinnerService,
    private _codeMasterService: CodesMasterService,
    private _exportService: ExportService
  ) {
    super(injector, 'SCR_EMBASSY_PAY', 'allowView', _commonService)
    this._commonService.updatePageName(this._translate.transform("EMB_PAYMENT_PAYMENTS"));
    this.userComment = {
      comments: ""
    }
  }

  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.spinner.hide();
    this.prePaySearch = {
      orgId: this.orgId,
      finYear: this.financialYear,
      fromBookDate: this.plusMinusMonthToCurrentDate(-1),
      toBookDate: new Date(),
      status: [],
    };
    this.prePaySearch.status.push("SUBMITTED");
    this.prePaySearch.status.push("RETURNED");
    this.postPaySearch = {
      orgId: this.orgId,
      finYear: this.financialYear,
      fromDate: this.plusMinusMonthToCurrentDate(-1),
      toDate: new Date(),
      status: [],
    };
    this.postPaySearch.status.push("SUBMITTED");
    this.postPaySearch.status.push("RETURNED");
    this.getDefaults();
    this.showHideOrgFinyear('MNU_EMBASSY_PAYMENT');

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


    this.prePaySearch.exportType = exportType;
    this.prePaySearch.exportHeaderText = "Embassy - " + this.getActiveTabName();
    var _url = "downloadEmbassy" + this.getActiveTabName();
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: "Embassy " + this.getActiveTabName(),
      _request: this.prePaySearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }
  getActiveTabName() {

    if (this.activeTabIndex == 0) {
      return "PrePayment";
    }
    else {
      return "PostPayment";
    }
  }
  // get history data --------------------------------------------------------------------------------------------------
  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != '') {

      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
        this.switchTab(index_value, true);
      }
      else {
        this.getPrePayments();
      }
    }
    else {
      this.getPrePayments();
    }
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
    this.embassies = await this._finService.getEmbassyList(false, this._translate);
    this.currencies = await this._finService.getCurrencies(false, this._translate);
    this.statuses = await this._codeMasterService.getCodesDetailByGroupCode("FINTRANSSTATUS", false, false, this._translate);

    this.allHighlight = true;
    this.getHistoryData();
  }

  async getPrePayments() {
    this.prePaySearch.finYear = this.financialYear;
    this.prePaySearch.orgId = this.orgId;
    this.prePayments = await this._webApiService.post("getEmbPrePayments", this.prePaySearch);
    if (this.prePayments.length == 0)
      this.pdfdisabled = false;
    else
      this.pdfdisabled = true;
    if (this.prePayments) {
      this.prePayments.forEach(element => {
        var embassy = this.embassies.filter(a => a.id == element.embassyId);
        if (embassy && embassy.length > 0)
          element.embassyName = embassy[0].embassyName;

      });

    }
  }

  async getPostPayments() {
    this.postPaySearch.finYear = this.financialYear;
    this.postPaySearch.orgId = this.orgId;
    this.postPayments = await this._webApiService.post("getEmbPostPayments", this.postPaySearch);
    if (this.postPayments.length == 0)
      this.pdfdisabled = false;
    else
      this.pdfdisabled = true;
    if (this.postPayments) {
      this.postPayments.forEach(element => {
        var embassy = this.embassies.filter(a => a.id == element.embassyId);
        if (embassy && embassy.length > 0)
          element.embassyName = embassy[0].embassyName;
        var led = this.ledgers;
        var ledger = this.ledgers.filter(a => a.ledgerCode == element.ledgerCode);
        if (ledger && ledger.length > 0)
          element.ledger = ledger[0].ledgerDescCode;

      });

    }
  }
  getGridContextMenu(item) {
    this.gridContextMenu = [];
    if (item.status == "SUBMITTED") {
      if (this.isCompGranted('SCR_EMB_PRE_PAYMENTS', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editPrePayment(item) } };
        this.gridContextMenu.push(Edit);
      }
      if (this.isCompGranted('SCR_EMB_PRE_PAYMENTS', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deletePrePayment(item) } };
        this.gridContextMenu.push(Delete);
      }
      if (this.isCompGranted('SCR_EMB_PRE_PAYMENTS', this.actionType.allowApprove)) {
        let Approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRetWindow(item, 'APPROVED') } };
        this.gridContextMenu.push(Approve);
      }
      if (this.isCompGranted('SCR_EMB_PRE_PAYMENTS', this.actionType.allowApprove)) {
        let Return: MenuItem = { label: this._translate.transform("APP_RETURN"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRetWindow(item, 'RETURNED') } };
        this.gridContextMenu.push(Return);
      }

    }
    else if (item.status == "RETURNED") {
      this.gridContextMenu = [];
      if (this.isCompGranted('SCR_EMB_PRE_PAYMENTS', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editPrePayment(item) } };
        this.gridContextMenu.push(Edit);
      }
      if (this.isCompGranted('SCR_EMB_PRE_PAYMENTS', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deletePrePayment(item) } };
        this.gridContextMenu.push(Delete);
      }
    }
    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "EMBREPAYHDR") } };
    let sidebar: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.embPrePayHdrhistory(item) } };
    this.gridContextMenu.push(attach);
    this.gridContextMenu.push(sidebar);
  }
  getGridDetailsContextMenu(item) {
    this.gridDaetailsContextMenu = [];

    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "EMBREPAYDETHDR") } };
    let Detail: MenuItem = { label: this._translate.transform("Details"), icon: 'pi pi-list', command: (event) => { this.showInvpreDetails(item) } };
    let sidebar: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.prePaymentEmbDetHistory(item) } };
    this.gridDaetailsContextMenu.push(attach);
    this.gridDaetailsContextMenu.push(Detail);
    this.gridDaetailsContextMenu.push(sidebar);
  }
  postGridDetailsContextMenu(item) {
    this.postgridDaetailsContextMenu = [];
    if (item.status == "SUBMITTED") {
      if (this.isCompGranted('SCR_EMB_POST_PAYMENTS', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editPostPayment(item) } };
        this.postgridDaetailsContextMenu.push(Edit);
      }
      if (this.isCompGranted('SCR_EMB_POST_PAYMENTS', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deletePostPayment(item) } };
        this.postgridDaetailsContextMenu.push(Delete);
      }
      if (this.isCompGranted('SCR_EMB_POST_PAYMENTS', this.actionType.allowApprove)) {
        let Approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRetWindow(item, 'APPROVED') } };
        this.postgridDaetailsContextMenu.push(Approve);
      }
      if (this.isCompGranted('SCR_EMB_POST_PAYMENTS', this.actionType.allowApprove)) {
        let Return: MenuItem = { label: this._translate.transform("APP_RETURN"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRetWindow(item, 'RETURNED') } };
        this.postgridDaetailsContextMenu.push(Return);
      }

    }
    else if (item.status == "RETURNED") {
      this.postgridDaetailsContextMenu = [];
      if (this.isCompGranted('SCR_EMB_POST_PAYMENTS', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editPostPayment(item) } };
        this.postgridDaetailsContextMenu.push(Edit);
      }
      if (this.isCompGranted('SCR_EMB_POST_PAYMENTS', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deletePostPayment(item) } };
        this.postgridDaetailsContextMenu.push(Delete);
      }
    }
    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "EMBPOSTPAYHDR") } };
    let sidebar: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.embPosthistory(item) } };
    let print: MenuItem = { label: this._translate.transform("APP_PRINT"), icon: 'pi pi-print', command: (event) => { this.printPostPayment(item) } };
    this.postgridDaetailsContextMenu.push(attach);
    this.postgridDaetailsContextMenu.push(print);
    this.postgridDaetailsContextMenu.push(sidebar);

  }

  async embPosthistory(item) {
    this.visibleSidebar4 = true;
    this.postPayHistory = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      historyStatus: []
    }
    this.loadEmbPostComments();
    this.loadEmbPostAttachments();
    this.loadEmbPostPayStatusHistory();
    this.loadEmbPostPayHistory();
  }

  async loadEmbPostAttachments() {

    var result = await this._webApiService.get("getEmbPostPaymentAttachments/" + this.postPayHistory.selectedItem.id);
    this.postPayHistory.appDocument = [];
    result.forEach(element => {
      this.postPayHistory.appDocument.push({
        id: element.id,
        fileName: element.fileName,
        createdDate: element.createdDate,
        userName: element.userName,
      })
    });

  }
  async loadEmbPostComments() {

    this.postPayHistory.comments = [];

    var result = await this._webApiService.get("getEmbPostPaymentHdrHistComment/" + this.postPayHistory.selectedItem.id);
    if (result) {
      result.forEach(element => {
        this.attachbtn = false;
        if (element.appDocuments.length != 0) {
          this.attachbtn = true;
        }
        this.postPayHistory.comments.push({
          id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
          attachbtn: this.attachbtn,
        })
      });
    }
  }

  async loadEmbPostPayStatusHistory() {
    if (!this.postPayHistory.historyStatus || this.postPayHistory.historyStatus.length == 0) {
      var result = await this._webApiService.get("GetEmbPostPaymentHistoryStatus/" + this.postPayHistory.selectedItem.id);
      this.postPayHistory.historyStatus = [];
      result.forEach(element => {
        var status = this.statuses.find(x => x.code == element.status);

        this.postPayHistory.historyStatus.push({
          id: element.id,
          Status: status.description,
          CreatedDate: element.createdDate,
          UserName: element.userName,
          Comments: element.comments
        })
      });
    }
  }

  async loadEmbPostPayHistory() {
    if (!this.postPayHistory.history || this.postPayHistory.history.length == 0) {
      var result = await this._webApiService.get("getEmbPostPaymentHistory/" + this.postPayHistory.selectedItem.id);
      this.postPayHistory.history = [];
      result.forEach(element => {
        this.postPayHistory.history.push({
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

  async saveEmbPostComment() {

    if (this.userComment4.comments == "")
      return;

    this.userComment4.appDocuments = this.uploadConfig.FileContent
    this.userComment4.embPostPaymentId = this.postPayHistory.selectedItem.id;

    var result = await this._webApiService.post("saveEmbPostPaymentHdrHistComment", this.userComment4);
    if (result) {
      this.loadEmbPostComments();
      this.loadEmbPostAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment4.comments = "";
    }
  }

  showApproveRetWindow(item: any, status: string) {
    this.showApproveReturn = true;
    this.selectedTrans = JSON.parse(JSON.stringify(item));
    this.approveRetRemarks = "";

    if (status == "APPROVED") {
      this.approveReturnHdr = this._translate.transform("APP_APPROVE_FOR_TRANSNO") + item.transNo;
      this.showApproveButton = true;
      this.showReturnButton = false;
    }
    else {
      this.approveReturnHdr = this._translate.transform("APP_RETURN_FOR_TRANSNO") + item.transNo;;
      this.showApproveButton = false;
      this.showReturnButton = true;
    }
  }
  onClickStatus(status) {
    this.prePaySearch.status = [];
    if (this.activeTabIndex == 0) {
      if (status != "ALL") {
        this.selectedStatus = this.statuses.find(a => a.code == status);
        this.prePaySearch.status.push(this.selectedStatus.code);
        this.prePaySearch.fromBookDate = "";
        this.prePaySearch.toBookDate = "";
        this.prePaySearch.ledgerCode = "";
        this.prePaySearch.embassyId = "";
        this.allHighlight = false;
        if (status == "SUBMITTED") {
          this.pendHighlight = true;
          this.retHighlight = false;
          this.approvedHighlight=false;
        }
        else {
          this.pendHighlight = false;
          this.retHighlight = true;
          this.approvedHighlight=false;
        }
         if(status=="APPROVED"){
          this.pendHighlight = false;
          this.retHighlight = false;
          this.approvedHighlight=true;
         }
      }
      else {
        this.allHighlight = true;
        this.pendHighlight = false;
        this.retHighlight = false;
        this.approvedHighlight=false;
        this.prePaySearch.fromBookDate = this.plusMinusMonthToCurrentDate(-1),
          this.prePaySearch.toBookDate = new Date(),
          this.prePaySearch.ledgerCode = "";
        this.prePaySearch.embassyId = "";
      }

      this.getPrePayments();
    }
    else if (this.activeTabIndex == 1) {
      this.postPaySearch.status = [];
      if (status != "ALL") {
        this.selectedStatus = this.statuses.find(a => a.code == status);
        this.postPaySearch.status.push(this.selectedStatus.code);
        this.postPaySearch.fromBookDate = "";
        this.postPaySearch.toBookDate = "";
        this.postPaySearch.ledgerCode = [];
        this.postPaySearch.embassyId = "";
        this.allHighlight = false;
        if (status == "SUBMITTED") {
          this.pendHighlight = true;
          this.retHighlight = false;
          this.approvedHighlight=false;
        }
        else {
          this.pendHighlight = false;
          this.retHighlight = true;
          this.approvedHighlight=false;
        }
        if(status=="APPROVED"){
          this.pendHighlight = false;
          this.retHighlight = false;
          this.approvedHighlight=true;
         }
      }
      else {
        this.allHighlight = true;
        this.pendHighlight = false;
        this.retHighlight = false;
        this.approvedHighlight=false;
        this.postPaySearch.fromBookDate = this.plusMinusMonthToCurrentDate(-1),
          this.postPaySearch.toBookDate = new Date(),
          this.postPaySearch.embassyId = "";
        this.postPaySearch.ledgerCode = "";
      }
      this.getPostPayments();
    }

  }
  attachmentLink(data, type) {
    if (data != "") {
      if (data.transNo != "" && data.transNo != undefined) {
        this.header = this._translate.transform("FILE_ATTACHMENT_FOR_TRANSNO") + data.transNo;
      }
      else if (data.embassyName != "" && data.embassyName != undefined) {
        this.header = this._translate.transform("FILE_ATTACHMENT_FOR_EMPASSYNAME") + data.embassyName;
      }
      else if (data.invNo != "" && data.invNo != undefined) {
        this.header = this._translate.transform("FILE_ATTACHMENT_FOR_INVNO") + data.invNo;
      }
      else {
        this.header = this._translate.transform("FILE_ATTACHMENT_COMMENTS");
      }
    }
    else {
      this.header = this._translate.transform("FILE_ATTACHMENT");
    }
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

    this.dialogRef = this._dialogService.open(FileUploadComponent, {
      header: this.header,
      width: "700px",
      closable: false,
      contentStyle: { "height": "500px", overflow: "auto" },
      baseZIndex: 500,
      dismissableMask: true,
      data: this.uploadConfig
    });
  }

  async embPrePayHdrhistory(item) {
    this.visibleSidebar1 = true;
    this.prePayHdrHis = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      historyStatus: []
    }
    this.loadEmbPreComments();
    this.loadEmbPreAttachments();
    this.loadEmbPrePayStatusHis();
    this.loadEmbPrePayStatusHis();
  }

  async loadEmbPreAttachments() {
    if (!this.prePayHdrHis.appDocument || this.prePayHdrHis.appDocument.length == 0) {
      var result = await this._webApiService.get("getEmbPrePaymentAttachments/" + this.prePayHdrHis.selectedItem.id);
      this.prePayHdrHis.appDocument = [];
      result.forEach(element => {
        this.prePayHdrHis.appDocument.push({
          id: element.id,
          fileName: element.fileName,
          createdDate: element.createdDate,
          userName: element.userName,
        })
      });
    }
  }
  async loadEmbPreComments() {
    this.prePayHdrHis.comments = [];
    var result = await this._webApiService.get("getEmbPrePaymentHdrHistComment/" + this.prePayHdrHis.selectedItem.id);
    if (result) {
      result.forEach(element => {
        this.attachbtn = false;
        if (element.appDocuments.length != 0) {
          this.attachbtn = true;
        }
        this.prePayHdrHis.comments.push({
          id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
          attachbtn: this.attachbtn,
        })
      });
    }
  }

  async loadEmbPrePayStatusHis() {
    if (!this.prePayHdrHis.historyStatus || this.prePayHdrHis.historyStatus.length == 0) {
      var result = await this._webApiService.get("getEmbPrePaymentHistoryStatus/" + this.prePayHdrHis.selectedItem.id);
      this.prePayHdrHis.historyStatus = [];
      result.forEach(element => {
        var status = this.statuses.find(x => x.code == element.status);

        this.prePayHdrHis.historyStatus.push({
          id: element.id,
          Status: status.description,
          CreatedDate: element.createdDate,
          UserName: element.userName,
          Comments: element.comments
        })
      });
    }
  }
  async loadEmbPrePayHistory() {
    if (!this.prePayHdrHis.history || this.prePayHdrHis.history.length == 0) {
      var result = await this._webApiService.get("getEmbPrePaymentHistory/" + this.prePayHdrHis.selectedItem.id);
      this.prePayHdrHis.history = [];
      result.forEach(element => {
        this.prePayHdrHis.history.push({
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


  async saveEmbPreComment() {
    if (this.userComment.comments == "")
      return;

    this.userComment.appDocuments = this.uploadConfig.FileContent
    this.userComment.embPrePaymentHdrId = this.prePayHdrHis.selectedItem.id;

    var result = await this._webApiService.post("saveEmbPrePaymentHdrHistComment", this.userComment);
    if (result) {
      this.loadEmbPreComments();
      this.loadEmbPreAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async prePaymentEmbDetHistory(item) {
    this.visibleSidebar2 = true;
    this.prePayEmbDetHis = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      historyStatus: []
    }
    this.loadPrePayEmbDetailsComments();
    this.loadPrePayEmbDetailsAttachments();
    this.loadPrePayEmbDetHistory();
  }

  async loadPrePayEmbDetailsAttachments() {
    var result = await this._webApiService.get("getEmbPrePaymentdetAttachments/" + this.prePayEmbDetHis.selectedItem.id);
    this.prePayEmbDetHis.appDocument = [];
    result.forEach(element => {
      this.prePayEmbDetHis.appDocument.push({
        id: element.id,
        fileName: element.fileName,
        createdDate: element.createdDate,
        userName: element.userName,
      })
    });
  }

  async loadPrePayEmbDetailsComments() {
    this.prePayEmbDetHis.comments = [];

    var result = await this._webApiService.get("getEmbPrePaymentDetHistComment/" + this.prePayEmbDetHis.selectedItem.id);
    if (result) {
      result.forEach(element => {
        this.attachbtn = false;
        if (element.appDocuments.length != 0) {
          this.attachbtn = true;
        }
        this.prePayEmbDetHis.comments.push({
          id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
          attachbtn: this.attachbtn,
        })
      });
    }
  }

  async loadPrePayEmbDetHistory() {
    if (!this.prePayEmbDetHis.history || this.prePayEmbDetHis.history.length == 0) {
      var result = await this._webApiService.get("getEmbPrePaymentDetHistory/" + this.prePayEmbDetHis.selectedItem.id);
      this.prePayEmbDetHis.history = [];
      result.forEach(element => {
        this.prePayEmbDetHis.history.push({
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
  async savePrePayEmbDetailsComment() {
    if (this.userComment2.comments == "")
      return;

    this.userComment2.appDocuments = this.uploadConfig.FileContent
    this.userComment2.embPrePaymentEmbDetId = this.prePayEmbDetHis.selectedItem.id;

    var result = await this._webApiService.post("saveEmbPrePaymentDetHistComment", this.userComment2);
    if (result) {
      this.loadPrePayEmbDetailsComments();
      this.loadPrePayEmbDetailsAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment2.comments = "";
    }
  }


  async embPreInvDetailshistory(item) {
    this.visibleSidebar3 = true;
    this.prePayInvDetHis = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      historyStatus: []
    }
    this.loadEmbPreInvDetailsComments();
    this.loadEmbPreInvDetailsAttachments();
    this.loadEmbPrePayInvDetHistory();
  }
  async saveEmbPreInvDetailsComment() {
    if (this.userComment3.comments == "")
      return;

    this.userComment3.appDocuments = this.uploadConfig.FileContent
    this.userComment3.embPrePaymentInvDetId = this.prePayInvDetHis.selectedItem.id;

    var result = await this._webApiService.post("saveEmbPreInvPaymentDetHistComment", this.userComment3);
    if (result) {
      this.loadEmbPreInvDetailsComments();
      this.loadEmbPreInvDetailsAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment2.comments = "";
    }
  }

  async loadEmbPreInvDetailsAttachments() {
    var result = await this._webApiService.get("GetEmbPrePaymentInvdetAttachments/" + this.prePayInvDetHis.selectedItem.id);
    this.prePayInvDetHis.appDocument = [];
    result.forEach(element => {
      this.prePayInvDetHis.appDocument.push({
        id: element.id,
        fileName: element.fileName,
        createdDate: element.createdDate,
        userName: element.userName,
      })
    });
  }

  async loadEmbPreInvDetailsComments() {
    this.prePayInvDetHis.comments = [];
    var result = await this._webApiService.get("getEmbPrePaymentInvDetHistComment/" + this.prePayInvDetHis.selectedItem.id);
    if (result) {
      result.forEach(element => {
        this.attachbtn = false;
        if (element.appDocuments.length != 0) {
          this.attachbtn = true;
        }
        this.prePayInvDetHis.comments.push({
          id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
          attachbtn: this.attachbtn,
        })
      });
    }
  }


  async loadEmbPrePayInvDetHistory() {
    if (!this.prePayEmbDetHis.history || this.prePayEmbDetHis.history.length == 0) {
      var result = await this._webApiService.get("getEmbPrePaymentDetHistory/" + this.prePayEmbDetHis.selectedItem.id);
      this.prePayEmbDetHis.history = [];
      result.forEach(element => {
        this.prePayEmbDetHis.history.push({
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

  async processApproveOrReject(status: string) {
    if (this.approveRetRemarks == "") {
      this._toastrService.error(this._translate.transform("APP_REMARKS_REQ"));
      return;
    }

    if (this.selectedTrans) {

      this.selectedTrans.approverRemarks = this.approveRetRemarks;
      this.selectedTrans.status = status;
      if (this.activeTabIndex == 0) {
        this.selectedTrans.action = 'M';
        var result = await this._webApiService.post("approveReturnEmbPrePay", this.selectedTrans);
        if (result) {
          var output = result as any;
          if (output.validations == null) {
            if (output.status == "DATASAVESUCSS") {
              this._toastrService.success(this._translate.transform("APP_SUCCESS"));
              this.getPrePayments();
              this.showApproveReturn = false;
              this.showApproveButton = false;
              this.showReturnButton = false;
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
      else {
        this.selectedTrans.action = 'M';
        var result = await this._webApiService.post("approveReturnEmbPostPay", this.selectedTrans);
        if (result) {
          var output = result as any;
          if (output.validations == null) {
            if (output.status == "DATASAVESUCSS") {
              this._toastrService.success(this._translate.transform("APP_SUCCESS"));
              this.getPostPayments();
              this.showApproveReturn = false;
              this.showApproveButton = false;
              this.showReturnButton = false;
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
    }

  }

  createNewRecrod() {
    if (this.activeTabIndex == 0) {
      //this.router.navigateByUrl("finance/addedit-embassy-pre-payment");
      this.router.navigate(["finance/addedit-embassy-pre-payment"], {
        state: { setActiveTabIndex: this.activeTabIndex },
      });
    }
    else {
      //this.router.navigateByUrl("finance/addedit-embassy-post-payment");
      this.router.navigate(["finance/addedit-embassy-post-payment"], {
        state: { setActiveTabIndex: this.activeTabIndex },
      });
    }
  }

  async editPrePayment(data: any) {
    var result = await this._webApiService.get("getEmbPrePaymentById/" + data.id);
    this.router.navigate(["finance/addedit-embassy-pre-payment"], {
      state: { transactionId: data.id, setActiveTabIndex: this.activeTabIndex },
    });

  }

  async editPostPayment(data: any) {
    var result = await this._webApiService.get("getEmbpostPaymentById/" + data.id);
    this.router.navigate(["finance/addedit-embassy-post-payment"], {
      state: { transactionId: data.id, setActiveTabIndex: this.activeTabIndex },
    });

  }

  exportExcel() {
    if (this.activeTabIndex == 0)
      this.exportPrePaymentsToExcel();
    else
      this.exportPostPaymentsToExcel();
  }

  exportPrePaymentsToExcel() {

    let exportConfig = new AppExportExcelConfig();
    exportConfig.HeaderConfig = [];
    var header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_EMB_NAME");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 40;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_BOOK_DATE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 15;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_BOOK_NO");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Right;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 15;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_AMOUNT");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    header.ColumnFormat = "#,##0";
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_DUE_AMOUNT");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 30;
    header.ColumnFormat = "#,##0";
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_CURRENCY");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 25;
    exportConfig.HeaderConfig.push(header);

    var exportData = [];
    exportConfig.SheetName = this._translate.transform("EMB_PAYMENT_PRE_PAYMENT");

    this.prePayments.forEach(element => {
      var object = {
        embassyName: element.embassyName,
        bookNo: element.bookNo,
        bookDate: this._datePipe.transform(element.bookDate, 'dd-MM-yyyy'),
        amount: Number(element.amount),
        dueAmount: Number(element.dueAmount),
        currencyCode: element.currencyCode,
      }
      exportData.push(object);

    });
    exportConfig.ExcelData = exportData;
    exportConfig.FileName = this._translate.transform("EMB_PAYMENT_PRE_PAYMENT") + ".xlsx";
    this._export.excelConfig = exportConfig;
    this._export.exportExcel()
  }


  exportPostPaymentsToExcel() {

    let exportConfig = new AppExportExcelConfig();
    exportConfig.HeaderConfig = [];
    var header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_EMB_NAME");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 40;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_PAYDATE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 15;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_BOOK_NO");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Right;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 15;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("CASHMGMT_LEDGER");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Right;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 15;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_AMOUNT");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    header.ColumnFormat = "#,##0";
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_CURCY_RATE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 30;
    header.ColumnFormat = "0.00";
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_CURCY_AMOUNT");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 30;
    header.ColumnFormat = "#,##0";
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("EMB_PAYMENT_CURRENCY");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 25;
    exportConfig.HeaderConfig.push(header);

    var exportData = [];
    exportConfig.SheetName = this._translate.transform("EMB_PAYMENT_POST_PAYMENT");

    this.postPayments.forEach(element => {
      var object = {
        embassyName: element.embassyName,
        paymentDate: this._datePipe.transform(element.paymentDate, 'dd-MM-yyyy'),
        bookNo: element.bookNo,
        ledger: element.ledger,
        amount: Number(element.amount),
        currencyRate: Number(element.currencyRate),
        currencyAmount: Number(element.currencyAmount),
        currencyCode: element.currencyCode,
      }
      exportData.push(object);
    });
    exportConfig.ExcelData = exportData;
    exportConfig.FileName = this._translate.transform("EMB_PAYMENT_POST_PAYMENT") + ".xlsx";
    this._export.excelConfig = exportConfig;
    this._export.exportExcel()
  }

  async showEmbDetails(hdr: any) {
    if (!hdr.data.embDetail) {
      var result = await this._webApiService.get("getEmbPrePaymentById/" + hdr.data.id);
      hdr.data.embDetail = [];
      if (result && result.validatations == null)
        hdr.data.embDetail = result.embPrePaymentEmbDet;
    }
  }
  async showInvpreDetails(hdr: any) {
    this.showInvoices = true;
    var result = await this._webApiService.get("getEmbPrePayInvbDetByEmbDetId/" + hdr.id);
    this.invpreDetail = [];
    if (result && result.validatations == null)
      this.invpreDetail = result;
    this.invDetTitle = this._translate.transform("EMB_ADD_INVOICE_TITLE") + (hdr.clearanceOrdNo ? hdr.clearanceOrdNo : "");
  }

  clearPrePaymentSearch() {
    this.prePaySearch = {
      fromBookDate: this.plusMinusMonthToCurrentDate(-1),
      toBookDate: new Date(),
    };
  }

  clearPostPaymentSearch() {
    this.postPaySearch = {
      fromDate: this.plusMinusMonthToCurrentDate(-1),
      toDate: new Date(),
    };
  }

  async deletePrePayment(trans) {

    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + trans.transNo + "<br>" + this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        trans.active = "N";
        var details=trans.embPrePaymentEmbDet;
        details.forEach(element => {
          element.active = 'N'
        });
        trans.embPrePaymentEmbDet=[];
        trans.embPrePaymentEmbDet = details;
        var result = await this._webApiService.post("saveEmbPrePayment", trans);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getPrePayments();
          }
          else {
            console.log(output.messages[0]);
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });
  }

  async deletePostPayment(trans) {

    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + trans.transNo + "<br>" + this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        trans.active = "N";
        var result = await this._webApiService.post("saveEmbPostPayment", trans);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getPostPayments();
          }
          else {
            console.log(output.messages[0]);
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });
  }

  printPostPayment(item) {
    this.spinner.show();
    let url = window.location.pathname;
    this.router.navigate(["print/print-emb-post-payment"], {
      state: { transactionId: item.id, setActiveTabIndex: this.activeTabIndex, setBackUrlForPrint: url },
    });
  }

  switchTab(event, takeIndex: boolean = false) {
    this.prePaySearch.status=[];
    this.postPaySearch.status=[];
    let index = (takeIndex == true) ? event : event.index;
    this.activeTabIndex = index;
    if (index == 0) {
      this.allHighlight = true;
      this.pendHighlight = false;
      this.retHighlight = false;
      this.approvedHighlight=false;
      this.prePaySearch.status.push("SUBMITTED");
      this.prePaySearch.status.push("RETURNED");
      this.prePaySearch.fromInvDate = this.plusMinusMonthToCurrentDate(-1);
      this.prePaySearch.toInvDate = new Date();
      this.getPrePayments();
      this.isImportExcelButtonVisible = true;
    }
    else {
      this.allHighlight = true;
      this.pendHighlight = false;
      this.retHighlight = false;
      this.approvedHighlight=false;
      this.postPaySearch.status.push("SUBMITTED");
      this.postPaySearch.status.push("RETURNED");
      this.postPaySearch.fromDate = this.plusMinusMonthToCurrentDate(-1);
      this.postPaySearch.toDate = new Date();
      this.getPostPayments();
      this.isImportExcelButtonVisible = false;
    }

  }


  showImportPrePayOption(event: any) {


    /* wire up file reader */
    const target: DataTransfer = <DataTransfer>(event.target);
    if (target.files.length !== 1) {
      throw new Error('Cannot use multiple files');
    }
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(target.files[0]);
    reader.onload = (e: any) => {
      /* create workbook */
      const binarystr: string = e.target.result;
      const wb: XLSX.WorkBook = XLSX.read(binarystr, { type: 'binary' });

      /* selected the first sheet */
      var wsname = wb.SheetNames[0];
      const wsBooks: XLSX.WorkSheet = wb.Sheets[wsname];

      this.uplPrePayment = {};
      /* save data */
      this.uplPrePayment.bookData = XLSX.utils.sheet_to_json(wsBooks);

      wsname = wb.SheetNames[1];
      const wsEmbs: XLSX.WorkSheet = wb.Sheets[wsname];

      /* save data */
      this.uplPrePayment.embassyData = XLSX.utils.sheet_to_json(wsEmbs);

      wsname = wb.SheetNames[2];
      const wsInvoices: XLSX.WorkSheet = wb.Sheets[wsname];

      /* save data */
      this.uplPrePayment.invoiceData = XLSX.utils.sheet_to_json(wsInvoices);
      // to get 2d array pass 2nd parameter as object {header: 1}
      //console.log(this.upload_file); // Data will be logged in array format containing objects
      return this.uplPrePayment

    };
  }


  importExcelDialog() {
    this.isImportExcelDialogVisible = true;
  }

  async importPrePayment() {
    this.isImportExcelDialogVisible = false;
    const exceldata: any = Object.values(this.uplPrePayment);
    var validation = true;
    // Get embassy data from Excel
    let prePayments = []
    exceldata[0].forEach(element => {

      var matchingEmbs = exceldata[1].filter(a => a.Book_No == element.Book_No);
      var embassies = [];

      if (matchingEmbs != null) {
        matchingEmbs.forEach(emb => {
          var embassy = this.embassies.find(a => a.embassyNumber == emb.Embassy_No);
          if (!embassy) {
            this._toastrService.error(this._translate.transform("EMB_PAYMENT_EMB_NAME_REQ"));
            validation = false;;
          }

          var currency = this.currencies.find(a => a.code == emb.Currency);
          if (!currency) {
            this._toastrService.error(this._translate.transform("EMB_PAYMENT_CURRENCY_REQ"));
            validation = false;;
          }

          if (validation) {
            var matchingInvs = exceldata[2].filter(a => a.Embassy_No == emb.Embassy_No);
            var invoices = [];
            if (matchingInvs != null) {
              matchingInvs.forEach(inv => {
                invoices.push({
                  invDate: this.convertExcelDateToJSDate(inv.Inv_Date),
                  invNo: inv.Inv_No,
                  telexRef: inv.Telex_Ref,
                  amount: inv.Amount,
                  currencyRate: inv.Currency_Rate,
                  currencyAmount: inv.Currency_Amount,
                  remarks: inv.Remarks,
                  finYear: this.financialYear,
                  orgId: this.orgId,
                  active: "Y",
                  appDocuments: [],
                })
              });
            }

            embassies.push({
              embassyId: embassy.id,
              currencyCode: currency.code,
              amount: emb.Amount,
              remarks: emb.Remarks,
              appDocuments: [],
              finYear: this.financialYear,
              orgId: this.orgId,
              embPrePaymentInvDet: invoices,
              active: "Y"
            });
          }
        });
      }

      if (!validation)
        return;

      prePayments.push({
        bookNo: element.Book_No,
        bookDate: this.convertExcelDateToJSDate(element.Book_Date),
        remarks: element.Remarks,
        finYear: this.financialYear,
        orgId: this.orgId,
        embPrePaymentEmbDet: embassies,
        status: "SUBMITTED",
        active: 'Y',
        appDocuments: [],
      });

    });

    //-------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    if (validation) {
      // Insert and Update to DB
      var result = await this._webApiService.post("saveMultipleEmbPrePayment", prePayments)
      if (result != true) {
        var output = result as any;
        if (output.status == "DATASAVESUCSS") {
          this._toastrService.success(this._translate.transform("APP_SUCCESS"));
          this.isImportExcelDialogVisible = false;
        }
        else {
          this.isImportExcelDialogVisible = false;
          console.log(output.messages[0]);
          this._toastrService.error(output.messages[0])
        }
      }
      //--------------------------------------------------------------------------------------------

      //Refresh the embassy pre payment page

      this.getPrePayments();
    }
    //--------------------------------------------------------------------------------------------
  }

  convertExcelDateToJSDate(date) {
    return new Date(Math.round((date - 25569) * 86400 * 1000));
  }
  downloadPrePayExcelTemplate() {
    const link = document.createElement('a');
    link.setAttribute('target', '_blank');
    link.setAttribute('href', '/assets/excel/EmbPrePaymentTemplate.xlsx');
    link.setAttribute('download', `EmbPrePaymentTemplate.xlsx`);
    document.body.appendChild(link);
    link.click();
    link.remove();
  }
  async downloadFile(id) {
    var results = await this._webApiService.get("getFileDowenload/" + id);
    if (results) {
      var decodedString = atob(results.fileContent);
      saveAs.saveAs(decodedString, results.fileName);
    }
  }
  sidenavClosed() {
    this.index = this.lastIndex;
    this.addoredit = false;
  }
}




