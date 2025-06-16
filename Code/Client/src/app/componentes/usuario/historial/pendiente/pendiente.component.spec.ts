import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PendienteComponent } from './pendiente.component';

describe('PendienteComponent', () => {
  let component: PendienteComponent;
  let fixture: ComponentFixture<PendienteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PendienteComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PendienteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
