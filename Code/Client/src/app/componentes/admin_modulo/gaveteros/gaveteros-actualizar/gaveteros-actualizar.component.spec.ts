import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GaveterosActualizarComponent } from './gaveteros-actualizar.component';

describe('GaveterosActualizarComponent', () => {
  let component: GaveterosActualizarComponent;
  let fixture: ComponentFixture<GaveterosActualizarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GaveterosActualizarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GaveterosActualizarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
