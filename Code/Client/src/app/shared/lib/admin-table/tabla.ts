import { BaseTablaComponent } from './base';
import { AdminTableSort } from './admin-table-sort';
import { AdminTableTab } from './admin-table-tab';

export abstract class Tabla extends BaseTablaComponent {
  public columnas: string[] = [];
  activeTab: AdminTableTab = 'tabla';
  sortColumn = '';
  sortDirection: AdminTableSort['dir'] = 'asc';

  seleccionarTab(tab: AdminTableTab): void {
    this.activeTab = tab;
  }

  estaTabActiva(tab: AdminTableTab): boolean {
    return this.activeTab === tab;
  }

  protected formatDate(date: Date | string | null): string {
    if (!date) return '';

    const parsedDate = new Date(date);

    if (Number.isNaN(parsedDate.getTime())) return '';

    const localDateStr = this.toLocalISOString(parsedDate);
    const [year, month, day] = localDateStr.split('-');

    return `${day}/${month}/${year}`;
  }

  protected normalizeText(text: unknown): string {
    return String(text ?? '')
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '');
  }

  protected sortableValue<T extends object>(item: T, key: keyof T): string {
    return this.normalizeText(item[key]);
  }

  ordenarPorColumna(columna: string): void {
    const columnaOrdenable = columna.trim();

    if (!columnaOrdenable) return;

    this.sortDirection =
      this.sortColumn === columnaOrdenable && this.sortDirection === 'asc'
        ? 'desc'
        : 'asc';
    this.sortColumn = columnaOrdenable;
    this.sortTable({ col: columnaOrdenable, dir: this.sortDirection });
  }

  protected aplicarOrdenActualSiExiste(): void {
    if (!this.sortColumn) return;

    this.sortTable({ col: this.sortColumn, dir: this.sortDirection });
  }

  esColumnaOrdenada(columna: string): boolean {
    return this.sortColumn === columna.trim();
  }

  iconoOrdenColumna(columna: string): string {
    if (!this.esColumnaOrdenada(columna)) return 'fa-sort';

    return this.sortDirection === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
  }

  protected compareSortableValues(
    firstValue: unknown,
    secondValue: unknown,
    direction: AdminTableSort['dir'],
  ): number {
    const result = this.compareUnknownValues(firstValue, secondValue);

    return direction === 'asc' ? result : -result;
  }

  abstract aplicarFiltros(event?: [string, string]): void;

  sortTable(event: AdminTableSort): void {}

  protected toLocalISOString(date: Date): string {
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60000);

    return localDate.toISOString().split('T')[0];
  }

  private compareUnknownValues(
    firstValue: unknown,
    secondValue: unknown,
  ): number {
    const firstNumber = Number(firstValue);
    const secondNumber = Number(secondValue);
    const bothAreNumbers =
      firstValue !== null &&
      firstValue !== '' &&
      secondValue !== null &&
      secondValue !== '' &&
      Number.isFinite(firstNumber) &&
      Number.isFinite(secondNumber);

    if (bothAreNumbers) return firstNumber - secondNumber;

    const firstDate = this.toComparableTime(firstValue);
    const secondDate = this.toComparableTime(secondValue);

    if (Number.isFinite(firstDate) && Number.isFinite(secondDate))
      return firstDate - secondDate;

    return this.normalizeText(firstValue).localeCompare(
      this.normalizeText(secondValue),
      undefined,
      { numeric: true, sensitivity: 'base' },
    );
  }

  private toComparableTime(value: unknown): number {
    if (value instanceof Date) return value.getTime();

    if (typeof value !== 'string') return NaN;

    const normalizedValue = value.trim();

    if (!normalizedValue) return NaN;
    if (Number.isFinite(Number(normalizedValue))) return NaN;

    return Date.parse(normalizedValue);
  }
}
