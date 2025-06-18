import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GruposEquiposTablaComponent } from './grupos-equipos-tabla.component';

describe('GruposEquiposTablaComponent', () => {
  let component: GruposEquiposTablaComponent;
  let fixture: ComponentFixture<GruposEquiposTablaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GruposEquiposTablaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GruposEquiposTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
