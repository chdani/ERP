import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { UserService } from 'app/core/user/user.service';
import { WebApiService } from 'app/shared/webApiService';
import { TranslateService } from '@ngx-translate/core';
import { AppCommonService } from 'app/shared/services/app-common.service';

@Injectable()
export class AuthService {
    private _authenticated: boolean = false;

    /**
     * Constructor
     */
    constructor(
        private _httpClient: HttpClient,
        private _userService: UserService,
        private _webApiservice: WebApiService,
        private _translateService: TranslateService,
        public _commonService: AppCommonService
    ) {
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Setter & getter for access token
     */
    set accessToken(token: string) {
        localStorage.setItem('accessToken', token);
    }

    get accessToken(): string {
        return localStorage.getItem('accessToken') ?? '';
    }


    async signIn(userInput: any) {
        var response = {
            status: false,
            message: ""
        }

        var result = await this._webApiservice.post("userLogin", userInput);
        if (result) {
            var userData = result as any;
            if (!result.validations) {
                response.status = true;
                this.afterLogin(userData);
                //  setTimeout(() => {
                //      this._commonService.updateReloginStatus(true);
                //  }, 3000);
            }
            else {
                console.log(result.validations.messages[0]);
                // Set the alert
                response.message = result.validations.messages[0];
                response.status = false;
            }
        }
        return response;
    }

    afterLogin(userData: any) {
        this.accessToken = userData.userContext.token;
        this._authenticated = true;
        this.setAppContext(userData, false);
    }

    async setAppContext(userData, loadFromDB) {
        if (loadFromDB) {
            var result = await this._webApiservice.get("getUserAccessInfo");
            if (result) {
                var userInfo = result as any;
                userData.appMenus = userInfo.appMenus;
                userData.userRoles = userInfo.userRoles;
            }
        }

        var menuList = Array<any>();
        menuList = [];
        var item = {
            id: 'dashboard',
            title: 'Dashboard',
            type: 'basic',
            link: '/dashboard'
        }
        menuList.push(item);

        try {
            //menuList.push(fun);
            userData.appMenus.forEach(element => {
                var module = {
                    id: element.moduleCode,
                    title: element.moduleName,
                    type: 'group',
                    displayOrder: element.displayOrder,
                    icon: element.moduleIcon,
                }
                var userMenus = Array<any>();

                element.userMenus.forEach(x => {
                    var Mmenu = {
                        id: x.mainMenuCode,
                        title: x.mainMenuName,
                        type: 'collapsable',
                        displayOrder: x.displayOrder,
                        icon: x.mainMenuIcon,
                    }
                    var SmenuList = Array<any>();
                    var subNames = Array<any>();
                    subNames = Array.from(x?.subMenus.reduce((m, t) => m.set(t.subMmenuCode, t), new Map()).values());
                    subNames.forEach(y => {
                        var Smenau = {
                            id: y.subMmenuCode,
                            title: y.subMenuName,
                            type: 'basic',
                            link: y.screenUrl,
                            icon: y.subMenuIcon
                        }
                        SmenuList.push(Smenau)
                    });
                    Mmenu["children"] = SmenuList;
                    userMenus.push(Mmenu);
                });
                module["children"] = userMenus;
                menuList.push(module);
            });
        }
        catch (e) {

        }
        userData["MenuList"] = menuList;
        this._userService.user = userData;
        localStorage.setItem("LUser", JSON.stringify(userData))
        var language = userData.userContext.language;
        if (!language) language = "en";
        this._translateService.use(language);
        this._commonService.updateLanguage();

        //await this.getOrganizationListForUser();
        localStorage.setItem("appOrgs", JSON.stringify(userData.userContext.organizations));

        var input = { language: language };
        result = await this._webApiservice.post("FetchAllCodesMasterDataLangBased", input);
        if (result && !result.validation) {
            var codesMaster = result as any;
            localStorage.setItem("appCodeMaster", JSON.stringify(codesMaster));
        }

    }

    // async getOrganizationListForUser()
    // {
    //     var orgs = [];
    //     if (localStorage.getItem("appOrgs"))
    //         orgs = JSON.parse(localStorage.getItem("appOrgs"));
    //     else {

    //         var userData = JSON.parse(localStorage.getItem("LUser")) as any;
    //         var userId = userData.userContext.id;

    //         var result = await this._webApiservice.get("GetUserOrganizations/" + userId);
    //         if (result && !result.validation) {
    //             orgs = result as any;
    //             localStorage.setItem("appOrgs", JSON.stringify(orgs));
    //         }
    //     }
    //     return orgs;
    // }
    // async getUserOrganizationList(userId)
    // {
    //     var orgs = [];

    //     if (localStorage.getItem("appLUserOrgs"))
    //         orgs = JSON.parse(localStorage.getItem("appLUserOrgs"));
    //     else {
    //         var result = await this._webApiservice.get("GetUserOrganizations/"+userId);
    //         if (result && !result.validation) {
    //             orgs = result as any;
    //             localStorage.setItem("appLUserOrgs", JSON.stringify(orgs));
    //         }
    //     }
    //     return orgs;
    // }
    /**
     * Sign out
     */
    signOut(): Observable<any> {
        // Remove the access token from the local storage
        localStorage.clear();
        // Set the authenticated flag to false
        this._authenticated = false;

        // Return the observable
        return of(true);
    }


    /**
     * Check the authentication status
     */
    check(): Observable<boolean> {
        // Check if the user is logged in
        if (this._authenticated) {
            return of(true);
        }

        // Check the access token availability
        if (!this.accessToken) {
            return of(false);
        }

        return of(true);
    }
}
