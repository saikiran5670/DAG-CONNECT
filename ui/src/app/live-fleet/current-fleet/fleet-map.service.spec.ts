import { TestBed } from '@angular/core/testing';

import { FleetMapService } from './fleet-map.service';

describe('FleetMapService', () => {
  let service: FleetMapService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FleetMapService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
