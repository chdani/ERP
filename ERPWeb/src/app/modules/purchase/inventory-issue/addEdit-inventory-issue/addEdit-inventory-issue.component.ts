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
  selector: 'addEdit-inventory-issue',
  templateUrl: './addEdit-inventory-issue.component.html',
  styleUrls: ['./addEdit-inventory-issue.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService]
})

export class AddEditInvIssueComponent extends AppComponentBase implements OnInit, OnDestroy {
  issueInfo: any;
  prevIssueInfo: any;
  whLocations: any = [];
  products: any = [];
  issueTypes: any = [];
  units: any = [];
  employees: any = [];
  approvedServReqs: any[];
  availabableStock: any[];

  selectedWhLocation: any;
  selectedProduct: any;
  selectedUnit: any;
  selectedEmployee: any;
  selectedIssueType: any;
  showSRBasedControls: boolean = false;
  lang: any;
  transNo:any;
  prevServReqNo: any;
  inventoryissueTotelQuantity:any;

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
    super(injector,'SCR_INVENTORY_ISSUE','allowAdd', _commonService );
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this._primengConfig.ripple = true;
  }

  ngOnInit(): void {
    this.issueInfo = {
      id: '',
      serviceRequestId: '',
      employeeId: '',
      remarks: '',
      type:  '',
      completeServReq: '',
      serviceReqNo: '',
      transNo: '',
      seqNo:'',
      transDate: new Date(),
      prodInvIssueDet: []
    };
    this.selectedEmployee = {
      name: ""
    };
    this.loadDefaults();
    this.inventoryissueTotelQuantity = 0;

  }

  async loadDefaults() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let getTransactionId = history.state.reqId
      this.transNo=history.state.transNo;
      if (getTransactionId != null && getTransactionId != undefined && getTransactionId != '') {
        this.issueInfo.id = getTransactionId;
        this._commonService.updatePageName(this._translate.transform("INV_ISSUE_EDIT"));
      }
      else {
        this._commonService.updatePageName(this._translate.transform("INV_ISSUE_ADD"));
      }
    }
    else {
      this._commonService.updatePageName(this._translate.transform("INV_ISSUE_ADD"));
    }
    await this.getAll();
    if(this.transNo){
      this.issueInfo.serviceReqNo=this.transNo;
      this.loadServiceDetails(this.transNo);
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
    this.products = await this._purchaseService.getProductMaster(false, true, this._translate, "", true,true);
    this.employees = await this._globalService.getEmployeeList(true, false, this._translate, false);
    this.issueTypes = await this._codeMasterService.getCodesDetailByGroupCode("INVISSUETYPE", false, false, this._translate);
    this.selectedIssueType = this.issueTypes.find(a => a.code == "INVISSUEEMP");
    this.showSRBasedControls = true;
 
    this.approvedServReqs = await this._webApiService.get("getServRequestsWithApprovalServiceDepartment");

    if (this.issueInfo.id && this.issueInfo.id != '') {
      var result = await this._webApiService.get("getProdInvIssueById/" + this.issueInfo.id);
      if (result) {
        this.prevIssueInfo = result as any;
        if (this.prevIssueInfo.validations == null) {
          this.selectedEmployee = this.employees.find(a => a.id == this.prevIssueInfo.employeeId);
          this.selectedIssueType = this.issueTypes.find(a => a.code == this.prevIssueInfo.type);

          var servReqdetail = this.approvedServReqs.find(a => a.id == this.prevIssueInfo.serviceRequestId);

          this.issueInfo = {
            id: this.prevIssueInfo.id,
            serviceReqNo: servReqdetail?.transNo,
            remarks: this.prevIssueInfo.remarks,
            transNo: this.prevIssueInfo.transNo,
            seqNo:this.prevIssueInfo.seqNo,
            transDate: this.prevIssueInfo.transDate,
            serviceRequestId: this.prevIssueInfo.serviceRequestId, 
            employeeId: this.prevIssueInfo.employeeId, 
            status: this.prevIssueInfo.status, 
            prodInvIssueDet : []
          };
          this.prevServReqNo = this.issueInfo.serviceReqNo;
          if (this.selectedIssueType.code == "INVISSUEEMP")
            this.showSRBasedControls = true
          else
            this.showSRBasedControls = false;
          this.selectedWhLocation = this.whLocations.find(a => a.id == this.prevIssueInfo.prodInvIssueDet[0].wareHouseLocationId);
          await this.loadStock();
          this.prevIssueInfo.prodInvIssueDet.forEach(element => {
            var product = this.products.find(a => a.id == element.productMasterId);
            var unit = this.units.find(a => a.id == element.unitMasterId);
            var det = JSON.parse(JSON.stringify(element));
            det.selectedProduct = product;
            det.selectedUnit = unit;
            
            this.loadStockQty(det);

            this.issueInfo.prodInvIssueDet.push(det);
          });
        }
      }
    }
    await this.totelQuantityInventoryissue();
  }
  async totelQuantityInventoryissue(){
    this.inventoryissueTotelQuantity = 0;
 if(this.issueInfo.prodInvIssueDet){
  this.issueInfo.prodInvIssueDet.forEach(element=>{
    this.inventoryissueTotelQuantity += element.quantity;
  })
}
  }


  async createOredit() {

    if (!this.selectedWhLocation || !this.selectedWhLocation.id) {
      this._toastrService.error(this._translate.transform("PUR_WAREHOUSE_LOCATION_SELECT"));
      validattion = false;;
    }   
 
    if (this.selectedIssueType.code == "INVISSUEEMP" && this.issueInfo.serviceRequestId == "") {
        this._toastrService.error(this._translate.transform("INV_ISSUE_SR_REQ_NO_INVALID"));
        return;
    }

     if (!this.issueInfo.prodInvIssueDet || this.issueInfo.prodInvIssueDet.length == 0) {
      this._toastrService.error(this._translate.transform("SR_MINIMUM_PRODUCT"));
      return;
    }

    var validattion = true;
    this.issueInfo.prodInvIssueDet.forEach((element, eleIdx) => {
      if (!element.selectedProduct || !element.selectedProduct.id ) {
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
      if (Number(element.quantity) > Number(element.avlQuantity)) {
        this._toastrService.error(this._translate.transform("INV_ISSUE_EXECEEDS_STOCK"));
        validattion = false;;
      }

      this.issueInfo.prodInvIssueDet.forEach((duplicate, dupIdx) => {
        if (dupIdx != eleIdx && duplicate.selectedProduct.id == element.selectedProduct.id ) {
          this._toastrService.error(this._translate.transform("SR_DUPLICATE_PRODUCT"));
          validattion = false;
        }
      });
      element.productMasterId = element.selectedProduct.id;
      element.unitMasterId = element.selectedUnit.id;
      element.wareHouseLocationId = this.selectedWhLocation.id;
    });

    if (!validattion)
      return;

    if (this.issueInfo.id && this.issueInfo.id != "") {
      this.issueInfo.action = 'M';
      this.issueInfo.modifiedDate = this.prevIssueInfo.modifiedDate;

      this.prevIssueInfo.prodInvIssueDet.forEach(element => {
        var det = this.issueInfo.prodInvIssueDet.find(a => a.id == element.id);
        if (det) {
          det.modifiedDate = element.modifiedDate;
          det.action = "M"
        }
        else {
          element.active = "N";
          element.action = "M";
          this.issueInfo.prodInvIssueDet.push(element);
        }

      });

    }
    else 
      this.issueInfo.action = 'N';
  
    this.issueInfo.active = 'Y';
    this.issueInfo.type = this.selectedIssueType.code;
    var result = await this._webApiService.post("saveProdInvIssue", this.issueInfo)
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.router.navigateByUrl("purchase/inventory-issue");
      }
      else {
        console.log(output.messages[0]);
        this._toastrService.error(output.messages[0])
      }
    }
  }

  cancelAddEdit() {
    this.router.navigateByUrl("purchase/inventory-issue");
  }

 
  addNewRecord() {
    var newDet =
    {
      productMasterId: '',
      unitMasterId: '',
      selectedProduct: {},
      selectedUnit: {}, 
      quantity: '',
      avlQuantity:'',
      productName: '',
      unitName:'',
      remarks: '',
      id: '',
      active: "Y"
    }
    if (!this.issueInfo.prodInvIssueDet)
      this.issueInfo.prodInvIssueDet = [];
    this.issueInfo.prodInvIssueDet.unshift(newDet);
  }


  deleteDetRecord(det) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.issueInfo.prodInvIssueDet.indexOf(det);
        if (index >= 0)
          this.issueInfo.prodInvIssueDet.splice(index, 1);
          await this.totelQuantityInventoryissue();
      }
    });
  }

  async loadServiceDetails(event)
  {

    if (this.prevServReqNo == this.issueInfo.serviceReqNo)
      return;
    else
      this.prevServReqNo = this.issueInfo.serviceReqNo;
    
    this.selectedEmployee = {
      name: ""
    };
    this.issueInfo.prodInvIssueDet = [];

    if (this.issueInfo.serviceReqNo && this.issueInfo.serviceReqNo != "") {
      var servReq = this.approvedServReqs.find(a => a.transNo == this.issueInfo.serviceReqNo)
      if (servReq) {
        this.issueInfo.employeeId = servReq.employeeId;
        this.issueInfo.serviceRequestId = servReq.id;
        if(servReq) {
          var product = this.products.find(a => a.id == servReq.productMasterId);
          var unit = this.units.find(a => a.id == servReq.unitMasterId);

          var item ={
            selectedProduct : product,
            selectedUnit : unit,
            quantity: servReq.quantity,
            remarks: "",
            active : "Y"
          };
           
          this.issueInfo.prodInvIssueDet.push(item);
          await this.totelQuantityInventoryissue();
          this.loadStockQty(item);
          
        }  
       
        this.selectedEmployee = this.employees.find(a => a.id == servReq.employeeId)
      }
      else 
        this._toastrService.error(this._translate.transform("INV_ISSUE_SR_REQ_NO_INVALID"));
    }
    else
      this._toastrService.error(this._translate.transform("INV_ISSUE_SR_REQ_NO_INVALID"));
  } 

  async onChangeProduct(item) {
    var unit = this.units.find(a => a.id == item.selectedProduct.defaultUnitId);
    item.selectedUnit = unit;
    this.loadStockQty(item);
  }

  loadStockQty(item)
  {
    var conversionUnit = (!item.selectedUnit.conversionUnit || item.selectedUnit.conversionUnit == 0) ? 1 : item.selectedUnit.conversionUnit
    item.avlQuantity = 0;
    var stockList = this.availabableStock.filter(a => a.productMasterId == item.selectedProduct.id);
    if (stockList && stockList.length > 0) {
      stockList.forEach(element => {
        item.avlQuantity += (element.avlQuantity / conversionUnit);
      });
    }

    if (item.avlQuantity <= 0)
      this._toastrService.error(this._translate.transform("INV_ISSUE_NO_STOCK"));
  }

  onChangeIssueType()
  {
    this.issueInfo.serviceRequestId = '';
    this.issueInfo.employeeId = '';
    this.issueInfo.completeServReq = '';
    this.issueInfo.serviceReqNo = '';
    this.issueInfo.prodInvIssueDet = [];
    
    this.selectedEmployee = {
      name: ""
    };
 
    if (this.selectedIssueType.code == "INVISSUEEMP")
      this.showSRBasedControls = true
    else
      this.showSRBasedControls = false;
  }

  async loadStock() {
    var input = {
      wareHouseLocationId: this.selectedWhLocation.id
    }
    this.availabableStock = await this._webApiService.post("getProdInventoryBalance", input);
    if (this.selectedIssueType.code == "INVISSUEEMP" && this.issueInfo.serviceReqNo != "") {
      this.prevServReqNo = "";
      if (!this.issueInfo.id || this.issueInfo.id == "" )
        this.loadServiceDetails(null);
    }
      
  }
}
