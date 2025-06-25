export class Comentario {
    Id: string ;
    CarnetUsuario: string | null;
    NombreUsuario: string | null; 
    ApellidoPaternoUsuario: string | null;
    IdGrupoEquipo: number | null;
    Contenido: string| null;
    Likes: number | null;
    FechaCreacion: string  | null;

    constructor() {
        this.Id = '';
        this.CarnetUsuario = null;
        this.NombreUsuario = null; 
        this.ApellidoPaternoUsuario = null;
        this.IdGrupoEquipo = null;
        this.Contenido = null;
        this.Likes = null;
        this.FechaCreacion = null;
    }


}