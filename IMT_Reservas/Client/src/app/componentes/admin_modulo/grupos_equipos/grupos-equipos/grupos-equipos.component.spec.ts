import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GruposEquiposComponent } from './grupos-equipos.component';

describe('GruposEquiposComponent', () => {
  let component: GruposEquiposComponent;
  let fixture: ComponentFixture<GruposEquiposComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GruposEquiposComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GruposEquiposComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
