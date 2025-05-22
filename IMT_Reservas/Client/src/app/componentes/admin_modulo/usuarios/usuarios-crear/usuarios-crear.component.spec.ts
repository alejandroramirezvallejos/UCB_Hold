import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuariosCrearComponent } from './usuarios-crear.component';

describe('UsuariosCrearComponent', () => {
  let component: UsuariosCrearComponent;
  let fixture: ComponentFixture<UsuariosCrearComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsuariosCrearComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsuariosCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
