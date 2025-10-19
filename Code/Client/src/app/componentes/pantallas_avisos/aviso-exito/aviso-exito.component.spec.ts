import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvisoExitoComponent } from './aviso-exito.component';

describe('AvisoExitoComponent', () => {
  let component: AvisoExitoComponent;
  let fixture: ComponentFixture<AvisoExitoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvisoExitoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AvisoExitoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
