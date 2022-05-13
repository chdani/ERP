import { Component, OnInit, Injector, ViewEncapsulation } from '@angular/core';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
@Component({
  selector: 'app-system-setting',
  templateUrl: './system-settings.component.html',
  styleUrls: ['./system-settings.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    TranslatePipe
  ],
})
export class SystemSettingsComponent extends AppComponentBase implements OnInit {
  systemSettingsInfo: any = [];
  allowSystemSettingsSave: boolean = true;
  language: any;
  selectedIndex: any;
  constructor(
    injector: Injector,
    private _translate: TranslatePipe,
    private _toastrService: ToastrService,
    private _webApiservice: WebApiService,
    public _commonService: AppCommonService,
  ) {
    super(injector, 'SCR_SYSTEM_SETTING', 'allowView', _commonService)
  }

  ngOnInit(): void {
    this._commonService.updatePageName(this._translate.transform("SYSTEM_SETTINGS"));
    this.showHideOrgFinyear('SCR_SYSTEM_SETTINGS');
    this.getSearch();
  }
  async getSearch() {
    var userData = JSON.parse(localStorage.getItem("LUser")) as any;
    var language = userData.userContext.language;
    if (!language) language = "en";

    var input = { language: language };
    this.systemSettingsInfo = await this._webApiservice.post("getLangBasedDataForSystemSettings", input);
  }

  async systemSettingsSave() {

    var result = await this._webApiservice.post('saveSystemSettings', this.systemSettingsInfo);
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("SAVE_SUCCESS"));
        this.allowSystemSettingsSave = true;
        await this.getSearch();
      }
      else {
        this._toastrService.error(output.messages[0])
      }
    }
  }

  saveFuctionAllow() {
    this.allowSystemSettingsSave = false;
  }
  async onTabChanged(event) {
    if (event.index == 0) {
      await this.getSearch();
    }
  }

}



