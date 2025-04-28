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

            var conexion = new NpgsqlConnection("Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=Fernando");

            conexion.Open();

            var comando = new NpgsqlCommand("SELECT * FROM public.grupos_equipos", conexion);

            var resultado = comando.ExecuteReader();

            var mandar = new List<object>();

            while (resultado.Read())
            {
                var fila = new 
                {
                     nombre = resultado["nombre"].ToString(),

                };

                mandar.Add(fila);
            }

            conexion.Close();
            return Ok(mandar);
        }
    }
}
