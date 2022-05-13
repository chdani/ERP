import { animate, state, style, transition, trigger } from '@angular/animations';
import { DatePipe } from '@angular/common';
import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppGlobalDataService } from 'app/shared/services/app-global-data-service';
import { ExportService } from 'app/shared/services/export-service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { PurchaseService } from '../purchase.service';
import { Router, ActivatedRoute } from "@angular/router";
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { of } from 'rxjs';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { ExportModel } from 'app/shared/model/export-model';
import { saveAs } from 'file-saver';
import { CreateEditPurchaseRequestComponent } from './create-edit-purchase-request/create-edit-purchase-request.component';
@Component({
  selector: 'app-purchase-request',
  templateUrl: './purchase-request.component.html',
  styleUrls: ['./purchase-request.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService, DatePipe, CreateEditPurchaseRequestComponent],
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
export class PurchaseRequestComponent extends AppComponentBase implements OnInit {
  gridContextMenu: MenuItem[] = [];
  gridDetailsContextMenu: MenuItem[] = [];
  userComment: any = {};
  vendorMaster: any = [];
  expandedRows: any = {};
  units: any = [];
  products: any = [];
  CatgoryId: string = "";
  statuses: any = [];
  adSearch: any = [];
  selectedVendorMaster: any;
  selectedUnitMaster: any;
  selectedStatus: any;
  selectedVendorQuotation: any;

  purchaseRequests: any = [];
  allHighlight: boolean = false;
  pendHighlight: boolean = false;
  completedHighlight: boolean = false;
  rejectHighlight: boolean = false;
  historyInfo: any;
  uploadConfig: FileUploadConfig;
  showReqHistory: boolean = false;
  showReqDetHistory: boolean = false;
  header: any = [];
  addoredit: boolean = false;
  dialogRef: DynamicDialogRef;
  selectedTrans: any;
  showApproveReject: boolean = false;
  approveRejRemarks: string = "";
  approveRejectHdr: string = "";
  showApproveButton: boolean = false;
  showRejectButton: boolean = false;
  disableExport: boolean = true;
  index: number = -1;
  purchaseDetList: any = [];
  lastIndex = -1;
  exportMenus: MenuItem[];
  vendorQuotation:any =[];
  lang;

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    private _globalService: AppGlobalDataService,
    public _commonService: AppCommonService,
    private _toastrService: ToastrService,
    private _datePipe: DatePipe,
    private formBuilder: FormBuilder,
    private _exportService: ExportService,
    private _translate: TranslatePipe,
    private _codeMasterService: CodesMasterService,
    private _confirmService: ConfirmationService,
    private dialogService: DialogService,
    private _purchaseService: PurchaseService
  ) {
    super(injector,'SCR_PURCHASE_REQ','allowView', _commonService )
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
      ReadOnly: this.addoredit,
      ScanEnabled: this.addoredit,
      ShowSaveButton: !this.addoredit,
      FileContent: []
    };
    this._commonService.updatePageName(this._translate.transform("PR_PERCHASE_REQUEST"));
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
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;

  }
  export(exportType) {

    this.adSearch.exportType = exportType;
    this.adSearch.exportHeaderText = this._translate.transform("PR_PERCHASE_REQUEST");
    var _url = "downloadPurchaseReq";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: this._translate.transform("PR_PERCHASE_REQUEST"),
      _request: this.adSearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }
  
  async getAll() {
    var searchInput = {
        isApproved: "True"
      };
    this.vendorQuotation = await this._webApiService.post("getVendorQuotationListByActive", searchInput);
    this.statuses = await this._codeMasterService.getCodesDetailByGroupCode("PURTRANSSTATUS", true, false, this._translate);
    this.vendorMaster = await this._purchaseService.getVendorMasters(true, false, this._translate, false);
    this.units = await this._purchaseService.getProdUnitMaster(true, false, this._translate, false);
    this.products = await this._purchaseService.getProductMaster(true, false, this._translate, this.CatgoryId = "", false,false);
    this.clearSearch();
    this.searchService();
  }
  async searchService() {
    this.adSearch.status = this.selectedStatus.code;
    this.adSearch.vendorMasterId = this.selectedVendorMaster.id;
    this.adSearch.vendorQuotationId = this.selectedVendorQuotation.id;
    this.purchaseRequests = [];

    this.expandedRows = {};
    let result = await this._webApiService.post('getPurchaseRequestList', this.adSearch);
    if (result && result.length > 0) {
      this.disableExport = false;
      this.purchaseRequests = result ?? of([]);
      this.purchaseRequests.forEach(element => {
        var vendorQuotationTitle = this.vendorQuotation.find(a => a.id == element.vendorQuotationId);
        var vendorMasterTitle = this.vendorMaster.find(a => a.id == element.vendorMasterId)
        element.vendorQuotationTitle = vendorQuotationTitle?.transNo;
        element.vendorMasterTitle = vendorMasterTitle?.title + "."+ vendorMasterTitle?.name;
      });
    }

    else
      this.disableExport = true;

  }
  async showDetail(event) {
    if (!event.data.reqDetails) {
      var result = await this._webApiService.get("getPurchaseReqDetByHdrId/" + event.data.id);
      if (result) {
        this.purchaseDetList = result as any;
        this.purchaseDetList.forEach(element => {
          var productNamedata = this.products.find(a => a.id == element.productMasterId);
          var unitNamedata = this.units.find(a => a.id == element.unitMasterId);
          element.productName = productNamedata?.name;
          element.unitName = unitNamedata?.unitName;
        });
      }
    }
  }
  onClickStatus(status) {
    this.selectedStatus = {};
    if (status != "ALL") {
      this.selectedStatus = this.statuses.find(a => a.code == status);
      this.adSearch.fromTransDate = "";
      this.adSearch.toTransDate = "";
      this.allHighlight = false;
      if (status == "PURTRNSTSSUBMITTED") {
        this.pendHighlight = true;
        this.completedHighlight = false;
        this.rejectHighlight = false;
      }
      else if (status == "PURTRNSTSREJECTED") {
        this.pendHighlight = false;
        this.completedHighlight = false;
        this.rejectHighlight = true;
      }
      else {
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

      this.adSearch.fromTransDate = this.plusMinusMonthToCurrentDate(-1);
      this.adSearch.toTransDate = new Date();
      this.selectedStatus = this.statuses.find(a => a.code == "");
    }
    this.searchService();
  }
  clearSearch() {
    this.adSearch = {
      transNo: "",
      fromTransDate: this.plusMinusMonthToCurrentDate(-1),
      toTransDate: new Date(),
      vendorQuotationId: "",
      vendorMasterId: "",
      Status: "",
    }
    this.selectedVendorQuotation = this.vendorMaster.find(a => a.id == "");
    this.selectedVendorMaster = this.vendorMaster.find(a => a.id == "");
    this.selectedStatus = this.statuses.find(a => a.code == "");
    this.allHighlight = true;
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
      this.selectedTrans.id = this.selectedTrans.id;
      this.selectedTrans.action = 'M';
      var result = await this._webApiService.post("approvePurchaseRequest", this.selectedTrans);
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

  createServiceReq() {
    this.router.navigate(["purchase/create-edit-purchase-request"], {
      state: { reqId: "" },
    });
  }
  attachmentLink(data, type, showSaveButton) {
    if (data != "") {

      if (data.transNo != "" && data.transNo != undefined) {
        this.header = this._translate.transform("FILE_ATTACHMENT_FOR_TRANSNO") + data.transNo;
      }
      else if (data.comments == "") {
        this.header = this._translate.transform("FILE_ATTACHMENT_COMMENTS");
      }
      else {
        this.header = this._translate.transform("FILE_ATTACHMENT");
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
      ReadOnly: false,
      ScanEnabled: this.addoredit,
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
  async editPurchaseReq(selectedItem) {
    var result = await this._webApiService.get("getPurchaseRequestById/" + selectedItem.id); // dummy request to check active session
    this.router.navigate(["purchase/create-edit-purchase-request"], {
      state: { reqId: selectedItem.id },
    });
  }

  async deleteServiceReq(item) {
    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + item.transNo + "<br>" + this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("savePurchaseRequest", item)
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
    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "PURCHASEREQDET", true) } };
    let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showReqDetHistoryInfo(item) } };
    this.gridDetailsContextMenu.push(attach);
    this.gridDetailsContextMenu.push(history);
  }
  async showReqDetHistoryInfo(item) {
    this.showReqHistory = false;
    this.showReqDetHistory = true;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      appDocument: []
    }
    this.loadPurchaseReqDetComments();
    this.loadPurchaseReqDetAttachments();
    this.loadPurchaseReqDetHistory();
  }

  async loadPurchaseReqDetHistory() {
    var result = await this._webApiService.get("getPurchaseReqDetHistory/" + this.historyInfo.selectedItem.id);
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

  async savePurchaseReqDetComment() {
    if (this.userComment.comments == "")
      return;

    if (this.uploadConfig && this.uploadConfig.FileContent)
      this.userComment.appDocuments = this.uploadConfig.FileContent

    this.userComment.purchaseRequestDetId = this.historyInfo.selectedItem.id;
    this.userComment.active = "Y";
    var result = await this._webApiService.post("savePurchaseReqDetComment", this.userComment);
    if (result) {
      this.loadPurchaseReqDetComments();
      this.loadPurchaseReqDetAttachments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadPurchaseReqDetComments() {
    this.historyInfo.comments = [];
    var result = await this._webApiService.get("getPurchaseReqDetComment/" + this.historyInfo.selectedItem.id);

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

  async loadPurchaseReqDetAttachments() {

    var result = await this._webApiService.get("getPurchaseReqDetAttachments/" + this.historyInfo.selectedItem.id);
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
    if (item.status != "PURTRNSTSAPPROVED") {
      if (this.isGranted('SCR_PURCHASE_REQ', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editPurchaseReq(item) } };
        this.gridContextMenu.push(Edit);
      }
      if (this.isGranted('SCR_PURCHASE_REQ', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deleteServiceReq(item) } };
        this.gridContextMenu.push(Delete);
      }
      if (item.status != "PURTRNSTSREJECTED") {
        if (this.isGranted('SCR_PURCHASE_REQ', this.actionType.allowApprove)) {
          let approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRejectWindow(item, 'APPROVE') } };
          this.gridContextMenu.push(approve);

          let reject: MenuItem = { label: this._translate.transform("APP_REJECT"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRejectWindow(item, 'REJECT') } };
          this.gridContextMenu.push(reject);
        }
      }

    }
    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "PURCHASEREQ", true) } };
    let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showReqHistoryInfo(item) } };
    this.gridContextMenu.push(attach);
    this.gridContextMenu.push(history);
    if(item.status == "PURTRNSTSAPPROVED" &&  this.isGranted('SCR_PURCHASE_ORD', this.actionType.allowAdd)){     
        let recivePurchas: MenuItem = { label: this._translate.transform("RECIVE_PURCHASEORD"), icon: 'pi pi-plus', command: (event) => { this.recivePurchasOrd(item) } };
        this.gridContextMenu.push(recivePurchas);
      
    }
  }
  async recivePurchasOrd(item){
    this.router.navigate(["purchase/create-edit-purchase-order"], {
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
    this.loadPurchaseComments();
    this.loadPurchaseReqDocuments();
    this.loadPurchaseReqStatusHistory();
    this.loadPurchaseReqHistory();
  }


  async savePurchaseReqComment() {
    if (this.userComment.comments == "")
      return;

    if (this.uploadConfig && this.uploadConfig.FileContent)
      this.userComment.appDocuments = this.uploadConfig.FileContent;

    this.userComment.purchaseRequestId = this.historyInfo.selectedItem.id;
    this.userComment.active = "Y";
    var result = await this._webApiService.post("savePurchaseRequestComment", this.userComment);
    if (result) {
      this.loadPurchaseComments();
      this.loadPurchaseReqDocuments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadPurchaseComments() {
    this.historyInfo.comments = [];

    var result = await this._webApiService.get("getPurchaseReqComment/" + this.historyInfo.selectedItem.id);
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

  async loadPurchaseReqDocuments() {

    var result = await this._webApiService.get("getPurchaseReqAttachments/" + this.historyInfo.selectedItem.id);
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

  async loadPurchaseReqStatusHistory() {
    if (!this.historyInfo.statusHistory || this.historyInfo.statusHistory.length == 0) {
      var result = await this._webApiService.get("getPurchaseReqStatusHistory/" + this.historyInfo.selectedItem.id);
      this.historyInfo.statusHistory = [];
      result.forEach(element => {
        var status = this.statuses.find(x => x.code == element.status);
        this.historyInfo.statusHistory.push({
          id: element.id,
          Status: status?.description,
          CreatedDate: element.createdDate,
          UserName: element.userName,
          Comments: element.comments
        })
      });
    }

  }

  async loadPurchaseReqHistory() {
    if (!this.historyInfo.history || this.historyInfo.history.length == 0) {
      var result = await this._webApiService.get("getPurchaseReqHistory/" + this.historyInfo.selectedItem.id);
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
  sidenavClosed() {
    this.index = this.lastIndex;
    this.addoredit = false;
  }
  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
  async downloadFile(id) {
    var results = await this._webApiService.get("getFileDowenload/" + id);
    if (results) {
      var decodedString = atob(results.fileContent);
      saveAs.saveAs(decodedString, results.fileName);
    }
  }
}
