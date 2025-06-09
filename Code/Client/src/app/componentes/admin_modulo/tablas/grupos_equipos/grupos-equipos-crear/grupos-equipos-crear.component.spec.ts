import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GruposEquiposCrearComponent } from './grupos-equipos-crear.component';

describe('GruposEquiposCrearComponent', () => {
  let component: GruposEquiposCrearComponent;
  let fixture: ComponentFixture<GruposEquiposCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GruposEquiposCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GruposEquiposCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
