import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'app/shared/shared.module';
import { EmptyLayoutComponent } from 'app/layout/layouts/empty/empty.component';
import {  ButtonModule } from 'primeng/button';
import {  ToolbarModule } from 'primeng/toolbar';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
    declarations: [
        EmptyLayoutComponent
    ],
    imports     : [
        RouterModule,
        SharedModule,
    ],
    exports     : [
        EmptyLayoutComponent
    ]
})
export class EmptyLayoutModule
{
}
