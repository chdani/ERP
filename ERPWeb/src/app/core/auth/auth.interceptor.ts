import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { AppCommonService } from 'app/shared/services/app-common.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor
{
    /**
     * Constructor
     */
    constructor(private _authService: AuthService,
        private _webapiService: WebApiService,
        public _commonService: AppCommonService
    )
    {
    }

    /**
     * Intercept
     *
     * @param req
     * @param next
     */
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>>
    {
        // Clone the request object
        let newReq = req.clone();

        // Request
        //
        // If the access token didn't expire, add the Authorization header.
        // We won't add the Authorization header if the access token expired.
        // This will force the server to return a "401 Unauthorized" response
        // for the protected API routes which our response interceptor will
        // catch and delete the access token from the local storage while logging
        // the user out from the app.
        if ( this._authService.accessToken  )
        {
            newReq = req.clone({
                headers: req.headers.set('Authorization', 'Bearer ' + this._authService.accessToken)
            });
        }

        // Response
        return next.handle(newReq).pipe(
            catchError((error) => {

                // Catch "401 Unauthorized" responses
                if ( error instanceof HttpErrorResponse && error.status === 401 )
                {
                    // Sign out
                    // this._authService.signOut();

                    // // Reload the app
                    // location.reload();
                    this._commonService.updateReloginStatus(true);

                }
                else 
                    alert("Unexpected Error Occured. Please contact IT!!!");
                
                this._webapiService.showHideProgress(false);
                console.log(error);
                return throwError(error);
            })
        );
    }
}
