import { Component, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProfileComponent } from './profile/profile.component';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FuseCardModule } from '@fuse/components/card';
import { SharedModule } from 'app/shared/shared.module';

const routing =[{
   path:'',
   component :ProfileComponent 
}]

@NgModule({
  declarations: [ProfileComponent],
  imports: [
    RouterModule.forChild(routing),
    CommonModule,
    MatButtonModule,
    MatDividerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatTooltipModule,
    FuseCardModule,
    SharedModule
  ]
})
export class ProfileModule { }
