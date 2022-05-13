
import { AppCommonService } from 'app/shared/services/app-common.service';
import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroupDirective, FormGroup, FormControl, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { FuseAlertType } from '@fuse/components/alert';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from 'app/core/auth/auth.service';
import { User } from 'app/shared/model/user.model';
import { WebApiService } from 'app/shared/webApiService';
import { cond } from 'lodash';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { takeUntil } from 'rxjs/operators';
import { MenuItem } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { MessageService } from 'primeng/api';
import { MatStepperModule } from '@angular/material/stepper';
import { ErrorStateMatcher } from '@angular/material/core';
import { HumanResourceService } from 'app/modules/human-resource/human.resource.service';
import { ValidatorFn, AbstractControl } from '@angular/forms';
import { Moment } from 'moment';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { validateVerticalPosition } from '@angular/cdk/overlay';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { DES } from 'crypto-js';
/**
 * @title Stepper overview
 */
@Component({
  selector: 'app-employee',
  templateUrl: './create-edit-employee.component.html',
  styleUrls: ['./create-edit-employee.component.scss'],
  providers: [MatStepperModule]


})
export class createeditemployeeComponend extends AppComponentBase implements OnInit {
  isLinear = true;
  invalid = false;
  invalidDate = false;
  invaliddependentdob = false;
  submitted = false;
  MaritialStatus: any = [];
  EmpEducationList: any = [];
  EmpDepentedList: any = [];
  Establishment: any = [];
  EduLevel: any = [];
  Relation: any = [];
  NATIONALITY: any = [];
  jobPosition: any = [];
  department: any = []
  employeelist: any;
  defaultEmployeeList: any = [];
  basicInfo: any;
  id: any;
  header: any = [];
  uploadConfig: FileUploadConfig;
  lang: any;
  dialogRef: DynamicDialogRef;
  selectedUploadData: any = {};
   
  employee: any = {
    id: "",
    empNumber: "",
    qatariID: "",
    fullNameArb: "",
    fullNameEng: "",
    passport: "",
    dob: "",
    placeOfBirth: "",
    nationality: "",
    address: "",
    phoneNumber: "",
    email: "",
    managerId: "",
    createUser: "",
    isDepratmentHead: "",
    maritalStatusCode: "",
    currDepartmentId: "",
    currPositionId: "",
    currentGrade: "",
    empEducation: [],
    empDependent: [],
  }


  constructor(
    injector:Injector,
    private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private _codeMasterService: CodesMasterService,
    private _humanresourceService: HumanResourceService,
    private _translate: TranslatePipe,
    private _router: Router,
    private _toastrService: ToastrService,
    private _webApiservice: WebApiService,
    public _commonService: AppCommonService,
    private dialogService: DialogService

  ) {
     super(injector,'SCR_HR_EMPLOYEE_MASTER','allowAdd', _commonService );
     this._commonService.fileObserver.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((file: any) => {
        this.selectedUploadData.appDocuments = file;
      });


}

  ngOnInit() {
    this.EmpEducationList = [];
    if (history.state && history.state.employeeId && history.state.employeeId != '') {
      this.id = history.state.employeeId;
    }
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("EMPLOYEE_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("EMPLOYEE_ADD"));

    this.getall();
  }
  
    async getall() {
    await this.LoadDefault();
    if (this.id && this.id !== "") {
      this.employee = await this._webApiservice.get("GetEmployeeById/" + this.id);
      let filterEmployee = this.defaultEmployeeList.filter(i => i.id !== this.id);
      this.defaultEmployeeList = filterEmployee;
      this.employee.empEducation.forEach(element => {
        var educode = this.EduLevel.filter(a => a.code == element.eduLevelCode)[0];
        var estucode = this.Establishment.filter(a => a.code == element.establishmentCode)[0];
        this.EmpEducationList.push({
          id: element.id,
          eduLevelCode: educode, establishmentCode: estucode,
          specialization: element.specialization, completedYear: element.completedYear,
          gradePercentage: element.gradePercentage, remarks: element.remarks, createdby: element.createdBy,
          createdDate: element.createdDate, modifiedBy: element.modifiedBy, modifiedDate: element.modifiedDate
        })
      });;
      this.employee.empDependent.forEach(element => {
        var relation = this.Relation.filter(a => a.code == element.relationCode)[0];
        this.EmpDepentedList.push({
          id: element.id,
          fullName: element.fullName, qatariID: element.qatariID, passport: element.passport,
          dob: new Date(element.dob), placeOfBirth: element.placeOfBirth, relationCode: relation, createdby: element.createdBy,
          createdDate: element.createdDate, modifiedBy: element.modifiedBy, modifiedDate: element.modifiedDate
        })
      });
    }

    
  }
  async LoadDefault() {
    this.Relation = await this._codeMasterService.getCodesDetailByGroupCode("RELATIONCODE", false, false, this._translate);
    this.EduLevel = await this._codeMasterService.getCodesDetailByGroupCode("EDUCATIONLEVEL", false, false, this._translate);
    this.Establishment = await this._codeMasterService.getCodesDetailByGroupCode("ESTABLISHMENTCODE", false, false, this._translate);
    this.MaritialStatus = await this._codeMasterService.getCodesDetailByGroupCode("MARITIALSTATUS", false, false, this._translate);
    this.NATIONALITY = await this._codeMasterService.getCodesDetailByGroupCode("NATIONALITY", false, false, this._translate);
    this.jobPosition = await this._humanresourceService.jobposition();
    this.department = await this._humanresourceService.parentdepartment();
    this.defaultEmployeeList = await this._webApiservice.get('getEmployeeList');
  }

  EmpCancel() {
    this._router.navigateByUrl("human-resource/employee");
  }
  valuechangedependentdob(item) {
    if (item) {
      var dob = new Date(item);
      var currentTime = new Date();
      var dob1 = dob.getFullYear();
      var year = currentTime.getFullYear();
      var age = year;
      if (dob > currentTime) {
        this._toastrService.error(this._translate.transform("Invalid Date Of Birth!! "));
        this.invaliddependentdob = true;
      } else {
        this.invaliddependentdob = false;
      }

      return this.invalidDate;
    }
  }
  removeEducation(empEducation) {
    this.EmpEducationList.forEach((item, index) => {
      if (item === empEducation) this.EmpEducationList.splice(index, 1);
    });
  }
  removeDependent(empDependent) {
    this.EmpDepentedList.forEach((item, index) => {
      if (item === empDependent) this.EmpDepentedList.splice(index, 1);
    });
  }

  addNewEducation() {
    var newedu =
    {
      eduLevelCode: '',
      establishmentCode: '',
      specialization: '',
      completedYear: '',
      gradePercentage: '',
      remarks: '',
      active: "Y"
    }

    this.EmpEducationList.unshift(newedu);
  }
  addNewDependent() {
    var newdeb =
    {
      fullName: "",
      qatariID: '',
      passport: '',
      dob: '',
      placeOfBirth: '',
      relationCode: '',
      active: "Y"
    }
    this.EmpDepentedList.unshift(newdeb);
  }
  attachmentLink() {
  
if(this.employee.appDocument !=null)
    this.loadEmployeeDocuments();

else{

    this.header = this._translate.transform("EMP_SIGNATURE_FILE");
    this.selectedUploadData = this.employee;
    this.uploadConfig =
    {
      TransactionId: this.employee.id,
      TransactionType: 'EMPLOYEE',
      AllowedExtns: ".png,.jpg,.jpeg",
      DocTypeRequired: false,
      DocumentReference: "",
      ReadOnly:true,
      ScanEnabled: true,
      ShowSaveButton: false,
      FileContent: this.employee.appDocuments
    };

    this.dialogRef = this.dialogService.open(FileUploadComponent, {
      header: this.header,
      width: "700px",
      closable: false,
      contentStyle: { "height": "500px", overflow: "auto" },
      baseZIndex: 500,
      dismissableMask: true,
      data: this.uploadConfig
    });

  }
  }
  async loadEmployeeDocuments() {

    var result = this.employee.appDocument;
    
    this.employee.appDocument = [];
    result.forEach(element => {
      debugger;
      this.employee.appDocument.push({
        id: element.id,
        fileName: element.fileName,
        createdDate: element.createdDate,
        userName: element.userName,
      })


      if (this.employee.appDocument != "") {

        this.header = this._translate.transform("FILE_ATTACHMENT_FOR_TRANSNO") + this.employee.empNumber;
        
      }
      
      this.uploadConfig =
      {
        TransactionId: this.employee.id,
        TransactionType: "",
        AllowedExtns: ".png,.jpg,.gif,.jpeg,.bmp,.docx,.doc,.pdf,.msg",
        DocTypeRequired: false,
        DocumentReference: "",
        ReadOnly: false,
        ScanEnabled: true,
        ShowSaveButton:true,
        FileContent: []
      };
  
      this.dialogRef = this.dialogService.open(FileUploadComponent, {
        header: this.header,
        width: "700px",
        closable: false,
        contentStyle: { "height": "500px", overflow: "auto" },
        baseZIndex: 500,
        dismissableMask: true,
        data: this.uploadConfig
      });


    });





  }
  async employeesave() {
    this.submitted = true;
    if (this.employee.fullNameArb == "" || this.employee.fullNameEng == "" || this.employee.empNumber == "" || this.employee.qatariID == ""
      || this.employee.dob == "" || this.employee.currDepartmentId == "" || this.employee.currPositionId == "" || this.employee.currentGrade == 0 || this.employee.currentGrade == ""
      || this.employee.phoneNumber == "" || this.employee.email == "") {
      this._toastrService.error(this._translate.transform("PLEASE_ALL_REQ"));
      return;
    }
    const status = this.EmpDepentedList.some(user => {
      let counter = 0;
      for (const iterator of this.EmpDepentedList) {
        if (iterator.qatariID === user.qatariID) {
          counter += 1;
        }
      }
      return counter > 1;
    });
    if (!status) {
      const status1 = this.EmpEducationList.some(user => {
        let counter = 0;
        for (const iterator of this.EmpEducationList) {
          if (iterator.eduLevelCode.code === user.eduLevelCode.code && iterator.specialization === user.specialization) {
            counter += 1;
          }
        }
        return counter > 1;
      });
      if (!status1) {

        this.employee.empEducation = [];
        this.EmpEducationList.forEach(element => {
          this.employee.empEducation.push({
            eduLevelCode: element.eduLevelCode.code, establishmentCode: element.establishmentCode.code,
            specialization: element.specialization, completedYear: element.completedYear,
            gradePercentage: element.gradePercentage, remarks: element.remarks, createdby: this.employee.createdBy,
            createdDate: this.employee.createdDate, modifiedBy: this.employee.modifiedBy, modifiedDate: this.employee.modifiedDate
          })
        });;
        this.employee.empDependent = [];
        this.EmpDepentedList.forEach(element => {
          this.employee.empDependent.push({
            fullName: element.fullName, qatariID: element.qatariID, passport: element.passport,
            dob: element.dob, placeOfBirth: element.placeOfBirth, relationCode: element.relationCode.code, createdby: this.employee.createdBy,
            createdDate: this.employee.createdDate, modifiedBy: this.employee.modifiedBy, modifiedDate: this.employee.modifiedDate
          })
        });

        if (this.employee.id && this.employee.id != "") {
          this.employee.createdBy = this.employee.createdBy;
          this.employee.createdDate = this.employee.createdDate;
          this.employee.modifiedBy = this.employee.modifiedBy;
          this.employee.modifiedDate = this.employee.modifiedDate;
        }
        debugger;
        var result = await this._webApiservice.post("SaveEmployee", this.employee);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this._router.navigateByUrl("human-resource/employee");
          }
          else
            this._toastrService.error(output.messages[0])
        }
      } else {
        this._toastrService.error(this._translate.transform("DUPLICATE_DATA_ENTRY"));
      }
    } else {
      this._toastrService.error(this._translate.transform("DUPLICATE_QATARIID_DEPENDENT"));
    }


  }

}

