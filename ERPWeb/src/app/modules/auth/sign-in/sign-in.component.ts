import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { FuseAlertType } from '@fuse/components/alert';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from 'app/core/auth/auth.service';
import { User } from 'app/shared/model/user.model';
import { WebApiService } from 'app/shared/webApiService';
import { cond } from 'lodash';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ReloginComponent } from '../relogin/relogin.component';

@Component({
    selector: 'auth-sign-in',
    templateUrl: './sign-in.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations,
    providers: [TranslatePipe, DialogService]
})
export class AuthSignInComponent implements OnInit {
    @ViewChild('signInNgForm') signInNgForm: NgForm;

    alert: { type: FuseAlertType; message: string } = {
        type: 'success',
        message: ''
    };
    signInForm: FormGroup;
    showAlert: boolean = false;    
    SSOLoader:boolean = false;
    dialogRef: DynamicDialogRef;
    /**
     * Constructor
     */
    constructor(
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _formBuilder: FormBuilder,
        private _router: Router,
        private _webApiservice: WebApiService,
        private dialogService: DialogService,
        
    ) {
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        // Create the form
        this.signInForm = this._formBuilder.group({
            email: ['', [Validators.required]],
            password: ['', Validators.required],
            rememberMe: ['']
        });
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    async SSOLogin(){
        var result = await this._webApiservice.get("userLoginWindows");
        if (result) {
            var userData = result as any;
            if (!result.validations) {
                this._authService.afterLogin(userData);
                this.navigateAfterLogin();
                return;
            }
        }
        else {
            if (result.validations.status == "UNHDLDEX") {
                console.log(result.validations.messages[0]);
                // Set the alert
                this.alert = {
                    type: 'error',
                    message: result.validations.messages[0]
                };
            }
        }

        // Set the alert
        this.alert = {
            type: 'error',
            message: 'Invalid Credentials'
        };

        // Show the alert
        this.showAlert = true;
    }

    /**
     * Sign in
     */
    async userLogin() {
        debugger
        if (this.signInForm.invalid) {
            return;
        }

        // Disable the form
        this.signInForm.disable();

        // Hide the alert
        this.showAlert = false;
        var userinput = {
            username: this.signInForm.value.email,
            password: this.signInForm.value.password,
        }
        var result = await this._authService.signIn(userinput);
        if (result) {
            var output = result as any;
            if (output.status) {
                this.navigateAfterLogin();
                return;
            }
            else {
                this.alert = {
                    type: 'error',
                    message: output.message
                };
                // Show the alert
                this.showAlert = true;
            }
        }
        else {
            // Reset the form
            this.signInNgForm.resetForm();

            // Set the alert
            this.alert = {
                type: 'error',
                message: 'Wrong username or password'
            };

            // Show the alert
            this.showAlert = true;
        }
        // Re-enable the form
        this.signInForm.enable();
    }

    navigateAfterLogin()
    {
        const redirectURL = this._activatedRoute.snapshot.queryParamMap.get('redirectURL') || '/signed-in-redirect';
        this._router.navigateByUrl(redirectURL).then(success => console.log(`routing status: ${success}`));;
    }
}
