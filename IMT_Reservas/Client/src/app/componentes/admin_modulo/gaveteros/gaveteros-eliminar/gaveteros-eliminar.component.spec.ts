import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GaveterosEliminarComponent } from './gaveteros-eliminar.component';

describe('GaveterosEliminarComponent', () => {
  let component: GaveterosEliminarComponent;
  let fixture: ComponentFixture<GaveterosEliminarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GaveterosEliminarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GaveterosEliminarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
