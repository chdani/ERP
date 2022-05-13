import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { FileUploadConfig } from "../model/file-upload.model";

interface UploadControlsList {
    ID: string;
    UploadControl: FileUploadConfig;
  }

@Injectable({ providedIn: "root" })
export class UploadControlService {
    private uploadControlsList: UploadControlsList[] = [];

    private control: FileUploadConfig = null;
    private controls = new BehaviorSubject(this.control);
    public currentControl: Observable<FileUploadConfig> = this.controls.asObservable();

    remove(id: string) {
        this.uploadControlsList.splice(this.uploadControlsList.findIndex(x => x.ID === id));
    }

    public LoadUploadControl(uploadControl: FileUploadConfig) {
     
    }

    public getUploadControl(id: string) {
      const uploadControl = this.uploadControlsList.find(x => x.ID === id);
      if (uploadControl) {
        return uploadControl.UploadControl;
      }
    }

    //upload file method
uploadFile(file) {
  debugger;
  const formData: FormData = new FormData();
  formData.append('file', file, file.name);
   //append any other key here if required
  //return this.http.post(`<upload URL>`, formData);
}
}