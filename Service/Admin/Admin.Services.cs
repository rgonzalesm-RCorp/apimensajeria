using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ApiMensajeria
{
    public class AdminService
    {
        private const string NOMBREARCHIVO = "Admin.Service.cs";
        public async Task<ResponseListaMensajes> GetListaMensajes(string logTransaccionId, string codigoEmpresaCC, int tipoMensajeId, string fechaInicio, string fechaFin)
        {
            var adminDat = new AdminData();
            ResponseListaMensajes responseListaMensajes = new ResponseListaMensajes
            {
                status = "999",
                message = "Hay errores",
                data = new List<dynamic>()
            };

            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Inicio del Metodo RegistroUsuarioService", $"codigoEmpresaCC: {codigoEmpresaCC}");

            List<ListaMensajes> listaMensajes = adminDat.GetListaMensajes(logTransaccionId, codigoEmpresaCC, tipoMensajeId, fechaInicio, fechaFin);
            if (listaMensajes == null || listaMensajes.Count == 0)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "No se encontraron mensajes", $"CodigoEmpresaCC: {codigoEmpresaCC}");
                responseListaMensajes.message = "No se encontraron mensajes";
                return responseListaMensajes;
            }
            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo RegistroUsuarioService", $"Mensajes encontrados: {listaMensajes.Count}");
            responseListaMensajes.status = "000";
            responseListaMensajes.message = "Mensajes encontrados correctamente";
            responseListaMensajes.data = listaMensajes.Cast<dynamic>().ToList();
            return responseListaMensajes;
        }
        public async Task<DetailsSmsResponse> GetDetalleMensaje(string logTransaccionId, int smsId)
        {
            var adminDat = new AdminData();
            var archivos = new SaveDocumentoFisico();
            DetailsSmsResponse responseListaMensajes = new DetailsSmsResponse
            {
                status = "999",
                message = "Hay errores",
                data = new DetailMensaje()
            };

            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Inicio del Metodo GetDetalleMensaje", $"smsId: {smsId}");
            SmsDetails? sms = await adminDat.GetMensajeData(logTransaccionId, smsId);


            if (sms == null)
            {
                
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "No se encontró el mensaje", $"smsId: {smsId}");
                responseListaMensajes.message = "No se encontró el mensaje";
                return responseListaMensajes;
            }
            if (sms.TipoSmsId != 1)
            {
                var (base64Archivo, success) = await archivos.ConvertirArchivoABase64(sms.Documento);
                responseListaMensajes.data.Base64 = success ? base64Archivo : "";
                if (success)
                { 
                    responseListaMensajes.data.Extension = Path.GetExtension(sms.Documento);
                }
                else
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error al convertir archivo a Base64", $"smsId: {smsId}");
                }
            }

            responseListaMensajes.data.SmsId = sms.SmsId;
            responseListaMensajes.data.TipoSmsId = sms.TipoSmsId;
            responseListaMensajes.data.Mensaje = sms.Mensaje;
            responseListaMensajes.data.Documento = sms.Documento;
            responseListaMensajes.data.Caption = sms.Caption; 

            List<ListaDestinatario> listaDestinatarios = adminDat.GetDetalleMensaje(logTransaccionId, smsId);
            if (listaDestinatarios == null || listaDestinatarios.Count == 0)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "No se encontraron destinatarios", $"smsId: {smsId}");
                responseListaMensajes.message = "No se encontraron destinatarios";
                return responseListaMensajes;
            }
            responseListaMensajes.data.Destinatarios = listaDestinatarios;
            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo GetDetalleMensaje", $"Destinatarios encontrados: {listaDestinatarios.Count}");
            responseListaMensajes.status = "000";
            responseListaMensajes.message = "Destinatarios encontrados correctamente";
            return responseListaMensajes;
        }
    }
}
