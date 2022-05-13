import { animate, state, style, transition, trigger } from '@angular/animations';
import { Byte } from '@angular/compiler/src/util';
import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { AppExportExcelConfig, AppExportExcelHeaderConfig, AppExportExcelHorizantolAlign, AppExportExcelVerticalAlign, AppExportService } from 'app/shared/services/app-export.service';
import { of, Subject } from 'rxjs';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AddEditBudgetAllocationComponent } from './addEdit-budget/addEdit-budget.component';
import { ExportModel } from 'app/shared/model/export-model';
import { ExportService } from 'app/shared/services/export-service';
import { DatePipe } from '@angular/common';
import { saveAs } from 'file-saver';
import { AppComponentBase } from 'app/shared/component/app-component-base';

import { FileUploadComponent } from '../../common/file-upload/file-upload.component';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
@Component({
  selector: 'budget-allocation',
  templateUrl: './budget-allocation.component.html',
  styleUrls: ['./budget-allocation.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService, DatePipe, AddEditBudgetAllocationComponent],
  animations: [
    trigger('rowExpansionTrigger', [
      state('void', style({
        transform: 'translateX(-10%)',
        opacity: 0
      })),
      state('active', style({
        transform: 'translateX(0)',
        opacity: 1
      })),
      transition('* <=> *', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)'))
    ])
  ]
})
export class BudgetAllocationComponent extends AppComponentBase implements OnInit, OnDestroy {
  gridContextMenu: MenuItem[] = [];
  gridDetailsContextMenu: MenuItem[] = [];
  public budgetAllocations: any = [];
  public budgetDetails: any = [];
  public showOrHideOrgFinyear: boolean = false;
  displayModal: boolean = false;
  selectedRow: any;
  expandedRows: any = {};
  showApproveReturn: boolean = false;
  approveRetRemarks: string = "";
  approveReturnHdr: string = "";
  activeTabIndex: number = 0;
  showApproveButton: boolean = false;
  showReturnButton: boolean = false;
  visibleSidebar: boolean = false;
  visibleDetailsSidebar: boolean = false;
  attachbtn: boolean = false;
  pdfdisabled: boolean = true;
  selectedTrans: any;
  statuses: any = [];

  dialogRef: DynamicDialogRef;
  submitted: boolean = false;
  id: any;
  budgetform: any = [];
  budgetTypes: any = [];
  budgetId: any;
  allBugetList: any = [];
  item: MenuItem[];

  selectedItem: any = [];
  uploadConfig: FileUploadConfig;
  HistoryHdrDeatils: any = [];
  historyHdr: any = [];
  addoredit: boolean = false;
  index: number = -1;
  lastIndex = -1;
  historyInfo: any;
  header: any = [];
  userComment: any = {};
  allHighlight: boolean = false;
  pendHighlight: boolean = false;
  retHighlight: boolean = false;
  lang;
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _toastrService: ToastrService,
    private _export: AppExportService,
    private _datePipe: DatePipe,
    private formBuilder: FormBuilder,
    private _exportService: ExportService,
    private _translate: TranslatePipe,
    private _codeMasterService: CodesMasterService,
    private _confirmService: ConfirmationService,
    private dialogService: DialogService

  ) {
    super(injector, 'SCR_BUDGT_ALLOCATION', 'allowView', _commonService)
    this.userComment = {
      comments: ""
    }
  }


  ngOnInit(): void {
    this._commonService.updatePageName(this._translate.transform("BUDGT_ALLOCATION"));
    this.budgetAllocations = [];
    this.getAll()
    this.showHideOrgFinyear('MNU_BUDGT_ALLOCATION');
    this.budgetform = {
      budgetType: "",
      fromDate: this.plusMinusMonthToCurrentDate(-1),
      toDate: new Date(),
      orgId: "",
      SelectedStatus: "",
      budgetAmount: "",
      id: "",

    }

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
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
  }
  export(exportType) {

    this.budgetform.exportType = exportType;
    this.budgetform.exportHeaderText = "Budget Allocation";
    var _url = "downloadBudgetAllocation";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: "Budget Allocation",
      _request: this.budgetform,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }

  onClickStatus(status) {
    this.budgetform.SelectedStatus = [];
    if (status != "ALL") {
      var status1 = this.statuses.find(a => a.code == status);
      this.budgetform.SelectedStatus.push(status1.code);
      this.budgetform.fromDate = "";
      this.budgetform.toDate = "";
      this.allHighlight = false;
      if (status == "SUBMITTED") {
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

      this.budgetform.fromDate = this.plusMinusMonthToCurrentDate(-1);
      this.budgetform.toDate = new Date();
    }
    this.searchService();
  }
  async searchService() {
    this.budgetform.orgId = this.orgId;
    this.budgetform.finYear = this.financialYear;

    this.expandedRows = {};
    let result = await this._webApiService.post('getBudgetAllocationSearch', this.budgetform);
    if (result == 0)
      this.pdfdisabled = false;
    else
      this.pdfdisabled = true;
    if (result) {
      this.budgetAllocations = result ?? of([]);
      if (this.budgetTypes && this.budgetTypes.length > 0)
        this.budgetAllocations.forEach(element => {
          var budgetType = this.budgetTypes.find(a => a.code == element.budgetType);
          if (budgetType)
            element.typeDesc = budgetType.description;
        });
    }
  }
  async getAll() {
    this.statuses = await this._codeMasterService.getCodesDetailByGroupCode("FINTRANSSTATUS", false, false, this._translate);
    this.budgetTypes = await this._codeMasterService.getCodesDetailByGroupCode("BUDGTDOCTYPE", false, false, this._translate);

    this.allHighlight = true;
    this.searchService();

  }

  clearSearch() {
    this.budgetform = {
      budgetType: "",
      fromDate: this.plusMinusMonthToCurrentDate(-1),
      toDate: new Date(),
      budgetAmount: "",
      status: "",
      SelectStatus: [],
      SelectBudgetType: ""
    };
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
        var result = await this._webApiService.post("SaveBudgetAllocation", this.selectedTrans);
        if (result) {
          var output = result as any;
          if (output.validations == null) {
            if (output.status == "DATASAVESUCSS") {
              this._toastrService.success(this._translate.transform("APP_SUCCESS"));
              this.getAll();
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
        var result = await this._webApiService.post("approveReturnBudgtAlloc", this.selectedTrans);
        if (result) {
          var output = result as any;
          if (output.validations == null) {
            if (output.status == "DATASAVESUCSS") {
              this._toastrService.success(this._translate.transform("APP_SUCCESS"));
              this.getAll();
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
  async showDetail(event) {
    if (!event.data.budgetDetails) {
      var result = await this._webApiService.get("getBudgetAllocDetByHdrId/" + event.data.id);
      if (result) {
        var output = result as any;
        if (output.validations == null) {
          event.data.budgetDetails = output;
          //this.displayModal = true;
        }
      }
    }
  }
  attachmentLink(data, type) {
    if (data != "") {

      if (data.transNo != "" && data.transNo != undefined) {
        this.header = this._translate.transform("FILE_ATTACHMENT_FOR_TRANSNO") + data.transNo;
      }
      else if (data.ledgerDesc != "" && data.ledgerDesc != undefined) {
        this.header = this._translate.transform("FILE_ATTACHMENT_LEDGERCODE") + data.ledgerCode;
      }
      else if (data.comments == "") {
        this.header = this._translate.transform("FILE_ATTACHMENT_COMMENTS");
      }
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

  createBudget() {
    this.router.navigate(["finance/addEdit-budget"], {
      state: { budgetId: "" },
    });
  }

  async editBudgetAlloction(selectedItem) {
    var result = await this._webApiService.get("getBudgetAlloctionById/" + selectedItem.id);
    this.router.navigate(["finance/addEdit-budget"], {
      state: { budgetId: selectedItem.id },
    });
  }

  async deleteBudgetAlloction(item) {
    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + item.transNo + "<br>" + this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.get("getBudgetAllocDetByHdrId/" + item.id);
        item.budgAllocDet = result;
        item.budgAllocDet.forEach(element => {
          element.active = "N";
        });

        var result = await this._webApiService.post("saveBudgetAllocation", item)
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getAll();
          }
          else {
            console.log(output.messages[0]);
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });

  }
  getGridDetailsContextMenu(item) {
    this.gridDetailsContextMenu = [];
    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "BUDGETALLODET") } };
    let sidebar: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showHistoryDetails(item) } };
    this.gridDetailsContextMenu.push(attach);
    this.gridDetailsContextMenu.push(sidebar);
  }
  async showHistoryDetails(item) {
    this.visibleSidebar = false;
    this.visibleDetailsSidebar = true;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      appDocument: []
    }
    this.loadDetailsComments(null);
    this.loadBudgetDetailsAttachments();
  }

  async getBudgetDetailsDetHistory() {
    var result = await this._webApiService.get("getBudgAlocDetHist/" + this.historyInfo.id);
    this.HistoryHdrDeatils = [];
    result.forEach(element => {
      this.HistoryHdrDeatils.push({
        id: element.id, fieldName: element.fieldName, createdDate: element.createdDate, userName: element.userName, currentValue: element.currentValue,
        prevValue: element.prevValue
      })
    });
  }

  async saveDetDetailsComment() {
    if (this.userComment.comments == "")
      return;

    this.userComment.appDocuments = this.uploadConfig.FileContent
    this.userComment.budgAllocDetId = this.historyInfo.selectedItem.id;

    var result = await this._webApiService.post("saveBudgtAllocDetComment", this.userComment);
    if (result) {
      this.loadDetailsComments(result);
      this.loadBudgetAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadDetailsComments(result) {
    this.historyInfo.comments = [];
    if (!result)
      result = await this._webApiService.get("getBudgtAllocDetComments/" + this.historyInfo.selectedItem.id);

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

  async loadBudgetDetailsAttachments() {

    var result = await this._webApiService.get("getBudgAlocDetAttachments/" + this.historyInfo.selectedItem.id);
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

  getGridContextMenu(item) {
    this.gridContextMenu = [];
    if (item.status == "SUBMITTED") {
      if (this.isGranted('SCR_BUDGT_ALLOCATION', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editBudgetAlloction(item) } };
        this.gridContextMenu.push(Edit);
      }
      if (this.isGranted('SCR_BUDGT_ALLOCATION', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deleteBudgetAlloction(item) } };
        this.gridContextMenu.push(Delete);
      }
      if (this.isGranted('SCR_BUDGT_ALLOCATION', this.actionType.allowApprove)) {
        let Approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRetWindow(item, 'APPROVED') } };
        this.gridContextMenu.push(Approve);
      }
      if (this.isGranted('SCR_BUDGT_ALLOCATION', this.actionType.allowApprove)) {
        let Return: MenuItem = { label: this._translate.transform("APP_RETURN"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRetWindow(item, 'RETURNED') } };
        this.gridContextMenu.push(Return);
      }
    }

    else if (item.status == "RETURNED") {
      this.gridContextMenu = [];
      if (this.isGranted('SCR_BUDGT_ALLOCATION', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editBudgetAlloction(item) } };
        this.gridContextMenu.push(Edit);
      }
      if (this.isGranted('SCR_BUDGT_ALLOCATION', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deleteBudgetAlloction(item) } };
        this.gridContextMenu.push(Delete);
      }
    }
    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "BUDGETALLOCATION") } };
    let sidebar: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showHistory(item) } };
    this.gridContextMenu.push(attach);
    this.gridContextMenu.push(sidebar);
  }
  async showHistory(item) {
    this.visibleSidebar = true;
    this.visibleDetailsSidebar = false;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      historyStatus: []
    }
    this.loadComments(null);
    this.loadBudgetAttachments();
    this.loadBudgetHdrStatusHistory();
    this.loadBudgetHdrHistory();
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

  async saveHdrComment() {
    if (this.userComment.comments == "")
      return;

    this.userComment.appDocuments = this.uploadConfig.FileContent;
    this.userComment.budgAllocHdrId = this.historyInfo.selectedItem.id;

    var result = await this._webApiService.post("saveBudgtAllocHdrComment", this.userComment);
    if (result) {
      this.loadComments(result);
      this.loadBudgetAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadComments(result) {
    this.historyInfo.comments = [];
    if (!result)
      result = await this._webApiService.get("getBudgtAllocHdrComments/" + this.historyInfo.selectedItem.id);

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

  async loadBudgetAttachments() {

    var result = await this._webApiService.get("getBudgAlocHdrAttachments/" + this.historyInfo.selectedItem.id);
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

  async loadBudgetHdrStatusHistory() {
    if (!this.historyInfo.historyStatus || this.historyInfo.historyStatus.length == 0) {
      var result = await this._webApiService.get("getHistoryStatus/" + this.historyInfo.selectedItem.id);
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

  async loadBudgetHdrHistory() {
    if (!this.historyInfo.history || this.historyInfo.history.length == 0) {
      var result = await this._webApiService.get("getBudgAlocHdrHist/" + this.historyInfo.selectedItem.id);
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
  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
  sidenavClosed() {
    this.index = this.lastIndex;
    this.addoredit = false;
  }

  async downloadFile(id) {
    var results = await this._webApiService.get("getFileDowenload/" + id);
    if (results) {
      var decodedString = atob(results.fileContent);
      saveAs.saveAs(decodedString, results.fileName);
    }
  }
}

