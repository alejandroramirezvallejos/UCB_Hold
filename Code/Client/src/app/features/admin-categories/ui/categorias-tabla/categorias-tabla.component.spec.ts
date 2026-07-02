import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { Categorias } from '@entities/admin';
import { CategoriasTablaComponent } from './categorias-tabla.component';
describe('CategoriasTablaComponent', () => {
  let component: CategoriasTablaComponent;
  let fixture: ComponentFixture<CategoriasTablaComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [CategoriasTablaComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(CategoriasTablaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render the compact table without sortable headers', () => {
    component.categorias = [
      Object.assign(new Categorias(), { Id: 1, Nombre: 'Zeta' }),
      Object.assign(new Categorias(), { Id: 2, Nombre: 'Alpha' }),
    ];
    fixture.detectChanges();

    const sortableHeader = fixture.nativeElement.querySelector('.sortable-th');
    const headers = fixture.nativeElement.querySelectorAll('thead th');

    expect(sortableHeader).toBeNull();
    expect(headers.length).toBe(2);
  });
});
