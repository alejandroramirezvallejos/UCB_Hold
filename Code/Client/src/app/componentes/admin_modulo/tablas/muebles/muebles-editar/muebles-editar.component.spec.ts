import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MueblesEditarComponent } from './muebles-editar.component';

describe('MueblesEditarComponent', () => {
  let component: MueblesEditarComponent;
  let fixture: ComponentFixture<MueblesEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MueblesEditarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MueblesEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
