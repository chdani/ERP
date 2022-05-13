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
  selector: 'addEdit-service-request',
  templateUrl: './addEdit-service-request.component.html',
  styleUrls: ['./addEdit-service-request.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService]
})

export class AddEditServiceReqComponent extends AppComponentBase implements OnInit, OnDestroy {
  serviceReq: any;
  prevServiceReq: any;
  categories: any = [];
  products: any = [];
  allProducts: any = [];
  units: any = [];

  selectedCatgory: any;
  selectedProduct: any;
  selectedUnit: any;
  prevSelectedCategory: any;
  lang: any;
  servReqDetAdd: boolean = true;
  allProductSubCategory: any = [];
  productSubCategory: any = [];
  ProdConfiguration: any = [];
  disabledCategory: boolean = false;
  disabledProductSubCategory: boolean = true;
  disabledUnit: boolean = false;
  disabledRequiredQty: boolean = false;
  disabledRemarks: boolean = false;
  selectedProductSubCatgory: any;
  hideProductQuantityByUnHideForApprove: boolean = true;
  approvalProConfi: any;
  prodConfie: boolean = false;
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
    super(injector, 'SCR_SERVICE_REQUEST', 'allowAdd', _commonService);
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this._primengConfig.ripple = true;
  }

  ngOnInit(): void {
    this.serviceReq = {
      id: '',
      prodCategoryId: '',
      prodSubCategoryId: '',
      productMasterId: '',
      unitMasterId: '',
      quantity: '',
      prodConfiguration: '',
      employeeId: '',
      remarks: '',
      curApprovalLevel: '',
      nextApprovalLevel: '',
      transNo: '',
      seqNo: '',
      requiredQty: '',
      statusCode: '',
      approverRemarks: '',
      transDate: new Date(),
      servRequestDet: []
    };

    this.loadDefaults();

  }
  async loadDefaults() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let getTransactionId = history.state.reqId
      this.hideProductQuantityByUnHideForApprove = true;
      if (history.state.approveSerRequest) {
        this.hideProductQuantityByUnHideForApprove = false;
        this.prodConfie = false;
        this.ProdConfiguration = await this._webApiService.get("getServRequestsWithIdDepartment");
        var ProdConfig = this.ProdConfiguration.find(a => a.id == getTransactionId);
        if (ProdConfig) {
          this.approvalProConfi = "";
          this.prodConfie = true;
        }
      }
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '' && this.hideProductQuantityByUnHideForApprove) {
        this.serviceReq.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("SR_SERVICE_REQUEST_EDIT"));
      }
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '' && !this.hideProductQuantityByUnHideForApprove) {
        this.serviceReq.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("SR_SERVICE_REQUEST_APPROVE"));
      }
      if (getTransactionId == '' && this.hideProductQuantityByUnHideForApprove) {
        this._commonService.updatePageName(this._translate.transform("SR_SERVICE_REQUEST_ADD"));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("SR_SERVICE_REQUEST_ADD"));
    }
    await this.getAll();
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  async getAll() {
    this.categories = await this._purchaseService.getProductCategory(false, true, this._translate, true);
    this.units = await this._purchaseService.getProdUnitMaster(false, true, this._translate, true);
    this.allProducts = await this._purchaseService.getProductMaster(false, true, this._translate, "", true, false);
    this.allProductSubCategory = await this._purchaseService.getProductSubCategory(false, true, this._translate, true);

    if (this.serviceReq.id && this.serviceReq.id != '') {
      var result = await this._webApiService.get("getServiceRequestById/" + this.serviceReq.id);
      if (result) {
        this.prevServiceReq = result as any;
        if (this.prevServiceReq.validations == null) {
          this.selectedCatgory = await this.categories.find(a => a.code == this.prevServiceReq.prodCategoryId);
          await this.selectProductSubCategory(this.selectedCatgory);
          //await this.onChangeCategory();
          //this.selectedProduct = await this.products.find(a => a.id == this.prevServiceReq.productMasterId);
          this.selectedUnit = await this.units.find(a => a.id == this.prevServiceReq.unitMasterId);
          this.selectedProductSubCatgory = this.productSubCategory.find(a => a.id == this.prevServiceReq.prodSubCategoryId);
          if (!this.hideProductQuantityByUnHideForApprove) {
            await this.selectProduct(this.selectedProductSubCatgory);
          }
          this.serviceReq = {
            id: this.prevServiceReq.id,
            prodCategoryId: this.prevServiceReq.prodCategoryId,
            employeeId: this.prevServiceReq.employeeId,
            remarks: this.prevServiceReq.remarks,
            productMasterId: this.prevServiceReq.productMasterId,
            unitMasterId: this.prevServiceReq.unitMasterId,
            quantity: this.prevServiceReq.quantity,
            requiredQty: this.prevServiceReq.quantity,
            curApprovalLevel: this.prevServiceReq.curApprovalLevel,
            nextApprovalLevel: this.prevServiceReq.nextApprovalLevel,
            transNo: this.prevServiceReq.transNo,
            seqNo: this.prevServiceReq.seqNo,
            transDate: this.prevServiceReq.transDate,
            createdBy: this.prevServiceReq.createdBy,
            createdDate: this.prevServiceReq.createdDate,
            modifiedBy: this.prevServiceReq.modifiedBy,
            modifiedDate: this.prevServiceReq.modifiedDate,
          };
        }
      }
    }
    if (!this.hideProductQuantityByUnHideForApprove) {
      this.disabledCategory = true;
      this.disabledProductSubCategory = true;
      this.disabledUnit = true;
      this.disabledRequiredQty = true;
      this.disabledRemarks = true;
    }
  }
  async selectProductSubCategory(selectProdCategory) {
    this.productSubCategory = [];
    this.allProductSubCategory.forEach(proSubCat => {
      if (proSubCat.prodCategoryId == selectProdCategory.code) {
        this.productSubCategory.push(proSubCat);
        this.disabledProductSubCategory = false;
      }

    });
    if (this.productSubCategory.length == 0) {
      this.disabledProductSubCategory = true;
    }
  }
  async selectProduct(selectedProductSubCatgory) {
    this.allProducts.forEach(product => {
      if (selectedProductSubCatgory.id == product.prodSubCategoryId) {
        this.products.push(product);
      }
    })
  }

  async createOredit() {
    if (this.hideProductQuantityByUnHideForApprove) {
      if (!this.selectedCatgory || this.selectedCatgory.code == "") {
        this._toastrService.error(this._translate.transform("SR_CATEGORY_SELECT"));
        return;
      }
      var validattion = true;
      if (!this.selectedUnit || this.selectedUnit.id == "") {
        this._toastrService.error(this._translate.transform("SR_UNIT_SELECT"));
        validattion = false;;
      }
      if (this.serviceReq.requiredQty == "" || Number(this.serviceReq.requiredQty) == 0) {
        this._toastrService.error(this._translate.transform("SR_REQUIREDQUANTITY"));
        validattion = false;;
      }

      if (!validattion)
        return;

      if (this.serviceReq.id && this.serviceReq.id != "") {
        this.serviceReq.action = 'M';
        this.serviceReq.modifiedDate = this.prevServiceReq.modifiedDate;
      }
      else
        this.serviceReq.action = 'N';
      this.serviceReq.unitMasterId = this.selectedUnit.id;
      this.serviceReq.active = 'Y';
      this.serviceReq.employeeId = this.userContext.employeeId;
      this.serviceReq.prodCategoryId = this.selectedCatgory.code;
      this.serviceReq.prodSubCategoryId = this.selectedProductSubCatgory.id;
      this.serviceReq.quantity = this.serviceReq.requiredQty;
      var result = await this._webApiService.post("saveServiceRequest", this.serviceReq)
      if (result) {
        var output = result as any;
        if (output.status == "DATASAVESUCSS") {
          this._toastrService.success(this._translate.transform("APP_SUCCESS"));
          this.router.navigateByUrl("purchase/service-request");
        }
        else {
          console.log(output.messages[0]);
          this._toastrService.error(output.messages[0])
        }
      }
    }
    //approver Section
    else {
      if (!this.selectedProduct || this.selectedProduct.id == "") {
        this._toastrService.error(this._translate.transform("SR_PRODUCT_SELECT"));
        validattion = false;;
      }
      if (this.serviceReq.approverRemarks == "" || this.serviceReq.approverRemarks == undefined) {
        this._toastrService.error(this._translate.transform("APP_REMARKS_REQ"));
        return;
      }
      if (this.serviceReq.requiredQty < this.serviceReq.quantity || Number(this.serviceReq.quantity) == 0) {
        this._toastrService.error(this._translate.transform("PROVIDE_VALID_QUANTITY"));
        return;
      }
      if (this.approvalProConfi) {
        this.serviceReq.ProdConfiguration = this.approvalProConfi;
      }
      if (this.serviceReq) {
        this.serviceReq.statusCode = "SERREQAPPROVED";
        this.serviceReq.unitMasterId = this.selectedUnit.id;
        this.serviceReq.prodCategoryId = this.selectedCatgory.code;
        this.serviceReq.prodSubCategoryId = this.selectedProductSubCatgory.id;
        this.serviceReq.productMasterId = this.selectedProduct.id;
        this.serviceReq.createdBy = this.prevServiceReq.createdBy;
        this.serviceReq.createdDate = this.prevServiceReq.createdDate;
        this.serviceReq.modifiedBy = this.prevServiceReq.modifiedBy;
        this.serviceReq.modifiedDate = this.prevServiceReq.modifiedDate;
        this.serviceReq.action = 'M';
        this.serviceReq.active = 'Y';
        var result = await this._webApiService.post("approveServiceRequest", this.serviceReq);
        if (result) {
          var output = result as any;
          if (output.validations == null) {
            if (output.status == "DATASAVESUCSS") {
              this._toastrService.success(this._translate.transform("APP_SUCCESS"));
              this.router.navigateByUrl("purchase/service-request");
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

  }

  cancelAddEdit() {
    this.router.navigateByUrl("purchase/service-request");
  }

  // async onChangeCategory() {
  //   this.products = await this._purchaseService.getProductMaster(false, true, this._translate, this.selectedCatgory.code, true, false);
  //   this.prevSelectedCategory = this.selectedCatgory;
  // }
  // async onChangeProduct() {
  //   this.selectedUnit = this.units.find(a => a.id == this.selectedProduct.defaultUnitId);
  // }
}
