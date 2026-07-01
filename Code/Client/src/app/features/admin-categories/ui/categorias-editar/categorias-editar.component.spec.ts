import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { CategoriasEditarComponent } from './categorias-editar.component';
describe('CategoriasEditarComponent', () => {
  let component: CategoriasEditarComponent;
  let fixture: ComponentFixture<CategoriasEditarComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [CategoriasEditarComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(CategoriasEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
