export interface Accesorio {
  id: number;
  nombre: string;
  modelo: string;
  tipo: string;
  descripcion?: string;
  codigo_imt: string;
  precio?: number|null;
  url_data_sheet?: string|null;
  nombreEquipoAsociado?: string;
}
