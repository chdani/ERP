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
import { ExportModel } from 'app/shared/model/export-model';
import { ExportService } from 'app/shared/services/export-service';
import { DatePipe } from '@angular/common';
import { saveAs } from 'file-saver';
import { AppComponentBase } from 'app/shared/component/app-component-base';

import { FileUploadComponent } from '../../common/file-upload/file-upload.component';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { PurchaseService } from '../purchase.service';
import { AppGlobalDataService } from 'app/shared/services/app-global-data-service';
@Component({
  selector: 'purchase-order',
  templateUrl: './purchase-order.component.html',
  styleUrls: ['./purchase-order.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService, DatePipe],
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
export class PurchaseOrderComponent extends AppComponentBase implements OnInit, OnDestroy {

  gridContextMenu: MenuItem[] = [];
  gridDetailsContextMenu: MenuItem[] = [];
  purchaseOrderList: any = [];
  selectedVendor: any;
  selectedStatus: any;
  statuses: any = [];
  vendors: any = [];
  products: any = [];
  units: any = [];

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
  addoredit: boolean = false;
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
    super(injector, 'SCR_PURCHASE_ORD', 'allowView', _commonService)
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
    this._commonService.updatePageName(this._translate.transform("PURCHASEORDER"));
    this.showHideOrgFinyear('MNU_PURCHASE_ORD');
    this.getAll()

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
    this.adSearch.exportHeaderText = this._translate.transform("PURCHASEORDER");
    var _url = "downloadPurchaseOrder";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: this._translate.transform("PURCHASEORDER"),
      _request: this.adSearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
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

  async getAll() {
    this.statuses = await this._codeMasterService.getCodesDetailByGroupCode("PURTRANSSTATUS", true, false, this._translate);
    this.products = await this._purchaseService.getProductMaster(true, false, this._translate, "", false, false);
    this.units = await this._purchaseService.getProdUnitMaster(true, false, this._translate, false);
    this.vendors = await this._purchaseService.getVendorMaster(true, false, this._translate, false);
    this.clearSearch();
    this.searchRecords();
  }

  async searchRecords() {
    this.adSearch.status = this.selectedStatus.code;
    this.adSearch.vendorMasterId = this.selectedVendor.id;
    this.adSearch.orgId = this.orgId;
    this.adSearch.finYear = this.financialYear;

    this.purchaseOrderList = [];

    this.expandedRows = {};
    let result = await this._webApiService.post('getPurchaseOrderList', this.adSearch);
    if (result && result.length > 0) {
      this.disableExport = false;
      this.purchaseOrderList = result ?? of([]);
      this.purchaseOrderList.forEach(element => {
        var vendor = this.vendors.find(a => a.id == element.vendorMasterId);
        element.vendorName = vendor?.name;
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
      fromInvDate: "",
      toInvDate: "",
      InvNo: "",
      vendorMasterId: "",
      status: ""
    }
    this.selectedVendor = this.vendors.find(a => a.id == "");
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
      this.selectedTrans.action = 'M';
      var result = await this._webApiService.post("approvePurchaseOrder", this.selectedTrans);
      if (result) {
        var output = result as any;
        if (output.validations == null) {
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            if (output.referenceId && this.selectedTrans.status == "PURTRNSTSAPPROVED") {
              var email = await this._webApiService.get("sendPurchaseOrderMail/" + result.referenceId);
            }
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
    if (!event.data.details) {
      event.data.details = [];
      var result = await this._webApiService.get("getPurchaseOrderDetByHdrId/" + event.data.id);
      if (result) {
        var details = result as any;
        if (details.validations == null) {
          details.forEach(element => {
            var product = this.products.find(a => a.id == element.productMasterId);
            var unit = this.units.find(a => a.id == element.unitMasterId);
            element.productName = product?.name;
            element.unitName = unit?.unitName;
            event.data.details.push(element);
          });
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

  createNewRecord() {
    this.router.navigate(["purchase/create-edit-purchase-order"], {
      state: { reqId: "" },
    });
  }

  async editRecord(selectedItem) {
    var result = await this._webApiService.get("getGoodsRecNoteById/" + selectedItem.id); // dummy request to check active session
    this.router.navigate(["purchase/create-edit-purchase-order"], {
      state: { reqId: selectedItem.id },
    });
  }

  async deleteRecord(item) {
    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + item.transNo + "<br>" + this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("savePurchaseOrder", item)
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
    this.loadPurchaseDocuments();
    this.loadDetailHistory();
  }

  async loadDetailHistory() {
    var result = await this._webApiService.get("getPurchaseOrderDetHistory/" + this.historyInfo.selectedItem.id);
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
  async loadPurchaseDocuments() {

    var result = await this._webApiService.get("getPurchaseOrderDetAttachments/" + this.historyInfo.selectedItem.id);
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

  async saveDetailComment() {
    if (this.userComment.comments == "")
      return;

    if (this.uploadConfig && this.uploadConfig.FileContent)
      this.userComment.appDocuments = this.uploadConfig.FileContent

    this.userComment.purchaseOrderDetId = this.historyInfo.selectedItem.id;
    this.userComment.active = "Y";

    var result = await this._webApiService.post("savePurchaseOrderDetComment", this.userComment);
    if (result) {
      this.loadDetailComments();
      this.loadPurchaseDocuments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadDetailComments() {
    this.historyInfo.comments = [];
    var result = await this._webApiService.get("getPurchaseOrderDetComment/" + this.historyInfo.selectedItem.id);

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
      if (this.isGranted('SCR_PURCHASE_ORD', this.actionType.allowEdit)) {
        let Edit: MenuItem = { label: this._translate.transform("SCREEN_EDIT_CHK"), icon: 'pi pi-pencil', command: (event) => { this.editRecord(item) } };
        this.gridContextMenu.push(Edit);
      }
      if (this.isGranted('SCR_PURCHASE_ORD', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("SCREEN_DELETE_CHK"), icon: 'pi pi-trash', command: (event) => { this.deleteRecord(item) } };
        this.gridContextMenu.push(Delete);
      }

      if (item.status != "PURTRNSTSREJECTED") {
        if (this.isGranted('SCR_PURCHASE_ORD', this.actionType.allowApprove)) {
          let approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRejectWindow(item, 'APPROVE') } };
          this.gridContextMenu.push(approve);

          let reject: MenuItem = { label: this._translate.transform("APP_REJECT"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRejectWindow(item, 'REJECT') } };
          this.gridContextMenu.push(reject);
        }
      }
    }

    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "PURCHASEORDER", true) } };
    let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showHdrHistoryInfo(item) } };
    this.gridContextMenu.push(attach);
    this.gridContextMenu.push(history);
    if (item.status == "PURTRNSTSAPPROVED" && this.isGranted('SCR_G_R_NOTES', this.actionType.allowAdd)) {
      let recivePurchas: MenuItem = { label: this._translate.transform("RECIVE_PURCHASEGRN"), icon: 'pi pi-send', command: (event) => { this.recivePurchasReq(item) } };
      this.gridContextMenu.push(recivePurchas);
    }
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
    this.loadHdrStatusHistory();
    this.loadHdrHistory();
  }
  async recivePurchasReq(item) {
    this.router.navigate(["purchase/addEdit-goods-receipt-notes"], {
      state: { transNo: item.transNo },
    });
  }

  async saveHdrComment() {
    debugger
    if (this.userComment.comments == "")
      return;

    if (this.uploadConfig && this.uploadConfig.FileContent)
      this.userComment.appDocuments = this.uploadConfig.FileContent;

    this.userComment.PurchaseOrderId = this.historyInfo.selectedItem.id;
    this.userComment.active = "Y";
    var result = await this._webApiService.post("savePurchaseOrderComment", this.userComment);
    if (result) {
      this.loadHdrComments();
      this.loadHdrDocuments();
      this.uploadConfig.FileContent = [];
      this.userComment.comments = "";
    }
  }

  async loadHdrComments() {
    this.historyInfo.comments = [];

    var result = await this._webApiService.get("getPurchaseOrderComment/" + this.historyInfo.selectedItem.id);
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

    var result = await this._webApiService.get("getPurchaseOrderAttachments/" + this.historyInfo.selectedItem.id);
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

  async loadHdrStatusHistory() {
    var result = await this._webApiService.get("getPurchaseOrderStatusHistory/" + this.historyInfo.selectedItem.id);
    this.historyInfo.statusHistory = [];
    result.forEach(element => {
      var status = this.statuses.find(x => x.code == element.status);
      this.historyInfo.statusHistory.push({
        id: element.id,
        Status: status.description,
        CreatedDate: element.createdDate,
        UserName: element.userName,
        Comments: element.comments
      })
    });
  }

  async loadHdrHistory() {
    var result = await this._webApiService.get("getPurchaseOrderHistory/" + this.historyInfo.selectedItem.id);
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

