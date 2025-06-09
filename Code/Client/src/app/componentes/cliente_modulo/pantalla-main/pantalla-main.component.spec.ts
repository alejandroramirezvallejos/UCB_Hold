import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PantallaMainComponent } from './pantalla-main.component';

describe('PantallaMainComponent', () => {
  let component: PantallaMainComponent;
  let fixture: ComponentFixture<PantallaMainComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PantallaMainComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(PantallaMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
