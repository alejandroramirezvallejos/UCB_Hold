export interface Carrito {
  [id: number]: {
    nombre: string;
    cantidad: number;
    fecha_inicio: Date | null;
    fecha_final: Date | null;
    imagen : string; 
  };
}
