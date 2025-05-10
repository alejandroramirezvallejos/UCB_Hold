import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GruposEquiposActualizarComponent } from './grupos-equipos-actualizar.component';

describe('GruposEquiposActualizarComponent', () => {
  let component: GruposEquiposActualizarComponent;
  let fixture: ComponentFixture<GruposEquiposActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GruposEquiposActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GruposEquiposActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
