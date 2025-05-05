import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PantallaMainComponent } from './pantalla-main.component';
import { ObjetoComponent } from '../objeto/objeto.component';

describe('PantallaMainComponent', () => {
  let component: PantallaMainComponent;
  let fixture: ComponentFixture<PantallaMainComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PantallaMainComponent, ObjetoComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(PantallaMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
