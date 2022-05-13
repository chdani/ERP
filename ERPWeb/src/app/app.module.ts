import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ExtraOptions, PreloadAllModules, RouterModule } from '@angular/router';
import { MarkdownModule } from 'ngx-markdown';
import { FuseModule } from '@fuse';
import { FuseConfigModule } from '@fuse/services/config';
import { FuseMockApiModule } from '@fuse/lib/mock-api';
import { CoreModule } from 'app/core/core.module';
import { appConfig } from 'app/core/config/app.config';
import { mockApiServices } from 'app/mock-api';
import { LayoutModule } from 'app/layout/layout.module';
import { AppComponent } from 'app/app.component';
import { appRoutes } from 'app/app.routing';
import { NgxSpinnerModule } from 'ngx-spinner';
import { HttpClient } from '@angular/common/http';
import { TranslateLoader, TranslateModule } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { FileSaverModule } from 'ngx-filesaver';
import { ReloginComponent } from './modules/auth/relogin/relogin.component';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FuseCardModule } from '@fuse/components/card';
import { FuseAlertModule } from '@fuse/components/alert';
import { SharedModule } from 'primeng/api';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FileModule } from './modules/common/file-upload/file-upload.module';
import { DWTModule } from './shared/component/dwt/dwt.component.module';

const routerConfig: ExtraOptions = {
    preloadingStrategy       : PreloadAllModules,
    scrollPositionRestoration: 'enabled'
};

@NgModule({
    declarations: [
        AppComponent,
        ReloginComponent
    ],
    imports     : [
        BrowserModule,
        BrowserAnimationsModule,
        CommonModule,
        ReactiveFormsModule,
        RouterModule.forRoot(appRoutes, routerConfig),
        NgxSpinnerModule,
        // Fuse, FuseConfig & FuseMockAPI
        FuseModule,
        FuseConfigModule.forRoot(appConfig),
        FuseMockApiModule.forRoot(mockApiServices),

        // Core module of your application
        CoreModule,

        // Layout module of your application
        LayoutModule,

        // 3rd party modules that require global configuration via forRoot
        MarkdownModule.forRoot({}),

        FormsModule,
        MatButtonModule,
        MatCheckboxModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatProgressSpinnerModule,
        FuseCardModule,
        FuseAlertModule,
        SharedModule,

        FileSaverModule,
        //Tranlation
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: createTranslateLoader,
                deps: [HttpClient],
            },
        }),
        FileModule,
        DWTModule
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA], 
    bootstrap   : [
        AppComponent
    ],
    //ng build --base-href /web/
    // providers: [{ provide: APP_BASE_HREF, useValue: '/Web' }] 
})
export class AppModule
{
}
export function createTranslateLoader(http: HttpClient) {
    return new TranslateHttpLoader(http, "./assets/i18n/", ".json");
}

