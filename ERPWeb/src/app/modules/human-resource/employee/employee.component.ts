import { Component, ChangeDetectorRef, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { HumanResourceService } from '../human.resource.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
@Component({
    selector: 'app-employee',
    templateUrl: './employee.component.html',
    styleUrls: ['./employee.component.scss'],
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe,
        ConfirmationService, TranslatePipe
    ],
})
export class EmployeeComponent extends AppComponentBase implements OnInit {
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _changeDetectorRef: ChangeDetectorRef,
        private _formBuilder: FormBuilder,
        private _confirmService: ConfirmationService,
        private _translate: TranslatePipe,
        private _router: Router,
        private _codeMasterService: CodesMasterService,
        private _humanresourceService: HumanResourceService,
        private _toastrService: ToastrService,
        private _webApiservice: WebApiService,
        public _commonService: AppCommonService,

    ) {
        super(injector,'SCR_HR_EMPLOYEE_MASTER','allowView', _commonService )
    }
    gridDetailsContextMenu: MenuItem[] = [];
    selectedRow: any;
    employeeForm: any;
    basicInfo: any;
    employeeModel: any = [];
    isLoading: boolean = false;
    NATIONALITY: any = [];
    panelOpenState = false;
    jobPosition: any = [];
    department: any = [];
    Establishment: any = [];
    EduLevel: any = [];
    Relation: any = [];
    MaritialStatus: any = [];
    Parent: any = [];
    employeeEductionlist: any = [];
    employeeDependentslist: any = [];
    employeeExpentationlist: any = [];
    Expentationlist: any = [];
    public showOrHideOrgFinyear: boolean = false;
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("EMPLOYEE_MASTER"));
        this.employeeForm = {
            id: "",
            EmpNumber: "",
            QatariID: "",
            fullNameArb: "",
            fullNameEng: "",
            FullNameArb: "",
            Passport: "",
            DOB: "",
            FromDobDate: "",
            ToDobDate: "",
            PlaceOfBirth: "",
            Nationality: "",
            Address: "",
            PhoneNumber: "",
            Email: "",
            MaritalStatusCode: "",
            CurrDepartmentId: "",
            CurrPositionId: "",
            CurrentGrade: "",
            Others: "",
        }


        this.getSearch();
        this.showHideOrgFinyear('MNU_EMPLOYEE_MASTER');
    }
    async getSearch() {
        await this.LoadDefault();
        if (this.employeeForm.currDepartmentId)
            this.employeeForm.currDepartmentId = this.employeeForm.currDepartmentId.id;
        if (this.employeeForm.currPositionId)
            this.employeeForm.currPositionId = this.employeeForm.currPositionId.id;
        if (this.employeeForm.nationality)
            this.employeeForm.nationality = this.employeeForm.nationality.code;
        this.employeeModel = await this._webApiservice.post("getEmployeesBySearchCriteria", this.employeeForm)
    }
    async LoadDefault() {
        this.jobPosition = await this._humanresourceService.jobposition();
        this.department = await this._humanresourceService.parentdepartment();
        this.NATIONALITY = await this._codeMasterService.getCodesDetailByGroupCode("NATIONALITY", false, false, this._translate);
        this.EduLevel = await this._codeMasterService.getCodesDetailByGroupCode("EDUCATIONLEVEL", false, false, this._translate);
        this.MaritialStatus = await this._codeMasterService.getCodesDetailByGroupCode("MARITIALSTATUS", false, false, this._translate);
        this.Establishment = await this._codeMasterService.getCodesDetailByGroupCode("ESTABLISHMENTCODE", false, false, this._translate);
        this.Relation = await this._codeMasterService.getCodesDetailByGroupCode("RELATIONCODE", false, false, this._translate);
    }
    getGridDetailsContextMenu(item) {
        this.selectedRow = item;
        this.gridDetailsContextMenu = [];
        if (this.isGranted('SCR_HR_EMPLOYEE_MASTER', this.actionType.allowEdit)) {
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditEmployee(item.id) } };
            this.gridDetailsContextMenu.push(edit);
        }
        if (this.isGranted('SCR_HR_EMPLOYEE_MASTER', this.actionType.allowDelete)) {
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markEmployeeInactive(item.id) } };
            this.gridDetailsContextMenu.push(Delete);
        }



       
    }

    async markEmployeeInactive(id) {
        this._confirmService.confirm({
            message: this._translate.transform("EMPLOYEE_MASTER_DELETE_CONF"),
            accept: async () => {
                var result = await this._webApiservice.get("markEmployeeInactive/" + id);
                if (result) {
                    var output = result as any;
                    if (output.status == "DATASAVESUCSS") {
                        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                        this.getSearch();
                    }
                    else {
                        this._toastrService.error(output.messages[0])
                    }
                }
            }
        });
    }



    
    async listExpentation(event) {
        this.employeeDependentslist = [];
        this.employeeEductionlist = [];
        this.employeeExpentationlist = [];

        var result = await this._webApiservice.get("GetEmployeeDetailesList/" + event.data.id);
        if (result) {

            result.employees.forEach(element => {
                var national = this.NATIONALITY.filter(a => a.code == element.nationality)[0];
                var maritial = this.MaritialStatus.filter(a => a.code == element.maritalStatusCode)[0];
                this.employeeExpentationlist.push({
                    id: element.id,
                    nationality: national.description, address: element.address, relationCode: maritial.description,
                    placeOfBirth: element.placeOfBirth, passport: element.passport,
                })
            });
            result.educations.forEach(element => {
                var educode = this.EduLevel.filter(a => a.code == element.eduLevelCode)[0];
                var estucode = this.Establishment.filter(a => a.code == element.establishmentCode)[0]
                this.employeeEductionlist.push({
                    id: element.id,
                    eduLevelCode: educode.description, establishmentCode: estucode.description,
                    specialization: element.specialization, completedYear: element.completedYear,
                    gradePercentage: element.gradePercentage, remarks: element.remarks,
                })
            });;
            result.dependents.forEach(element => {
                var relation = this.Relation.filter(a => a.code == element.relationCode)[0];
                this.employeeDependentslist.push({
                    id: element.id,
                    fullName: element.fullName, qatariID: element.qatariID, passport: element.passport,
                    dob: new Date(element.dob), placeOfBirth: element.placeOfBirth, relationCode: relation.description,
                })
            });
        }

    }
    async createOreditEmployee(data?: any) {
        var result = await this._webApiservice.get("GetEmployeeById/" + data);
        this._router.navigate(["human-resource/create-edit-employee"], {
            state: { employeeId: data },
        });
    }
    addEmployee() {
        this._router.navigate(["human-resource/create-edit-employee"], {
            state: { employeeId: "" },
        });
    }
    clearSearchCriteria() {
        this.employeeForm = {
            id: "",
            EmpNumber: "",
            QatariID: "",
            fullNameArb: "",
            fullNameEng: "",
            FullNameArb: "",
            Passport: "",
            DOB: "",
            FromDobDate: "",
            ToDobDate: "",
            PlaceOfBirth: "",
            Nationality: "",
            Address: "",
            PhoneNumber: "",
            Email: "",
            MaritalStatusCode: "",
            CurrDepartmentId: "",
            CurrPositionId: "",
            CurrentGrade: "",
            Others: "",
        }

    }

}
