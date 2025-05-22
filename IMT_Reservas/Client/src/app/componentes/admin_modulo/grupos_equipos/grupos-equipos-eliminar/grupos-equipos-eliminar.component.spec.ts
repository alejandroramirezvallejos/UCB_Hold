import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GruposEquiposEliminarComponent } from './grupos-equipos-eliminar.component';

describe('GruposEquiposEliminarComponent', () => {
  let component: GruposEquiposEliminarComponent;
  let fixture: ComponentFixture<GruposEquiposEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GruposEquiposEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GruposEquiposEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
