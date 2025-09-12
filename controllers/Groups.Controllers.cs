using Microsoft.AspNetCore.Mvc;

namespace ApiMensajeria.Controllers
{
    [ApiController]
    [Route("")]
    public class GroupsControllers : ControllerBase
    {
        string NOMBREARCHIVO = "Groups.Controllers.cs";

        [HttpPost("get/groups")]
        public async Task<ActionResult> GetGroupsControllers(string codigoEmpresaCC)
        {
            TextoPredeterminadoService textoPredeterminadoService = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            GroupsService groupsService = new();
            (List<WhatsAppGroup> lista, string mensaje, bool status) = await groupsService.GetGroupsService(logTransaccionId.ToString(), codigoEmpresaCC);
            return Ok(new
            {
                status = status? "000": "999",
                message = status? "No hay errores": mensaje,
                data = lista,
            });
        }
    }
}