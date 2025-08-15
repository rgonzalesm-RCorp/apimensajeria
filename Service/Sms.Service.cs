using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json;
using RestSharp;

namespace ApiMensajeria
{
    public class SmsService
    {
        private const string NOMBREARCHIVO = "Sms.Service.cs";

        private static (string mensaje, bool responseFunction) ValidacionesSendWA(string logTransaccionId
        , string tipoMensaje, string codigoEmpresaCC, int codigoUsuarioXP, string mensaje, string dataBase64, string Caption, out int tipoSmsId, out List<TipoSms> responseTipoSms, out InstanciaxEmpresa instancia, out SmsSave sms)
        {
            tipoSmsId = 0;
            instancia = new();
   
            responseTipoSms = new List<TipoSms>();
            sms = new();
            var smsService = new SmsData();
            var instanciaService = new InstanciaData();
            var configuracionEmpresa = new ConfiguracionPorEmpresasData();
            tipoSmsId = Convert.ToInt32(AppSettings.GetSetting($"TipoSms:{tipoMensaje}"));

            responseTipoSms = smsService.GetTipoSmsData(logTransaccionId, tipoSmsId);
            if (responseTipoSms.Count <= 0)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"No se puedo Obtener los tipos de mensajes");
                return ("No se puedo Obtener los tipos de mensajes", false);
            }
            instancia = instanciaService.GetInstanciaDataXEmpresaCC(logTransaccionId, codigoEmpresaCC);
            if (instancia == null || instancia.InstanciaId <= 0)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"No se puedo Obtener la instancia");
                return ("No se puedo Obtener la instancia", false);

            }
  
            sms = new SmsSave
            {
                InstanciaId = instancia.InstanciaId,
                EmpreaId = instancia.EmpresaId,
                Mensaje = mensaje,
                CodigoUsuario = codigoUsuarioXP,
                TipoSmsId = tipoSmsId,
                Documento = dataBase64,
                Caption = Caption
            };
            return ("No hay errores", true);
        }
        public async Task<(string message, bool responseFunction)> SendText(string logTransaccionId, RequestChatSms objBody)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Incio SendText", $"objBody: {JsonConvert.SerializeObject(objBody)}");
            var smsService = new SmsData();
            var instanciaService = new InstanciaData();
            var configuracionEmpresa = new ConfiguracionPorEmpresasData();

            try
            {
                var (mensaje, status) = ValidacionesSendWA(
                    logTransaccionId, "CHAT", objBody.codigoEmpresaCC, objBody.codigoUsuarioXP
                    , objBody.mensaje, "", "", out int tipoSmsId, out List<TipoSms> responseTipoSms, out InstanciaxEmpresa instancia
                    , out SmsSave sms);

                if (!status) return (mensaje, status);

                int smsId = await smsService.SaveSmsData(logTransaccionId, sms);

                foreach (var destinatario in objBody.numeros)
                {
                    try
                    {
                        int destinatarioId = await smsService.SaveDestinatarioData(logTransaccionId, smsId, objBody.codigoUsuarioXP, destinatario);

                        /*var parametros = new List<ParamsBodyRequest>
                        {
                            new() { clave = "to", valor = destinatario.NroTelefono },
                            new() { clave = "body", valor = sms.Mensaje }
                        };

                        ApiService API_SERVICE = new();

                        string rawResponse = await API_SERVICE.HttpUltraMsg(
                            ruta: responseTipoSms[0].Ruta,
                            http: Method.Post,
                            instanceId: instancia.InstanceIdUltraMsg,
                            token: instancia.Token,
                            objBody: parametros
                        );

                        var response = !string.IsNullOrEmpty(rawResponse) ? JsonConvert.DeserializeObject<SendTextResponseUltraMsg>(rawResponse) : null;

                        string message = response?.message ?? "Sin respuesta";

                        await smsService.UpdateDestinatarioData(logTransaccionId, destinatarioId, message, rawResponse);*/
                    }
                    catch (Exception ex)
                    {
                        LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Error enviando mensaje a {destinatario.NroTelefono}: {ex.Message}");
                    }
                }
                return ("Se completo correctamente", true);
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Error: {ex.Message}");
                return (ex.Message, false);
            }
        }
        public async Task<(string message, bool responseFunction)> SendImage(string logTransaccionId, RequestSendImage objBody)
        {
            bool responseFunction = false;
            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Incio SendText", $"objBody: {JsonConvert.SerializeObject(objBody)}");
            var smsService = new SmsData();
            var instanciaService = new InstanciaData();
            var configuracionEmpresa = new ConfiguracionPorEmpresasData();

            try
            {

                var (mensaje, status) = ValidacionesSendWA(
                    logTransaccionId, "IMAGE", objBody.codigoEmpresaCC, objBody.codigoUsuarioXP
                    , objBody.mensaje, objBody.dataBase64, "", out int tipoSmsId, out List<TipoSms> responseTipoSms, out InstanciaxEmpresa instancia
                    , out SmsSave sms);

                if (!status) return (mensaje, status);

                string ruta = SaveDocumentoFisico.GuardarArchivoBase64(instancia.EmpresaId, objBody.dataBase64, objBody.extension);
                sms.Documento = ruta;
                int smsId = await smsService.SaveSmsData(logTransaccionId, sms);

                foreach (var destinatario in objBody.numeros)
                {
                    try
                    {
                        int destinatarioId = await smsService.SaveDestinatarioData(logTransaccionId, smsId, objBody.codigoUsuarioXP, destinatario);
                        /*var parametros = new List<ParamsBodyRequest>
                        {
                            new() { clave = "to", valor = destinatario.NroTelefono },
                            new() { clave = "image", valor = objBody.dataBase64 },
                            new() { clave = "caption", valor = sms.Mensaje },
                        };

                        ApiService API_SERVICE = new();
                        string rawResponse = await API_SERVICE.HttpUltraMsg(
                            ruta: responseTipoSms[0].Ruta,
                            http: Method.Post,
                            instanceId: instancia.InstanceIdUltraMsg,
                            token: instancia.Token,
                            objBody: parametros
                        );

                        var response = !string.IsNullOrEmpty(rawResponse)
                            ? JsonConvert.DeserializeObject<SendTextResponseUltraMsg>(rawResponse)
                            : null;

                        string message = response?.message ?? "Sin respuesta";

                        await smsService.UpdateDestinatarioData(logTransaccionId, destinatarioId, message, rawResponse);*/
                    }
                    catch (Exception ex)
                    {
                        LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Error enviando mensaje a {destinatario.NroTelefono}: {ex.Message}");
                    }
                }
                responseFunction = true;
                return ("Se completo correctamente", true);
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Error: {ex.Message}");
                return (ex.Message, false);
            }
        }
        public async Task<(string message, bool responseFunction)> SendDocument(string logTransaccionId, RequestSendDocument objBody)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Incio SendText", $"objBody: {JsonConvert.SerializeObject(objBody)}");
            var smsService = new SmsData();
            var instanciaService = new InstanciaData();
            var configuracionEmpresa = new ConfiguracionPorEmpresasData();
            try
            {

                var (mensaje, status) = ValidacionesSendWA(
                    logTransaccionId, "DOCUMENT", objBody.codigoEmpresaCC, objBody.codigoUsuarioXP
                    , objBody.mensaje, objBody.dataBase64, objBody.fileName, out int tipoSmsId, out List<TipoSms> responseTipoSms, out InstanciaxEmpresa instancia, out SmsSave sms);

                if (!status) return (mensaje, status);

                string ruta = SaveDocumentoFisico.GuardarArchivoBase64(instancia.EmpresaId, objBody.dataBase64, objBody.extension);
                sms.Documento = ruta;

                int smsId = await smsService.SaveSmsData(logTransaccionId, sms);

                foreach (var destinatario in objBody.numeros)
                {
                    try
                    {
                        int destinatarioId = await smsService.SaveDestinatarioData(
                            logTransaccionId, smsId, objBody.codigoUsuarioXP, destinatario);
                        /*var parametros = new List<ParamsBodyRequest>
                        {
                            new() { clave = "to", valor = destinatario.NroTelefono },
                            new() { clave = "filename", valor = objBody.fileName },
                            new() { clave = "document", valor = objBody.dataBase64 },
                            new() { clave = "caption", valor = sms.Mensaje },
                        };

                        ApiService API_SERVICE = new();
                        string rawResponse = await API_SERVICE.HttpUltraMsg(
                            ruta: responseTipoSms[0].Ruta,
                            http: Method.Post,
                            instanceId: instancia.InstanceIdUltraMsg,
                            token: instancia.Token,
                            objBody: parametros
                        );

                        var response = !string.IsNullOrEmpty(rawResponse)
                            ? JsonConvert.DeserializeObject<SendTextResponseUltraMsg>(rawResponse)
                            : null;

                        string message = response?.message ?? "Sin respuesta";

                        await smsService.UpdateDestinatarioData(
                            logTransaccionId, destinatarioId, message, rawResponse);*/
                    }
                    catch (Exception ex)
                    {
                        LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Error enviando mensaje a {destinatario.NroTelefono}: {ex.Message}");
                    }
                }
                return ("Se completo correctamente", true);
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Error: {ex.Message}");
                return (ex.Message, false);
            }
        }
        public async Task<bool> SendVideo(string logTransaccionId, RequestSendVideo objBody)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Incio SendVideo", $"objBody: {JsonConvert.SerializeObject(objBody)}");
            var smsService = new SmsData();
            var instanciaService = new InstanciaData();

            try
            {
                int tipoSmsId = Convert.ToInt32(AppSettings.GetSetting("TipoSms:VIDEO"));

                List<TipoSms> responseTipoSms = smsService.GetTipoSmsData(logTransaccionId, tipoSmsId);

                var instancia = instanciaService.GetInstanciaDataXEmpresaCC(logTransaccionId, objBody.codigoEmpresaCC);
                if (instancia == null || instancia.InstanciaId <= 0)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"No se puedo Obtener la instancia");
                }

                var sms = new SmsSave
                {
                    InstanciaId = instancia.InstanciaId,
                    EmpreaId = instancia.EmpresaId,
                    Mensaje = objBody.mensaje,
                    CodigoUsuario = objBody.codigoUsuarioXP,
                    TipoSmsId = tipoSmsId
                };

                int smsId = await smsService.SaveSmsData(logTransaccionId, sms);

                foreach (var destinatario in objBody.numeros)
                {
                    try
                    {
                        int destinatarioId = await smsService.SaveDestinatarioData(
                            logTransaccionId, smsId, objBody.codigoUsuarioXP, destinatario);
                        var parametros = new List<ParamsBodyRequest>
                        {
                            new() { clave = "to", valor = destinatario.NroTelefono },
                            new() { clave = "video", valor = objBody.dataBase64 },
                            new() { clave = "caption", valor = sms.Mensaje },
                        };

                        ApiService API_SERVICE = new();
                        string rawResponse = await API_SERVICE.HttpUltraMsg(
                            ruta: responseTipoSms[0].Ruta,
                            http: Method.Post,
                            instanceId: instancia.InstanceIdUltraMsg,
                            token: instancia.Token,
                            objBody: parametros
                        );

                        var response = !string.IsNullOrEmpty(rawResponse)
                            ? JsonConvert.DeserializeObject<SendTextResponseUltraMsg>(rawResponse)
                            : null;

                        string message = response?.message ?? "Sin respuesta";

                        await smsService.UpdateDestinatarioData(
                            logTransaccionId, destinatarioId, message, rawResponse);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Error enviando mensaje a {destinatario.NroTelefono}: {ex.Message}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Error: {ex.Message}");
                return false;
            }
        }
        public List<TipoSms> GetTipoSms(string logTransaccionId)
        {
            var smsService = new SmsData();
            List<TipoSms> lista = [];
            try
            {
                lista = smsService.GetTipoSmsData(logTransaccionId, 0);
            }
            catch (System.Exception)
            {
                lista = [];
            }
            return lista;
        }
    }
}
