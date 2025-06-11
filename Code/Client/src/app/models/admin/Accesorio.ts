export interface Accesorio {
  id: number;
  nombre: string;
  modelo: string;
  tipo: string;
  descripcion?: string;
  codigo_imt: string;
  precio: number;
  url_data_sheet?: string;
  nombreEquipoAsociado ? : string;
}
