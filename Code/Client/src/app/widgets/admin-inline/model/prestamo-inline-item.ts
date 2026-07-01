export interface PrestamoInlineItem {
  Id: number;
  NombreGrupoEquipo: string | null;
  EstadoPrestamo: string | null;
  FechaSolicitud: string | Date | null;
  FechaDevolucionEsperada: string | Date | null;
}
