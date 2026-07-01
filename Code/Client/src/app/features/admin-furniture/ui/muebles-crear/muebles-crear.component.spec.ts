import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { MueblesCrearComponent } from './muebles-crear.component';
describe('MueblesCrearComponent', () => {
  let component: MueblesCrearComponent;
  let fixture: ComponentFixture<MueblesCrearComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [MueblesCrearComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(MueblesCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
