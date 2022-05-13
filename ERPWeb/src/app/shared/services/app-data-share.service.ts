import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class DataService {

  private messageSource = new Subject<any>();

  constructor() { }

  public getData(): Observable<any> {
    return this.messageSource.asObservable();
  }

  public setData(message: any) {
    return this.messageSource.next(message);
  }

  public readFileAsync(file) {
    return new Promise<any>((resolve, reject) => {
      let reader = new FileReader();
  
      reader.onload  = () => {
        resolve(reader.result);
      };
  
      reader.onerror = reject;
  
      reader.readAsDataURL(file);
    })
  }

}