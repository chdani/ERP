import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { WebApiService } from 'app/shared/webApiService';
import { TranslatePipe } from '@ngx-translate/core';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem, PrimeNGConfig } from 'primeng/api';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { FinanceService } from '../finance.service';
import { AppExportExcelConfig, AppExportExcelHeaderConfig, AppExportExcelHorizantolAlign, AppExportExcelVerticalAlign, AppExportService } from 'app/shared/services/app-export.service';
import { DatePipe, JsonPipe } from '@angular/common';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AddEditBudgetAllocationComponent } from '../budget-allocation/addEdit-budget/addEdit-budget.component';
import { ExportService } from 'app/shared/services/export-service';
import { ExportModel } from 'app/shared/model/export-model';
import { AddeditDirectInvoicePrePaymentComponent } from './addedit-direct-invoice-pre-payment/addedit-direct-invoice-pre-payment.component';
import { AddeditDirectInvoicePostPaymentComponent } from './addedit-direct-invoice-post-payment/addedit-direct-invoice-post-payment.component';
import { dateInputsHaveChanged } from '@angular/material/datepicker/datepicker-input-base';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { saveAs } from 'file-saver';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';

@Component({
  selector: 'app-direct-invoice',
  templateUrl: './direct-invoice.component.html',
  styleUrls: ['./direct-invoice.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DatePipe, DialogService, AddEditBudgetAllocationComponent, AddeditDirectInvoicePrePaymentComponent, AddeditDirectInvoicePostPaymentComponent]
})
export class DirectInvoiceComponent extends AppComponentBase implements OnInit {

  constructor(
    injector: Injector,
    public router: Router,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _confirmService: ConfirmationService,
    private _primengConfig: PrimeNGConfig,
    private _financeService: FinanceService,
    private _export: AppExportService,
    private _datePipe: DatePipe,
    private dialogService: DialogService,
    private _codeMasterService: CodesMasterService,
    private _exportService: ExportService

  ) {
    super(injector, 'SCR_DIRECT_INVOICE', 'allowView', _commonService)
    this._primengConfig.ripple = true;
    this.userComment = {
      comments: ""
    }
  }

  dialogRef: DynamicDialogRef;
  directInvoice: any = [];
  public ledgerCodes: any = [];
  public filteredLedgers: any = [];
  public showOrHideOrgFinyear: boolean = false;
  public costCenters: any = [];
  userComment: any = {};
  public vendorMaster: any = [];
  public filteredVendor: any = [];
  HistoryHdrDeatils: any = [];
  public selectedLedger: any;
  public selectedVendor: any;
  public directInvoiceSearch: any;
  approveReturnHdr: string = "";
  showApproveReturn: boolean = false;
  visibleSidebar1: boolean = false;
  visibleSidebar2: boolean = false;
  pdfdisabled: boolean = true;
  addoredit: boolean = false;

  approveRetRemarks: string = "";
  showApproveButton: boolean = false;
  attachbtn: boolean = false;
  showReturnButton: boolean = false;
  selectedTrans: any;
  historyInfo: any;
  selectedUploadData: any = {};
  vendorMasterId: any = [];
  activeTabIndex: number = 0;
  prePayments: any = [];
  header: any = [];
  postPayments: any = [];
  Filecontent: any = [];
  uploadConfig: FileUploadConfig;
  selectedStatus: any;
  statuses: any = [];
  allHighlight: boolean = false;
  pendHighlight: boolean = false;
  retHighlight: boolean = false;
  pregridContextMenu: MenuItem[] = [];
  postgridContextMenu: MenuItem[] = [];
  items: MenuItem[] = [];
  lang;
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
    this._commonService.updatePageName(this._translate.transform("DIRECT_INVOICE_TITLE"));
    this.directInvoiceSearch = {
      VendorMasterId: '',
      documentNo: '',
      orgId: this.orgId,
      finYear: this.financialYear,
      InvoiceNo: '',
      fromDocDate: this.plusMinusMonthToCurrentDate(-1),
      toDocDate: new Date(),
      status:[],
      DocumentDate: '',
      LedgerCode: [],
      CostCenterCode: '',
    };

    this.getDefaults();
    this.showHideOrgFinyear('MNU_DIRECT_INVOICE');
  }
  export(exportType, id) {
    this.directInvoiceSearch.exportType = exportType;
    this.directInvoiceSearch.exportHeaderText = "Direct Invoice - " + this.getActiveTabName();
    var _url = "download" + this.getActiveTabName();
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: this.getActiveTabName(),
      _request: this.directInvoiceSearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }

  getActiveTabName() {
    if (this.activeTabIndex == 0) {
      return "Prepayment";
    }
    else {
      return "Postpayment";
    }
  }

  getActiveTabSearchCriteria() {
    let req = {};
    if (this.activeTabIndex == 0) {
      req = {
        finYear: this.financialYear,
        OrgId: this.orgId,
      };
    }
    if (this.activeTabIndex == 1) {
      req = {
        finYear: this.financialYear,
        OrgId: this.orgId,
      };
    }
    return req;
  }

  // get history data --------
  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
        this.switchTab(this.activeTabIndex, true);
      }
      else {
        this.getDirectInvoiceSearch()
      }
    }
    else {
      this.getDirectInvoiceSearch()
    }
    this.allHighlight = true;
  }
  async AllledgerCodes() {
    this.ledgerCodes = await this._financeService.getLedgerAccounts("", false, false, this._translate, this.orgId);
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
    this.statuses = await this._codeMasterService.getCodesDetailByGroupCode("FINTRANSSTATUS", false, false, this._translate);
    this.costCenters = await this._financeService.getCostCenters(false, false, this._translate);
    this.vendorMaster = await this._financeService.getVendorMaster();
    this.selectedStatus = this.statuses.find(a => a.code == "");

    this.getHistoryData();
  }

  async statuschange(item) {
    this.directInvoiceSearch.status=[];
    if (item != "ALL") {
     var status1 = this.statuses.find(a => a.code == item);   
     this.directInvoiceSearch.status.push(status1.code);
      this.directInvoiceSearch.fromDocDate = '';
      this.directInvoiceSearch.toDocDate = '';
      this.allHighlight = false;
      if (item == "SUBMITTED") {
        this.pendHighlight = true;
        this.retHighlight = false;
      }
      else {
        this.pendHighlight = false;
        this.retHighlight = true;
      }
    }
    else {
      this.allHighlight = true;
      this.pendHighlight = false;
      this.retHighlight = false;
      this.directInvoiceSearch.fromDocDate = this.plusMinusMonthToCurrentDate(-1);
      this.directInvoiceSearch.toDocDate = new Date();
    }
    this.getDirectInvoiceSearch();


  }

  createNewInvoice() {
    if (this.activeTabIndex == 0) {
      this.router.navigate(["finance/invoice-pre-payments"], {
        state: { setActiveTabIndex: this.activeTabIndex },
      });
    }
    else {
      this.router.navigate(["finance/invoice-post-payments"], {
        state: { setActiveTabIndex: this.activeTabIndex },
      });
    }
  }

  async createOreditInvoice(data: any) {
    let result = await this._webApiService.get('getDirectInvPrePayById/' + data.id);
    this.router.navigate(["finance/invoice-pre-payments"], {
      state: { transactionId: data.id, setActiveTabIndex: this.activeTabIndex },
    });
  }

  async editPostDirInv(data: any) {
    let result = await this._webApiService.get('getDirectInvPostPayById/' + data.id);
    this.router.navigate(["finance/invoice-post-payments"], {
      state: { transactionId: data.id, setActiveTabIndex: this.activeTabIndex },
    });
  }

  async getDirectInvoiceSearch() {
    if (this.activeTabIndex == 0) {
      this.prePayments = await this._webApiService.post('getDirectInvPrePayList', this.directInvoiceSearch);
      if (this.prePayments.length == 0)
        this.pdfdisabled = false;
      else
        this.pdfdisabled = true;
      console.log(this.directInvoiceSearch);
    }
    else {
      this.postPayments = await this._webApiService.post('getDirectInvPostPayList', this.directInvoiceSearch);
      if (this.postPayments.length == 0)
        this.pdfdisabled = false;
      else
        this.pdfdisabled = true;
      console.log(this.directInvoiceSearch);
    }
  }


  clearInvoiceSearch() {
    this.directInvoiceSearch = {
      VendorMasterId: '',
      InvoiceNo: '',
      InvoiceDate: '',
      fromDocDate: this.plusMinusMonthToCurrentDate(-1),
      toDocDate: new Date(),
      DocumentDate: '',
      LedgerCode: [],
      CostCenterCode: '',
      status:[],
    };
    this.selectedStatus = this.statuses.find(a => a.code == "");
  }

  exportExcel() {
    if (this.activeTabIndex == 0) {
      this.exportPrePaymentsToExcel()
    }
    else {
      this.exportPostPaymentsToExcel();
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
      this.approveReturnHdr = this._translate.transform("APP_RETURN_FOR_TRANSNO") + item.transNo;
      this.showApproveButton = false;
      this.showReturnButton = true;
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
        var result = await this._webApiService.post("saveDirectInvPrePay", this.selectedTrans);
        if (result) {
          var output = result as any;
          if (output.validations == null) {
            if (output.status == "DATASAVESUCSS") {
              this._toastrService.success(this._translate.transform("APP_SUCCESS"));
              this.getDirectInvoiceSearch();
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
        var result = await this._webApiService.post("approveReturnDirInvoicePostPay", this.selectedTrans);
        if (result) {
          var output = result as any;
          if (output.validations == null) {
            if (output.status == "DATASAVESUCSS") {
              this._toastrService.success(this._translate.transform("APP_SUCCESS"));
              this.getDirectInvoiceSearch();
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
  preGetGridContextMenu(item) {
    this.pregridContextMenu = [];
    if (item.status == "SUBMITTED") {
      if (this.isCompGranted('SCR_DIRECT_INVOICE_PRE', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.createOreditInvoice(item) } };
        this.pregridContextMenu.push(Edit);
      }
      if (this.isCompGranted('SCR_DIRECT_INVOICE_PRE', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deletePreInvoice(item) } };
        this.pregridContextMenu.push(Delete);
      }
      if (this.isCompGranted('SCR_DIRECT_INVOICE_PRE', this.actionType.allowApprove)) {
        let Approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRetWindow(item, 'APPROVED') } };
        this.pregridContextMenu.push(Approve);
      }
      if (this.isCompGranted('SCR_DIRECT_INVOICE_PRE', this.actionType.allowApprove)) {
        let Return: MenuItem = { label: this._translate.transform("APP_RETURN"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRetWindow(item, 'RETURNED') } };
        this.pregridContextMenu.push(Return);
      }
    }
    else if (item.status == "RETURNED") {
      this.pregridContextMenu = [];
      if (this.isCompGranted('SCR_DIRECT_INVOICE_PRE', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.createOreditInvoice(item) } };
        this.pregridContextMenu.push(Edit);
      }
      if (this.isCompGranted('SCR_DIRECT_INVOICE_PRE', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deletePreInvoice(item) } };
        this.pregridContextMenu.push(Delete);
      }
    }
    let attach: MenuItem = { label: 'File Attachment', icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "DIRECTINVOICE") } };
    let sidebar: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showPrePayHistory(item) } };

    this.pregridContextMenu.push(attach);
    this.pregridContextMenu.push(sidebar);
  }
  async showPrePayHistory(item) {
    this.visibleSidebar1 = true;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      historyStatus: []
    }
    this.loadComments();
    this.loadDirInvPrePayAttachments();
    this.loadPrePayStatusHistory();
    this.loadPrePayHistory();
  }

  async loadPrePayStatusHistory() {
    if (!this.historyInfo.historyStatus || this.historyInfo.historyStatus.length == 0) {
      var result = await this._webApiService.get("getDirInvPrePayStatusHistListById/" + this.historyInfo.selectedItem.id);
      this.historyInfo.historyStatus = [];
      result.forEach(element => {
        var status = this.statuses.find(x => x.code == element.status);

        this.historyInfo.historyStatus.push({
          id: element.id,
          Status: status.description,
          CreatedDate: element.createdDate,
          UserName: element.userName,
          Comments: element.comments
        })
      });
    }
  }

  async loadPrePayHistory() {
    if (!this.historyInfo.history || this.historyInfo.history.length == 0) {
      var result = await this._webApiService.get("getDirInvPrePayHistListById/" + this.historyInfo.selectedItem.id);
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

  async getBudgetDetHistory() {
    var result = await this._webApiService.get("getBudgAlocDetHist/" + this.historyInfo.id);
    this.HistoryHdrDeatils = [];
    result.forEach(element => {
      this.HistoryHdrDeatils.push({
        id: element.id, fieldName: element.fieldName, createdDate: element.createdDate, userName: element.userName, currentValue: element.currentValue,
        prevValue: element.prevValue
      })
    });
  }

  async savePrePayHdrComment() {
    if (this.userComment.comments == "")
      return;

    this.userComment.appDocuments = this.uploadConfig.FileContent
    this.userComment.directInvPrePaymentId = this.historyInfo.selectedItem.id;

    var result = await this._webApiService.post("saveDirInvPrePayComment", this.userComment);
    if (result) {
      this.loadComments();
      this.loadDirInvPrePayAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadComments() {
    this.historyInfo.comments = [];

    var result = await this._webApiService.get("getDirInvPrePayCommentListById/" + this.historyInfo.selectedItem.id);

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

  async loadDirInvPrePayAttachments() {

    var result = await this._webApiService.get("getDirInvPrePayAttachmentsById/" + this.historyInfo.selectedItem.id);
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
  postGetGridContextMenu(item) {
    this.postgridContextMenu = [];
    if (item.status == "SUBMITTED") {
      if (this.isCompGranted('SCR_DIRECT_INVOICE_POST', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editPostDirInv(item) } };
        this.postgridContextMenu.push(Edit);
      }
      if (this.isCompGranted('SCR_DIRECT_INVOICE_POST', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deletePostInvoice(item) } };
        this.postgridContextMenu.push(Delete);
      }
      if (this.isCompGranted('SCR_DIRECT_INVOICE_POST', this.actionType.allowApprove)) {
        let Approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRetWindow(item, 'APPROVED') } };
        this.postgridContextMenu.push(Approve);
      }
      if (this.isCompGranted('SCR_DIRECT_INVOICE_POST', this.actionType.allowApprove)) {
        let Return: MenuItem = { label: this._translate.transform("APP_RETURN"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRetWindow(item, 'RETURNED') } };
        this.postgridContextMenu.push(Return);
      }
    }
    else if (item.status == "RETURNED") {
      this.postgridContextMenu = [];
      if (this.isCompGranted('SCR_DIRECT_INVOICE_POST', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editPostDirInv(item) } };
        this.postgridContextMenu.push(Edit);
      }
      if (this.isCompGranted('SCR_DIRECT_INVOICE_POST', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deletePostInvoice(item) } };
        this.postgridContextMenu.push(Delete);
      }
    }
    let attach: MenuItem = { label: 'File Attachment', icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "DIRECTINVOICE") } };
    let sidebar: MenuItem = { label: 'History', icon: 'pi pi-history', command: (event) => { this.dirInvPosthistory(item) } };

    this.postgridContextMenu.push(attach);
    this.postgridContextMenu.push(sidebar);
  }


  async dirInvPosthistory(item) {
    this.visibleSidebar2 = true;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      historyStatus: []
    }
    this.loadDirPostComments();
    this.loadDirPostAttachments();
    this.loadDirInvPostPayHistory();
    this.loadDirInvPostStatusHistory();
  }

  async loadDirPostAttachments() {

    var result = await this._webApiService.get("getDirInvPostPayAttachments/" + this.historyInfo.selectedItem.id);
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

  async loadDirPostComments() {
    this.historyInfo.comments = [];

    var result = await this._webApiService.get("getDirInvPostPayComment/" + this.historyInfo.selectedItem.id);
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

  async loadDirInvPostStatusHistory() {
    if (!this.historyInfo.historyStatus || this.historyInfo.historyStatus.length == 0) {
      var result = await this._webApiService.get("getDirInvPostPayStatusHist/" + this.historyInfo.selectedItem.id);
      this.historyInfo.historyStatus = [];
      result.forEach(element => {
        var status = this.statuses.find(x => x.code == element.status);
        this.historyInfo.historyStatus.push({
          id: element.id,
          Status: status.description,
          CreatedDate: element.createdDate,
          UserName: element.userName,
          Comments: element.comments
        })
      });
    }
  }

  async loadDirInvPostPayHistory() {
    if (!this.historyInfo.history || this.historyInfo.history.length == 0) {
      var result = await this._webApiService.get("getDirInvPostPayHistroy/" + this.historyInfo.selectedItem.id);
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


  async saveDirPostComment() {
    if (this.userComment.comments == "")
      return;

    this.userComment.appDocuments = this.uploadConfig.FileContent
    this.userComment.directInvPostPaymentId = this.historyInfo.selectedItem.id;

    var result = await this._webApiService.post("saveDirInvPostPayComment", this.userComment);
    if (result) {
      this.loadDirPostComments();
      this.loadDirPostAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
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
      else if (data.comments == "") {
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

  exportPrePaymentsToExcel() {
    let exportConfig = new AppExportExcelConfig();
    exportConfig.HeaderConfig = [];
    var header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_VENDOR_NAME");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 40;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_INV_NO");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_INV_DATE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_DOC_DATE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_LEDGER");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 30;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_COST_CENTRE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 30;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_AMOUNT");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    header.ColumnFormat = "#,##0";
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_DUE_AMOUNT");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    header.ColumnFormat = "#,##0";
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_REMARKS");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 40;
    exportConfig.HeaderConfig.push(header);

    var exportData = [];
    exportConfig.SheetName = this._translate.transform("DIRECT_INVOICE_PRE_PAYMENT_TITLE");
    this.prePayments.forEach(element => {
      var object = {
        vendorName: element.vendorTitle,
        invNo: element.invoiceNo,
        invDate: this._datePipe.transform(element.invoiceDate, 'dd-MM-yyyy'),
        docDate: this._datePipe.transform(element.documentDate, 'dd-MM-yyyy'),
        ledger: Number(element.ledgerCode),
        costCenter: Number(element.costCenterCode),
        amount: Number(element.amount),
        dueAmount: Number(element.dueAmount),
        remarks: element.remarks,
      }
      exportData.push(object);
    });
    exportConfig.ExcelData = exportData;
    exportConfig.FileName = this._translate.transform("DIRECT_INVOICE_PRE_PAYMENT_TITLE") + ".xlsx";
    this._export.excelConfig = exportConfig;
    this._export.exportExcel()
  }


  exportPostPaymentsToExcel() {
    let exportConfig = new AppExportExcelConfig();
    exportConfig.HeaderConfig = [];
    var header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_VENDOR_NAME");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 40;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_INV_NO");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_INV_DATE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_DOC_DATE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_LEDGER");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 30;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_COST_CENTRE");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 30;
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_AMOUNT");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 20;
    header.ColumnFormat = "#,##0";
    exportConfig.HeaderConfig.push(header);

    header = new AppExportExcelHeaderConfig();
    header.HeaderText = this._translate.transform("DIRECT_INVOICE_REMARKS");
    header.HorizontalAlign = AppExportExcelHorizantolAlign.Center;
    header.VerticalAlign = AppExportExcelVerticalAlign.Middle;
    header.ColumnWidth = 40;
    exportConfig.HeaderConfig.push(header);

    var exportData = [];
    exportConfig.SheetName = this._translate.transform("DIRECT_INVOICE_POST_PAYMENT_TITLE");
    this.postPayments.forEach(element => {
      var object = {
        vendorName: element.vendorTitle,
        invNo: element.invoiceNo,
        invDate: this._datePipe.transform(element.invoiceDate, 'dd-MM-yyyy'),
        docDate: this._datePipe.transform(element.documentDate, 'dd-MM-yyyy'),
        ledger: Number(element.ledgerCode),
        costCenter: Number(element.costCenterCode),
        amount: Number(element.amount),
        remarks: element.remarks,
      }
      exportData.push(object);
    });
    exportConfig.ExcelData = exportData;
    exportConfig.FileName = this._translate.transform("DIRECT_INVOICE_POST_PAYMENT_TITLE") + ".xlsx";
    this._export.excelConfig = exportConfig;
    this._export.exportExcel()
  }

  async deletePreInvoice(item) {
    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + item.transNo + "<br>" + this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("saveDirectInvPrePay", item);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getDefaults();
          }
          else {
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });
  }

  async deletePostInvoice(item) {
    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + item.transNo + "<br>" + this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("saveDirectInvPostPay", item);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getDirectInvoiceSearch();
          }
          else {
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });
  }


  switchTab(event, takeIndex: boolean = false) {
    let index = (takeIndex == true) ? event : event.index;
    this.activeTabIndex = index;
    if (index == 0) {
      this.allHighlight = true;
      this.pendHighlight = false;
      this.retHighlight = false;
      this.directInvoiceSearch.status=[];
      this.directInvoiceSearch.fromDocDate = this.plusMinusMonthToCurrentDate(-1);
      this.directInvoiceSearch.toDocDate = new Date();
      this.getDirectInvoiceSearch();
    }
    else {
      this.allHighlight = true;
      this.pendHighlight = false;
      this.retHighlight = false;
      this.directInvoiceSearch.status=[];
      this.directInvoiceSearch.fromDocDate = this.plusMinusMonthToCurrentDate(-1);
      this.directInvoiceSearch.toDocDate = new Date();
      this.getDirectInvoiceSearch();
    }
  }


}

interface status {
  name: string;
  code: string;
}