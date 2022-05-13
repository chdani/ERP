import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { PurchaseService } from '../../purchase.service';
import { FinanceService } from 'app/modules/finance/finance.service';

@Component({
  selector: 'create-edit-purchase-order',
  templateUrl: './create-edit-purchase-order.component.html',
  styleUrls: ['./create-edit-purchase-order.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService]
})

export class CreateEditPurchaseOrderComponent extends AppComponentBase implements OnInit, OnDestroy {
  purchaseorderInfo: any;
  prepurchaseorderInfo: any;
  products: any = [];
  units: any = [];
  vendors: any = [];
  approvedPurchaseRequest: any[];

  selectedProduct: any;
  selectedUnit: any;
  selectedVendor: any;
  transNo: any;
  lang: any;
  purchaseOrderTotelAmount: any;
  purchaseOrderTotelQuantity: any;
  ledgerCodes: any = [];
  filteredLedgers: any = [];
  costCenters: any = [];
  selectedLedger: any;
  selectCostCenterCode: any;
  constructor(
    injector: Injector,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _toastrService: ToastrService,
    private _primengConfig: PrimeNGConfig,
    private _purchaseService: PurchaseService,
    private _financeService: FinanceService,
  ) {
    super(injector, 'SCR_PURCHASE_ORD', 'allowAdd', _commonService);
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this._primengConfig.ripple = true;
  }

  ngOnInit(): void {
    this.purchaseorderInfo = {
      id: '',
      prodCategoryId: '',
      accountCode: '',
      remarks: '',
      transNo: '',
      CostCenterCode: '',
      transDate: new Date,
      orgId: this.orgId,
      finYear: this.financialYear
    };
    this.selectedVendor = {
      name: ""
    };
    this.loadDefaults();
    this.purchaseOrderTotelQuantity = 0;
    this.purchaseOrderTotelAmount = 0;
  }
  async loadDefaults() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let getTransactionId = history.state.reqId
      this.transNo = history.state.transNo
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.purchaseorderInfo.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("PURCHASE_ORDER_EDIT"));
      }
      else {
        this._commonService.updatePageName(this._translate.transform("PURCHASE_ORDER_ADD"));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("PURCHASE_ORDER_ADD"));
    }
    await this.getAll();
    if (this.transNo) {
      this.purchaseorderInfo.purchaseRequestNo = this.transNo;
      this.loadVendor(this.transNo);
    }
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
  async filterLedgerAccounts(event: any) {
    this.filteredLedgers = await this.ledgerCodes.filter(a => a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase())
      && a.balance > 0);
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
    }
  }
  async getAll() {
    await this.AllledgerCodes();
    this.costCenters = await this._financeService.getCostCenters(false, false, this._translate);
    this.units = await this._purchaseService.getProdUnitMaster(false, true, this._translate, true);
    this.products = await this._purchaseService.getProductMaster(false, true, this._translate, "", true, false);
    this.vendors = await this._purchaseService.getVendorMaster(false, true, this._translate, true);
    var searchInput = {
      Status: "PURTRNSTSAPPROVED"
    };
    this.approvedPurchaseRequest = await this._webApiService.post("getPurchaseRequest", searchInput)

    if (this.purchaseorderInfo.id && this.purchaseorderInfo.id != '') {
      var result = await this._webApiService.get("getPurchaseOrderById/" + this.purchaseorderInfo.id);
      if (result) {
        this.prepurchaseorderInfo = result as any;
        if (this.prepurchaseorderInfo.validations == null) {
          this.selectedVendor = this.vendors.find(a => a.id == this.prepurchaseorderInfo.vendorMasterId);
          this.selectedLedger = this.ledgerCodes.find(a => a.ledgerCode == this.prepurchaseorderInfo.ledgerCode);
          this.selectCostCenterCode = this.costCenters.find(a => a.code == this.prepurchaseorderInfo.costCenterCode);
          var POdetail = await this._webApiService.get("getPurchaseRequestId/" + this.prepurchaseorderInfo.purchaseRequestId) as any;

          this.purchaseorderInfo = {
            id: this.prepurchaseorderInfo.id,
            purchaseRequestNo: POdetail?.transNo,
            remarks: this.prepurchaseorderInfo.remarks,
            transNo: this.prepurchaseorderInfo.transNo,
            seqNo:this.prepurchaseorderInfo.seqNo,
            transDate: this.prepurchaseorderInfo.transDate,
            purchaseRequestId: this.prepurchaseorderInfo.purchaseRequestId,
            vendorMasterId: this.prepurchaseorderInfo.vendorMasterId,
            status: this.prepurchaseorderInfo.status,
            orgId: this.prepurchaseorderInfo.orgId,
            finYear: this.prepurchaseorderInfo.finYear,
            ledgerCode: this.prepurchaseorderInfo.ledgerCode,
            costCenterCode: this.prepurchaseorderInfo.costCenterCode,
            totalAmount: this.prepurchaseorderInfo.totalAmount,
            purchaseOrderDet: []
          };

          this.prepurchaseorderInfo.purchaseOrderDet.forEach(element => {
            var product = this.products.find(a => a.id == element.productMasterId);
            var unit = this.units.find(a => a.id == element.unitMasterId);

            var det = JSON.parse(JSON.stringify(element));
            det.selectedProduct = product;
            det.selectedUnit = unit;
            this.purchaseorderInfo.purchaseOrderDet.push(det);
          });
        }
        this.totelAmountAndQuantityOfPurchaseOrder();
      }
    }
  }

  calculateAmount(event, item) {
    item.amount = item.price * item.quantity;
  }
  async createOredit() {
    if (!this.selectedVendor || this.selectedVendor.code == "") {
      this._toastrService.error(this._translate.transform("PUR_CATEGORY_SELECT"));
      return;
    }

    if (this.purchaseorderInfo.purchaseRequestId == "") {
      this._toastrService.error(this._translate.transform("PUR_REQUEST_NO_REQ"));
      return;
    }
    if (!this.selectedLedger || this.selectedLedger.ledgerCode == "") {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_LEDGER_REQ"));
      return;
    }
    if (!this.selectCostCenterCode || this.selectCostCenterCode.code == "") {
      this._toastrService.error(this._translate.transform("DIRECT_INVOICE_CC_REQ"));
      return;
    }
    if (!this.purchaseorderInfo.purchaseOrderDet || this.purchaseorderInfo.purchaseOrderDet.length == 0) {
      this._toastrService.error(this._translate.transform("SR_MINIMUM_PRODUCT"));
      return;
    }

    var validattion = true;
    this.purchaseorderInfo.purchaseOrderDet.forEach((element, eleIdx) => {
      if (!element.selectedProduct || !element.selectedProduct.id) {
        this._toastrService.error(this._translate.transform("PUR_PRODUCT_SELECT"));
        validattion = false;;
      }
      if (!element.selectedUnit || !element.selectedUnit.id) {
        this._toastrService.error(this._translate.transform("PUR_UNIT_SELECT"));
        validattion = false;;
      }

      if (element.quantity == "" || Number(element.quantity) == 0) {
        this._toastrService.error(this._translate.transform("PUR_PROVIDE_QUANTITY"));
        validattion = false;;
      }
      if (element.price == "" || Number(element.price) == 0) {
        this._toastrService.error(this._translate.transform("PUR_PIRCE_REQ"));
        validattion = false;;
      }

      this.purchaseorderInfo.purchaseOrderDet.forEach((duplicate, dupIdx) => {
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

    if (this.purchaseorderInfo.id && this.purchaseorderInfo.id != "") {
      this.purchaseorderInfo.action = 'M';
      this.purchaseorderInfo.modifiedDate = this.prepurchaseorderInfo.modifiedDate;

      this.prepurchaseorderInfo.purchaseOrderDet.forEach(element => {
        var det = this.purchaseorderInfo.purchaseOrderDet.find(a => a.id == element.id);
        if (det) {
          det.modifiedDate = element.modifiedDate;
          det.action = "M"
        }
        else {
          element.active = "N";
          element.action = "M";
          this.purchaseorderInfo.purchaseOrderDet.push(element);
        }

      });

    }
    else
      this.purchaseorderInfo.action = 'N';
    this.purchaseorderInfo.accountCode = this.selectedVendor.ledgerCode;
    this.purchaseorderInfo.ledgerCode = this.selectedLedger.ledgerCode;
    this.purchaseorderInfo.costCenterCode = this.selectCostCenterCode.code;
    this.purchaseorderInfo.totalAmount = this.purchaseOrderTotelAmount;
    this.purchaseorderInfo.active = 'Y';

    var result = await this._webApiService.post("savePurchaseOrder", this.purchaseorderInfo)
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.router.navigateByUrl("purchase/purchase-order");
      }
      else {
        console.log(output.messages[0]);
        this._toastrService.error(output.messages[0])
      }
    }
  }

  cancelAddEdit() {
    this.router.navigateByUrl("purchase/purchase-order");
  }
  totelAmountAndQuantityOfPurchaseOrder() {
    this.purchaseOrderTotelAmount = 0;
    this.purchaseOrderTotelQuantity = 0;
    if (this.purchaseorderInfo.purchaseOrderDet) {
      this.purchaseorderInfo.purchaseOrderDet.forEach(element => {
        this.purchaseOrderTotelQuantity += element.quantity;
        this.purchaseOrderTotelAmount += element.amount;
      })
    }
  }
  async loadVendor(event) {
    this.selectedVendor = {
      name: ""
    };
    this.purchaseOrderTotelAmount = 0;
    this.purchaseOrderTotelQuantity = 0;
    if (this.purchaseorderInfo.purchaseRequestNo && this.purchaseorderInfo.purchaseRequestNo != "") {
      var po = this.approvedPurchaseRequest.find(a => a.transNo == this.purchaseorderInfo.purchaseRequestNo)
      if (po) {
        this.purchaseorderInfo.purchaseOrderDet = [];
        this.purchaseorderInfo.vendorMasterId = po.vendorMasterId;
        this.purchaseorderInfo.purchaseRequestId = po.id;
        var result = await this._webApiService.get("getPurchaseRequestDetByHdrId/" + po.id);
        if (result) {
          var poDet = result as any;
          poDet.forEach(element => {
            var product = this.products.find(a => a.id == element.productMasterId);
            var unit = this.units.find(a => a.id == element.unitMasterId);
            this.purchaseorderInfo.purchaseOrderDet.push({
              selectedProduct: product,
              selectedUnit: unit,
              quantity: element.quantity,
              price: element.price,
              amount: element.amount,
              remarks: element.remarks,
              active: "Y"
            });

          });
          this.totelAmountAndQuantityOfPurchaseOrder();
        }
        this.selectedVendor = this.vendors.find(a => a.id == po.vendorMasterId);
      }
      else {
        this._toastrService.error(this._translate.transform("GRN_PUR_REQUEST_NO_REQ"));
        this.purchaseorderInfo.purchaseOrderDet = [];
      }

    }
    else {
      this._toastrService.error(this._translate.transform("GRN_PUR_REQUEST_NO_REQ"));
      this.purchaseorderInfo.purchaseOrderDet = [];
    }
  }
}
