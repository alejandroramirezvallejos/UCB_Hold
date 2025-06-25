import { Basemodel } from "../base/Basemodel";

export class Categorias extends Basemodel {
    Nombre?: string | null;

    constructor(){
        super();
        this.Nombre = null;
    }

}