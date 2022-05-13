import { Injectable } from '@angular/core';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppPrintService {


  private getActiveTabIndex: BehaviorSubject<[number, string]> =
    new BehaviorSubject<[number, string]>([0, '']);

  constructor(
    private router: Router
  ) { }

  setActiveTabIndex_value(status, goBackPage) {
    debugger
    this.getActiveTabIndex.next([status, goBackPage]);
  }

  getActiveTabIndex_value() {
    debugger
    return this.getActiveTabIndex.asObservable();
  }


  //print --------------------------------------------------------------------------------------------------------
  printContent() {
    debugger
    let pasData = document.getElementsByClassName('download_To_Pdf')[0] as HTMLElement;
    let title = document.getElementById('pdf_title').textContent

    html2canvas(pasData).then(canvas => {
      const imgWidth = 208;
      const pageHeight = 295;
      const imgHeight = canvas.height * imgWidth / canvas.width;
      const heightLeft = imgHeight;
      const contentDataURL = canvas.toDataURL('image/png');
      const pdf = new jsPDF('p', 'mm', 'a4');
      const position = 0;
      pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight);
      pdf.save(title + '.pdf');


      this.router.navigate([this.getActiveTabIndex.value[1]], {
        state: { setActiveTabIndex: this.getActiveTabIndex.value[0] },
      });
    });
  }

}

