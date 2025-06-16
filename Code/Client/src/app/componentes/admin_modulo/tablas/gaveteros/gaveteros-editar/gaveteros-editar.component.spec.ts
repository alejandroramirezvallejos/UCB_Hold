import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GaveterosEditarComponent } from './gaveteros-editar.component';

describe('GaveterosEditarComponent', () => {
  let component: GaveterosEditarComponent;
  let fixture: ComponentFixture<GaveterosEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GaveterosEditarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GaveterosEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
