import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { PurchaseService } from 'app/modules/purchase/purchase.service';
import { ConfirmationService } from 'primeng/api';
import { FinanceService } from 'app/modules/finance/finance.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';
@Component({
  selector: 'app-quotation-request',
  templateUrl: './create-edit-quotation-request.component.html',
  styleUrls: ['./create-edit-quotation-request.component.scss',],
  encapsulation: ViewEncapsulation.None,
  providers: [
    TranslatePipe,
  ],
})
export class CreateEditQuotationRequestComponent extends AppComponentBase implements OnInit, OnDestroy {

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private _purchaseService: PurchaseService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    private _financeService: FinanceService,
    private _router: Router,
    private _webApiservice: WebApiService,
    private _confirmService: ConfirmationService,
    public _commonService: AppCommonService,
  ) { super(injector, 'SCR_QUOTATION_REQ', 'allowAdd', _commonService) }

  productMaster: any = [];
  quotationDetReq: any = [];
  id: any;
  showServicePen: boolean = false;
  public checkAllService = false;
  quotationRequest: any;
  details: any = [];
  vendorList: any = [];
  UnitMaster: any = [];
  quotationForm: any;
  approvedServReqsDet: any = [];
  vendor: any = [];
  vendorMasterSelected: any = [];
  vendorProduct: any = [];
  vendorMasterSelectedData: any = [];
  vendorMaster: any = [];
  submitted = false;
  quotationReqTotelQuantity:any;


  ngOnInit(): void {
    this.quotationRequest = {
      id: "",
      transDate: "",
      remarks: "",
      status: "",
      quantity: "",
      quotationReqDet: [],
    }
    this.getall();
    if (history.state && history.state.quotationId && history.state.quotationId != '') {
      this.id = history.state.quotationId;
    }
    this.id = history.state.quotationId;
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("QUOTATION_REQUEST_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("QUOTATION_REQUEST_ADD"));
  }
  async getall() {
    await this.getProductDetails();
    await this.servicePending();
    if (history.state.quotationId && history.state.quotationId !== "") {
      var id = history.state.quotationId;
      this.quotationRequest = await this._webApiservice.get("getQuotationRequestById/" + id);
      this.quotationRequest.transDate = new Date(this.quotationRequest.transDate);
      this.details = this.quotationRequest.quotationReqDet;
      this.quotationRequest.quotationReqDet.forEach(element => {
        var productmaster = this.productMaster.find(a => a.id == element.productMasterId);
        var unitmaster = this.UnitMaster.find(a => a.id == element.unitMasterId);
        this.quotationDetReq.push({
          productMaster: productmaster, UnitMaster: unitmaster, quantity: element.quantity, remarks: element.remarks, id: element.id,
          createdBy: element.createdBy, createdDate: element.createdDate, modifiedBy: element.modifiedBy, modifiedDate: element.modifiedDate
        })
        this.approvedServReqsDet.forEach(ele => {
          if (ele.UnitMaster.id == element.unitMasterId && ele.productMaster.id == element.productMasterId && ele.quantity == element.quantity) {
            ele.selected = true
          }
        });
      });
      await this.selectVendorMaster();
      await this.quantityCalculate();
      await this.totelQuantityQuotationRequest();
    }
  }
  async servicePending() {
    this.approvedServReqsDet = [];
    this.quotationForm = {};
    var iTDept = await this._webApiservice.get("getServRequestsWithProdConfiguration")
    var serviceDept = await this._webApiservice.get("getServRequestsWithApprovalServiceDepartment");
    iTDept.forEach(element => {
      if (element.prodConfiguration) {
        var product = this.productMaster.find(a => a.id == element.productMasterId)
        var unit = this.UnitMaster.find(a => a.id == element.unitMasterId)
        this.approvedServReqsDet.push({
          productName: product?.name, unitName: unit.unitName, quantity: element.quantity,
          productMaster: product, UnitMaster: unit, remarks: element.prodConfiguration
        })
      }
    })
    serviceDept.forEach(ele => {
      var product = this.productMaster.find(a => a.id == ele.productMasterId)
      var unit = this.UnitMaster.find(a => a.id == ele.unitMasterId)
      this.approvedServReqsDet.push({
        productName: product?.name, unitName: unit.unitName, quantity: ele.quantity,
        productMaster: product, UnitMaster: unit
      })
    });
  }
  async onServiceDeptSelecDeselect(event, type) {
    if (type == "ALL") {
      this.approvedServReqsDet.forEach(element => {
        element.selected = event.checked;
        this.quotationDetReq.push({
          quantity: element.quantity, productMaster: element.productMaster, UnitMaster: element.UnitMaster, remarks: element.remarks
        })
      });
      await this.DuplicateRemove();
      await this.selectVendorMaster();
    }
    if (event.checked == false) {
      await this.deleteServiceDet();
    }
    else {
      var checkedItems = this.approvedServReqsDet.filter(a => a.selected == true);
      checkedItems.forEach(element => {
        this.quotationDetReq.push({
          quantity: element.quantity, productMaster: element.productMaster, UnitMaster: element.UnitMaster, remarks: element.remarks
        })
      });
      await this.DuplicateRemove();
      await this.selectVendorMaster();
      this.checkAllService = checkedItems.length == this.approvedServReqsDet.length;
    }
    this.quotationRequest.quantity = 0;
    await this.quantityCalculate();
    await this.totelQuantityQuotationRequest();
  }
  async quantityCalculate() {
    this.approvedServReqsDet.forEach(element => {
      if (element.selected)
        this.quotationRequest.quantity += element.quantity;
    });
  }
  async DuplicateRemove() {
    this.quotationDetReq = this.quotationDetReq.filter((value, index, self) =>
      index === self.findIndex((t) => (
        t.unitmaster === value.unitmaster && t.productMaster === value.productMaster && t.quantity === value.quantity
      )))
  }
  async deleteServiceDet() {
    this.approvedServReqsDet.forEach(ele => {
      if (ele.selected == false) {
        this.quotationDetReq.forEach(element => {
          if (element.unitmaster === ele.unitmaster && element.productMaster === ele.productMaster && element.quantity === ele.quantity) {
            this.quotationDetReq.forEach((item, index) => {
              if (item === element) this.quotationDetReq.splice(index, 1);
            });
          }
        });
      }
    });
    var checkedItems = this.approvedServReqsDet.filter(a => a.selected == true);
    this.checkAllService = checkedItems.length == this.approvedServReqsDet.length;
    await this.selectVendorMaster();
  }
  async totelQuantityQuotationRequest(){
    this.quotationReqTotelQuantity = 0 ;
    this.quotationDetReq.forEach(element=>{
if(element.quantity != ""){
  this.quotationReqTotelQuantity+=element.quantity;
}
    });
  }
  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
  async onChangeProduct(item) {
    var unit = this.UnitMaster.find(a => a.id == item.productMaster.defaultUnitId);
    item.UnitMaster = unit;
    await this.selectVendorMaster();
  }
   async addNewQuotationDetReq() {
    var newQuotationDet =
    {
      productMaster: "",
      UnitMaster: "",
      quantity: "",
      remarks: "",
      active: "Y"
    }

    this.quotationDetReq.unshift(newQuotationDet);
  }
  async deleteQuotationDet(item) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.quotationDetReq.indexOf(item);
        if (index >= 0)
          this.quotationDetReq.splice(index, 1);
        this.approvedServReqsDet.forEach(ele => {
          if (ele.unitName == item.UnitMaster.unitName && ele.productMaster == item.productMaster && ele.quantity == item.quantity) {
            ele.selected = false
          }
        });
        var checkedItems = this.approvedServReqsDet.filter(a => a.selected == true);
        this.checkAllService = checkedItems.length == this.approvedServReqsDet.length;
        await this.selectVendorMaster();
        await this.totelQuantityQuotationRequest();
      }
    });

  }
  async selectVendorMaster() {
    this.vendorMasterSelectedData = [];
    this.vendorList = this.vendorMaster;
    this.quotationDetReq.forEach(element => {
      this.vendorProduct.forEach(ele => {
        var master = ele.productMasterId == element.productMaster.id;
        if (master) {
          let vendor = this.vendorMaster.find(a => a.id == ele.vendorMasterId);
          let list = this.vendorMasterSelectedData.filter(a => a.id == ele.vendorMasterId)
          if (list.length == 0 && vendor != null) {
            this.vendorMasterSelectedData.push({
              vendorName: vendor.vendorName, id: vendor.id
            })
            this.vendorList = this.vendorList.filter(a => a.id !== ele.vendorMasterId)
          }
        }
      });
    });
    this.vendorMasterSelectedData = this.vendorMasterSelectedData;
  }

  async getProductDetails() {
    this.productMaster = await this._purchaseService.getProductMaster(false, false, this._translate, "", true, false);
    this.UnitMaster = await this._purchaseService.getProdUnitMaster(false, false, this._translate, true);
    this.vendorMaster = await this._financeService.getVendorMaster();
    this.vendorList = this.vendorMaster;
    this.vendorProduct = await this._webApiservice.get("getVendorproductList");
  }

  cancelAddEdit() {
    this._router.navigateByUrl("purchase/quotation-request");
  }

  async onSubmit() {
    this.submitted = true;
    this.quotationRequest.quotationReqDet = [];
    if (this.quotationDetReq.length == 0) {
      this._toastrService.error(this._translate.transform("QUOTATION_DET_REQ"));
      return;
    }
    if (this.vendorMasterSelectedData.length == 0) {
      this._toastrService.error(this._translate.transform("VENDOR_DET_REQ"));
      return;
    }
    this.vendor = [];
    this.vendorMasterSelectedData.forEach(element => {
      this.vendor.push({
        vendorMasterId: element.id, active: "Y", createdBy: this.quotationRequest.createdBy, createdDate: this.quotationRequest.createdDate,
        modifiedBy: this.quotationRequest.modifiedBy, modifiedDate: this.quotationRequest.modifiedDate
      })
    });
    this.quotationRequest.quotaReqVendorDets = this.vendor;
    this.quotationRequest.transDate = new Date;
    this.quotationRequest.status = "PURTRNSTSSUBMITTED";
    this.quotationRequest.active = "Y";
    this.quotationRequest.quototionReqDets = [];

    this.quotationDetReq.forEach(element => {
      this.quotationRequest.quotationReqDet.push({
        productMasterId: element.productMaster.id, unitMasterId: element.UnitMaster.id,
        quantity: element.quantity, remarks: element.remarks, active: "Y", createdBy: element.createdBy, createdDate: element.createdDate,
        modifiedBy: element.modifiedBy, modifiedDate: element.modifiedDate, id: element.id
      })
    });


    this.details.forEach(element => {
      var det = this.quotationRequest.quotationReqDet.find(a => a.id == element.id);
      if (det) {
        det.modifiedDate = element.modifiedDate;
        det.action = "M"
      }
      else {
        element.active = "N";
        element.action = "M";
        this.quotationRequest.quotationReqDet.push(element);
      }
    });

    if (this.quotationRequest.id && this.quotationRequest.id != "") {
      this.quotationRequest.createdBy = this.quotationRequest.createdBy;
      this.quotationRequest.createdDate = this.quotationRequest.createdDate;
      this.quotationRequest.modifiedBy = this.quotationRequest.modifiedBy;
      this.quotationRequest.modifiedDate = this.quotationRequest.modifiedDate;
    }
    var result = await this._webApiservice.post("saveQuotationRequest", this.quotationRequest);
    if (result) {
      if (result.status == "ONEORMOREERR") {
        this._toastrService.error(this._translate.transform("Please provide all mandatory fields"));
      }
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.cancelAddEdit();
      }
    }
  }
}

