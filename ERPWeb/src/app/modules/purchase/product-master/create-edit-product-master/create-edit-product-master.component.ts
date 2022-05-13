import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { finalize } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { FormControl } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { PurchaseService } from '../../purchase.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';
@Component({
  selector: 'app-product-master',
  templateUrl: './create-edit-product-master.component.html',
  styleUrls: ['./create-edit-product-master.component.scss',],
  encapsulation: ViewEncapsulation.None,
  providers: [
    TranslatePipe,
  ],
})
export class CreateEditProductMasterComponent extends AppComponentBase implements OnInit {

  constructor(
    injector: Injector,
    private formBuilder: FormBuilder,
    private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private _formBuilder: FormBuilder,
    private _purchaseService: PurchaseService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    private _router: Router,
    private _webApiservice: WebApiService,
    public _commonService: AppCommonService,
  ) { super(injector, 'SCR_PRODUCT_MASTER', 'allowAdd', _commonService) }

  selectedProduct: any;
  selectedProductSubCategory: any;
  productCategory: any = [];
  allProductSubCategory: any = [];
  productSubCategory: any = [];
  selectedUnit: any;
  selectedExpirable: any;
  selectedManageStacks: any;
  id: any;
  productMaster: any;
  ProductMasterInfo: any;
  UnitMaster: any = [];
  submitted = false;
  disabledProductSubCategory: boolean = true;


  ngOnInit(): void {
    this.productMaster = {
      id: "",
      prodCode: "",
      ProdCategoryId: "",
      ProdDescription: "",
      Barcode: "",
      ReOrderLevel: "",
      IsExpirable: "",
      IsStockable: "",
      DefaultUnitId: "",
      prodSubCategoryId: ""

    }
    this.getall();
    if (history.state && history.state.productMasterId && history.state.productMasterId != '') {
      this.id = history.state.productMasterId;
    }
    this.id = history.state.productMasterId;
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("PRODUCT_MASTER_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("PRODUCT_MASTER_ADD"));


  }
  async getall() {
    await this.getProductDetails();
    if (history.state.productMasterId && history.state.productMasterId !== "") {
      var id = history.state.productMasterId;

      var result = await this._webApiservice.get("getProductMasterById/" + id);
      this.ProductMasterInfo = result as any;

      var product = this.productCategory.find(a => a.code == this.ProductMasterInfo.prodCategoryId);
      var unitmaster = this.UnitMaster.find(a => a.id == this.ProductMasterInfo.defaultUnitId);
      var productSubCat = this.allProductSubCategory.find(a => a.id == this.ProductMasterInfo.prodSubCategoryId);
      this.selectedProduct = product ? product.code : "";
      this.selectedUnit = unitmaster ? unitmaster.id : "";
      await this.selectProductSubCategory(this.selectedProduct);
      this.selectedProductSubCategory = productSubCat ? productSubCat.id : "";
      this.selectedExpirable = this.ProductMasterInfo.isExpirable;
      this.selectedManageStacks = this.ProductMasterInfo.isStockable;
      this.productMaster = {
        id: this.ProductMasterInfo.id,
        prodCode: this.ProductMasterInfo.prodCode,
        ProdDescription: this.ProductMasterInfo.prodDescription,
        Barcode: this.ProductMasterInfo.barcode,
        ReOrderLevel: this.ProductMasterInfo.reOrderLevel
      };
    }
  }

  async getProductDetails() {
    this.productCategory = await this._purchaseService.getProductCategory(false, false, this._translate, true);
    this.UnitMaster = await this._purchaseService.getProdUnitMaster(false, false, this._translate, true);
    this.allProductSubCategory = await this._purchaseService.getProductSubCategory(false, false, this._translate, true);
  }

  cancelAddEdit() {
    this._router.navigateByUrl("purchase/product-master");
  }
  async selectProductSubCategory(prodCategoryId) {
    this.productSubCategory = [];
    this.allProductSubCategory.forEach(proSubCat => {
      if (proSubCat.prodCategoryId == prodCategoryId) {
        this.productSubCategory.push(proSubCat);
        this.disabledProductSubCategory = false;
      }

    });
    if (this.productSubCategory.length == 0) {
      this.disabledProductSubCategory = true;
    }
  }

  async onSubmit() {
    this.submitted = true;
    if (this.productMaster.prodCode == "") {
      this._toastrService.error(this._translate.transform("PRODUCT_CODE_REQ"));
      return;
    }
    if (this.productMaster.ProdDescription == "") {
      this._toastrService.error(this._translate.transform("PRODUCT_NAME_REQ"));
      return;
    }
    if (!this.selectedUnit) {
      this._toastrService.error(this._translate.transform("PRODUCT_UNITMASTER_REQ"));
      return;
    }

    if (!this.selectedProduct) {
      this._toastrService.error(this._translate.transform("PRODUCT_PRODUCTCATEGORY_REQ"));
      return;
    }
    if (this.selectedProductSubCategory != "") {
      this.productMaster.prodSubCategoryId = this.selectedProductSubCategory;
    }
    if (this.productMaster.Barcode == "") {
      this._toastrService.error(this._translate.transform("PRODUCT_BARCODE_REQ"));
      return;
    }
    this.productMaster.active = "Y";
    this.productMaster.IsExpirable = this.selectedExpirable;
    this.productMaster.IsStockable = this.selectedManageStacks;
    this.productMaster.DefaultUnitId = this.selectedUnit;
    this.productMaster.ProdCategoryId = this.selectedProduct;

    if (this.productMaster.id && this.productMaster.id != "") {
      this.productMaster.createdBy = this.ProductMasterInfo.createdBy;
      this.productMaster.createdDate = this.ProductMasterInfo.createdDate;
      this.productMaster.modifiedBy = this.ProductMasterInfo.modifiedBy;
      this.productMaster.modifiedDate = this.ProductMasterInfo.modifiedDate;
    }

    var result = await this._webApiservice.post("saveProductMaster", this.productMaster);
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this._router.navigateByUrl("purchase/product-master");
        localStorage.removeItem("appProductMaster");
      }
      else
        this._toastrService.error(output.messages[0])
    }
  }
}

