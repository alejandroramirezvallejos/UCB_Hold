import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarrerasObtenerComponent } from './carreras-obtener.component';

describe('CarrerasObtenerComponent', () => {
  let component: CarrerasObtenerComponent;
  let fixture: ComponentFixture<CarrerasObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarrerasObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarrerasObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
