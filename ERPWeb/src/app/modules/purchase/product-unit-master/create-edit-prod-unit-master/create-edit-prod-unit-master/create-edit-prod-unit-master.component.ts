import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { result } from 'lodash';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs/operators';
import { WebApiService } from 'app/shared/webApiService';
import { TranslatePipe } from '@ngx-translate/core';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { UserManagementService } from 'app/modules/user-management/user-management.service';

import { Router } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api';
import { AppComponentBase } from 'app/shared/component/app-component-base';


@Component({
  selector: 'app-create-edit-prod-unit-master',
  templateUrl: './create-edit-prod-unit-master.component.html',
  styleUrls: ['./create-edit-prod-unit-master.component.scss'],
  providers: [
    TranslatePipe
  ]
})
export class CreateEditProdUnitMasterComponent extends AppComponentBase implements OnInit {

  constructor(
    injector:Injector,
    private formBuilder: FormBuilder,
    private spinner: NgxSpinnerService,
    private _toastrService: ToastrService,
    private _webApiService: WebApiService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _userManagementService: UserManagementService,
    private _primengConfig: PrimeNGConfig,
    private _router: Router) {
      super(injector,'SCR_UNIT_MASTER','allowAdd', _commonService )
  }

  formFieldHelpers: string[] = [''];
  roleList: any = [];
  filteredRoles: any = [];
  selectedRole: any;
  userRoleList: any = [];
  userTypeList: any = {};
  checked: boolean = false;
  prodUnitlist: any;
  id: any;
  prodUnitInfo: any = {
    id: "",
    unitCode: "",
    unitName: "",
    conversionUnit: "",
    createdby: "",
    createdate: "",
    modifiedby: "",
    modifieddate: "",
    active: ""
  }

  ngOnInit(): void {
    if (history.state && history.state.id && history.state.id != '') {
      this.id = history.state.id;
    }
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("PRODUCT_UNIT_MASTER_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("PRODUCT_UNIT_MASTER_ADD"));

    if (this.id && this.id !== "") {
      this.getByIdEdit(this.id);
    }


  }
  async getByIdEdit(id) {
    if (id != null) {
      var result = await this._webApiService.get("getProdUnitMasterById/" + id);
      if (result != null) {
        this.prodUnitlist = result as any
        this.prodUnitInfo.id = this.id,
          this.prodUnitInfo.unitCode = this.prodUnitlist.unitCode,
          this.prodUnitInfo.unitName = this.prodUnitlist.unitName,
          this.prodUnitInfo.conversionUnit = this.prodUnitlist.conversionUnit
      }
    }


  }

  async onSubmit() {
    this.prodUnitInfo;
    if (this.prodUnitInfo.unitCode == "") {
      this._toastrService.error(this._translate.transform("PROVIDE_UNIT_CODE"));
    }
    if (this.prodUnitInfo.unitName == "") {
      this._toastrService.error(this._translate.transform("PROVIDE_UNIT_NAME"));
    }
    if (this.prodUnitInfo.conversionUnit == "" && Number(this.prodUnitInfo.conversionUnit) == 0) {
      this._toastrService.error(this._translate.transform("PROVIDE_VALID_CONVERSION_UNIT"));
    }
    else {
      this.prodUnitInfo.active = "Y";
      if (this.prodUnitlist, this.id && this.prodUnitlist.id != "") {
        this.prodUnitInfo.createdBy = this.prodUnitlist.createdBy;
        this.prodUnitInfo.createdDate = this.prodUnitlist.createdDate;
        this.prodUnitInfo.modifiedBy = this.prodUnitlist.modifiedBy;
        this.prodUnitInfo.modifiedDate = this.prodUnitlist.modifiedDate;
      }
      var result = await this._webApiService.post("saveProdUnitMaster", this.prodUnitInfo);

      if (result) {
        var output = result as any;
        if (output.status == "DATASAVESUCSS") {

          this._toastrService.success(this._translate.transform("APP_SUCCESS"));
          this._router.navigateByUrl("purchase/unit-master");
        }
        else {
          this._toastrService.error(output.messages[0])
        }
      }
    }
  }

  cancelAddEdit() {
    this._router.navigateByUrl("purchase/unit-master");
  }
}