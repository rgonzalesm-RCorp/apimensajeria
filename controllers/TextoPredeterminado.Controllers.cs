using Microsoft.AspNetCore.Mvc;

namespace ApiMensajeria.Controllers
{
    [ApiController]
    [Route("")]
    public class TextoPredeterminadoControllers : ControllerBase
    {
        string NOMBREARCHIVO = "TextoPredeterminado.Controllers.cs";

        [HttpPost("guardar/texto/predeterminado")]
        public async Task<ActionResult> GuardarTextoPredeterminado(Request_TextoPredeterminado_Save request)
        {
            TextoPredeterminadoService textoPredeterminadoService = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ResponseTextoPredeterminado responseTextoPredeterminado = textoPredeterminadoService.GuardarTextoPredeterminadoService(logTransaccionId.ToString(), request);
            return Ok(new
            {
                status = responseTextoPredeterminado.status,
                message = responseTextoPredeterminado.message,
                data = responseTextoPredeterminado.data,
            });
        }
        [HttpPost("update/texto/predeterminado")]
        public async Task<ActionResult> UpdateTextoPredeterminado(Request_TextoPredeterminado_Update request)
        {
            TextoPredeterminadoService textoPredeterminadoService = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ResponseTextoPredeterminado responseTextoPredeterminado = textoPredeterminadoService.UpdateTextoPredeterminadoService(logTransaccionId.ToString(), request);
            return Ok(new
            {
                status = responseTextoPredeterminado.status,
                message = responseTextoPredeterminado.message,
                data = responseTextoPredeterminado.data,
            });
        }
        [HttpGet("list/texto/predeterminado")]
        public async Task<ActionResult> GetListaTextoPredeterminado(string codigoEmpresaCC)
        {
            try
            {
                TextoPredeterminadoService textoPredeterminadoService = new();
                long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var (listaTextoPredeterminado, response) = textoPredeterminadoService.GetListaTextoPredeterminadoService(logTransaccionId.ToString(), codigoEmpresaCC);
                return Ok(new
                {
                    status = response ? "000" : "999",
                    message = response ? "Lista de textos predeterminados obtenida correctamente." : "Error al obtener la lista de textos predeterminados.",
                    data = listaTextoPredeterminado,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "999",
                    message = $"Error al procesar la solicitud: {ex.Message}",
                    data = new List<Lista_TextoPredeterminado>()
                });
            }
        }
        [HttpPost("delete/texto/predeterminado")]
        public async Task<ActionResult> DeleteTextoPredeterminado(int textoPredeterminadoId)
        {
            TextoPredeterminadoService textoPredeterminadoService = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            bool response = textoPredeterminadoService.DeleteTextoPredeterminadoService(logTransaccionId.ToString(), textoPredeterminadoId);
            return Ok(new
            {
                status = response ? "000" : "999",
                message = response ? "Texto predeterminado eliminado correctamente." : "Error al eliminar el texto predeterminado.",
                data = ""
            });
        }
    }
}