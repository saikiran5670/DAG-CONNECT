import { Component, Input, Output, EventEmitter,OnInit, ViewChild, ElementRef } from '@angular/core';

declare var H: any;

@Component({
  selector: 'app-manage-corridor',
  templateUrl: './manage-corridor.component.html',
  styleUrls: ['./manage-corridor.component.less']
})

export class ManageCorridorComponent implements OnInit {
  @Input() translationData: any;
  @Output() tabVisibility: EventEmitter<boolean> = new EventEmitter();
  createEditStatus = false;
  actionType : string;
  showMap: boolean = false;
  map: any;
  
  @ViewChild("map")
  public mapElement: ElementRef;
  platform: any;

  
  constructor() {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

  ngOnInit(){ }

  public ngAfterViewInit() {
    setTimeout(() => {
    this.initMap();
    }, 0);
  }

  initMap(){
    let defaultLayers = this.platform.createDefaultLayers();
    this.map = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 50, lng: 5 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.map.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
    var ui = H.ui.UI.createDefault(this.map, defaultLayers);
  }

  onNewCorridor(){
    this.actionType = "create";
    this.createEditStatus = true;
    this.tabVisibility.emit(false);

  }

  onBackToPage(_eventObj) {
    this.createEditStatus = false;

    this.tabVisibility.emit(true);
  }
}
