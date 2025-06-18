namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IGrupoEquipoControllerTest
    {
        void GetGruposEquipos_ConDatos_RetornaOk();
        void GetGruposEquipos_SinDatos_RetornaOkVacia();
        void GetGruposEquipos_ServicioError_RetornaBadRequest();
        void CrearGrupoEquipo_Valido_RetornaCreated();
        void CrearGrupoEquipo_Invalido_RetornaBadRequest(CrearGrupoEquipoComando comando, System.Exception excepcionLanzada);
        void CrearGrupoEquipo_RegistroExistente_RetornaConflict();
        void CrearGrupoEquipo_ServicioError_RetornaError500();
        void ActualizarGrupoEquipo_Valido_RetornaOk();
        void ActualizarGrupoEquipo_Invalido_RetornaBadRequest(ActualizarGrupoEquipoComando comando, System.Exception excepcionLanzada);
        void ActualizarGrupoEquipo_NoEncontrado_RetornaNotFound();
        void EliminarGrupoEquipo_Valido_RetornaNoContent();
        void EliminarGrupoEquipo_NoEncontrado_RetornaNotFound();
        void EliminarGrupoEquipo_EnUso_RetornaConflict();
    }
}

