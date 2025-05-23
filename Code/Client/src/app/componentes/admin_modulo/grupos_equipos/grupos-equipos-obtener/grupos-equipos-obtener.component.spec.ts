import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GruposEquiposObtenerComponent } from './grupos-equipos-obtener.component';

describe('GruposEquiposObtenerComponent', () => {
  let component: GruposEquiposObtenerComponent;
  let fixture: ComponentFixture<GruposEquiposObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GruposEquiposObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GruposEquiposObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
