import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GaveterosComponent } from './gaveteros.component';

describe('GaveterosComponent', () => {
  let component: GaveterosComponent;
  let fixture: ComponentFixture<GaveterosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GaveterosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GaveterosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
