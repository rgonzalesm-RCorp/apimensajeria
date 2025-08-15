using Microsoft.AspNetCore.Mvc;

namespace ApiMensajeria.Controllers
{
    [ApiController]
    [Route("")]
    public class SmsControllers : ControllerBase
    {
        string NOMBREARCHIVO = "Instancia.Controllers.cs";
        [HttpPost("send/text")]
        public async Task<ActionResult> SendTextControllers(RequestChatSms objBody)
        {
            SmsService SMS_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var (message, responseFunction) = await SMS_SERVICE.SendText(logTransaccionId.ToString(), objBody);
            return Ok(new
            {
                status = responseFunction ? "000" : "999",
                message = responseFunction ? "No hay errores" : "Hay errores",
                data = message
            });
        }
        [HttpPost("send/image")]
        public async Task<ActionResult> SendImageControllers(RequestSendImage objBody)
        {
            SmsService SMS_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var (message, responseFunction) = await SMS_SERVICE.SendImage(logTransaccionId.ToString(), objBody);
            return Ok(new
            {
                status = responseFunction ? "000" : "999",
                message = responseFunction ? "No hay errores" : "Hay errores",
                data = message
            });
        }
        [HttpPost("send/document")]
        public async Task<ActionResult> SendDocumentControllers(RequestSendDocument objBody)
        {
            SmsService SMS_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var (message, responseFunction) = await SMS_SERVICE.SendDocument(logTransaccionId.ToString(), objBody);
            return Ok(new
            {
                status = responseFunction ? "000" : "999",
                message = responseFunction ? "No hay errores" : "Hay errores",
                data = message
            });
        }
        [HttpPost("get/tipo/sms")]        
        public async Task<ActionResult> SendVideoControllers()
        {
            SmsService SMS_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            List<TipoSms> response = SMS_SERVICE.GetTipoSms(logTransaccionId.ToString());
            return Ok(new
            {
                status = "000",
                message = "No hay errores",
                data = response
            });
        }
        /*[HttpPost("send/video")]        
        public async Task<ActionResult> SendVideoControllers(RequestSendVideo objBody)
        {
            SmsService SMS_SERVICE = new();
            long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            bool response = await SMS_SERVICE.SendVideo(logTransaccionId.ToString(), objBody);
            return Ok(new
            {
                status = "000",
                message = "No hay errores",
                data = response
            });
        }*/
    }

}