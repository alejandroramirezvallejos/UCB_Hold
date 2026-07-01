import { BaseTablaComponent } from './base';
import { AdminTableSort } from './admin-table-sort';
import { AdminTableTab } from './admin-table-tab';

export abstract class Tabla extends BaseTablaComponent {
  public columnas: string[] = [];
  activeTab: AdminTableTab = 'tabla';

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

  abstract aplicarFiltros(event?: [string, string]): void;

  sortTable(event: AdminTableSort): void {}

  protected toLocalISOString(date: Date): string {
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60000);

    return localDate.toISOString().split('T')[0];
  }
}
