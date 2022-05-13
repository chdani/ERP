import { animate, state, style, transition, trigger } from '@angular/animations';
import { Byte } from '@angular/compiler/src/util';
import { Component, Injector, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { of, Subject } from 'rxjs';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AddEditServiceReqComponent } from './addEdit-service-request/addEdit-service-request.component';
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
  selector: 'service-request',
  templateUrl: './service-request.component.html',
  styleUrls: ['./service-request.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService, DatePipe, AddEditServiceReqComponent],
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
export class ServiceReqComponent extends AppComponentBase implements OnInit, OnDestroy {

  gridContextMenu: MenuItem[] = [];
  gridDetailsContextMenu: MenuItem[] = [];
  serviceRequests: any = [];
  selectedProduct: any;
  selectedEmployee: any;
  selectedStatus: any;
  statuses: any = [];
  selectedCatgory: any = [];
  employees: any = [];
  products: any = [];
  categories: any = [];
  productsList: any = [];

  expandedRows: any = {};
  showApproveReject: boolean = false;
  approveRejRemarks: string = "";
  approveRejectHdr: string = "";
  showApproveButton: boolean = false;
  showRejectButton: boolean = false;
  showReqHistory: boolean = false;
  showReqDetHistory: boolean = false;
  hiddenUpdateTransAndSeqNoButton:boolean =true;

  disableExport: boolean = true;
  selectedTrans: any;

  dialogRef: DynamicDialogRef;
  adSearch: any = [];
  exportMenus: MenuItem[];

  uploadConfig: FileUploadConfig;
  historyInfo: any;
  approvalProConfi: any;
  header: any = [];
  servDept: any = [];
  userComment: any = {};
  allHighlight: boolean = false;
  pendHighlight: boolean = false;
  completedHighlight: boolean = false;
  prodConfie: boolean = false;
  rejectHighlight: boolean = false;
  selectedUploadData: any = {};
  units: any = [];
  reqDetails: any = [];
  ProdConfiguration: any = [];
  lang;
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
    private _purchaseService: PurchaseService

  ) {
    super(injector, 'SCR_SERVICE_REQUEST', 'allowView', _commonService)
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
    this._commonService.updatePageName(this._translate.transform("SR_SERVICE_REQUEST"));
    this.getAll()
    this.showHideOrgFinyear('MNU_SERVICE_REQUEST');

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
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
  }
  export(exportType) {

    this.adSearch.exportType = exportType;
    this.adSearch.exportHeaderText = this._translate.transform("SR_SERVICE_REQUEST");
    var _url = "downloadServiceReq";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: this._translate.transform("SR_SERVICE_REQUEST"),
      _request: this.adSearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }

  async updateTransNoAndSeqNo(){
    var result = await this._webApiService.get("updateTransNoSeqNo");
        if (result) {
            var output = result as any;
            if (output.status == "DATASAVESUCSS") {

                this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                this.searchService();
            }
            else {
                this._toastrService.error(output.messages[0])
            }
        }

  }
  onClickStatus(status) {
    this.selectedStatus = {};
    if (status != "ALL") {
      this.selectedStatus = this.statuses.find(a => a.code == status);
      this.adSearch.fromDate = "";
      this.adSearch.toDate = "";
      this.allHighlight = false;
      if (status == "SERREQPENDING") {
        this.pendHighlight = true;
        this.completedHighlight = false;
        this.rejectHighlight = false;
      }
      if (status == "SERREQREJECTED") {
        this.pendHighlight = false;
        this.completedHighlight = false;
        this.rejectHighlight = true;
      }
      if(status == "SERREQCOMPLETE"){
        this.pendHighlight = false;
        this.completedHighlight = true;
        this.rejectHighlight = false;
      }
    }
    else {
      this.allHighlight = true;
      this.pendHighlight = false;
      this.completedHighlight = false;
      this.rejectHighlight = false;

      this.adSearch.fromDate = this.plusMinusMonthToCurrentDate(-1);
      this.adSearch.toDate = new Date();
      this.selectedStatus = this.statuses.find(a => a.code == "");
    }
    this.searchService();
  }

  async getAll() {
    this.statuses = await this._codeMasterService.getCodesDetailByGroupCode("SERREQSTATUS", true, false, this._translate);
    this.products = await this._purchaseService.getProductMaster(true, false, this._translate, "", false, false);
    this.categories = await this._purchaseService.getProductCategory(true, false, this._translate, false);
    this.employees = await this._globalService.getEmployeeList(true, false, this._translate, false);
    this.units = await this._purchaseService.getProdUnitMaster(false, true, this._translate, true);

    this.clearSearch();
    this.searchService();
  }

  async searchService() {
    this.adSearch.status = this.selectedStatus.code;
    this.adSearch.productId = this.selectedProduct.id;
    this.adSearch.employeeId = this.selectedEmployee.id;
    this.serviceRequests = [];

    this.expandedRows = {};
    let result = await this._webApiService.post('getServiceRequestList', this.adSearch);
    if (result && result.length > 0) {
      this.disableExport = false;
      this.serviceRequests = result ?? of([]);
      this.serviceRequests.forEach(element => {
        var employee = this.employees.find(a => a.id == element.employeeId);
        var category = this.categories.find(a => a.code == element.prodCategoryId)
        element.employeeName = employee?.empName;
        element.categoryName = category?.name;
      });
    }
    else
      this.disableExport = true;

  }


  clearSearch() {
    this.adSearch = {
      transNo: "",
      fromTransDate: this.plusMinusMonthToCurrentDate(-1),
      toTransDate: new Date(),
      productId: "",
      employeeId: "",
      status: "",
      categoryId: ""
    }
    this.selectedProduct = this.products.find(a => a.id == "");
    this.selectedStatus = this.statuses.find(a => a.code == "");
    this.selectedEmployee = this.employees.find(a => a.id == "");
    this.allHighlight = true;
  }

  async showApproveRejectWindow(item: any, status: string) {
    this.prodConfie = false;
    this.showApproveReject = true;
    this.selectedTrans = JSON.parse(JSON.stringify(item));
    this.approveRejRemarks = "";

    if (status == "APPROVE") {
      this.ProdConfiguration = await this._webApiService.get("getServRequestsWithIdDepartment");
      var ProdConfig = this.ProdConfiguration.find(a => a.id == item.id);
      this.approveRejectHdr = this._translate.transform("APP_APPROVE_FOR_TRANSNO") + item.transNo;
      this.showApproveButton = true;
      this.showRejectButton = false;
      if (ProdConfig) {
        this.approvalProConfi = "";
        this.prodConfie = true;
      }
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
      if (this.approvalProConfi) {
        this.selectedTrans.ProdConfiguration = this.approvalProConfi;
      }

      this.selectedTrans.approverRemarks = this.approveRejRemarks;
      this.selectedTrans.statusCode = status;
      this.selectedTrans.action = 'M';
      var result = await this._webApiService.post("approveServiceRequest", this.selectedTrans);
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
  async showDetail(event) {
    this.reqDetails = [];
    var result = await this._webApiService.get("getServiceRequestById/" + event.data.id);
    if (result) {
      var output = result as any;
      if (output.validations == null) {
        this.selectedCatgory = this.categories.find(a => a.code == output.prodCategoryId);
        var unit = this.units.find(a => a.id == output.unitMasterId);
        this.productsList = await this._purchaseService.getProductMaster(false, true, this._translate, this.selectedCatgory.code, true, false);
        var product = this.productsList.find(a => a.id == output.productMasterId);
        this.reqDetails.push({
          categoryName: output.prodCategoryName, productName: product?.name, unitName: unit?.unitName,
          quantity: output.quantity, prodSubCategoryName: output.prodSubCategoryName,
        })
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

  createServiceReq() {
    this.router.navigate(["purchase/addEdit-service-request"], {
      state: { reqId: "", approveSerRequest: false },
    });
  }

  async editServiceReq(selectedItem) {
    var result = await this._webApiService.get("getServiceRequestById/" + selectedItem.id); // dummy request to check active session
    this.router.navigate(["purchase/addEdit-service-request"], {
      state: { reqId: selectedItem.id, approveSerRequest: false },
    });
  }
  async approveServiceReq(selectedItem) {
    this.router.navigate(["purchase/addEdit-service-request"], {
      state: { reqId: selectedItem.id, approveSerRequest: true },
    });
  }

  async deleteServiceReq(item) {
    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + item.transNo + "<br>" + this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("saveServiceRequest", item)
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
  async getGridContextMenu(item) {
    this.servDept = await this._webApiService.get("getServRequestsWithApprovalServiceDepartment");
    var CreateInvIssus = this.servDept.find(a => a.id == item.id);
    this.gridContextMenu = [];
    if (item.curApprovalLevel == "0") {
      if (this.isGranted('SCR_SERVICE_REQUEST', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editServiceReq(item) } };
        this.gridContextMenu.push(Edit);
      }
      if (this.isGranted('SCR_SERVICE_REQUEST', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deleteServiceReq(item) } };
        this.gridContextMenu.push(Delete);
      }
      if (this.isGranted('SCR_SERVICE_REQUEST', this.actionType.allowApprove) && item.canApproveReject) {
        let approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.approveServiceReq(item) } };
        this.gridContextMenu.push(approve);

        let reject: MenuItem = { label: this._translate.transform("APP_REJECT"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRejectWindow(item, 'REJECT') } };
        this.gridContextMenu.push(reject);
      }
    }
    else if (item.curApprovalLevel != item.nextApprovalLevel && item.canApproveReject && this.isGranted('SCR_SERVICE_REQUEST', this.actionType.allowApprove)) {
      let approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.approveServiceReq(item) } };
      this.gridContextMenu.push(approve);

      let reject: MenuItem = { label: this._translate.transform("APP_REJECT"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRejectWindow(item, 'REJECT') } };
      this.gridContextMenu.push(reject);
    }
    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "SERVREQUEST", true) } };
    let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showReqHistoryInfo(item) } };
    this.gridContextMenu.push(attach);
    this.gridContextMenu.push(history);
    if (CreateInvIssus) {
      if (item.id == CreateInvIssus.id && this.isGranted('SCR_INVENTORY_ISSUE', this.actionType.allowAdd)) {
        let Create: MenuItem = { label: this._translate.transform("CREATE_INVENTORYISSUE"), icon: 'pi pi-plus', command: (event) => { this.CreateInvIssue(item) } };
        this.gridContextMenu.push(Create);
      }
    }
  }
  async CreateInvIssue(item) {
    this.router.navigate(["purchase/addEdit-inventory-issue"], {
      state: { transNo: item.transNo },
    });
  }
  async showReqHistoryInfo(item) {
    this.showReqHistory = true;
    this.showReqDetHistory = false;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      statusHistory: []
    }
    this.loadServReqComments();
    this.loadServReqDocuments();
    this.loadServiceReqStatusHistory();
    this.loadServReqHistory();
  }


  async saveServReqComment() {
    if (this.userComment.comments == "")
      return;

    if (this.uploadConfig && this.uploadConfig.FileContent)
      this.userComment.appDocuments = this.uploadConfig.FileContent;

    this.userComment.serviceRequestId = this.historyInfo.selectedItem.id;
    this.userComment.active = "Y";
    var result = await this._webApiService.post("saveServiceRequestComment", this.userComment);
    if (result) {
      this.loadServReqComments();
      this.loadServReqDocuments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadServReqComments() {
    this.historyInfo.comments = [];

    var result = await this._webApiService.get("getServiceReqComment/" + this.historyInfo.selectedItem.id);
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

  async loadServReqDocuments() {

    var result = await this._webApiService.get("getServReqAttachments/" + this.historyInfo.selectedItem.id);
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

  async loadServiceReqStatusHistory() {
    if (!this.historyInfo.statusHistory || this.historyInfo.statusHistory.length == 0) {
      var result = await this._webApiService.get("getServiceReqStatusHistory/" + this.historyInfo.selectedItem.id);
      this.historyInfo.statusHistory = [];
      result.forEach(element => {
        var status = this.statuses.find(x => x.code == element.status);
        this.historyInfo.statusHistory.push({
          id: element.id,
          Status: status.description,
          CreatedDate: element.createdDate,
          UserName: element.userName,
          Comments: element.remarks
        })
      });
    }

  }

  async loadServReqHistory() {
    if (!this.historyInfo.history || this.historyInfo.history.length == 0) {
      var result = await this._webApiService.get("getServiceReqHistory/" + this.historyInfo.selectedItem.id);
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

  }

  async downloadFile(id) {
    var results = await this._webApiService.get("getFileDowenload/" + id);
    if (results) {
      var decodedString = atob(results.fileContent);
      saveAs.saveAs(decodedString, results.fileName);
    }
  }
}

