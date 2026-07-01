export interface AuditLogApiItem {
  Id: number;
  AdminCarnet: string | null;
  AdminNombre: string | null;
  Accion: string | null;
  Entidad: string | null;
  EntidadId: string | number | null;
  Detalle: string | null;
  Timestamp?: string | Date | null;
}
