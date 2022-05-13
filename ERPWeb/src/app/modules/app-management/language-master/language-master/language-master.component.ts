import { Component, Injector, OnInit } from '@angular/core';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe } from '@ngx-translate/core';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { ConfirmationService } from 'primeng/api';
import { AppComponentBase } from 'app/shared/component/app-component-base';

@Component({
  selector: 'app-language-master',
  templateUrl: './language-master.component.html',
  styleUrls: ['./language-master.component.scss'],
  providers: [
    TranslatePipe,
    ConfirmationService
  ],
})
export class LanguageMasterComponent extends AppComponentBase implements OnInit {

  constructor(
    injector:Injector,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _confirmService: ConfirmationService,
  ) {super(injector,'SCR_TRANSLATION','allowView', _commonService ) }
  getAllTranslation: any = [];
  translation: any = [];
  codeBasedEnglishAndArabic: any = [];
  selectedTranslation: any = [];
  translationInfo: any = [];
  codesDetailsData: any = [];
  descriptionInArabic: any;
  displayOrderInArabic: any;
  hideEngAndArbDisplayOrder: boolean = true;
  hideSaveButton: boolean = true;
  ngOnInit(): void {
    this._commonService.updatePageName(this._translate.transform("LANGUAGE_MASTER"));
    this.translationInfo = {
      code: "",
      codeType: ""
    }
    this.codeBasedEnglishAndArabic = {
      id: "",
      codesBasedList: "",
      code: "",
      englishDescription: "",
      englishDisplayOrder: "",
      arabicDescription: "",
      arabicDisplayOrder: "",
    }
    this.loadDefault();
  }
  async loadDefault() {
    this.codeBasedEnglishAndArabic = [];
    this.translation = [];
    this.hideSaveButton == true;
    this.translationInfo.codeType = "";
    this.translationInfo.code = "LANGUAGEMODELTYPE";
    var result = await this._webApiService.post('getAllCodesMasterListByCode', this.translationInfo);
    this.getAllTranslation = result;
    this.getAllTranslation.forEach(transData => {
      transData.codesDetail.forEach(translationDetails => {
        this.translation.push(translationDetails);
      });
    });
  }
  async onSelectTranslation() {
    if (this.hideSaveButton == true) {
      this.loadLanguageData();
    }
    else {
      this._confirmService.confirm({
        message: this._translate.transform("DO_YOU_WANT_TO_SAVE_THE_CHANGES"),
        accept: async () => {
          this.codesMastersave();
        },
        reject: async () => {
          this.loadLanguageData();
        }
      });
    }

  }
  async loadLanguageData() {
    this.hideSaveButton = true;
    this.codeBasedEnglishAndArabic = [];
    this.hideEngAndArbDisplayOrder = true;
    this.translationInfo.codeType = this.selectedTranslation.code;
    this.codeBasedEnglishAndArabic = await this._webApiService.post('getTranslationDataByCodeType', this.translationInfo);

    if (this.codeBasedEnglishAndArabic != null) {
      if (this.translationInfo.codeType == "CODESMASTERDETAILS") {
        this.hideEngAndArbDisplayOrder = false;
      }
      if (this.translationInfo.codeType == "MENUS") {
        this.hideEngAndArbDisplayOrder = false;
      }
      if (this.translationInfo.codeType == "SUBMENUS") {
        this.hideEngAndArbDisplayOrder = false;
      }

    }
  }
  async codesMastersave() {
    var result = await this._webApiService.post('saveTranslationData', this.codeBasedEnglishAndArabic);
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("SAVE_SUCCESS"));
        this.loadLanguageData();
      }
      else {
        this._toastrService.error(output.messages[0])
      }
    }
  }
  saveFuctionAllow() {
    this.hideSaveButton = false;
  }

}
