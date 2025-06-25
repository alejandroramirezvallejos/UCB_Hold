export class GrupoEquipo {
  id: number;
  nombre: string | null;
  modelo?: string | null;
  nombreCategoria?: string | null;
  descripcion?: string | null;
  url_data_sheet?: string | null;
  marca?: string | null;
  link?: string | null;

  constructor() {
    this.id = 0;
    this.nombre = null;
    this.modelo = null;
    this.nombreCategoria = null;
    this.descripcion = null;
    this.url_data_sheet = null;
    this.marca = null;
    this.link = null;
  }
  
}
