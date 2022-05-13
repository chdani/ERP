import { Route } from '@angular/router';
import { AuthGuard } from 'app/core/auth/guards/auth.guard';
import { NoAuthGuard } from 'app/core/auth/guards/noAuth.guard';
import { LayoutComponent } from 'app/layout/layout.component';
import { InitialDataResolver } from 'app/app.resolvers';

export const appRoutes: Route[] = [

    {path: '', pathMatch : 'full', redirectTo: 'dashboard'},
    {path: 'signed-in-redirect', pathMatch : 'full', redirectTo: 'dashboard'},
    
    {
        path: '',
        canActivate: [NoAuthGuard],
        canActivateChild: [NoAuthGuard],
        component: LayoutComponent,
        data: {
            layout: 'empty'
        },
        children: [
           {path: 'account', loadChildren: () => import('app/modules/auth/auth.module').then(m => m.AuthModule)}]
    },
    
    {
        path: '',
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        component: LayoutComponent,
        data: {
            layout: 'empty'
        },
        children: [
            {path: 'sign-out', loadChildren: () => import('app/modules/auth/sign-out/sign-out.module').then(m => m.AuthSignOutModule)},
        ]
    },
    {
        path: '',
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        component: LayoutComponent,
        data: {
            layout: 'print'
        },
        children: [
            { path: 'print', loadChildren: () => import('app/modules/common/print-to-pdf/app-print.module').then(m => m.AppPrintModule) },
        ]
    },
 
    // Module routes
    {
        path       : '',
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        component  : LayoutComponent,
        resolve    : {
            initialData: InitialDataResolver,
        },
        children   : [
            { path: 'dashboard', loadChildren: () => import('app/modules/admin/admin.module').then(m => m.AdminModule) },
            { path: 'user-management', loadChildren: () => import('app/modules/user-management/user-managment.module').then(m => m.UserManagmentModule) },
            { path: 'user-profile', loadChildren: () => import('app/modules/Profile/profile.module').then(m => m.ProfileModule) },
            { path: 'finance', loadChildren: () => import('app/modules/finance/finance.module').then(m => m.FinanceModule) },
            {path: 'human-resource',loadChildren:() =>import('app/modules/human-resource/human-resource.module').then(m =>m.HRModule)},
            {path: 'purchase', loadChildren:() =>(import('app/modules/purchase/purchase.module').then(m=>m.PurchaseModule))},
            {path:'app-management', loadChildren:() =>(import('app/modules/app-management/app-management.module').then(m=>m.AppManagementModule))},
        ]
    },
];
