export class Notificacion {

    Id: string;
    CarnetUsuario: string | null;
    Titulo: string | null;
    Contenido: string | null;
    FechaEnvio: string | null;
    Leido : boolean | null;

    constructor() {
        this.Id = '';
        this.CarnetUsuario = null;
        this.Titulo = null;
        this.Contenido = null;
        this.FechaEnvio = null;
        this.Leido = null;
    }

}