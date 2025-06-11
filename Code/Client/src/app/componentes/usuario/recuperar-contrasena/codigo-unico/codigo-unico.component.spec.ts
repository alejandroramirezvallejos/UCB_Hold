import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CodigoUnicoComponent } from './codigo-unico.component';

describe('CodigoUnicoComponent', () => {
  let component: CodigoUnicoComponent;
  let fixture: ComponentFixture<CodigoUnicoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CodigoUnicoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CodigoUnicoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
