import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquiposActualizarComponent } from './equipos-actualizar.component';

describe('EquiposActualizarComponent', () => {
  let component: EquiposActualizarComponent;
  let fixture: ComponentFixture<EquiposActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EquiposActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EquiposActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
