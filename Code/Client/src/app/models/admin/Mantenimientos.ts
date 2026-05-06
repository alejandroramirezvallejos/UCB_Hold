import { Basemodel } from "../base/Basemodel";
export class Mantenimientos extends Basemodel {
    IdEmpresa: number | null = null;
    NombreEmpresaMantenimiento: string | null = null;
    FechaMantenimiento: Date | null = null;
    FechaFinalDeMantenimiento: Date | null = null;
    Costo: number | null = null;
    Descripcion: string | null = null;
    TipoMantenimiento: string | null = null;
    NombreGrupoEquipo: string | null = null;
    CodigoImtEquipo: number | null = null;
    DescripcionEquipo: string | null = null;
}
