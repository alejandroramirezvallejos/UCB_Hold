import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquiposObtenerComponent } from './equipos-obtener.component';

describe('EquiposObtenerComponent', () => {
  let component: EquiposObtenerComponent;
  let fixture: ComponentFixture<EquiposObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EquiposObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EquiposObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
