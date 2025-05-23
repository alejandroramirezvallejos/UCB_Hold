import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MantenimientosObtenerComponent } from './mantenimientos-obtener.component';

describe('MantenimientosObtenerComponent', () => {
  let component: MantenimientosObtenerComponent;
  let fixture: ComponentFixture<MantenimientosObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MantenimientosObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MantenimientosObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
