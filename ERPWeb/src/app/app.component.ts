import { Component } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ReloginComponent } from './modules/auth/relogin/relogin.component';
import { AppCommonService } from './shared/services/app-common.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    providers: [DialogService, TranslatePipe]
})
export class AppComponent {
    /**
     * Constructor
     */
    userData: any;
    dialogRef: DynamicDialogRef;
    public direction = "ltr";
    //public showRelogin = false;
    _unsubscribeAll: Subject<any> = new Subject<any>();

    constructor(private _translateService: TranslateService,
        public _commonService: AppCommonService,
        private dialogService: DialogService,
        private translate: TranslatePipe
    ) {
        this._commonService.updateLanguage();
        this._commonService.directionInfo.pipe((takeUntil(this._unsubscribeAll)))
            .subscribe((appdirection: string) => {
                this.direction = appdirection;
            });

        this._commonService.reloginObserver.pipe((takeUntil(this._unsubscribeAll)))
            .subscribe((relogin: boolean) => {
                if (relogin)
                    this.showRelogin();
            });
    }

    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
    }

    showRelogin() {
        // this._authService.signOut();
        this.dialogRef = this.dialogService.open(ReloginComponent, {
            header: this.translate.transform("APP_SESSION_EXPIRED"),
            width: "450px",
            closable: false,
            contentStyle: { "height": "300px", overflow: "auto" },
            baseZIndex: 500
        });

    }
}
