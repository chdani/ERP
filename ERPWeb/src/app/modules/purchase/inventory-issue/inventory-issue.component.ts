import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, Injector, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { of } from 'rxjs';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AddEditInvIssueComponent } from './addEdit-inventory-issue/addEdit-inventory-issue.component';
import { ExportModel } from 'app/shared/model/export-model';
import { ExportService } from 'app/shared/services/export-service';
import { DatePipe } from '@angular/common';
import { saveAs } from 'file-saver';
import { AppComponentBase } from 'app/shared/component/app-component-base';

import { FileUploadComponent } from '../../common/file-upload/file-upload.component';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { PurchaseService } from '../purchase.service';
import { AppGlobalDataService } from 'app/shared/services/app-global-data-service';
import { takeUntil } from 'rxjs/operators';
@Component({
  selector: 'inventory-issue',
  templateUrl: './inventory-issue.component.html',
  styleUrls: ['./inventory-issue.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService, DatePipe, AddEditInvIssueComponent],
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
export class InventoryIssueComponent extends AppComponentBase implements OnInit, OnDestroy {

  gridContextMenu: MenuItem[] = [];
  gridDetailsContextMenu: MenuItem[] = [];
  issueList: any = [];
  selectedEmployee: any;
  selectedStatus: any;
  statuses: any = [];
  employees: any = [];
  products: any = [];
  units: any = [];
  whLocations: any = [];

  expandedRows: any = {};
  showApproveReject: boolean = false;
  approveRejRemarks: string = "";
  approveRejectHdr: string = "";
  showApproveButton: boolean = false;
  showRejectButton: boolean = false;
  showHdrHistory: boolean = false;
  showDetHistory: boolean = false;

  disableExport: boolean = true;
  selectedTrans: any;

  dialogRef: DynamicDialogRef;
  adSearch: any = [];
  exportMenus: MenuItem[];

  uploadConfig: FileUploadConfig;
  historyInfo: any;
  header: any = [];
  userComment: any = {};
  allHighlight: boolean = false;
  pendHighlight: boolean = false;
  approvedHighlight: boolean = false;
  rejectHighlight: boolean = false;
  selectedUploadData: any = {}

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    private _globalService: AppGlobalDataService,
    public _commonService: AppCommonService,
    private _toastrService: ToastrService,
    private _datePipe: DatePipe,
    private _exportService: ExportService,
    private _translate: TranslatePipe,
    private _codeMasterService: CodesMasterService,
    private _confirmService: ConfirmationService,
    private dialogService: DialogService,
    private  _purchaseService: PurchaseService

  ) {
    super(injector,'SCR_INVENTORY_ISSUE','allowView', _commonService )
    this.userComment = {
      comments: ""
    }
  }


  ngOnInit(): void {
    this.uploadConfig =
    {
      TransactionId: 0,
      TransactionType: '',
      AllowedExtns: ".png,.jpg,.gif,.jpeg,.bmp,.docx,.doc,.pdf,.msg",
      DocTypeRequired: false,
      DocumentReference: "",
      ReadOnly: false,
      ScanEnabled: true,
      ShowSaveButton: true,
      FileContent: []
    };
    this._commonService.updatePageName(this._translate.transform("INV_ISSUE"));
    this.getAll()
    this.showHideOrgFinyear('MNU_INVENTORY_ISSUE');

    this.exportMenus = [
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

    this._commonService.fileObserver.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((file: any) => {
        this.selectedUploadData.appDocuments = file;
      });
  }

  export(exportType) {

    this.adSearch.exportType = exportType;
    this.adSearch.exportHeaderText = this._translate.transform("INV_ISSUE");
    var _url = "downloadInvIssue";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: this._translate.transform("INV_ISSUE"),
      _request: this.adSearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }

  async getAll() {
    this.statuses = await this._codeMasterService.getCodesDetailByGroupCode("PURTRANSSTATUS", true, false, this._translate);
    this.products = await this._purchaseService.getProductMaster(true, false, this._translate, "", false,false);
    this.units = await this._purchaseService.getProdUnitMaster(true, false, this._translate, false);
    this.employees = await this._globalService.getEmployeeList(true, false, this._translate, false);
    this.whLocations = await this._purchaseService.getWarehouseAndLocation(true, false, this._translate, false);
    this.clearSearch();
    this.searchRecords();
  }

  async searchRecords() {
    this.adSearch.employeeId = this.selectedEmployee.id;
    this.adSearch.status = this.selectedStatus.code;
    this.issueList = [];

    this.expandedRows = {};
    let result = await this._webApiService.post('getProdInvIssueList', this.adSearch);
    if (result && result.length > 0)
    {
      this.disableExport = false;
      this.issueList = result ?? of([]);
      this.issueList.forEach(element => {
        var employee = this.employees.find(a => a.id == element.employeeId);
        element.empName = employee?.empName;
      });
    }
    else
      this.disableExport = true;
    
  }

  onClickStatus(status) {
    this.selectedStatus = {};
    if (status != "ALL") {
      this.selectedStatus = this.statuses.find(a => a.code == status);
      this.adSearch.fromDate = "";
      this.adSearch.toDate = "";
      this.allHighlight = false;
      if (status == "PURTRNSTSREJECTED") {
        this.pendHighlight = false;
        this.approvedHighlight = false;
        this.rejectHighlight = true;
      }
      else if (status == "PURTRNSTSAPPROVED") {
        this.pendHighlight = false;
        this.approvedHighlight = true;
        this.rejectHighlight = false;
      }
      else {
        this.pendHighlight = true;
        this.approvedHighlight = false;
        this.rejectHighlight = false;
      }
    }
    else {
      this.allHighlight = true;
      this.pendHighlight = false;
      this.approvedHighlight = false;
      this.rejectHighlight = false;

      this.adSearch.fromDate = this.plusMinusMonthToCurrentDate(-1);
      this.adSearch.toDate = new Date();
      this.selectedStatus = this.statuses.find(a => a.code == "");
    }
    this.searchRecords();
  }

  clearSearch() {
    this.adSearch = {
      transNo: "",
      fromTransDate: this.plusMinusMonthToCurrentDate(-1),
      toTransDate: new Date(),
      employeeId: ""
    }
    this.selectedEmployee = this.employees.find(a => a.id == "");
    this.selectedStatus = this.statuses.find(a => a.code == "");
  }
    
  async showDetail(event) {
    if (!event.data.details) {
      event.data.details = [];
      var result = await this._webApiService.get("getProdInvIssueDetByHdrId/" + event.data.id);
      if (result) {
        var details = result as any;
        if (details.validations == null) {
          details.forEach(element => {
            var product = this.products.find(a => a.id == element.productMasterId);
            var unit = this.units.find(a => a.id == element.unitMasterId);
            var whLocation = this.whLocations.find(a => a.id == element.wareHouseLocationId);
            element.whLocation = whLocation?.name;
            element.productName = product?.name;
            element.unitName = unit?.unitName;
            event.data.details.push(element);
          });
        }
      }
    }
  }

  showApproveRejectWindow(item: any, status: string) {
    this.showApproveReject = true;
    this.selectedTrans = JSON.parse(JSON.stringify(item));
    this.approveRejRemarks = "";

    if (status == "APPROVE") {
      this.approveRejectHdr = this._translate.transform("APP_APPROVE_FOR_TRANSNO") + item.transNo;
      this.showApproveButton = true;
      this.showRejectButton = false;
    }
    else {
      this.approveRejectHdr = this._translate.transform("APP_REJECT_FOR_TRANSNO") + item.transNo;
      this.showApproveButton = false;
      this.showRejectButton = true;
    }
  }

  async processApproveOrReject(status: string) {
    if (this.approveRejRemarks == "") {
      this._toastrService.error(this._translate.transform("APP_REMARKS_REQ"));
      return;
    }

    if (this.selectedTrans) {

      this.selectedTrans.approverRemarks = this.approveRejRemarks;
      this.selectedTrans.status = status;
      this.selectedTrans.action = 'M';
      var result = await this._webApiService.post("approveProdInvIssue", this.selectedTrans);
      if (result) {
        var output = result as any;
        if (output.validations == null) {
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getAll();
            this.showApproveReject = false;
            this.showApproveButton = false;
            this.showRejectButton = false;
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

  attachmentLink(data, type, showSaveButton) {
    if (data != "") {

      if (data.transNo != "" && data.transNo != undefined) {
        this.header = this._translate.transform("FILE_ATTACHMENT_FOR_TRANSNO") + data.transNo;
      }
      else if (data.comments == "") {
        this.header = this._translate.transform("FILE_ATTACHMENT_COMMENTS");
      }
    }
    
    this.selectedUploadData = data;
    
    this.uploadConfig =
    {
      TransactionId: data.id,
      TransactionType: type,
      AllowedExtns: ".png,.jpg,.gif,.jpeg,.bmp,.docx,.doc,.pdf,.msg",
      DocTypeRequired: false,
      DocumentReference: "",
      ReadOnly: false,
      ScanEnabled: data.id ? false : true,
      ShowSaveButton: showSaveButton,
      FileContent: data.appDocuments
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

  createNewRecord() {
    this.router.navigate(["purchase/addEdit-inventory-issue"], {
      state: { reqId: "" },
    });
  }

  async editRecord(selectedItem) {
    var result = await this._webApiService.get("getProdInvIssueById/" + selectedItem.id); // dummy request to check active session
    this.router.navigate(["purchase/addEdit-inventory-issue"], {
      state: { reqId: selectedItem.id },
    });
  }

  async deleteRecord(item) {
    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + item.transNo + "<br>" + this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("saveProdInvIssue", item)
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
    let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showDetailHistoryInfo(item) } };
    this.gridDetailsContextMenu.push(history);
  }

  async showDetailHistoryInfo(item) {
    this.showHdrHistory = false;
    this.showDetHistory = true;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      appDocument: []
    }
    this.loadDetailComments();
    this.loadDetailHistory();
  }

  async loadDetailHistory() {
    var result = await this._webApiService.get("getProdInvIssueDetHistory/" + this.historyInfo.selectedItem.id);
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

  async saveDetailComment() {
    if (this.userComment.comments == "")
      return;

    if (this.uploadConfig && this.uploadConfig.FileContent)
        this.userComment.appDocuments = this.uploadConfig.FileContent
    
    this.userComment.prodInvIssueDetId = this.historyInfo.selectedItem.id;
    this.userComment.active = "Y";

    var result = await this._webApiService.post("saveProdInvIssueDetComment", this.userComment);
    if (result) {
      this.loadDetailComments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadDetailComments() {
    this.historyInfo.comments = [];
    var result = await this._webApiService.get("getProdInvIssueDetComment/" + this.historyInfo.selectedItem.id);

    if (result) {
      result.forEach(element => {
        var attachbtn = false;
        if (element.appDocuments.length != 0) {
          attachbtn = true;
        }
        this.historyInfo.comments.push({
          id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
          attachbtn: attachbtn,
        })
      });
    }
  }
 
  getGridContextMenu(item) {
    this.gridContextMenu = [];
    if (item.status != "PURTRNSTSAPPROVED") {
      if (this.isGranted('SCR_INVENTORY_ISSUE', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editRecord(item) } };
        this.gridContextMenu.push(Edit);
      }
      if (this.isGranted('SCR_INVENTORY_ISSUE', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deleteRecord(item) } };
        this.gridContextMenu.push(Delete);
      }

      if (item.status != "PURTRNSTSREJECTED") {
        if (this.isGranted('SCR_INVENTORY_ISSUE', this.actionType.allowApprove)) {
          let approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRejectWindow(item, 'APPROVE') } };
          this.gridContextMenu.push(approve);

          let reject: MenuItem = { label: this._translate.transform("APP_REJECT"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRejectWindow(item, 'REJECT') } };
          this.gridContextMenu.push(reject);
        }
      }
    }
   
    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "PRODINVISS", true) } };
    let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showHdrHistoryInfo(item) } };
    this.gridContextMenu.push(attach);
    this.gridContextMenu.push(history);
  }

  async showHdrHistoryInfo(item) {
    this.showHdrHistory = true;
    this.showDetHistory = false;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      statusHistory: []
    }
    this.loadHdrComments();
    this.loadHdrDocuments();
    this.loadHdrHistory();
  }


  async saveHdrComment() {
    if (this.userComment.comments == "")
      return;
    
    if (this.uploadConfig && this.uploadConfig.FileContent)
      this.userComment.appDocuments = this.uploadConfig.FileContent;
    
    this.userComment.prodInvIssueId = this.historyInfo.selectedItem.id;
    this.userComment.active = "Y";
    var result = await this._webApiService.post("saveProdInvIssueComment", this.userComment);
    if (result) {
      this.loadHdrComments();
      this.loadHdrDocuments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadHdrComments() {
    this.historyInfo.comments = [];

    var result = await this._webApiService.get("getProdInvIssueComment/" + this.historyInfo.selectedItem.id);
    if (result) {
      result.forEach(element => {
        var attachbtn = false;
        if (element.appDocuments.length != 0) {
          attachbtn = true;
        }
        this.historyInfo.comments.push({
          id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
          attachbtn: attachbtn,
        })
      });
    }
  }

  async loadHdrDocuments() {

    var result = await this._webApiService.get("getProdInvIssuesAttachments/" + this.historyInfo.selectedItem.id);
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

  async loadHdrHistory() {
    var result = await this._webApiService.get("getProdInvIssueHistory/" + this.historyInfo.selectedItem.id);
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

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
  sidenavClosed() {
 
  }

  async downloadFile(id) {
    var results = await this._webApiService.get("getFileDowenload/" + id);
    if (results) {
      var decodedString = atob(results.fileContent);
      saveAs.saveAs(decodedString, results.fileName);
    }
  }
}

