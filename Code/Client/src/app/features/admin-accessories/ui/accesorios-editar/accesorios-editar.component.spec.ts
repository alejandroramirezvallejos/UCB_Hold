import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { AccesoriosEditarComponent } from './accesorios-editar.component';
describe('AccesoriosEditarComponent', () => {
  let component: AccesoriosEditarComponent;
  let fixture: ComponentFixture<AccesoriosEditarComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [AccesoriosEditarComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(AccesoriosEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
