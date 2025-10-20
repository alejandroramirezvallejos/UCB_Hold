import { signal, WritableSignal } from "@angular/core";

export abstract class BaseTablaComponent {

    error : WritableSignal<boolean> = signal(false);
    mensajeerror:string="Error desconocido , intente mas tarde";

    exito : WritableSignal<boolean> = signal(false);
    mensajeexito:string="Aviso informativo desconocido";

    aviso : WritableSignal<boolean> = signal(false);
    mensajeaviso:string="Aviso desconocido";

}