import { DatePipe } from '@angular/common';
import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { PurchaseService } from 'app/modules/purchase/purchase.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-create-edit-purchase-request',
  templateUrl: './create-edit-purchase-request.component.html',
  styleUrls: ['./create-edit-purchase-request.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService]
})
export class CreateEditPurchaseRequestComponent extends AppComponentBase implements OnInit {
  router: Router;
  puchaseReq: any;
  prevPuchaseReq: any;
  vendorMaster: any = [];
  products: any = [];
  units: any = [];
  vendorQuotation: any = [];

  selectedVendorQuotation: any;
  selectedVendorMaster: any;
  selectedVendor: any;
  selectedProduct: any;
  amount: any;
  totelAmount: any;
  transNo: any;
  lang: any;
  CatgoryId: string;
  purchaseReqTotelQuantity: any;
  purchaseReqTotelAmount: any;

  constructor(
    injector: Injector,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _toastrService: ToastrService,
    private _primengConfig: PrimeNGConfig,
    private _purchaseService: PurchaseService,
    private _confirmService: ConfirmationService,
  ) {
    super(injector, 'SCR_PURCHASE_REQ', 'allowAdd', _commonService);
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this._primengConfig.ripple = true;
  }

  ngOnInit(): void {
    this.selectedVendor = {
      name: ""
    };
    this.puchaseReq = {
      id: '',
      vendorQuotationId: '',
      vendorMasterId: '',
      transDate: new Date(),
      status: '',
      remarks: '',
      transNo: '',
      seqNo:'',
      purchaseRequestDetList: []
    };

    this.loadDefaults();
    this.purchaseReqTotelAmount = 0;
    this.purchaseReqTotelQuantity = 0;

  }
  async loadDefaults() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let getTransactionId = history.state.reqId
      this.transNo = history.state.transNo;
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.puchaseReq.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("PR_PURCHASE_REQUEST_EDIT"));
      }
      else {
        this._commonService.updatePageName(this._translate.transform("PR_PURCHASE_REQUEST_ADD"));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("PR_PURCHASE_REQUEST_ADD"));
    }
    await this.getAll();
    if (this.transNo) {
      this.selectedVendorQuotation = this.transNo;
      this.loadVendorQuotation(this.transNo);
    }
  }

  async getAll() {
    var searchInput = {
      isApproved: "True",
      status: "PURTRNSTSAPPROVED"
    };

    this.vendorQuotation = await this._webApiService.post("getVendorQuotationListByActive", searchInput);
    this.vendorMaster = await this._purchaseService.getVendorMasters(false, true, this._translate, true);
    this.units = await this._purchaseService.getProdUnitMaster(false, true, this._translate, true);
    this.products = await this._purchaseService.getProductMaster(false, true, this._translate, this.CatgoryId = "", true, false);

    if (this.puchaseReq.id && this.puchaseReq.id != '') {
      var result = await this._webApiService.get("getPurchaseRequestById/" + this.puchaseReq.id);
      this.prevPuchaseReq = result as any;
      var vendorQuotationTransNo = this.vendorQuotation.find(a => a.id == this.prevPuchaseReq.vendorQuotationId);
      this.selectedVendorQuotation = vendorQuotationTransNo.transNo;

      this.selectedVendor = this.vendorMaster.find(a => a.id == this.prevPuchaseReq.vendorMasterId);
      this.puchaseReq = {
        id: this.prevPuchaseReq.id,
        vendorQuotationId: vendorQuotationTransNo.id,
        vendorMasterId: this.selectedVendor.id,
        transDate: this.prevPuchaseReq.transDate,
        status: this.prevPuchaseReq.status,
        remarks: this.prevPuchaseReq.remarks,
        transNo: this.prevPuchaseReq.transNo,
        seqNo:this.prevPuchaseReq.seqNo,
        purchaseRequestDetList: []
      };
      this.prevPuchaseReq.purchaseRequestDetList.forEach(element => {
        var product = this.products.find(a => a.id == element.productMasterId);
        var unit = this.units.find(a => a.id == element.unitMasterId);
        var det = element;
        det.selectedProduct = product;
        det.selectedUnit = unit;
        this.puchaseReq.purchaseRequestDetList.push(det);
      });
      await this.totelAmountAndQuantityPurchaseReq();

    }
  }
  async totelAmountAndQuantityPurchaseReq() {
    this.purchaseReqTotelAmount = 0;
    this.purchaseReqTotelQuantity = 0;
    if (this.puchaseReq.purchaseRequestDetList) {
      this.puchaseReq.purchaseRequestDetList.forEach(element => {
        this.purchaseReqTotelAmount += element.amount;
        this.purchaseReqTotelQuantity += element.quantity;
      })
    }

  }

  async createOredit() {
    if (!this.selectedVendor || this.selectedVendor.id == "") {
      this._toastrService.error(this._translate.transform("SR_VENDORMASTER_SELECT"));
      return;
    }

    if (!this.puchaseReq.purchaseRequestDetList || this.puchaseReq.purchaseRequestDetList.length == 0) {
      this._toastrService.error(this._translate.transform("SR_MINIMUM_PRODUCT"));
      return;
    }

    var validattion = true;
    this.puchaseReq.purchaseRequestDetList.forEach((element, eleIdx) => {
      if (!element.selectedProduct || !element.selectedProduct.id) {
        this._toastrService.error(this._translate.transform("SR_PRODUCT_SELECT"));
        validattion = false;;
      }
      if (!element.selectedUnit || !element.selectedUnit.id) {
        this._toastrService.error(this._translate.transform("SR_UNIT_SELECT"));
        validattion = false;;
      }
      if (!element.price || Number(element.price) == 0) {
        this._toastrService.error(this._translate.transform("PR_PROVIDE_PRICE"));
        validattion = false;;
      }
      if (element.quantity == "" || Number(element.quantity) == 0) {
        this._toastrService.error(this._translate.transform("PR_PROVIDE_QUANTITY"));
        validattion = false;;
      }
      if (element.amount == "" || Number(element.amount) == 0) {
        this._toastrService.error(this._translate.transform("PR_PROVIDE_AMOUNT"));
        validattion = false;;
      }

      this.puchaseReq.purchaseRequestDetList.forEach((duplicate, dupIdx) => {
        if (dupIdx != eleIdx && duplicate.selectedProduct.id == element.selectedProduct.id) {
          this._toastrService.error(this._translate.transform("SR_DUPLICATE_PRODUCT"));
          validattion = false;
        }
      });
      element.productMasterId = element.selectedProduct.id;
      element.unitMasterId = element.selectedUnit.id;
    });

    if (!validattion)
      return;

    if (this.puchaseReq.id && this.puchaseReq.id != "") {
      this.puchaseReq.action = 'M';
      this.puchaseReq.modifiedDate = this.prevPuchaseReq.modifiedDate;
      this.puchaseReq.modifiedBy = this.prevPuchaseReq.modifiedBy;

      this.puchaseReq.purchaseRequestDetList.forEach(element => {
        var det = this.puchaseReq.purchaseRequestDetList.find(a => a.id == element.id);
        if (det) {
          det.modifiedDate = element.modifiedDate;
          det.action = "M"
        }
        else {
          element.active = "N";
          element.action = "M";
          this.puchaseReq.purchaseRequestDetList.push(element);
        }

      });

    }
    else
      this.puchaseReq.action = 'N';
    this.puchaseReq.status = 'PURTRNSTSSUBMITTED';

    this.puchaseReq.active = 'Y';
    this.puchaseReq.vendorMasterId = this.selectedVendor.id;
    var result = await this._webApiService.post("savePurchaseRequest", this.puchaseReq)
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.router.navigateByUrl("purchase/purchase-request");
      }
      else {
        console.log(output.messages[0]);
        this._toastrService.error(output.messages[0])
      }
    }
  }
  async loadVendorQuotation(event) {
    if (!this.selectedVendorQuotation) {
      this._toastrService.error(this._translate.transform("PR_ENTER_VENDORQUOTATION"));
      return;
    }
    else

      this.puchaseReq.purchaseRequestDetList = [];
    this.selectedVendor = {
      name: ""
    };
    if (this.selectedVendorQuotation) {
      var po = this.vendorQuotation.find(a => a.transNo == this.selectedVendorQuotation);

      if (po != null || po != undefined) {
        this.puchaseReq.vendorQuotationId = po.id;
        var result = await this._webApiService.get("getVendorQuotationDetById/" + po.id);
        if (result) {
          var poDet = result as any;
          poDet.forEach(element => {

            var product = this.products.find(a => a.id == element.productMasterId);
            var unit = this.units.find(a => a.id == element.unitMasterId);

            this.puchaseReq.purchaseRequestDetList.push({
              selectedProduct: product,
              selectedUnit: unit,
              selectedWhLocation: {},
              quantity: element.quantity,
              price: element.price,
              amount: element.amount,
              remarks: element.remarks,
              active: "Y"
            });

          });
          await this.totelAmountAndQuantityPurchaseReq();
        }
        this.selectedVendor = this.vendorMaster.find(a => a.id == po.vendorMasterId)
      }
      else {
        this._toastrService.error(this._translate.transform("VENDORQUOTATION_NO_REC"));
      }

    }
    else
      this._toastrService.error(this._translate.transform("VENDORQUOTATION_NO_REC"));
  }

  async calculateAmount(event, item) {
    item.amount = item.price * item.quantity;
    await this.totelAmountAndQuantityPurchaseReq();
  }

  cancelAddEdit() {
    this.router.navigateByUrl("purchase/purchase-request");
  }


  addNewPurchaseReqDet() {

    if (!this.selectedVendorQuotation || this.selectedVendorQuotation.id == "") {
      this._toastrService.error(this._translate.transform("PR_ENTER_VENDORQUOTATION"));
      return;
    }
    var reqDet =
    {
      productMasterId: '',
      unitMasterId: '',
      selectedProduct: {},
      selectedUnit: {},
      price: '',
      quantity: '',
      amount: '',
      remarks: '',
      id: '',
      active: "Y"
    }
    if (!this.puchaseReq.purchaseRequestDetList)
      this.puchaseReq.purchaseRequestDetList = [];
    this.puchaseReq.purchaseRequestDetList.unshift(reqDet);
  }


  deleteReqDet(det) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.puchaseReq.purchaseRequestDetList.indexOf(det);
        if (index >= 0)
          this.puchaseReq.purchaseRequestDetList.splice(index, 1);
        await this.totelAmountAndQuantityPurchaseReq();
      }
    });
  }
  async onChangeProduct(item) {
    var unit = this.units.find(a => a.id == item.selectedProduct.defaultUnitId);
    item.selectedUnit = unit;
  }
}