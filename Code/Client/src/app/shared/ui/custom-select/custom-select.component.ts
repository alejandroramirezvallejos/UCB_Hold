import {
  Component,
  ElementRef,
  forwardRef,
  HostListener,
  Input,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { OpcionSelect } from './opcion-select';

@Component({
  selector: 'app-custom-select',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './custom-select.component.html',
  styleUrl: './custom-select.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CustomSelectComponent),
      multi: true,
    },
  ],
})
export class CustomSelectComponent implements ControlValueAccessor {
  @Input() placeholder = 'Seleccionar';
  @Input() invalid = false;
  @Input() set opciones(valor: Array<OpcionSelect | string>) {
    this.opcionesNormalizadas = (valor ?? []).map((o) =>
      typeof o === 'string' ? { value: o, label: o } : o,
    );
  }

  opcionesNormalizadas: OpcionSelect[] = [];
  abierto = false;
  disabled = false;
  valor: unknown = null;

  private onChange: (valor: unknown) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(private readonly elementRef: ElementRef) {}

  get etiquetaActual(): string {
    const opcion = this.opcionesNormalizadas.find(
      (o) => o.value === this.valor,
    );
    return opcion ? opcion.label : this.placeholder;
  }

  get tienePlaceholder(): boolean {
    return !this.opcionesNormalizadas.some((o) => o.value === this.valor);
  }

  alternar(): void {
    if (this.disabled) return;
    this.abierto = !this.abierto;
    if (this.abierto) this.onTouched();
  }

  seleccionar(opcion: OpcionSelect): void {
    this.valor = opcion.value;
    this.onChange(this.valor);
    this.abierto = false;
  }

  @HostListener('document:click', ['$event'])
  onClickFuera(evento: Event): void {
    if (
      this.abierto &&
      !this.elementRef.nativeElement.contains(evento.target)
    ) {
      this.abierto = false;
    }
  }

  writeValue(valor: unknown): void {
    this.valor = valor;
  }
  registerOnChange(fn: (valor: unknown) => void): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }
  setDisabledState(disabled: boolean): void {
    this.disabled = disabled;
  }
}
