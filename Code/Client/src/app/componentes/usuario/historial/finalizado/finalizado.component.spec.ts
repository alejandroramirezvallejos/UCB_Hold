import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FinalizadoComponent } from './finalizado.component';

describe('FinalizadoComponent', () => {
  let component: FinalizadoComponent;
  let fixture: ComponentFixture<FinalizadoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FinalizadoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FinalizadoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
