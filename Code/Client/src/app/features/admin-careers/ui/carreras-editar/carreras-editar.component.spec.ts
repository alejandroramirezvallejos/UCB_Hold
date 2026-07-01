import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { CarrerasEditarComponent } from './carreras-editar.component';
describe('CarrerasEditarComponent', () => {
  let component: CarrerasEditarComponent;
  let fixture: ComponentFixture<CarrerasEditarComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [CarrerasEditarComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(CarrerasEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
