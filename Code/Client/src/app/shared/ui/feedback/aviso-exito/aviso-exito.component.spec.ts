import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { AvisoExitoComponent } from './aviso-exito.component';
describe('AvisoExitoComponent', () => {
  let component: AvisoExitoComponent;
  let fixture: ComponentFixture<AvisoExitoComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [AvisoExitoComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(AvisoExitoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
