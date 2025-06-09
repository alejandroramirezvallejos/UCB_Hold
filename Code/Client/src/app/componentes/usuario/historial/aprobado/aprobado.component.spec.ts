import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AprobadoComponent } from './aprobado.component';

describe('AprobadoComponent', () => {
  let component: AprobadoComponent;
  let fixture: ComponentFixture<AprobadoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AprobadoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AprobadoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
