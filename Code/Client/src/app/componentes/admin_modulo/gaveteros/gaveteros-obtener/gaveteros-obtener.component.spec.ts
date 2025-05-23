import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GaveterosObtenerComponent } from './gaveteros-obtener.component';

describe('GaveterosObtenerComponent', () => {
  let component: GaveterosObtenerComponent;
  let fixture: ComponentFixture<GaveterosObtenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GaveterosObtenerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GaveterosObtenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
