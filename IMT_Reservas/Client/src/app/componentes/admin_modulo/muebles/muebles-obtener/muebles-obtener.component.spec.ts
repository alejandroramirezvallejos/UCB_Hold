import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MueblesObtenerComponent } from './muebles-obtener.component';

describe('MueblesObtenerComponent', () => {
  let component: MueblesObtenerComponent;
  let fixture: ComponentFixture<MueblesObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MueblesObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MueblesObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
