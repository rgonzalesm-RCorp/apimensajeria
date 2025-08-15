using Microsoft.AspNetCore.Mvc;

namespace ApiMensajeria.Controllers
{
    [ApiController]
    [Route("")]
    public class InstanciaControllers : ControllerBase
    {
        string NOMBREARCHIVO = "Instancia.Controllers.cs";
        [HttpPost("registrar/usuario")]
        public async Task<ActionResult> RegistrarUsuario(int codigoUsuarioXp, string codigoEmpresaCC, int instanciaId)
        {
            InstaciaService INSTANCIASERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ResponseRegistroUsuario response = await INSTANCIASERVICE.RegistroUsuarioService(logTransaccionId.ToString(), codigoUsuarioXp, codigoEmpresaCC, instanciaId);
            return Ok(response);
        }
        [HttpGet("get/empresa")]
        public async Task<ActionResult> GetEmpresaController(string codigoEmpresaCC)
        {
            InstaciaService INSTANCIASERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Empresa responseEmpresa = INSTANCIASERVICE.GetEmpresaService(logTransaccionId.ToString(), codigoEmpresaCC);
            return Ok(new
            {
                status = "000",
                message = "No hay errores",
                data = responseEmpresa
            });
        }
        [HttpGet("get/instancia")]
        public async Task<ActionResult> GetInstanciaController(int instanceId)
        {
            InstaciaService INSTANCIASERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            List<Instancia> responseInstancia = INSTANCIASERVICE.GetInstanciaService(logTransaccionId.ToString(), instanceId);
            return Ok(new
            {
                status = "000",
                message = "No hay errores",
                data = responseInstancia
            });
        }
        [HttpGet("get/usuario")]
        public async Task<ActionResult> GetUsuarioController(int empresaId)
        {
            InstaciaService INSTANCIASERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UsuarioEmpresaInstancia responseUsuario = INSTANCIASERVICE.GetUsuarioEmrpesaInstanciaService(logTransaccionId.ToString(), empresaId);
            return Ok(new
            {
                status = "000",
                message = "No hay errores",
                data = responseUsuario
            });
        }
        [HttpGet("get/status")]
        public async Task<ActionResult> GetSatusController(int instanciaId, int usuarioId)
        {
            InstaciaService INSTANCIASERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var (responseUsuario, message, status) = await INSTANCIASERVICE.GetStatusService(logTransaccionId.ToString(), instanciaId, usuarioId);
            return Ok(new
            {
                status = status ? "000" : "999",
                message = message,
                data = responseUsuario
            });
        }
        [HttpPost("guardar/instancia")]
        public async Task<ActionResult> GuardarInstanciaController(Instancia request)
        {
            InstaciaService INSTANCIASERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            bool responseListaMensajes = INSTANCIASERVICE.SaveInstanciaData(logTransaccionId.ToString(), request);
            return Ok(new
            {
                status = responseListaMensajes ? "000" : "999",
                message = responseListaMensajes ? "No hay errores" : "Hay errores",
                data = responseListaMensajes
            });
        }
        [HttpPost("delete/instancia")]
        public async Task<ActionResult> DeleteInstanciaController(int instanciaId)
        {
            InstaciaService INSTANCIASERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            bool response = INSTANCIASERVICE.DeleteInstanciaData(logTransaccionId.ToString(), instanciaId);
            return Ok(new
            {
                status = response ? "000" : "999",
                message = response ? "No hay errores" : "Hay errores",
                data = response
            });
        }

    }

}