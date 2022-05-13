import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { PurchaseService } from '../../purchase.service';

@Component({
  selector: 'addEdit-goods-receipt-notes',
  templateUrl: './addEdit-goods-receipt-notes.component.html',
  styleUrls: ['./addEdit-goods-receipt-notes.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService]
})

export class AddEditGRNComponent extends AppComponentBase implements OnInit, OnDestroy {
  grnInfo: any;
  prevGrnInfo: any;
  whLocations: any = [];
  products: any = [];
  units: any = [];
  vendors: any = [];
  approvedPOs: any[];

  selectedWhLocation: any;
  selectedProduct: any;
  selectedUnit: any;
  selectedVendor: any;
  lang: any;
  transNo: any;
  prevPoNo: any;
  goodsReceiptTotelQuantity: any;
  goodsReceiptTotelAmount: any;
  disabledAddButtonGoodsReceiptNoteDet: boolean = false;

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
    super(injector, 'SCR_G_R_NOTES', 'allowAdd', _commonService);
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this._primengConfig.ripple = true;
  }

  ngOnInit(): void {
    this.grnInfo = {
      id: '',
      prodCategoryId: '',
      employeeId: '',
      remarks: '',
      curApprovalLevel: '',
      nextApprovalLevel: '',
      transNo: '',
      seqNo: '',
      transDate: new Date(),
      servRequestDet: []
    };
    this.selectedVendor = {
      name: ""
    };
    this.loadDefaults();
    this.goodsReceiptTotelQuantity = 0;
    this.goodsReceiptTotelAmount = 0;


  }
  async loadDefaults() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let getTransactionId = history.state.reqId
      this.transNo = history.state.transNo;

      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.grnInfo.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("GRN_EDIT"));
      }
      else {
        this._commonService.updatePageName(this._translate.transform("GRN_ADD"));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("GRN_ADD"));
    }
    await this.getAll();
    if (this.transNo) {
      this.grnInfo.poNo = this.transNo;
      this.loadVendor(this.transNo);
    }
  }

  deleteReqDet(det) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.grnInfo.goodsReceiptNoteDet.indexOf(det);
        if (index >= 0)
          this.grnInfo.goodsReceiptNoteDet.splice(index, 1);
        await this.totalQuantityAndAmountgoodsReceipt();
      }
    });
  }
  async totalQuantityAndAmountgoodsReceipt() {
    this.goodsReceiptTotelQuantity = 0;
    this.goodsReceiptTotelAmount = 0;
    if (this.grnInfo.goodsReceiptNoteDet) {
      this.grnInfo.goodsReceiptNoteDet.forEach(element => {
        this.goodsReceiptTotelQuantity += element.quantity;
        this.goodsReceiptTotelAmount += element.amount;
      })
    }
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  async getAll() {
    this.whLocations = await this._purchaseService.getWarehouseAndLocation(false, true, this._translate, true);
    this.units = await this._purchaseService.getProdUnitMaster(false, true, this._translate, true);
    this.products = await this._purchaseService.getProductMaster(false, true, this._translate, "", true, true);
    this.vendors = await this._purchaseService.getVendorMaster(false, true, this._translate, true);

    var searchInput = {
      Status: "PURTRNSTSAPPROVED"
    };

    this.approvedPOs = await this._webApiService.post("getPurchaseOrderList", searchInput)

    if (this.grnInfo.id && this.grnInfo.id != '') {
      var result = await this._webApiService.get("getGoodsRecNoteById/" + this.grnInfo.id);
      if (result) {
        this.prevGrnInfo = result as any;
        if (this.prevGrnInfo.validations == null) {
          var podetail = await this._webApiService.get("getPurchaseOrderById/" + this.prevGrnInfo.purchaseOrderId) as any;
          this.selectedVendor = this.vendors.find(a => a.id == podetail.vendorMasterId);
          this.selectedWhLocation = this.whLocations.find(a => a.id == this.prevGrnInfo.wareHouseLocationId);
          this.grnInfo = {
            id: this.prevGrnInfo.id,
            poNo: podetail?.transNo,
            remarks: this.prevGrnInfo.remarks,
            invoiceNo: this.prevGrnInfo.invoiceNo,
            invoiceDate: new Date(this.prevGrnInfo.invoiceDate),
            transNo: this.prevGrnInfo.transNo,
            transDate: this.prevGrnInfo.transDate,
            seqNo: this.prevGrnInfo.seqNo,
            purchaseOrderId: this.prevGrnInfo.purchaseOrderId,
            vendorMasterId: this.prevGrnInfo.vendorMasterId,
            status: this.prevGrnInfo.status,
            wareHouseLocationId: this.prevGrnInfo.wareHouseLocationId,
            goodsReceiptNoteDet: []
          };

          this.goodsReceiptTotelQuantity = 0;
          this.goodsReceiptTotelAmount = 0;

          this.prevGrnInfo.goodsReceiptNoteDet.forEach(element => {
            var product = this.products.find(a => a.id == element.productMasterId);
            var unit = this.units.find(a => a.id == element.unitMasterId);

            var det = JSON.parse(JSON.stringify(element));
            this.goodsReceiptTotelQuantity += element.quantity;
            this.goodsReceiptTotelAmount += element.amount;
            det.selectedProduct = product;
            det.selectedUnit = unit;
            det.shelveNo = element.shelveNo;
            det.barcode = element.barcode;
            this.grnInfo.goodsReceiptNoteDet.push(det);
          });
        }
      }
    }
  }


  async createOredit() {
    if (this.grnInfo.poNo != null)
      var approvedPONO = this.approvedPOs.find(a => a.transNo == this.grnInfo.poNo)
    if (approvedPONO == null) {
      this._toastrService.error(this._translate.transform("GRN_PUR_ORDER_NO_REQ"));
      return;
    }

    if (!this.selectedVendor || this.selectedVendor.code == "") {
      this._toastrService.error(this._translate.transform("SR_CATEGORY_SELECT"));
      return;
    }

    if (this.grnInfo.purchaseOrderId == "") {
      this._toastrService.error(this._translate.transform("GRN_PUR_ORDER_NO_REQ"));
      return;
    }

    if (this.grnInfo.invoiceNo == "") {
      this._toastrService.error(this._translate.transform("GRN_VENDOR_INVNO_REQ"));
      return;
    }

    if (this.grnInfo.invoiceDate == "") {
      this._toastrService.error(this._translate.transform("GRN_VENDOR_INVDATE_REQ"));
      return;
    }

    if (!this.grnInfo.goodsReceiptNoteDet || this.grnInfo.goodsReceiptNoteDet.length == 0) {
      this._toastrService.error(this._translate.transform("SR_MINIMUM_PRODUCT"));
      return;
    }
    if (!this.selectedWhLocation || !this.selectedWhLocation.id) {
      this._toastrService.error(this._translate.transform("PUR_WAREHOUSE_LOCATION_SELECT"));
      return;
    }

    var validattion = true;
    this.grnInfo.goodsReceiptNoteDet.forEach((element, eleIdx) => {
      if (!element.selectedProduct || !element.selectedProduct.id) {
        this._toastrService.error(this._translate.transform("SR_PRODUCT_SELECT"));
        validattion = false;;
      }
      if (!element.selectedUnit || !element.selectedUnit.id) {
        this._toastrService.error(this._translate.transform("SR_UNIT_SELECT"));
        validattion = false;;
      }
      if (element.quantity == "" || Number(element.quantity) == 0) {
        this._toastrService.error(this._translate.transform("SR_PROVIDE_QUANTITY"));
        validattion = false;;
      }
      if (element.price == "" || Number(element.price) == 0) {
        this._toastrService.error(this._translate.transform("GRN_PIRCE_REQ"));
        validattion = false;;
      }

      this.grnInfo.goodsReceiptNoteDet.forEach((duplicate, dupIdx) => {
        if (dupIdx != eleIdx && duplicate.selectedProduct.id == element.selectedProduct.id && duplicate.expiryDate == element.expiryDate) {
          this._toastrService.error(this._translate.transform("SR_DUPLICATE_PRODUCT"));
          validattion = false;
        }
      });
      element.productMasterId = element.selectedProduct.id;
      element.unitMasterId = element.selectedUnit.id;
    });

    if (!validattion)
      return;

    if (this.grnInfo.id && this.grnInfo.id != "") {
      this.grnInfo.action = 'M';
      this.grnInfo.modifiedDate = this.prevGrnInfo.modifiedDate;

      this.prevGrnInfo.goodsReceiptNoteDet.forEach(element => {
        var det = this.grnInfo.goodsReceiptNoteDet.find(a => a.id == element.id);
        if (det) {
          det.modifiedDate = element.modifiedDate;
          det.action = "M"
        }
        else {
          element.active = "N";
          element.action = "M";
          this.grnInfo.goodsReceiptNoteDet.push(element);
        }

      });

    }
    else
      this.grnInfo.action = 'N';

    this.grnInfo.active = 'Y';
    this.grnInfo.wareHouseLocationId = this.selectedWhLocation.id
    //this.grnInfo.vendorMasterId = this.selectedVendor.vendorMasterId;

    var result = await this._webApiService.post("saveGoodsRecNote", this.grnInfo)
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.router.navigateByUrl("purchase/goods-receipt-notes");
      }
      else {
        console.log(output.messages[0]);
        this._toastrService.error(output.messages[0])
      }
    }
  }

  cancelAddEdit() {
    this.router.navigateByUrl("purchase/goods-receipt-notes");
  }


  addNewRecord() {
    var newDet =
    {
      productMasterId: '',
      unitMasterId: '',
      whLocationId: '',
      selectedProduct: {},
      selectedUnit: {},
      selectedWhLocation: {},
      quantity: '',
      price: '',
      amount: '',
      remarks: '',
      id: '',
      active: "Y"
    }
    if (!this.grnInfo.goodsReceiptNoteDet)
      this.grnInfo.goodsReceiptNoteDet = [];
    this.grnInfo.goodsReceiptNoteDet.unshift(newDet);
  }


  deleteDetRecord(det) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.grnInfo.goodsReceiptNoteDet.indexOf(det);
        if (index >= 0)
          this.grnInfo.goodsReceiptNoteDet.splice(index, 1);

      }
    });
  }

  async calculateAmount(event, item) {
    item.amount = item.price * item.quantity;
    await this.totalQuantityAndAmountgoodsReceipt();
  }

  async loadVendor(event) {
    if (this.prevPoNo == this.grnInfo.poNo)
      return;
    else
      this.prevPoNo = this.grnInfo.poNo;

    this.grnInfo.goodsReceiptNoteDet = [];
    this.selectedVendor = {
      name: ""
    };
    if (this.grnInfo.poNo && this.grnInfo.poNo != "") {
      var po = this.approvedPOs.find(a => a.transNo == this.grnInfo.poNo)
      if (po) {
        this.grnInfo.vendorMasterId = po.vendorMasterId;
        this.grnInfo.purchaseOrderId = po.id;
        var result = await this._webApiService.get("getPurchaseOrderDetByHdrId/" + po.id);
        if (result) {
          var poDet = result as any;
          this.disabledAddButtonGoodsReceiptNoteDet = false;
          poDet.forEach(element => {

            var product = this.products.find(a => a.id == element.productMasterId);
            var unit = this.units.find(a => a.id == element.unitMasterId);

            this.grnInfo.goodsReceiptNoteDet.push({
              selectedProduct: product,
              selectedUnit: unit,
              selectedWhLocation: {},
              quantity: element.quantity,
              price: element.price,
              amount: element.amount,
              remarks: "",
              active: "Y"
            });

          });
          await this.totalQuantityAndAmountgoodsReceipt();
        }
        this.selectedVendor = this.vendors.find(a => a.id == po.vendorMasterId)
      }
      else {
        this.disabledAddButtonGoodsReceiptNoteDet = true;
        await this.totalQuantityAndAmountgoodsReceipt();
        this._toastrService.error(this._translate.transform("GRN_PUR_ORDER_NO_REQ"));
      }

    }
    else {
      this.disabledAddButtonGoodsReceiptNoteDet = true;
      await this.totalQuantityAndAmountgoodsReceipt();
      this._toastrService.error(this._translate.transform("GRN_PUR_ORDER_NO_REQ"));
    }

  }


  async onChangeProduct(item) {
    item.selectedUnit = this.units.find(a => a.id == item.selectedProduct.defaultUnitId);
  }
}
