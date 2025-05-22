import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MantenimientosEliminarComponent } from './mantenimientos-eliminar.component';

describe('MantenimientosEliminarComponent', () => {
  let component: MantenimientosEliminarComponent;
  let fixture: ComponentFixture<MantenimientosEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MantenimientosEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MantenimientosEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
