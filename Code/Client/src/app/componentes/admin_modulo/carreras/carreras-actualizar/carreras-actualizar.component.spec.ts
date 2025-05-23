import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarrerasActualizarComponent } from './carreras-actualizar.component';

describe('CarrerasActualizarComponent', () => {
  let component: CarrerasActualizarComponent;
  let fixture: ComponentFixture<CarrerasActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarrerasActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CarrerasActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
