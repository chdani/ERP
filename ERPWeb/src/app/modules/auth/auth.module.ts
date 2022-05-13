import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Route, RouterModule } from '@angular/router';
import { FuseAlertModule } from '@fuse/components/alert';
import { FuseCardModule } from '@fuse/components/card';
import { SharedModule } from 'app/shared/shared.module';
import { AuthResetpwdComponent } from './reset-password/reset-password.component';
import { AuthSignInComponent } from './sign-in/sign-in.component';

const authRouting: Route[] = [
    {path: '', pathMatch : 'full', redirectTo: 'singIn'},
    {path: 'singIn', component: AuthSignInComponent },
    {path:'reset-password', component:AuthResetpwdComponent },
];

@NgModule({
    declarations: [
        AuthSignInComponent ,
        AuthResetpwdComponent,
    ],
    imports     : [
        RouterModule.forChild(authRouting),
        MatButtonModule,
        MatCheckboxModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatProgressSpinnerModule,
        FuseCardModule,
        FuseAlertModule,
        SharedModule
    ]
})
export class AuthModule
{
}
