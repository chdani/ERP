import { Injector } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { AppCommonService } from "../services/app-common.service";

export abstract class AppComponentBase {

  _unsubscribeAll: Subject<any> = new Subject<any>();
  spinnerService: NgxSpinnerService;
  router: Router;
  activatedRouter: ActivatedRoute;

  actionType = ActionType;
  userContext: any = {}
  financialYear: string;
  orgId: string;

  constructor(injector: Injector, screenCode, type,
    public _commonService: AppCommonService) {
    this.router = injector.get(Router);
    this.activatedRouter = injector.get(ActivatedRoute);
    this.spinnerService = injector.get(NgxSpinnerService);
    if (screenCode == "SCR_EMB_PRE_PAYMENTS" || screenCode == "SCR_EMB_POST_PAYMENTS" || screenCode == "SCR_DIRECT_INVOICE_POST" ||
      screenCode == "SCR_DIRECT_INVOICE_PRE") {
      var result = this.isCompGranted(screenCode, type);
    } else
      var result = this.isGranted(screenCode, type);
    if (!result) {
      this.router.navigate(['app-management/unauthor']);
    } else {
      this.userContext = JSON.parse(localStorage.getItem("LUser")).userContext;
      _commonService.finYearInfo.pipe((takeUntil(this._unsubscribeAll)))
        .subscribe((finYear: string) => {
          this.financialYear = finYear;
        });
      _commonService.orgInfo.pipe((takeUntil(this._unsubscribeAll)))
        .subscribe((orgId: string) => {
          this.orgId = orgId;
        });
    }


  }

  isGranted(screenCode, type): boolean {
    var result = false;
    if (JSON.parse(localStorage.getItem("LUser")).appMenus) {
      var user_data = JSON.parse(localStorage.getItem("LUser")).appMenus;
      user_data.forEach(module => {
        module.userMenus.forEach(element => {
          if (element.subMenus && element.subMenus.length > 0) {
            var submenu = element.subMenus.find(x => x.screenCode == screenCode);
            if (submenu) {
              if (type == "allowView")
                result = true;
              if (submenu[type] == "Y")
                result = true;
            }
          }
        });
      });
    }
    return result;
  }

  showHideOrgFinyear(subMenuCode) {
    var showFinYear = false;
    var showOrg = false;
    if (JSON.parse(localStorage.getItem("LUser")).appMenus) {
      var user_data = JSON.parse(localStorage.getItem("LUser")).appMenus;
      user_data.forEach(module => {
        module.userMenus.forEach(element => {
          element.subMenus.forEach(subMenu => {
            if (subMenu.subMmenuCode == subMenuCode && subMenu.showFinYear)
              showFinYear = true;
            if (subMenu.subMmenuCode == subMenuCode && subMenu.showOrg)
              showOrg = true;
          });
        });
      });
    }
    this._commonService.showHideFinancialYear(showFinYear);
    this._commonService.showHideOrgId(showOrg);
  }

  isCompGranted(componentCode, type): boolean {
    var result = false;
    if (JSON.parse(localStorage.getItem("LUser")).userRoles) {
      var user_data = JSON.parse(localStorage.getItem("LUser")).userRoles;
      user_data.forEach(role => {
        if (role.accessCode == componentCode) {
          if (type == "allowView")
            result = true;
          if (role[type] == "Y")
            result = true;
        }
      });
    }
    return result;
  }

  plusMinusMonthToCurrentDate(numberOfMonth: number) {
    var currDate = new Date();
    var currMonth = currDate.getMonth();
    var newMonth = currMonth + numberOfMonth;
    if (newMonth < 0) {
      newMonth = 0;
    }
    else if (newMonth > 11)
      newMonth = 11;

    var validDate = false;
    var newDate = currDate;
    var day = currDate.getDate();

    while (!validDate) {
      try {
        newDate = new Date(currDate.getFullYear(), newMonth, day)
        validDate = true;
      } catch (error) {
        day--;
      }
    }
    return newDate;
  }

}
enum ActionType {
  allowDelete = "allowDelete",
  allowAdd = "allowAdd",
  allowEdit = "allowEdit",
  allowView = "allowView",
  allowApprove = "allowApprove"
}
