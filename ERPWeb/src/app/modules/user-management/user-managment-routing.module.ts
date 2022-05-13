import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserRoleComponent } from './user-role/user-role.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';
import { UserComponent } from './user/user.component';
import {CreateEditUserRoleComponent } from './user-role/create-edit-user-role/create-edit-user-role.component'
import {CreatEditUserComponent } from './user/creat-edit-user/creat-edit-user.component'

const routes: Routes = [
  {
    path: 'user',
    component: UserComponent,
  },
  {
    path: 'user-role',
    component: UserRoleComponent,
  },
  {
    path: 'user-settings',
    component: UserSettingsComponent
  },
  {
    path: 'create-edit-user-role',
    component: CreateEditUserRoleComponent,
  },
  {
    path: 'create-edit-user',
    component: CreatEditUserComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserManagmentRoutingModule { }
