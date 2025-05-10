import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GaveterosCrearComponent } from './gaveteros-crear.component';

describe('GaveterosCrearComponent', () => {
  let component: GaveterosCrearComponent;
  let fixture: ComponentFixture<GaveterosCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GaveterosCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GaveterosCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
