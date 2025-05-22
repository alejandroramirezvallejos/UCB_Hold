import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MantenimientosComponent } from './mantenimientos.component';

describe('MantenimientosComponent', () => {
  let component: MantenimientosComponent;
  let fixture: ComponentFixture<MantenimientosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MantenimientosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MantenimientosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
