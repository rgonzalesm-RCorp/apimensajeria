using Microsoft.AspNetCore.Mvc;

namespace ApiMensajeria.Controllers
{
    [ApiController]
    [Route("")]
    public class AdminControllers : ControllerBase
    {
        string NOMBREARCHIVO = "Admin.Controllers.cs";
        [HttpGet("get/lista/mensajes")]
        public async Task<ActionResult> GetListaMensajes(string codigoEmpresaCC, int tipoMensajeId, string fechaInicio, string fechaFin)
        {
            AdminService ADMIN_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ResponseListaMensajes responseListaMensajes = await ADMIN_SERVICE.GetListaMensajes(logTransaccionId.ToString(), codigoEmpresaCC, tipoMensajeId, fechaInicio, fechaFin);
            var meses = FechaHelper.ObtenerMeses();
            var anios = FechaHelper.ObtenerAnios();
            return Ok(new
            {
                status = responseListaMensajes.status,
                message = responseListaMensajes.message,
                data = responseListaMensajes.data,
            });
        }
        [HttpGet("get/details/mensajes")]
        public async Task<ActionResult> GetDetalleMensaje(int smsId)
        {
            AdminService ADMIN_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            DetailsSmsResponse responseListaMensajes = await ADMIN_SERVICE.GetDetalleMensaje(logTransaccionId.ToString(), smsId);
            var meses = FechaHelper.ObtenerMeses();
            var anios = FechaHelper.ObtenerAnios();
            return Ok(new
            {
                status = responseListaMensajes.status,
                message = responseListaMensajes.message,
                data = responseListaMensajes.data,
            });
        }
        [HttpGet("get/lista/empresas")]
        public async Task<ActionResult> GetListaEmpresas()
        {
            EmpresaService EMPRESA_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            List<EmpresaDetalle> empresas = await EMPRESA_SERVICE.GetListaEmpresas(logTransaccionId.ToString());
            return Ok(new
            {
                status = "000",
                message = "Empresas obtenidas correctamente",
                data = empresas,
            });
        }
        [HttpPost("delete/empresas")]
        public async Task<ActionResult> DeleteEmpresas(int empresaId)
        {
            EmpresaService EMPRESA_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ResponseListaMensajes responseListaMensajes = await EMPRESA_SERVICE.DeleteEmpresa(logTransaccionId.ToString(), empresaId);
            return Ok(new
            {
                status = responseListaMensajes.status,
                message = responseListaMensajes.message,
                data = responseListaMensajes.data,
            });
        }
        [HttpPost("guardar/empresa")]
        public async Task<ActionResult> GuardarEmpresa(SaveEmpresaRequest request)
        {
            EmpresaService EMPRESA_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ResponseListaMensajes responseListaMensajes = await EMPRESA_SERVICE.GuardarEmpresa(logTransaccionId.ToString(), request);
            return Ok(new
            {
                status = responseListaMensajes.status,
                message = responseListaMensajes.message,
                data = responseListaMensajes.data,
            });
        }
        
    }
}