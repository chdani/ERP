import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { FuseAlertType } from '@fuse/components/alert';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'auth-reset-password-in',
    templateUrl: './reset-password.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class AuthResetpwdComponent implements OnInit {
    constructor(
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _formBuilder: FormBuilder,
        private spinner: NgxSpinnerService,
        private _router: Router,
        private _webApiservice: WebApiService,
       )
        {}
    @ViewChild('resetNgForm') resetNgForm: NgForm;

    alert: { type: FuseAlertType; message: string } = {
        type: 'success',
        message: ''
    };
    resetpwdForms: FormGroup;
    public key:string;
    showAlert: boolean = false;
    resetpwd:boolean =false;
    User: any = { 
        id:'',userName: '', password: '',  conformpassword: '', 
      };
    
    ngOnInit(): void {
        this.resetpwdForms = this._formBuilder.group({  
            id:[''],
            userName:[''],          
            password: ['', Validators.required],
            conformpassword:['', Validators.required],
           
        }); 
        this._activatedRoute.queryParams.forEach(params => {
        this.key = params.data; 
            this.spinner.show(); 
        let results =  this._webApiservice.getObserver('ResetPassword/' + this.key)          
           .pipe(finalize(()=>{this.spinner.hide();})).subscribe(result=>{
               if(result.userName==null)
               {
                   this.resetpwd =true;
               }else{
                this.resetpwd =false;
               }              
            this.resetpwdForms.setValue({
                id:result.id,
                userName:result.userName,   
                password:"", 
                conformpassword:"",                       
              });
            }, error=>{});  
            
        });
       
        
        
    }
    async Submit() {
        var pwd =  this.resetpwdForms.value.conformpassword;
       var userinput = {
        password: this.resetpwdForms.value.password,
        id :this.resetpwdForms.value.id,
    } 
    if(pwd == userinput.password){
        var result = await this._webApiservice.post("RePassword", userinput)
        this._authService.signOut();
        this._router.navigate(['account/singIn']);
                                
       
    }else{
        var valid = "Invalied password"
        if (pwd != userinput.password) {
            console.log(valid[0]);
            this.alert = {
                type: 'error',
                message: valid[0]
            };
            this.showAlert = true;
        }
    }
    this.resetpwdForms.enable();
    this.alert = {
        type: 'error',
        message: 'password and conform password dose not match'
    };
    this.showAlert = true;
    }
}
