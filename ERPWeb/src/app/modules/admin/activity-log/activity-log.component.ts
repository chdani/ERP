import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { ActivityLogService } from 'app/modules/admin/activity-log/activity-log.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { LazyLoadEvent, PrimeNGConfig } from 'primeng/api';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-activity-log',
  templateUrl: './activity-log.component.html',
  styleUrls: ['./activity-log.component.scss'],
  providers: [TranslatePipe]
})
export class ActivityLogComponent implements OnInit {

  public activityLogType: string = "activitylog";
  public exceptionLogType: string = "exceptionlog";
  public selectedType:string = this.activityLogType;
  public defaultPageFrom :number = 0;
  public defaultPageTo :number = 5;

  selectOptions: any[] = [
    { value: this.activityLogType, viewValue: 'Activity Log' },
    { value: this.exceptionLogType, viewValue: 'Exception Log' },
  ];

  // Date range picker
  range = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });

  //Table Data 
  public ActivityLog_DataSource: any = [];
  public ExceptionLog_DataSource: any = [];

  public totalRecords: number;
  public isActivityLog: boolean = true;
  public pageFrom:number = this.defaultPageFrom;
  public pageTo:number = this.defaultPageTo;

  public startDate: any;
  public endDate: any;
  loading: boolean;

  constructor(
    private _activityLogService: ActivityLogService,
    private _primConfig: PrimeNGConfig,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private datePipe: DatePipe
  ) { }

  ngOnInit() {
    this._commonService.updatePageName(this._translate.transform("Activity Log"));
  }

  public body = {
    LogType: this.activityLogType,
    FromDate: null,
    ToDate: null,
    PageFrom: this.defaultPageFrom,
    PageTo: this.defaultPageTo
  }

  async loadExceptionLog(event: LazyLoadEvent) {
    await this.applyFilter(event);
  }

  async loadActivityLog(event: LazyLoadEvent) {
    await this.applyFilter(event);
  }

  applyFilter(event: LazyLoadEvent = null) {

    var from = this.range.get('start').value;
    var to = this.range.get('end').value;
    if((from != null && to == null) || (from == null && to != null)){
        alert('Please select both the dates');
        return;
    }; 

    this.isActivityLog = this.selectedType == this.activityLogType ? true : false;
    this.startDate = this.datePipe.transform(from, 'yyyy-MM-dd');
    this.endDate = this.datePipe.transform(to, 'yyyy-MM-dd');
    this.setBody(event);
    if(this.isActivityLog)
      this.getActivityLogData();
    else
      this.getExceptionLogData();
  }
 
  async getActivityLogData() {
    this.loading = true;
    let activityLogList = await this._activityLogService.getActivityLogList(this.body);
    this.ActivityLog_DataSource = activityLogList.activity;
    this.loading = false;
    this.setTableConfig(activityLogList);
  }

  async getExceptionLogData() {
    this.loading = true;
    let exceptionLogList = await this._activityLogService.GetExceptionLogList(this.body);
    this.ExceptionLog_DataSource = exceptionLogList.exception;
    this.loading = false;
    this.setTableConfig(exceptionLogList);
  }

  setBody(event: LazyLoadEvent = null) {
    let eventpageFirst:number = (event == null ? 0 : event.first);
    this.body.LogType = this.isActivityLog ? this.activityLogType : this.exceptionLogType;
    this.body.FromDate = this.startDate;
    this.body.ToDate = this.endDate;
    this.body.PageFrom = eventpageFirst;
    this.body.PageTo = this.defaultPageTo;
  }

  setTableConfig(data: any) {
    this.totalRecords = data.rowCount;
    this._primConfig.ripple = true;
  }

  typeChange(data: any) {
    this.selectedType = data.target.value;
  }

  public getMethodType(type:string){
    switch(type){
      case "POST":
        return "ADD/UPDATE";
      case "GET":
        return "FETCH";
      case "PUT":
        return "UPDATE";
      case "DELETE":
          return "DELETE";  
      default:
        return "-";  
    }
  }

  // trackByEmpCode(index: number, employee: any): string {
  //   return employee.code;
  // }
}