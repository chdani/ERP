import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { DWTModel } from 'app/shared/model/file-upload.model';
import { DataService } from 'app/shared/services/app-data-share.service';
import Dynamsoft from 'dwt';
import { WebTwain } from 'dwt/dist/types/WebTwain';
import { SourceDetails } from 'dwt/dist/types/WebTwain.Acquire';
import { environment } from 'environments/environment';
import { ToastrService } from 'ngx-toastr';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-dwt',
  templateUrl: './dwt.component.html',
  styleUrls: ['./dwt.component.css'],
  providers: [TranslatePipe]
})
export class DwtComponent implements OnInit {
  DWObject: WebTwain;
  selectSources: HTMLSelectElement;
  containerId = 'dwtcontrolContainer';
  bWASM = false;
  sourceList:any=[];
  selectedSource:any;
  dialogueRef:any;
  uploadConfig:DWTModel;
  private serviceURL = environment.serviceUrl;
  constructor(
    private _translate: TranslatePipe,
    public config: DynamicDialogConfig,
    public ref: DynamicDialogRef,
    private dataService: DataService,
    private _toastrService: ToastrService,

  )
  { 

  }

  ngOnInit(): void {
    debugger;
    this.uploadConfig = this.config.data;
    Dynamsoft.DWT.Containers = [{ WebTwainId: 'dwtObject', ContainerId: this.containerId, Width: '600px', Height: '450px' }];
    Dynamsoft.DWT.RegisterEvent('OnWebTwainReady', () => { this.Dynamsoft_OnReady(); });                                                        
    Dynamsoft.DWT.ResourcesPath = 'assets/dwt-resources';
    Dynamsoft.DWT.ProductKey = "t0154KQMAALGcCXfWgz5xwOsjw9ekjq2FOTwDXSQA7/C7g/QyT8jcxhksNTa7B8Zi1ZHlAhWs5O8+ZSeasAOzYD4qKtSsgrf6hrRvGIHB6kL/lWxKleHDfNxIMXiocxl/68l8cVw06lSGB4z0jfenbcLMZw85zlwbHjDSN2HmysR91mz/uAtoK5mpZXjASN+UzHcjc8qLJP0BC0eetw==";
    Dynamsoft.DWT.Load();
  }
  /**
   * Dynamsoft_OnReady is called when a WebTwain instance is ready to use.
   * In this callback we do some initialization.
   */
  Dynamsoft_OnReady(): void {
    debugger;
    this.DWObject = Dynamsoft.DWT.GetWebTwain(this.containerId);
	this.bWASM = Dynamsoft.Lib.env.bMobile || !Dynamsoft.DWT.UseLocalService;
    if (this.bWASM) {
      this.DWObject.Viewer.cursor = "pointer";
    } else {
      //this.sourceList = this.DWObject.GetSourceNames();
      let source = this.DWObject.GetSourceNames();
      source.forEach(scr=>{
        let sc={sourceName:scr};
        this.sourceList.push(sc);
      })

      // this.selectSources = <HTMLSelectElement>document.getElementById("sources");
      // this.selectSources.options.length = 0;
      // for (let i = 0; i < sources.length; i++) {
      //   this.selectSources.options.add(new Option(<string>sources[i], i.toString()));
      // }
    }
  }

 UploadAsPDF() {
    var indices = [];
   
    if (this.DWObject) {
        if (this.DWObject.HowManyImagesInBuffer === 0) {
          this._toastrService.error(this._translate.transform("APP_SCANNER_NO_IMAGE"));
            return;
        }
        indices = this.DWObject.SelectedImagesIndices;
        let ds :DataService= this.dataService;
        let dwObject = this.DWObject;
        let dialogRef=this.ref;
        let failureCallback= this.asyncFailure;
          let i=0;
          let convertImage=function(_index){
            dwObject.ConvertToBase64([_index],
            Dynamsoft.DWT.EnumDWT_ImageType.IT_PDF,
            function(result,indices,type){
              let fg=result.getData(0,result.getLength());
              ds.setData(fg);
              dialogRef.close();
              i++;
              if(i<this.DWObject.HowManyImagesInBuffer){
                convertImage(i)
              }else{
                return;
              }
            },failureCallback)
          }

          convertImage(0);
    }
}

asyncFailure(errorcode,errorstring){
          
}
  /**
   * Acquire images from scanners or cameras or local files
   */
  acquireImage(): void {
    debugger;
    let rt=this.selectedSource;
    
    if (!this.DWObject)
      this.DWObject = Dynamsoft.DWT.GetWebTwain();
    if (this.bWASM) {
      this._toastrService.error(this._translate.transform("APP_SCANNER_MODE_NOT_SUPPORTED"));
    }
    else if (this.DWObject.SourceCount > 0 && this.selectedSource != "") {
      const onAcquireImageSuccess = () => { this.DWObject.CloseSource(); };
      const onAcquireImageFailure = onAcquireImageSuccess;
      this.DWObject.OpenSource();
      this.DWObject.AcquireImage({}, onAcquireImageSuccess, onAcquireImageFailure);
    } else {
      this._toastrService.error(this._translate.transform("APP_SCANNER_NO_SOURCE"));
    }
  }
  /**
   * Open local images.
   */
  openImage(): void {
    debugger;
    if (!this.DWObject)
      this.DWObject = Dynamsoft.DWT.GetWebTwain('dwtcontrolContainer');
    this.DWObject.IfShowFileDialog = true;
    /**
     * Note, this following line of code uses the PDF Rasterizer which is an extra add-on that is licensed seperately
     */
    this.DWObject.Addon.PDF.SetConvertMode(Dynamsoft.DWT.EnumDWT_ConvertMode.CM_RENDERALL);
    this.DWObject.LoadImageEx("", Dynamsoft.DWT.EnumDWT_ImageType.IT_ALL,
      () => {
        //success
      }, () => {
        //failure
      });
  }

  cancelDWT(){
    this.ref.close();
  }

}
