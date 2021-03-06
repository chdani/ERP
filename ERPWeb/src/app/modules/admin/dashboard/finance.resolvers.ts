import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { FinanceServiceOLD } from './financeOld.service';


@Injectable({
    providedIn: 'root'
})
export class FinanceResolver implements Resolve<any>
{
    /**
     * Constructor
     */
    constructor(private _financeService: FinanceServiceOLD)
    {
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Resolver
     *
     * @param route
     * @param state
     */
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any>
    {
        return this._financeService.getData();
    }
}
