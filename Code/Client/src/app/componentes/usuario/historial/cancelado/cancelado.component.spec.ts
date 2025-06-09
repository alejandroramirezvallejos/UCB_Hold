import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CanceladoComponent } from './cancelado.component';

describe('CanceladoComponent', () => {
  let component: CanceladoComponent;
  let fixture: ComponentFixture<CanceladoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CanceladoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CanceladoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
