export interface Carrito {
  [id: number]: {
    nombre: string;
    cantidad: number;
    fecha_inicio: string | null;
    fecha_final: string | null;
    imagen : string; 
  };
}
