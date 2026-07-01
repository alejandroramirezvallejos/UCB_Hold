import { ComponentFixture, TestBed } from '@angular/core/testing';
import { withDefaultTestingProviders } from '@shared/lib/testing';
import { GruposEquiposEditarComponent } from './grupos-equipos-editar.component';
describe('GruposEquiposEditarComponent', () => {
  let component: GruposEquiposEditarComponent;
  let fixture: ComponentFixture<GruposEquiposEditarComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule(
      withDefaultTestingProviders({
        imports: [GruposEquiposEditarComponent],
      }),
    ).compileComponents();
    fixture = TestBed.createComponent(GruposEquiposEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
