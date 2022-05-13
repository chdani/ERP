import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { FuseAlertType } from '@fuse/components/alert';
import { AuthService } from 'app/core/auth/auth.service';
import {DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
@Component({
    selector: 'auth-relogin',
    templateUrl: './relogin.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations,
    providers: [DialogService]
})
export class ReloginComponent  implements  OnInit {
    constructor(
        injector: Injector,
        public _commonService: AppCommonService,
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _formBuilder: FormBuilder,
        private _router: Router,
        private dialogRef: DynamicDialogRef,
       )
        { 
         
        }
        userContext : any = {}

    alert: { type: FuseAlertType; message: string } = {
        type: 'success',
        message: ''
    };
    ReloginForm: FormGroup;
    
    public key:string;
    showAlert: boolean = false;
    public userName: string;

    ngOnInit(): void {
        debugger
        this.userContext = JSON.parse(localStorage.getItem("LUser")).userContext;
        this.ReloginForm = this._formBuilder.group({            
            userName: this.userContext.userName ,           
            password: ['', Validators.required],
           
        });        

        this.userName = this.userContext.firstName + ' ' + this.userContext.lastName;        
    }
    
      async Submit() {
        if (this.ReloginForm.invalid) {
          return;
        }
     
        // Disable the form
        this.ReloginForm.disable();

        // Hide the alert
        this.showAlert = false;
        var userinput = {
            username: this.ReloginForm.value.userName,
            password: this.ReloginForm.value.password,
            }
        
        var result = await this._authService.signIn(userinput);
        if (result) {
            var output = result as any;
            if (output.status) {
                this.dialogRef.close();
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
            // Set the alert
            this.alert = {
                type: 'error',
                message: 'Wrong password'
            };
            // Show the alert
            this.showAlert = true;
        }
        this.ReloginForm.enable();
    }

    logout() {
      this.dialogRef.close();
      this._authService.signOut();      
      this._router.navigate(['account/singIn']);
    }
}
        