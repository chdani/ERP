import { ChangeDetectionStrategy, Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app/core/auth/auth.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { lang } from 'moment';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-user-settings',
    templateUrl: './user-settings.component.html',
    styleUrls: ['./user-settings.component.scss'],
  encapsulation  : ViewEncapsulation.None,
  providers: [ToastrService, TranslatePipe]
})
export class UserSettingsComponent {
  languages: any = [];
  selectedLang: string = "en";
  userData: any;

  constructor(private _webApiservice: WebApiService,
              private _toastrService: ToastrService,
              private _translate: TranslatePipe,
              private _authService: AuthService,
              public _commonService: AppCommonService
            )
  {
    this.loadDefaults();
    this._commonService.updatePageName(this._translate.transform("APP_SETTINGS"));
  }

  async loadDefaults()
  {
    var codes = JSON.parse(localStorage.getItem("appCodeMaster"));
    if (codes && codes.length > 0) {
      var langCode = codes.find(x => x.code == "LANGUAGES")
      
      if (langCode) {
        langCode.codesDetail.forEach(element => {
          this.languages.push(
            {
              description: element.description,
              code: element.code
            }
          )
        });

        var userStorage = localStorage.getItem("LUser")
        if (userStorage) {
          this.userData = JSON.parse(userStorage)
          this.selectedLang = this.userData.userContext.language;
        }
      }
    }
    else {
      this._toastrService.error(this._translate.transform("APP_ERROR"))
    };
  }

  async saveSetting()
  {
    var settings = [
      {
        configKey: "USR_LANG",
        configValue: this.selectedLang,
        active: "Y"
      }
    ];
    var result = await this._webApiservice.post("saveUserSettings", settings);
      if(result) {
        if (!result.validations) {
          this._toastrService.success(this._translate.transform("APP_SUCCESS"));
          this.userData.userContext.language = this.selectedLang;
          this.onsuccess()
        }
        else {
          console.log(result.validations.status + " " + result.validations.messages[0]);
          this._toastrService.error(result.validations.messages[0])
        }
      }
      else {
        this._toastrService.error(this._translate.transform("APP_ERROR"))
      }
  }

  async onsuccess()
  {
    await this._authService.setAppContext(this.userData, true);
    location.reload();
  }
}
