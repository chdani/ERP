import { Component, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Subject } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
    selector     : 'empty-layout',
    templateUrl  : './empty.component.html',
    encapsulation: ViewEncapsulation.None,
    providers: [TranslatePipe]
})
export class EmptyLayoutComponent implements OnDestroy
{
    private _unsubscribeAll: Subject<any> = new Subject<any>();

    /**
     * Constructor
     */
    constructor()
    {
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On destroy
     */
    ngOnDestroy(): void
    {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
    }
}
