import { Injectable } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { WebApiService } from 'app/shared/webApiService';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class HumanResourceService {
    private language: string;
    
    constructor(private _webApiService: WebApiService) {
        var userData = JSON.stringify(localStorage.getItem("LUser")) as any;
        if (userData && userData.userContext)
            this.language = userData.userContext.language;

        if (!this.language) this.language = "en";
    }   
    async removeDepartmentListCatche(){
        debugger
        localStorage.removeItem("appDepartmentList");    
    }
    async removejobpositionCatche(){
      debugger
      localStorage.removeItem("appJobpositionList");    
  }
   
    async parentdepartment() { 
        let parentId:any=[];  
        let department = JSON.parse(localStorage.getItem("appDepartmentList"));
        if (!department || department.length == 0) {
        department = await this._webApiService.get("Getparentdepartment" );
        }
        if(department){
            localStorage.setItem("appDepartmentList", JSON.stringify(department));
        department.forEach(element => {
      parentId.push({ 
        id: element.id,  name:element.name
            });
    
        }); 
       }  
        return parentId;
 }
 

      async jobposition(){
        let jobList:any=[];  
          let jobposition = JSON.parse(localStorage.getItem("appJobpositionList"));
          if(!jobposition || jobposition.length == 0 ){
          jobposition = await this._webApiService.get("getJobPosition");
          }
          if(jobposition){
              localStorage.setItem("appJobpositionList", JSON.stringify(jobposition));
              jobposition.forEach(element => {
                jobList.push({ 
                  id: element.id,  name:element.name
                      });
              
                  }); 
          }
          return jobList;

         }
      
         
}