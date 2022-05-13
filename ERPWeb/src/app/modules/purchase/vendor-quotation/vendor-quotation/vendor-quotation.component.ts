import { Component, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { FormControl } from '@angular/forms';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { FinanceService } from 'app/modules/finance/finance.service';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { DatePipe } from '@angular/common';
import { ExportModel } from 'app/shared/model/export-model';
import { ExportService } from 'app/shared/services/export-service';
@Component({
  selector: 'app-vendor-quotation',
  templateUrl: './vendor-quotation.component.html',
  styleUrls: ['./vendor-quotation.component.scss'],
  styles: [],
  encapsulation: ViewEncapsulation.None,
  providers: [
    TranslatePipe, DatePipe,
    ConfirmationService, TranslatePipe
  ],
})
export class VendorQuotationComponent extends AppComponentBase implements OnInit {
  searchInputControl: FormControl = new FormControl();
  selectedProductForm: FormGroup;
  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private _formBuilder: FormBuilder,
    private _confirmService: ConfirmationService,
    private _translate: TranslatePipe,
    private _router: Router,
    private _toastrService: ToastrService,
    private _finService: FinanceService,
    private _webApiservice: WebApiService,
    public _commonService: AppCommonService,
    private _datePipe: DatePipe,
    private _exportService: ExportService,
    private _codeMasterService: CodesMasterService,
    private dialogService: DialogService


  ) {
    super(injector, 'SCR_VENDOR_QUOT', 'allowView', _commonService)
  }


  vendorQuotationModel: any = [];
  vendorQuotation: any;
  vendorQuotationDetModel: any = [];
  vendorQuotationDet: any = [];
  dataaccount = [];
  gridDetailsContextMenu: MenuItem[] = [];
  gridVendorQuoDetContextMenu: MenuItem[] = [];
  vendorlist: any;
  displayModal: boolean = false;
  public showOrHideOrgFinyear: boolean = false;
  header: any = [];
  uploadConfig: FileUploadConfig;
  addoredit: boolean = false;
  dialogRef: DynamicDialogRef;
  historyInfo: any;
  showVendorHistory: boolean = false;
  showVendorDetHistory: boolean = false;
  userComment: any = {};
  statuses: any = [];
  index: number = -1;
  lastIndex = -1;
  showApproveReject: boolean = false;
  selectedTrans: any;
  approveRejRemarks: string = "";
  approveRejectHdr: string = "";
  showApproveButton: boolean = false;
  showRejectButton: boolean = false;
  exportMenus: MenuItem[];
  allHighlight: boolean = false;
  pendHighlight: boolean = false;
  completedHighlight: boolean = false;
  rejectHighlight: boolean = false;
  shortListHighlight: boolean = false;
  selectedStatus: any;
  disableExport: boolean = true;







  ngOnInit(): void {
    this._commonService.updatePageName(this._translate.transform("VENDOR_QUOTATION"));

    this.vendorQuotation = {


      id: '',
      quotationRequestId: '',
      vendorMasterId: '',
      transNo: '',
      transDate: '',
      quotationNo: '',
      quotationdDate: '',
      isApproved: '',
      status: '',
      remarks: '',
      other: ''

    }
    this.allHighlight = true;
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


    this.loadStutes();
    this.getAllVendorQuotation();

    this.showHideOrgFinyear('MNU_VENDOR_MASTER');

  }
  async loadStutes() {
    this.statuses = await this._codeMasterService.getCodesDetailByGroupCode("PURTRANSSTATUS", true, false, this._translate);
  }
  export(exportType) {
    this.vendorQuotation.exportType = exportType;
    this.vendorQuotation.exportHeaderText = this._translate.transform("VENDOR_QUOTATION");
    var _url = "downloadVendorQuotation";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: this._translate.transform("VENDOR_QUOTATION"),
      _request: this.vendorQuotation,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }
  onClickStatus(status, shortList) {
    this.selectedStatus = {};
    if (status != "ALL") {
      this.selectedStatus = this.statuses.find(a => a.code == status);
      this.allHighlight = false;
      if (status == "PURTRNSTSAPPROVED" && !shortList) {
        this.pendHighlight = false;
        this.completedHighlight = true;
        this.allHighlight = false;
        this.rejectHighlight = false;
        this.shortListHighlight = false;
        this.vendorQuotation.isApproved = "true";
      }
      if (status == "PURTRNSTSSUBMITTED" && !shortList) {
        this.pendHighlight = true;
        this.completedHighlight = false;
        this.allHighlight = false;
        this.rejectHighlight = false;
        this.shortListHighlight = false;
        this.vendorQuotation.isApproved = "false";
      }
      if (status == "PURTRNSTSREJECTED" && !shortList) {
        this.pendHighlight = false;
        this.completedHighlight = false;
        this.allHighlight = false;
        this.rejectHighlight = true;
        this.shortListHighlight = false;
        this.vendorQuotation.isApproved = "false";
      }
      if (status == "PURTRNSTSSUBMITTED" && shortList) {
        this.pendHighlight = false;
        this.completedHighlight = false;
        this.allHighlight = false;
        this.rejectHighlight = false;
        this.shortListHighlight = true;
        this.vendorQuotation.isApproved = "true";
      }
    }
    else {
      this.allHighlight = true;
      this.pendHighlight = false;
      this.completedHighlight = false;
      this.rejectHighlight = false;
      this.shortListHighlight = false;
      this.vendorQuotation.isApproved = "";
      this.selectedStatus = this.statuses.find(a => a.code == "");
    }
    this.vendorQuotation.status = this.selectedStatus.code;
    this.getAllVendorQuotation();
  }
  async getSearch() {
    this.vendorQuotation.status = this.selectedStatus.code;
    if (this.vendorQuotation.status == "PURTRNSTSSUBMITTED") {
      this.allHighlight = false;
      this.pendHighlight = true;
      this.completedHighlight = false;
      this.rejectHighlight = false;
      this.shortListHighlight = false;

    }
    if (this.vendorQuotation.status == "PURTRNSTSAPPROVED") {
      this.allHighlight = false;
      this.pendHighlight = false;
      this.completedHighlight = true;
      this.rejectHighlight = false;
      this.shortListHighlight = false;

    }
    if (this.vendorQuotation.status == "PURTRNSTSREJECTED") {
      this.allHighlight = false;
      this.pendHighlight = false;
      this.completedHighlight = false;
      this.rejectHighlight = true;
      this.shortListHighlight = false;

    }
    await this.getAllVendorQuotation();
  }


  async getAllVendorQuotation() {
    var result = await this._webApiservice.post("getVendorQuotationListByActive", this.vendorQuotation);
    this.vendorQuotationModel = result;
    if (this.vendorQuotationModel && this.vendorQuotationModel.length > 0) {
      this.disableExport = false;

    }
    else {
      this.disableExport = true;
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
      if (this.selectedTrans.status == "PURTRNSTSAPPROVED") {
        this.selectedTrans.isApproved = true;
      }
      if (this.selectedTrans.status == "PURTRNSTSREJECTED") {
        this.selectedTrans.isApproved = false;
      }
      var result = await this._webApiservice.post("approveVendorQuotation", this.selectedTrans);
      if (result) {
        var output = result as any;
        if (output.validations == null) {
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getAllVendorQuotation();
            this.showApproveReject = false;
            this.showApproveButton = false;
            this.showRejectButton = false;
          }
          else {
            this._toastrService.error(output.messages[0]);
            this.showApproveReject = false;
          }
        }
        else {
          this._toastrService.error(output.messages[0]);
          this.showApproveReject = false;
        }
      }

    }

  }

  getGridVendorQuoDetContextMenu(item) {
    this.gridVendorQuoDetContextMenu = [];
    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "VENDORQUODET", true) } };
    let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showVendorQuoDetHistoryInfo(item) } };
    this.gridVendorQuoDetContextMenu.push(attach);
    this.gridVendorQuoDetContextMenu.push(history);
  }
  async showVendorQuoDetHistoryInfo(item) {
    this.showVendorHistory = false;
    this.showVendorDetHistory = true;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      appDocument: []
    }
    this.loadVendorQuoDetComments();
    this.loadVendorQuoDetDocuments();
    this.loadVendorQuoDetHistory();
  }
  async loadVendorQuoDetDocuments() {
    var result = await this._webApiservice.get("getVendorQuoDetAttachments/" + this.historyInfo.selectedItem.Id);
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

  async loadVendorQuoDetHistory() {
    var result = await this._webApiservice.get("getVendorQuoDetHistory/" + this.historyInfo.selectedItem.Id);
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

  async saveVendorQuoDetComment() {
    if (this.userComment.comments == "")
      return;

    if (this.uploadConfig && this.uploadConfig.FileContent)
      this.userComment.appDocuments = this.uploadConfig.FileContent

    this.userComment.vendorQuotationDetId = this.historyInfo.selectedItem.Id;
    this.userComment.active = "Y";
    var result = await this._webApiservice.post("saveVendorQuotationDetComment", this.userComment);
    if (result) {
      this.loadVendorQuoDetComments();
      this.loadVendorQuoDetDocuments();
      this.userComment.comments = "";
      this.uploadConfig.FileContent = [];

    }
  }

  async loadVendorQuoDetComments() {
    this.historyInfo.comments = [];
    var result = await this._webApiservice.get("getVendorQuoDetComment/" + this.historyInfo.selectedItem.Id);

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



  getGridDetailsContextMenu(item) {
    this.gridDetailsContextMenu = [];
    if (item.status != "PURTRNSTSAPPROVED" && this.isGranted('SCR_VENDOR_QUOT', this.actionType.allowEdit)) {
      if (this.isGranted('SCR_VENDOR_QUOT', this.actionType.allowEdit)) {
        let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditVendorQuotation(item.id) } };
        this.gridDetailsContextMenu.push(edit);
      }
      if (this.isGranted('SCR_VENDOR_QUOT', this.actionType.allowDelete)) {
        let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markVendorQuotationInactive(item) } };
        this.gridDetailsContextMenu.push(Delete);
      }
    }
    if (item.status == "PURTRNSTSAPPROVED" && this.isGranted('SCR_PURCHASE_REQ', this.actionType.allowAdd)) {
      let create: MenuItem = { label: this._translate.transform("CREATE_PURCHASEREQ"), icon: 'pi pi-plus', command: (event) => { this.CreatePurchaseReq(item) } };
      this.gridDetailsContextMenu.push(create);
    }
    if (item.isApproved == true && item.status != "PURTRNSTSAPPROVED") {
      if (this.isGranted('SCR_VENDOR_QUOT', this.actionType.allowApprove)) {
        let approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRejectWindow(item, 'APPROVE') } };
        this.gridDetailsContextMenu.push(approve);
      }
      if (this.isGranted('SCR_VENDOR_QUOT', this.actionType.allowApprove)) {
        let reject: MenuItem = { label: this._translate.transform("APP_REJECT"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRejectWindow(item, 'REJECT') } };
        this.gridDetailsContextMenu.push(reject);
      }
      if (this.isGranted('SCR_VENDOR_QUOT', this.actionType.allowApprove)) {
        let unShortList: MenuItem = { label: this._translate.transform("UNSHORT_LIST"), icon: 'pi pi-thumbs-down', command: (event) => { this.shortOrUnshortListQuotation(item) } };
        this.gridDetailsContextMenu.push(unShortList);
      }
    }
    if (item.isApproved == false && item.status == "PURTRNSTSSUBMITTED") {
      if (this.isGranted('SCR_VENDOR_QUOT', this.actionType.allowApprove)) {
        let shortList: MenuItem = { label: this._translate.transform("SHORT_LIST"), icon: 'pi pi-thumbs-up', command: (event) => { this.shortOrUnshortListQuotation(item) } };
        this.gridDetailsContextMenu.push(shortList);
      }
    }

    let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "VENDORQUOTATION", true) } };
    let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showVendorHistoryInfo(item) } };
    this.gridDetailsContextMenu.push(attach);
    this.gridDetailsContextMenu.push(history);

  }
  async CreatePurchaseReq(item) {
    this.router.navigate(["purchase/create-edit-purchase-request"], {
      state: { transNo: item.transNo },
    });
  }
  async showVendorHistoryInfo(item) {
    this.showVendorHistory = true;
    this.showVendorDetHistory = false;
    this.historyInfo = {
      selectedItem: item,
      comments: [],
      history: [],
      appDocument: [],
      statusHistory: []
    }
    this.loadVendorQuotationComments();
    this.loadVendorQuotationDocuments();
    this.loadVendorQuotationStatusHistory();
    this.loadVendorQuotationHistory();
  }
  async saveVendorQuotationComment() {
    if (this.userComment.comments == "")
      return;

    if (this.uploadConfig && this.uploadConfig.FileContent)
      this.userComment.appDocuments = this.uploadConfig.FileContent;

    this.userComment.vendorQuotationId = this.historyInfo.selectedItem.id;
    this.userComment.active = "Y";
    var result = await this._webApiservice.post("saveVendorQuotationComment", this.userComment);
    if (result) {
      this.loadVendorQuotationComments();
      this.loadVendorQuotationDocuments();
      this.userComment.comments = "";
      this.uploadConfig.FileContent = [];

    }
  }

  async loadVendorQuotationComments() {
    this.historyInfo.comments = [];

    var result = await this._webApiservice.get("getVendorQuotationComment/" + this.historyInfo.selectedItem.id);
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

  async loadVendorQuotationDocuments() {

    var result = await this._webApiservice.get("getVendorQuotationAttachments/" + this.historyInfo.selectedItem.id);
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

  async loadVendorQuotationStatusHistory() {
    if (!this.historyInfo.statusHistory || this.historyInfo.statusHistory.length == 0) {
      var result = await this._webApiservice.get("getVendorQuotationStatusHistory/" + this.historyInfo.selectedItem.id);
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

  async loadVendorQuotationHistory() {
    if (!this.historyInfo.history || this.historyInfo.history.length == 0) {
      var result = await this._webApiservice.get("getVendorQuotationHistory/" + this.historyInfo.selectedItem.id);
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
  async markVendorQuotationInactive(item) {
    this._confirmService.confirm({
      message: this._translate.transform("TRANSNO_FOR") + item.transNo + "<br>" + this._translate.transform("VENDOR_QUOTATION_DELETE_CONF"),
      key: 'vendorQuotationDelete',
      accept: async () => {
        item.active = "N";


        var result = await this._webApiservice.post("saveVendorQuotation", item)
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getAllVendorQuotation()
          }
          else {
            console.log(output.messages[0]);
            this._toastrService.error(output.messages[0])
            this.getAllVendorQuotation();
          }
        }
      }
    });
  }
  async shortOrUnshortListQuotation(item) {
    if (item.isApproved == true) {
      this._confirmService.confirm({
        message: this._translate.transform("DO_YOU_WENT_TO_UNSHOTLIST_QUOTATION"),
        key: 'shortOrUnshortListQuotation',
        accept: async () => {
          item.isApproved = false;


          var result = await this._webApiservice.post("saveVendorQuotation", item)
          if (result) {
            var output = result as any;
            if (output.status == "DATASAVESUCSS") {
              this._toastrService.success(this._translate.transform("APP_SUCCESS"));
              this.getAllVendorQuotation()
            }
            else {
              console.log(output.messages[0]);
              this._toastrService.error(output.messages[0])
              this.getAllVendorQuotation();
            }
          }
        }
      });
    }
    else {
      this._confirmService.confirm({
        message: this._translate.transform("DO_YOU_WENT_TO_SHOTLIST_QUOTATION"),
        key: 'shortOrUnshortListQuotation',
        accept: async () => {
          item.isApproved = true;


          var result = await this._webApiservice.post("saveVendorQuotation", item)
          if (result) {
            var output = result as any;
            if (output.status == "DATASAVESUCSS") {
              this._toastrService.success(this._translate.transform("APP_SUCCESS"));
              this.getAllVendorQuotation()
            }
            else {
              console.log(output.messages[0]);
              this._toastrService.error(output.messages[0])
              this.getAllVendorQuotation();
            }
          }
        }
      });
    }

  }
  async createOreditVendorQuotation(data?: any) {
    console.log(data);
    this._router.navigate(["purchase/create-edit-vendor-quotation"], {
      state: { vendorQuotationId: data },
    });
  }

  async listExpentation(event) {
    this.vendorQuotationDetModel = [];


    var result = await this._webApiservice.get("GetVendorQuotationDetList/" + event.data.id);
    result.forEach(element => {
      this.vendorQuotationDet = {
        Id: element.id,
        proddescription: element.productDetail.prodDescription,

        unitName: element.prodUnit.unitName,
        quantity: element.quantity,
        price: element.price,
        amount: element.amount,
        remarks: element.remarks
      };
      this.vendorQuotationDetModel.push(this.vendorQuotationDet)

    });
  }

  addVendorQuote() {
    this._router.navigate(["purchase/create-edit-vendor-quotation"], {
      state: { vendorQuotationId: "" },
    });
  }


  sidenavClosed() {
    this.index = this.lastIndex;
    this.addoredit = false;
  }

  clearSearchCriteria() {
    this.vendorQuotation = {
      id: '',
      quotationRequestId: '',
      vendorMasterId: '',
      transNo: '',
      transDate: '',
      quotationNo: '',
      quotationdDate: '',
      isApproved: '',
      remarks: '',
      other: ''
    }

  }

}
