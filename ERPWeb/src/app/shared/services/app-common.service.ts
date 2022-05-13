import { Injectable } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable({ providedIn: "root" })

export class AppCommonService {
     userData: any;
     private pageName: string = "";
     private direction: string = "";
     private financialYear: string = "";
     private organizatioId: string = "";
     private relogin: boolean = false;
     public showHidefinancialYear: boolean = false;
     public showHideorganizatioId: boolean = false;
     private fileToUpload: any = [];

     private pageNameSub = new BehaviorSubject(this.pageName);
     public pageNameInfo: Observable<any> = this.pageNameSub.asObservable();

     private directionSubject = new BehaviorSubject(this.direction);
     public directionInfo: Observable<any> = this.directionSubject.asObservable();

     private finYearSub = new BehaviorSubject(this.financialYear);
     public finYearInfo: Observable<any> = this.finYearSub.asObservable();

     private orgSub = new BehaviorSubject(this.organizatioId);
     public orgInfo: Observable<any> = this.orgSub.asObservable();

     private showHidefinYearSub = new BehaviorSubject(this.showHidefinancialYear);
     public showHidefinYearInfo: Observable<any> = this.showHidefinYearSub.asObservable();

     private showHideorgSub = new BehaviorSubject(this.showHideorganizatioId);
     public showHideorgInfo: Observable<any> = this.showHideorgSub.asObservable();


     private reloginSub = new BehaviorSubject(this.relogin);
     public reloginObserver: Observable<any> = this.reloginSub.asObservable();

     private fileSub = new BehaviorSubject(this.fileToUpload);
     public fileObserver: Observable<any> = this.fileSub.asObservable();


     constructor(
          private _translateService: TranslateService
     ) { }

     updatePageName(config: string) {
          this.pageNameSub.next(config);
     }

     updateAppDirection(appDirection: string) {
          this.directionSubject.next(appDirection);
     }

     updateAppFinancialYear(finYear: string) {
          this.finYearSub.next(finYear);
     }

     updateCurrentOrgId(orgId: string) {
          this.orgSub.next(orgId);
     }

     showHideFinancialYear(finYear: boolean) {
          this.showHidefinYearSub.next(finYear);
     }

     showHideOrgId(orgId: boolean) {
          this.showHideorgSub.next(orgId);
     }


     updateReloginStatus(openWindow: boolean) {
          this.reloginSub.next(openWindow);
     }
     updateFileStatus(files: any) {
          this.fileSub.next(files);
     }
     updateLanguage() {
          var language = "en";

          var userStorage = localStorage.getItem("LUser")
          if (userStorage) {
               this.userData = JSON.parse(userStorage)
               language = this.userData.userContext.language;
          }
          if (language == "ar") {
               this.direction = "rtl";
          } else {
               this.direction = "ltr";
          }
          this._translateService.use(language);

          this.updateAppDirection(this.direction);
     }




}
