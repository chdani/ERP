import { Component, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem, SelectItem } from 'primeng/api';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { FinanceService } from 'app/modules/finance/finance.service';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { MatStepperModule } from '@angular/material/stepper';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePipe } from '@angular/common';
import { PurchaseService } from '../../purchase.service';
import { takeUntil } from 'rxjs/operators';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';


/**
 * @title Stepper overview
 */
@Component({
  selector: 'app-vendor',
  templateUrl: './create-edit-vendor.component.html',
  styleUrls: ['./create-edit-vendor.component.scss'],
  providers: [MatStepperModule, TranslatePipe, DialogService, DatePipe]


})
export class createeditvendorComponend extends AppComponentBase implements OnInit {
  isLinear = true;
  uploadConfig: FileUploadConfig;
  id: any;
  dataaccount = [];
  dataaccountOptions: SelectItem[];
  productMaster = [];
  ventorProduMaster = [];

  PaymentTerm = [];
  PaymentTermOptions: SelectItem[];
  ledgerAccountData: any = [];
  header: any = [];
  ledgerAccountFilterData: any = [];
  productMasterSelectedData: any = [];
  selectedUploadData: any = {};
  public allowDeletePermission: boolean = true;
  vendorContactsData: any = [];
  vendorContractsData: any = [];
  prevendorContracts: any = [];
  prevendorContacts: any = [];
  vendor: any = {
    id: "",
    name: "",
    title: "",
    address1: "",
    address2: "",
    countryName: "",
    mobile: "",
    email: "",
    telephone: "",
    poBox: "",
    bankCountryCode: "",
    bankCode: "",
    ibanSwifT: "",
    bankAccName: "",
    bankAccNo: "",
    ledgerCode: "",
    vendorContacts: {
      emailId: "",
      mobileNo: "",
      contactName: ""
    },
    vendorContracts: {
      duration: "",
      startDate: "",
      endDate: "",
      paymentTerm: "",
      ledgerCode: "",
      amountToHold: "",
      description: "",
      appDocuments: []
    },
    vendorProducts: {
      productMasterId: ""
    }
  }
  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private dialogService: DialogService,
    private _codeMasterService: CodesMasterService,
    private _translate: TranslatePipe,
    private _purchaseService: PurchaseService,
    private _router: Router,
    private _toastrService: ToastrService,
    private _webApiservice: WebApiService,
    public _commonService: AppCommonService,


  ) {
    super(injector, 'SCR_VENDOR_MASTER', 'allowAdd', _commonService)
    this._commonService.fileObserver.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((file: any) => {
        this.selectedUploadData.appDocuments = file;
      });
  }
  dialogRef: DynamicDialogRef;
  ngOnInit() {

    if (history.state && history.state.vendorId && history.state.vendorId != '') {
      this.id = history.state.vendorId;
    }
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("VENDOR_MASTER_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("VENDOR_MASTER_ADD"));

    this.LoadDefault();

    if (this.id && this.id !== "") {
      this.getall()
    }
  }
  async getall() {
    this.vendor = await this._webApiservice.get("getVendorMasterById/" + this.id);
    if (this.vendor?.vendorContracts) {
      this.prevendorContracts = this.vendor.vendorContracts;
      this.prevendorContacts = this.vendor.vendorContacts;
      this.vendor.vendorContracts.forEach(element => {
        if (element.startDate == "0001-01-01T00:00:00")
          element.startDate = "";
        if (element.endDate == "0001-01-01T00:00:00")
          element.endDate = "";
        var ledgerCodes = this.dataaccount.find(a => a.ledgercode == element.ledgerCode);
        element.ledgerCode = ledgerCodes?.code
        this.vendorContractsData.push({
          duration: element.duration,
          startDate: element.startDate,
          endDate: element.endDate,
          paymentTerm: element.paymentTerm,
          ledgerCode: element.ledgerCode,
          amountToHold: element.amountToHold, appDocuments: element.appDocuments,
          description: element.description, id: element.id,
          createdby: element.createdBy, createdDate: element.createdDate, modifiedBy: element.modifiedBy, modifiedDate: element.modifiedDate
        })
      });
    } else {
      this.vendor.vendorContracts = [];
    }

    if (!this.vendor.vendorContacts) {
      this.vendor.vendorContacts = [];
    }
    else {
      this.vendor.vendorContacts.forEach(element => {

        this.vendorContactsData.push({
          emailId: element.emailId, id: element.id,
          mobileNo: element.mobileNo,
          contactName: element.contactName,
          createdby: element.createdBy, createdDate: element.createdDate, modifiedBy: element.modifiedBy, modifiedDate: element.modifiedDate
        })
      });
    }

    var ledgerCode = this.dataaccount.find(a => a.ledgercode == this.vendor.ledgerCode);
    this.vendor.ledgerCode = ledgerCode?.code
    this.vendor.vendorProducts.forEach(element => {
      var productMaster = this.productMaster.find(a => a.id == element.productMasterId);
      this.ventorProduMaster.push({
        name: productMaster.name, id: productMaster.id
      })
    });
    this.productMasterSelectedData = this.ventorProduMaster
    this.productMasterSelectedData.forEach(element => {
      this.productMaster = this.productMaster.filter(a => a.id !== element.id)
    })
  }
  attachmentLink(data, type) {
    if (this.isGranted('SCR_VENDOR_MASTER', this.actionType.allowDelete)) {
      this.allowDeletePermission = false;
    }
    this.header = this._translate.transform("FILE_ATTACHMENT");
    this.selectedUploadData = data;
    this.uploadConfig =
    {
      TransactionId: data.id,
      TransactionType: type,
      AllowedExtns: ".png,.jpg,.gif,.jpeg,.bmp,.docx,.doc,.pdf,.msg",
      DocTypeRequired: false,
      DocumentReference: "",
      ReadOnly: this.allowDeletePermission,
      ScanEnabled: true,
      ShowSaveButton: false,
      FileContent: data.appDocuments
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
  async LoadDefault() {
    var input = {
      ledgerCodeFrom: "",
      ledgerCodeTo: ""
    }
    this.ledgerAccountData = await this._webApiservice.post("getLedgerAccountList", input);
    this.ledgerAccountData.forEach(element => {
      this.ledgerAccountFilterData.push({
        ledgercode: element.ledgerCode,
        code: element.ledgerCode + '-' + element.ledgerDesc,
        name: element.ledgerDesc
      })
    });
    this.dataaccount = this.ledgerAccountFilterData;
    this.dataaccountOptions = this.dataaccount.map((name) => {
      return { label: name.code, value: name.code }
    });
    this.productMaster = await this._purchaseService.getProductMaster(false, false, this._translate, "", true, false);
    this.PaymentTerm = await this._codeMasterService.getCodesDetailByGroupCode("PAYMENTTERM", false, false, this._translate);
    this.PaymentTermOptions = this.PaymentTerm.map((name) => {
      return { label: name.description, value: name.code }
    });
  }
  VendorCancel() {
    this._router.navigateByUrl("purchase/vendor");
  }
  async addNewContactDetReq() {
    var newContactDetReq =
    {
      emailId: "",
      mobileNo: "",
      contactName: "",
      active: "Y"
    }

    this.vendorContactsData.unshift(newContactDetReq);
  }
  async deleteContactDetReq(item) {
    var index = this.vendorContactsData.indexOf(item);
    if (index >= 0)
      this.vendorContactsData.splice(index, 1);

  }
  async addNewContractDetReq() {
    var newContractDetReq =
    {
      duration: "",
      startDate: "",
      endDate: "",
      paymentTerm: "",
      ledgerCode: "",
      amountToHold: "",
      description: "",
      appDocuments: [],
      active: "Y"
    }

    this.vendorContractsData.unshift(newContractDetReq);
  }
  async deleteContractDetReq(item) {
    var index = this.vendorContractsData.indexOf(item);
    if (index >= 0)
      this.vendorContractsData.splice(index, 1);

  }

  async Vendorsave() {
    if (this.vendor.title == "") {
      this._toastrService.error(this._translate.transform("VENDOR_TITLE_REQ"));
      return;
    }
    if (this.vendor.name == "") {
      this._toastrService.error(this._translate.transform("VENDOR_NAME_REQ"));
      return;
    }
    if (this.vendor.countryName == "") {
      this._toastrService.error(this._translate.transform("VENDOR_COUNTRYNAME_REQ"));
      return;
    }
    if (this.vendor.mobile == "") {
      this._toastrService.error(this._translate.transform("VENDOR_MOBILE_REQ"));
      return;
    }
    if (this.vendor.email == "") {
      this._toastrService.error(this._translate.transform("VENDOR_EMAIL_REQ"));
      return;
    }
    if (this.vendor.telephone == "") {
      this._toastrService.error(this._translate.transform("VENDOR_TELEPHONE_REQ"));
      return;
    }
    if (this.vendor.poBox == "") {
      this._toastrService.error(this._translate.transform("VENDOR_POBOX_REQ"));
      return;
    }
    // if (this.vendor.ledgerCode == "") {
    //   this._toastrService.error(this._translate.transform("VENDOR_POBOX_REQ"));
    //   return;
    // }
    if (this.vendor.address1 == "" || this.vendor.address2 == "") {
      this._toastrService.error(this._translate.transform("VENDOR_ADDRESS_REQ"));
      return;
    }
    this.vendor.vendorProducts = [];
    this.productMasterSelectedData.forEach(element => {
      this.vendor.vendorProducts.push({
        productMasterId: element.id, active: "Y", createdBy: this.vendor.createdBy, createdDate: this.vendor.createdDate,
        modifiedBy: this.vendor.modifiedBy, modifiedDate: this.vendor.modifiedDate,
      })
    });
    this.vendor.id = this.id !== "" ? this.id : "";
    if (this.vendor.ledgerCode) {
      var ledgerCode = this.dataaccount.find(a => a.code == this.vendor.ledgerCode);
      this.vendor.ledgerCode = ledgerCode.ledgercode;
    }
    if (this.vendor.vendorContracts.ledgerCode) {
      var ledgerCode = this.dataaccount.find(a => a.code == this.vendor.vendorContracts.ledgerCode);
      this.vendor.vendorContracts.ledgerCode = ledgerCode.ledgercode;
    }
    this.vendor.active = "Y";
    if (this.vendor.id && this.vendor.id != "") {
      this.vendor.createdBy = this.vendor.createdBy;
      this.vendor.createdDate = this.vendor.createdDate;
      this.vendor.modifiedBy = this.vendor.modifiedBy;
      this.vendor.modifiedDate = this.vendor.modifiedDate;
    }
    this.vendorContractsData.forEach(element => {
      var ledgerCodes = this.dataaccount.find(a => a.code == element.ledgerCode);
      element.ledgerCode = ledgerCodes?.ledgercode
    });
    this.prevendorContacts.forEach(element => {
      var det = this.vendorContactsData.find(a => a.id == element.id);
      if (det) {
        det.modifiedDate = element.modifiedDate;
        det.action = "M"
        det.active = "Y";
      }
      else {
        element.active = "N";
        element.action = "M";
        this.vendorContactsData.push(element);
      }
    });
    this.vendor.vendorContacts = this.vendorContactsData;
    this.prevendorContracts.forEach(element => {
      var det = this.vendorContractsData.find(a => a.id == element.id);
      if (det) {
        det.modifiedDate = element.modifiedDate;
        det.action = "M"
        det.active = "Y";
      }
      else {
        element.active = "N";
        element.action = "M";
        this.vendorContractsData.push(element);
      }
    });
    this.vendor.vendorContracts = this.vendorContractsData;

    var result = await this._webApiservice.post("saveVendor", this.vendor);
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this._router.navigateByUrl("purchase/vendor");
      }
      else
        this._toastrService.error(output.messages[0])
    }

  }

}


