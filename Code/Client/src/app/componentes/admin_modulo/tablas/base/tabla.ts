import { BaseTablaComponent } from "./base";

 

 export abstract class Tabla extends BaseTablaComponent {

   public columnas: string[] =[];
    

  protected formatDate(date: Date | string | null): string {
    if (!date) return '';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    const localDateStr = this.toLocalISOString(d);  // Usa toLocalISOString para zona local
    const [year, month, day] = localDateStr.split('-');
    return `${day}/${month}/${year}`;  // Formato DD/MM/YYYY
  }

  protected normalizeText(text: string): string {
    if (typeof text !== 'string') {
      return String(text || '').toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');
    }
    return text
      .toLowerCase()
      .normalize('NFD')  // Descompone caracteres con acentos
      .replace(/[\u0300-\u036f]/g, '');  // Elimina diacríticos
  }

  abstract aplicarFiltros(event?: [string, string]) : any ;

  protected toLocalISOString(date: Date): string {
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60000);
    return localDate.toISOString().split('T')[0];
  }

 }