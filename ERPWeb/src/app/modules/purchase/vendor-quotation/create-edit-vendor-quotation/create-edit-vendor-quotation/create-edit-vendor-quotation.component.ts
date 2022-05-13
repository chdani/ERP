import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { takeUntil } from 'rxjs/operators';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePipe } from '@angular/common';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';

import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs/operators';
import { PurchaseService } from 'app/modules/purchase/purchase.service';


@Component({
  selector: 'app-create-edit-vendor-quotation',
  templateUrl: './create-edit-vendor-quotation.component.html',
  styleUrls: ['./create-edit-vendor-quotation.component.scss']
})
export class CreateEditVendorQuotationComponent extends AppComponentBase implements OnInit, OnDestroy {


  quotationRequestDet: any = [];
  vendaorData: any = [];
  vendaorData1: any = [];
  vendaorArray: any = [];

  public quotationRequestNo: string;
  public quotationTransDate: any;
  gridDetailsContextMenu: MenuItem[] = [];
  public vendor: any;
  quotationRequestDataByTransNo: any = [];
  quotationDate: any;
  id: any;

  vendorQuotation: any;
  selectTransNo: any;
  transNo: any;
  vendorQuoTotelQuantity: any;
  vendorName: any;
  vendorQuoTotelAmount: any;
  allowSave: boolean = true;



  public activeTabIndex: any;
  public invDetTitle: string = "";

  allowDeletePermission: boolean = true;
  lang: any;
  dialogRef: DynamicDialogRef;
  selectedUploadData: any = {};
  header: any = [];
  uploadConfig: FileUploadConfig;


  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _purchaseService: PurchaseService,
    private _toastrService: ToastrService,


    private _confirmService: ConfirmationService,
    private dialogService: DialogService
  ) {
    super(injector, 'SCR_VENDOR_QUOT', 'allowAdd', _commonService);
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;

    this._commonService.fileObserver.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((file: any) => {
        this.selectedUploadData.appDocuments = file;
      });
  }

  ngOnInit(): void {
    if (history.state && history.state.vendorQuotationId && history.state.vendorQuotationId != '') {
      this.id = history.state.vendorQuotationId;
    }
    this.loadDefaults();
    this.transNo = history.state.transNo;
    if (this.transNo) {
      this.selectTransNo = this.transNo;
      this.filterQuotationRequest(this.selectTransNo);
    }
    this.vendorQuoTotelQuantity = 0;
    this.vendorQuoTotelAmount = 0;

    this.vendorQuotation = {
      id: '',
      quotationRequestId: '',
      vendorMasterId: '',
      transNo: '',
      transDate: new Date(),
      quotationNo: '',
      quotationdDate: '',
      isApproved: '',
      status: '',
      remarks: '',
      vendaorData: [],
      vendorquotationDets: [],
      quotationRequest: null,
      quotationReqDet: [],
      quotaReqVendorDets: []

    }


  }



  searchVendaorData(event): void {
    if (this.vendaorData != null) {
      this.vendaorArray = this.vendaorData.filter(c => c.name.toUpperCase().startsWith(event.query.toUpperCase()));
    }
    else {
      this.vendaorArray = [];
      this._toastrService.error(this._translate.transform("QUOTATIONREQ_NO_REC"));
    }

  }
  async filterQuotationRequest(event) {
    if (this.selectTransNo != null) {
      this.quotationRequestDataByTransNo = await this._webApiService.get("getQuotationRequestByTrans/" + this.selectTransNo);

      if (this.quotationRequestDataByTransNo != null) {

        this.vendorQuotation.quotationRequestId = this.quotationRequestDataByTransNo.id;

        this.vendorQuotation.quotationReqDet = this.quotationRequestDataByTransNo.quotationReqDet;
        this.vendorQuotation.quotaReqVendorDets = this.quotationRequestDataByTransNo.quotaReqVendorDets;
        if (this.vendorQuotation.quotaReqVendorDets.length > 0) {
          await this.pushVendorData();
        }
        await this.totelQuantityAndAmountVenQuoDet();
      }
      else {
        this.vendaorData = [];
        this.vendorQuotation.quotationReqDet = [];
        this._toastrService.error(this._translate.transform("QUOTATIONREQ_NO_REC"));
      }
    }
    else {
      this.vendaorData = [];
      this.vendorQuotation.quotationReqDet = [];
      this._toastrService.error(this._translate.transform("QUOTATIONREQ_PROVIDE_NO"));
    }
  }
  async pushVendorData() {
    this.vendaorData = [];
    this.vendorQuotation.quotaReqVendorDets.forEach(element => {
      var vendaorFilterData = this.vendaorData1.find(a => a.id == element.vendorMasterId);
      if (vendaorFilterData != null) {
        this.vendaorData.push(vendaorFilterData);
      }


    });
  }



  async loadDefaults() {
    this.vendaorData1 = await this._purchaseService.getVendorMaster(false, true, this._translate, true);
    if (history.state != null && history.state != undefined && history.state != '') {
      let vendorQuotationId = history.state.vendorQuotationId;
      this.vendorQuotation.Id = history.state.vendorQuotationId;
      if (vendorQuotationId != null && vendorQuotationId != undefined && vendorQuotationId != '') {
        console.log('ID' + this.vendorQuotation.Id);
        this._commonService.updatePageName(this._translate.transform("VENDOR_QUOTE_EDIT"));
        this.vendorQuotation = await this._webApiService.get("getVendorQuotationById/" + this.vendorQuotation.Id);
        this.vendorQuotation.quotationdDate = new Date(this.vendorQuotation.quotationdDate);
        this.vendorQuotation.quotationRequestId = this.vendorQuotation.quotationRequestId;
        this.vendorName = this.vendaorData1.find(a => a.id == this.vendorQuotation.vendorMasterId);
        this.selectTransNo = this.vendorQuotation.quotationRequest.transNo;
        this.vendorQuotation.quotationReqDet = this.vendorQuotation.vendorquotationDets;
        this.vendorQuotation.quotaReqVendorDets = this.vendorQuotation.quotationReqVendordet;
        await this.totelQuantityAndAmountVenQuoDet();
        await this.pushVendorData();

      }
      else {
        this._commonService.updatePageName(this._translate.transform("VENDOR_QUOTE_ADD"));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("VENDOR_QUOTE_EDIT"));
    }

  }
  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  async createOredit() {
    this.allowSave = true;
    this.vendorQuotation.Id = history.state.vendorQuotationId;
    if (this.selectTransNo == "") {
      this.allowSave = false;
      this._toastrService.error(this._translate.transform("VENDOR_QUOTATION_REQUEST_NO_REQ"));
      return;
    }

    if (!this.vendorName || this.vendorName.length == "") {
      this.allowSave = false;
      this._toastrService.error(this._translate.transform("VENDOR_QUOTATION_VENDOR_REQ"));
      return;
    }
    if (!this.vendorQuotation.quotationNo || this.vendorQuotation.quotationNo == "") {
      this.allowSave = false;
      this._toastrService.error(this._translate.transform("VENDOR_QUOTATION_NO_REQ"));
      return;
    }

    if (!this.vendorQuotation.quotationdDate || this.vendorQuotation.quotationdDate == "") {
      this.allowSave = false;
      this._toastrService.error(this._translate.transform("VENDOR_QUOTATION_DATE_REQ"));
      return;
    }
    this.vendorQuotation.vendorquotationDets = this.vendorQuotation.quotationReqDet;
    this.vendorQuotation.vendorquotationDets.forEach(vendorQuoDet => {
      if (vendorQuoDet.price == "" || vendorQuoDet.price == null) {
        this.allowSave = false;
        this._toastrService.error(this._translate.transform("VENDOR_QUOTATION_DET_PROVIDE"));
        return;
      }
      if (vendorQuoDet.quantity == "" || vendorQuoDet.quantity == null) {
        this.allowSave = false;
        this._toastrService.error(this._translate.transform("VENDOR_QUOTATION_DET_PROVIDE"));
        return;
      }
    });
    this.vendorQuotation.vendorMasterId = this.vendorName.id;
    this.vendorQuotation.status = "PURTRNSTSSUBMITTED";
    await this.saveVendorQuotation();

  }
  attachmentLink() {
    if (this.isGranted('SCR_VENDOR_QUOT', this.actionType.allowAdd)) {
      this.allowDeletePermission = false;
    }
    this.header = this._translate.transform("FILE_ATTACHMENT");
    this.selectedUploadData = this.vendorQuotation;
    this.uploadConfig =
    {
      TransactionId: this.vendorQuotation.id,
      TransactionType: 'VENDORQUOTATION',
      AllowedExtns: ".png,.jpg,.gif,.jpeg,.bmp,.docx,.doc,.pdf,.msg",
      DocTypeRequired: false,
      DocumentReference: "",
      ReadOnly: this.allowDeletePermission,
      ScanEnabled: true,
      ShowSaveButton: false,
      FileContent: this.vendorQuotation.appDocuments
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

  async saveVendorQuotation() {
    if (this.allowSave) {
      var result = await this._webApiService.post("saveVendorQuotation", this.vendorQuotation)
      if (result) {
        var output = result as any;
        if (output.status == "DATASAVESUCSS") {
          this._toastrService.success(this._translate.transform("APP_SUCCESS"));
          this.router.navigateByUrl("purchase/vendor-quotation");
        }
        else {
          console.log(output.messages[0]);
          this._toastrService.error(output.messages[0])
        }
      }
    }
  }
  async calculateAmount(event, item) {
    item.amount = item.price * item.quantity;
    await this.totelQuantityAndAmountVenQuoDet();
  }

  deleteVendorQuotationDet(item) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.vendorQuotation.quotationReqDet.indexOf(item);
        this.vendorQuotation.quotationReqDet.splice(index, 1);
        await this.totelQuantityAndAmountVenQuoDet();
      }
    });
  }

  async totelQuantityAndAmountVenQuoDet() {
    this.vendorQuoTotelQuantity = 0;
    this.vendorQuoTotelAmount = 0;
    if (this.vendorQuotation.quotationReqDet) {
      this.vendorQuotation.quotationReqDet.forEach(element => {
        this.vendorQuoTotelQuantity += element.quantity;
        this.vendorQuoTotelAmount += element.amount;
      })
    }
  }

  cancelAddEdit() {
    this.router.navigateByUrl("purchase/vendor-quotation");
  }

  calculateCurrAmount(event, item) {
    item.currencyAmount = item.amount * item.currencyRate;
  }


  markVendorQuotationDetInactive(id) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.vendaorData.vendorQuotationDet.indexOf(id);
        if (index >= 0)
          this.vendaorData.splice(index, 1);
      }
    });
  }
}
