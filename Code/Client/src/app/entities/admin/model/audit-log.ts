export interface AuditLogDto {
  Id?: number;
  AdminCarnet?: string;
  AdminNombre?: string;
  Accion?: string;
  Entidad?: string;
  EntidadId?: string;
  Detalle?: string;
  Timestamp?: Date;
}
