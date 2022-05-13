import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { items } from 'app/mock-api/apps/file-manager/data';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppGlobalDataService } from 'app/shared/services/app-global-data-service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { PurchaseService } from '../../purchase.service';
@Component({
  selector: 'app-create-edit-inventory-transfer',
  templateUrl: './create-edit-inventory-transfer.component.html',
  styleUrls: ['./create-edit-inventory-transfer.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService]
})
export class CreateEditInventoryTransferComponent extends AppComponentBase implements OnInit, OnDestroy {
  issueInfo: any;
  transferInfo: any;
  transferInfoDet: any;
  prevIssueInfo: any;
  whLocations: any = [];
  products: any = [];
  units: any = [];
  employees: any = [];
  availabableStock: any[];

  FromselectedWhLocation: any;
  ToselectedWhLocation: any;

  ProductShelveNo: any = [];
  selectedProduct: any;
  selectedUnit: any;
  selectedEmployee: any;
  lang: any;
  transNo: any;
  inventoryTransTotelQuantity: any;

  constructor(
    injector: Injector,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _toastrService: ToastrService,
    private _primengConfig: PrimeNGConfig,
    private _purchaseService: PurchaseService,
    private _confirmService: ConfirmationService,
    private _codeMasterService: CodesMasterService,
    private _globalService: AppGlobalDataService,
  ) {
    super(injector, 'SCR_INV_TRANSFER', 'allowAdd', _commonService)
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this._primengConfig.ripple = true;
  }

  ngOnInit(): void {


    this.issueInfo = {
      id: '',

      fromWareHouseLocationId: ''
      , toWareHouseLocationId: ''
      , remarks: ''
      , transNo: ''
      , transDate: new Date(),
      inventoryTransferDets: []
    };


    this.transferInfo = {
      id: '',
      serviceRequestId: '',
      employeeId: '',
      remarks: '',
      type: '',
      completeServReq: '',
      serviceReqNo: '',
      transNo: '',
      seqNo: '',
      transDate: new Date(),
      inventoryTransferDets: []
    };
    this.selectedEmployee = {
      name: ""
    };
    this.loadDefaults();
    this.inventoryTransTotelQuantity = 0;

  }

  async loadDefaults() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let getTransactionId = history.state.reqId
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.issueInfo.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("INV_TRANSFER_EDIT"));
      }
      else {
        this._commonService.updatePageName(this._translate.transform("INV_TRANSFER_ADD"));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("INV_TRANSFER_ADD"));
    }
    await this.getAll();

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
    this.employees = await this._globalService.getEmployeeList(true, false, this._translate, false);

    if (this.issueInfo.id && this.issueInfo.id != '') {
      var result = await this._webApiService.get("getInventoryTransferById/" + this.issueInfo.id);
      if (result) {

        this.prevIssueInfo = result as any;
        if (this.prevIssueInfo.validations == null) {

          this.issueInfo = {
            id: this.prevIssueInfo.id,
            remarks: this.prevIssueInfo.remarks,
            transNo: this.prevIssueInfo.transNo,
            seqNo: this.prevIssueInfo.seqNo,
            transDate: this.prevIssueInfo.transDate,
            status: this.prevIssueInfo.status,
            inventoryTransferDets: []
          };
          this.FromselectedWhLocation = this.whLocations.find(a => a.id == this.prevIssueInfo.fromWareHouseLocationId);
          this.ToselectedWhLocation = this.whLocations.find(a => a.id == this.prevIssueInfo.toWareHouseLocationId);

          await this.loadStock();
          this.inventoryTransTotelQuantity = 0;
          this.prevIssueInfo.inventoryTransferDets.forEach(element => {
            var product = this.products.find(a => a.id == element.productMasterId);
            var unit = this.units.find(a => a.id == element.unitMasterId);
            var det = JSON.parse(JSON.stringify(element));
            det.selectedProduct = product;
            det.selectedUnit = unit;
            det.shelveNo = element.shelveNo;
            det.barcode = element.barcode;
            this.inventoryTransTotelQuantity += element.quantity;
            this.loadStockQty(det);
            this.issueInfo.inventoryTransferDets.push(det);
          });
        }
      }
    }
  }
  async totelQuantityInventoryTrans() {
    this.inventoryTransTotelQuantity = 0;
    this.issueInfo.inventoryTransferDets.forEach(element => {
      if (this.issueInfo.inventoryTransferDets) {
        this.inventoryTransTotelQuantity += element.quantity;
      }
    })
  }
  async createOredit() {

    if (!this.FromselectedWhLocation || !this.FromselectedWhLocation.id) {
      this._toastrService.error(this._translate.transform("PUR_WAREHOUSE_LOCATION_SELECT"));
      return;
    }
    if (!this.ToselectedWhLocation || !this.ToselectedWhLocation.id) {
      this._toastrService.error(this._translate.transform("PUR_WAREHOUSE_LOCATION_SELECT"));
      return;
    }


    if (!this.issueInfo.inventoryTransferDets || this.issueInfo.inventoryTransferDets.length == 0) {
      this._toastrService.error(this._translate.transform("SR_MINIMUM_PRODUCT"));
      return;
    }

    var validattion = true;
    this.issueInfo.inventoryTransferDets.forEach((element, eleIdx) => {
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

      if (element.shelveNo == "" || (element.shelveNo) == 0) {
        this._toastrService.error(this._translate.transform("SR_PROVIDE_SHELVENO"));
        validattion = false;;
      }
      if (Number(element.quantity) > Number(element.avlQuantity)) {
        this._toastrService.error(this._translate.transform("INV_ISSUE_EXECEEDS_STOCK"));
        validattion = false;;
      }

      this.issueInfo.inventoryTransferDets.forEach((duplicate, dupIdx) => {
        if (dupIdx != eleIdx && duplicate.selectedProduct.id == element.selectedProduct.id) {
          this._toastrService.error(this._translate.transform("SR_DUPLICATE_PRODUCT"));
          validattion = false;
        }
      });
      element.productMasterId = element.selectedProduct.id;
      element.unitMasterId = element.selectedUnit.id;
    });


    this.issueInfo.fromWareHouseLocationId = this.FromselectedWhLocation.id;
    this.issueInfo.toWareHouseLocationId = this.ToselectedWhLocation.id;

    if (!validattion)
      return;

    if (this.issueInfo.id && this.issueInfo.id != "") {
      this.issueInfo.action = 'M';
      this.issueInfo.modifiedDate = this.prevIssueInfo.modifiedDate;

      this.prevIssueInfo.inventoryTransferDets.forEach(element => {
        var det = this.issueInfo.inventoryTransferDets.find(a => a.id == element.id);
        if (det) {
          det.modifiedDate = element.modifiedDate;
          det.action = "M"
        }
        else {
          element.active = "N";
          element.action = "M";
          this.issueInfo.inventoryTransferDets.push(element);
        }

      });
    }
    else
      this.issueInfo.action = 'N';
    this.issueInfo.active = 'Y';
    var result = await this._webApiService.post("saveInventoryTransfer", this.issueInfo)
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.router.navigateByUrl("purchase/inventory-transfer");
      }
      else {
        this._toastrService.error(output.messages[0])
      }
    }
  }

  cancelAddEdit() {
    this.router.navigateByUrl("purchase/inventory-transfer");
  }


  addNewRecord() {
    var newDet =
    {
      productMasterId: '',
      unitMasterId: '',
      selectedProduct: {},
      selectedUnit: {},
      quantity: '',
      shelveNo: '',
      barcode: '',
      avlQuantity: '',
      productName: '',
      unitName: '',
      remarks: '',
      id: '',
      active: "Y"
    }

    this.issueInfo.inventoryTransferDets.unshift(newDet);
  }

  deleteDetRecord(det) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.issueInfo.inventoryTransferDets.indexOf(det);
        if (index >= 0)
          this.issueInfo.inventoryTransferDets.splice(index, 1);
        await this.totelQuantityInventoryTrans();
      }
    });
  }

  async onChangeProduct(item) {
    var unit = this.units.find(a => a.id == item.selectedProduct.defaultUnitId);
    item.selectedUnit = unit;
    this.loadStockQty(item);

  }

  loadStockQty(item) {
    this.ProductShelveNo = [];
    var conversionUnit = (!item.selectedUnit.conversionUnit || item.selectedUnit.conversionUnit == 0) ? 1 : item.selectedUnit.conversionUnit
    item.avlQuantity = 0;
    var stockList = this.availabableStock.filter(a => a.productMasterId == item.selectedProduct.id);
    if (stockList && stockList.length > 0) {
      stockList.forEach(element => {
        item.avlQuantity += (element.avlQuantity / conversionUnit);
      });
      stockList.forEach(a => {
        if (a.shelveNo) {
          this.ProductShelveNo.push({
            shelveNo: a.shelveNo,
          })
        } else {
          item.shelveNo = "";
        }
      });
    } else {
      item.shelveNo = "";
    }
  }
  async loadStock() {
    var input = {
      wareHouseLocationId: this.FromselectedWhLocation.id
    }
    this.availabableStock = await this._webApiService.post("getProdInventoryBalance", input);

  }
}
