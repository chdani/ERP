import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormGroupDirective, FormGroup, FormControl, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';


/**
 * @title Stepper overview
 */
@Component({
  selector: 'app-warehouse',
  templateUrl: './create-edit-warehouse.component.html',
  styleUrls: ['./create-edit-warehouse.component.scss'],
  providers: []


})
export class CreateEditWareHouseComponent extends AppComponentBase implements OnInit, OnDestroy {
  submitted = false;
  validation: boolean = true;
  Location: any = [];
  warehouse: any = [];
  LocationDet: any = [];
  id: any;

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private _confirmService: ConfirmationService,
    private _translate: TranslatePipe,
    private _router: Router,
    private _toastrService: ToastrService,
    private _webApiservice: WebApiService,
    public _commonService: AppCommonService,

  ) { super(injector, 'SCR_WAREHOUSE', 'allowAdd', _commonService) }

  ngOnInit() {
    this.warehouse = {
      id: "",
      name: "",
      email: "",
      contactNo: "",
      address: "",
    }
    if (history.state && history.state.wareHouseId && history.state.wareHouseId != '') {
      this.id = history.state.wareHouseId;
    }
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("WAREHOUSE_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("WAREHOUSE_ADD"));
    this.getall();
  }
  async getall() {
    if (this.id && this.id !== "") {
      this.warehouse = await this._webApiservice.get("getWareHouseById/" + this.id);
      this.LocationDet = this.warehouse.wareHouseLocation;
      this.warehouse.wareHouseLocation.forEach(element => {
        this.Location.push({
          name: element.name, email: element.email, contactNo: element.contactNo, address: element.address, active: "Y", createdBy: element.createdBy,
          createdDate: element.createdDate, modifiedBy: element.modifiedBy, modifiedDate: element.modifiedDate, id: element.id
        })
      })
    }
  }
  addNewLocation() {
    var newLocation =
    {
      name: "",
      email: "",
      contactNo: "",
      address: "",
      active: "Y"
    }
    this.Location.unshift(newLocation);
  }
  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
  deleteLocation(item1) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        this.Location.forEach((item, index) => {
          if (item === item1) this.Location.splice(index, 1);
        });
      }
    });
  }

  WareHouseCancel() {
    this._router.navigateByUrl("purchase/warehouse");
  }
  async warehousesave() {
    this.submitted = true;
    this.validation = true;
    if (this.warehouse.name == "") {
      this._toastrService.error(this._translate.transform("WAREHOUSE_NAME_REQ"));
      return;
    }
    if (this.warehouse.address == "") {
      this._toastrService.error(this._translate.transform("WAREHOUSE_ADDRESS_REQ"));
      return;
    }
    if (this.Location.length == 0) {
      this._toastrService.error(this._translate.transform("WAREHOUSELOCATION_REQ"));
      return;
    } else {
      this.Location.forEach(element => {
        if (element.name == "" || element.address == "")
          this.validation = false;
      })
    }
    if (this.validation) {
      this.warehouse.active = "Y";
      this.LocationDet.forEach(element => {
        var det = this.Location.find(a => a.id == element.id);
        if (det) {
          det.modifiedDate = element.modifiedDate;
          det.action = "M"
        }
        else {
          element.active = "N";
          element.action = "M";
          this.Location.push(element);
        }
      });
      this.warehouse.wareHouseLocation = this.Location;
      const status = this.warehouse.wareHouseLocation.some(user => {
        let counter = 0;
        for (const iterator of this.warehouse.wareHouseLocation) {
          if (iterator.address === user.address) {
            counter += 1;
          }
        }
        return counter > 1;
      });
      if (!status) {
        if (this.warehouse.id && this.warehouse.id != "") {
          this.warehouse.createdBy = this.warehouse.createdBy;
          this.warehouse.createdDate = this.warehouse.createdDate;
          this.warehouse.modifiedBy = this.warehouse.modifiedBy;
          this.warehouse.modifiedDate = this.warehouse.modifiedDate;
        }
        var result = await this._webApiservice.post("saveWareHouse", this.warehouse);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this._router.navigateByUrl("purchase/warehouse");
          }
          else
            this._toastrService.error(output.messages[0])
        }

      } else {
        this._toastrService.error(this._translate.transform("ALREADY_EXCITING_ADDRESS"));
      }
    }
  }
}