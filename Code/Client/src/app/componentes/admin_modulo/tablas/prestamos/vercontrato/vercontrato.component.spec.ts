import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VercontratoComponent } from './vercontrato.component';

describe('VercontratoComponent', () => {
  let component: VercontratoComponent;
  let fixture: ComponentFixture<VercontratoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VercontratoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VercontratoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
