import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RechazadoComponent } from './rechazado.component';

describe('RechazadoComponent', () => {
  let component: RechazadoComponent;
  let fixture: ComponentFixture<RechazadoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RechazadoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RechazadoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
