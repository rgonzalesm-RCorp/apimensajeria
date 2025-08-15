using Microsoft.AspNetCore.Mvc;

namespace ApiMensajeria.Controllers
{
    [ApiController]
    [Route("")]
    public class UtilsControllers : ControllerBase
    {
        string NOMBREARCHIVO = "Utils.Controllers.cs";
        [HttpGet("get/fecha/helper")]
        public async Task<ActionResult> GetFechaHelper()
        {
            InstaciaService INSTANCIASERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var meses = FechaHelper.ObtenerMeses();
            var anios = FechaHelper.ObtenerAnios();
            return Ok(new
            {
                status = "000",
                message = "No hay errores",
                data = new { meses, anios }
            });
        }
        
    }

}