using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MiProyecto.Controllers
{
    [Route("api/productos")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
       
        [HttpGet]
        public IActionResult GetReservas()
        {
            var parametro = new Dictionary<string, object>();

            var conexion = new NpgsqlConnection("Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=273153");

            conexion.Open();

            var comando = new NpgsqlCommand("SELECT * FROM public.grupos_equipos", conexion);

            var resultado = comando.ExecuteReader();

            var mandar = new List<object>();

            while (resultado.Read())
            {
                var fila = new 
                {    id = resultado["id_grupo_equipo"].ToString(),
                    nombre = resultado["nombre"].ToString(),
                     link = resultado["link"].ToString(),
                     marca = resultado["marca"].ToString(),
                     modelo = resultado["modelo"].ToString(),
                };

                mandar.Add(fila);
            }

            conexion.Close();
            return Ok(mandar);
        }
    }
}
