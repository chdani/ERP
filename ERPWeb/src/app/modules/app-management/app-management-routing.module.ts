import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CodesMasterComponent } from './codes-master/codes-master.component';
import { CreateEditCodesMasterComponent } from './codes-master/create-edit-codes-master/create-edit-codes-master.component';
import { LanguageMasterComponent } from './language-master/language-master/language-master.component';
import { SystemSettingsComponent } from './system-settings/system-settings.components';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
 
const routes: Routes = [
  {
    path:'unauthor',
    component:UnauthorizedComponent,
  },
  {
    path: 'app-codes-master',
    component: CodesMasterComponent,
  },
  {
    path: 'create-edit-codes-master',
    component: CreateEditCodesMasterComponent,
  },
  {
    path: 'app-translation',
    component: LanguageMasterComponent,
  },
  {
    path: 'app-system-setting',
    component: SystemSettingsComponent,
  },
 
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppManagementRoutingModule { }
