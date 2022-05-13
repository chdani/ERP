import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DepartmentComponent } from './department/department.component';
import { CreateEditDepartmentComponent } from './department/create-edit-department/create-edit-department.component';
import { JobPositionComponent } from './job-position/job-positoion.component';
import { CreatEditjobpositionComponent } from './job-position/create-edit-job-position/create-edit-job-position.component';
import { createeditemployeeComponend } from './employee/create-edit-employee/create-edit-employee.component';
import { EmployeeComponent } from './employee/employee.component';


const routes: Routes = [
  {
    path: 'jdepartment',
    component: DepartmentComponent,
  },
  {
   path:'create-edit-department',
   component:CreateEditDepartmentComponent,
  },
  {
    path:'job-position',
    component:JobPositionComponent,
  },
  {
    path:'create-edit-job-position',
    component:CreatEditjobpositionComponent,
  },
  {
    path:'create-edit-employee',
    component:createeditemployeeComponend,
  },
  {
    path:'employee',
    component:EmployeeComponent,
  },
 
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HRRoutingModule { }
