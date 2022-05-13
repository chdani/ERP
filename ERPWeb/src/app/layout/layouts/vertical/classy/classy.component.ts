import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FuseMediaWatcherService } from '@fuse/services/media-watcher';
import { FuseNavigationService, FuseVerticalNavigationComponent } from '@fuse/components/navigation';
import { Navigation } from 'app/core/navigation/navigation.types';
import { NavigationService } from 'app/core/navigation/navigation.service';
import { User } from 'app/core/user/user.types';
import { UserService } from 'app/core/user/user.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { JsonpClientBackend } from '@angular/common/http';
import { AuthService } from 'app/core/auth/auth.service';

@Component({
    selector     : 'classy-layout',
    templateUrl: './classy.component.html',
    styleUrls: ['./classy.component.scss'],
    encapsulation: ViewEncapsulation.Emulated
})
export class ClassyLayoutComponent implements OnInit, OnDestroy
{
    isScreenSmall: boolean;
    navigation: Navigation;
    user: User;
    public pageName: string = "";
    public direction: string = "ltr";
    private _unsubscribeAll: Subject<any> = new Subject<any>();
    public selectedFinYear: string = "";
    public selectedOrgId: string = "";
    public availableYears: any = [];
    public organizations: any = [];
    public showHidefinancialYear: any = [];
    public showHideorgId: any = [];
    financialYear: boolean;
    orgId: boolean;
    /**
     * Constructor
     */
    constructor(
        private _navigationService: NavigationService,
        private _userService: UserService,
        private _fuseMediaWatcherService: FuseMediaWatcherService,
        private _fuseNavigationService: FuseNavigationService,
        public _commonService: AppCommonService,
        private _signInService: AuthService
    )
    {
        _commonService.showHidefinYearInfo.pipe((takeUntil(this._unsubscribeAll)))
        .subscribe((finYear: boolean) => {
          this.showHidefinancialYear = finYear;
        });
    _commonService.showHideorgInfo.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((orgId: boolean) => {
        this.showHideorgId = orgId;
      });
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Getter for current year
     */
    get currentYear(): number
    {
        return new Date().getFullYear();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void
    {
        this.getOrganizations();
        this.showHidefinancialYear=false;
        this.showHideorgId=false;

    }

    async getOrganizations()
    {
        var orgs =   JSON.parse(localStorage.getItem("appOrgs"));
        if (orgs && orgs.length > 0)
        {
            orgs.forEach(element => {
                this.organizations.push({
                    id : element.id,
                    orgCode : element.orgCode,
                    orgName: element.orgName,
                });
                
            });
            this.selectedOrgId = orgs[0].id;
        }
        this.loadDefaults();
    }

    loadDefaults() {
        var currYear = this.currentYear;
        this.availableYears.push({
            code: (currYear - 1).toString(),
            value: (currYear - 1).toString()
        });
        this.availableYears.push({
            code: currYear.toString(),
            value: currYear.toString()
        });
        this.availableYears.push({
            code: (currYear + 1).toString(),
            value: (currYear + 1).toString()
        });
        // Subscribe to navigation data
        this._navigationService.navigation$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((navigation: Navigation) => {
                this.navigation = navigation;
            });

        // Subscribe to the user service
        this._userService.user$
            .pipe((takeUntil(this._unsubscribeAll)))
            .subscribe((user: User) => {
                this.user = user;
            });

        // Subscribe to media changes
        this._fuseMediaWatcherService.onMediaChange$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe(({ matchingAliases }) => {

                // Check if the screen is small
                this.isScreenSmall = !matchingAliases.includes('md');
            });

        this._commonService.pageNameInfo.pipe((takeUntil(this._unsubscribeAll)))
            .subscribe((page: string) => {
                this.pageName = page;
            });

        this._commonService.directionInfo.pipe((takeUntil(this._unsubscribeAll)))
            .subscribe((appdirection: string) => {
                this.direction = appdirection;
            });

        var finYear = sessionStorage.getItem("currentFinYear")
        if (finYear && finYear != "")
            this.selectedFinYear = finYear;
        else {
            var currentDate = new Date();
            this.selectedFinYear = currentDate.getFullYear().toString();
            sessionStorage.setItem("currentFinYear", this.selectedFinYear);
        }
        this._commonService.updateAppFinancialYear(this.selectedFinYear);

        var orgId = sessionStorage.getItem("currentOrgId");
        if (orgId && orgId != "" && orgId != "00000000-0000-0000-0000-000000000000")
            this.selectedOrgId = orgId;
        else if (localStorage.getItem("appOrgs")) {
            var orgs = JSON.parse(localStorage.getItem("appOrgs"));
            this.selectedOrgId = orgs[0].id;
            sessionStorage.setItem("currentOrgId", this.selectedOrgId);
        }
        this._commonService.updateCurrentOrgId(this.selectedOrgId);

    }

    updateFinYear()
    {
        sessionStorage.setItem("currentFinYear", this.selectedFinYear);
        this._commonService.updateAppFinancialYear(this.selectedFinYear);
        location.reload();
    }

    updateOrgId()
    {
        sessionStorage.setItem("currentOrgId", this.selectedOrgId);
        this._commonService.updateCurrentOrgId(this.selectedOrgId);
        location.reload();
    }
    /**
     * On destroy
     */
    ngOnDestroy(): void
    {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Toggle navigation
     *
     * @param name
     */
    toggleNavigation(name: string): void
    {
        // Get the navigation
        const navigation = this._fuseNavigationService.getComponent<FuseVerticalNavigationComponent>(name);

        if ( navigation )
        {
            // Toggle the opened status
            navigation.toggle();
        }
    }
}
