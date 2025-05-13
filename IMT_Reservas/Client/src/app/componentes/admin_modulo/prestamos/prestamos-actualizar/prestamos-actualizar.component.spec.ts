import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrestamosActualizarComponent } from './prestamos-actualizar.component';

describe('PrestamosActualizarComponent', () => {
  let component: PrestamosActualizarComponent;
  let fixture: ComponentFixture<PrestamosActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrestamosActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrestamosActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
