import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MantenimientosEditarComponent } from './mantenimientos-editar.component';

describe('MantenimientosEditarComponent', () => {
  let component: MantenimientosEditarComponent;
  let fixture: ComponentFixture<MantenimientosEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MantenimientosEditarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MantenimientosEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
