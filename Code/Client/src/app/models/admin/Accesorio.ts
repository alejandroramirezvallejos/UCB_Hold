export class Accesorio {
  id: number;
  nombre: string | null;
  modelo: string | null;
  tipo: string | null;
  descripcion?: string | null;
  codigo_imt: string | null;
  precio?: number|null;
  url_data_sheet?: string|null;
  nombreEquipoAsociado?: string | null;

  constructor() {
    this.id = 0;
    this.nombre = null;
    this.modelo = null;
    this.tipo = null;
    this.descripcion = null;
    this.codigo_imt = null;
    this.precio = null;
    this.url_data_sheet = null;
    this.nombreEquipoAsociado = null;
  }

}
