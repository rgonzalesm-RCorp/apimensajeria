using System.Text.Json;
using Newtonsoft.Json;
using RestSharp;

namespace ApiMensajeria
{
    public class SendMensajesCronJob
    {
        private const string NOMBREARCHIVO = "SendMensajesCronJob.Service.cs";
        public async Task<bool> SendMsgCronJob(string logTransaccionId)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Inicio del Metodo SendMsgCronJob", $"objBody ");

            var smsData = new SmsData();
            var saveDocumentoFisico = new SaveDocumentoFisico();
            var apiService = new ApiService();
            var configuracionEmpresa = new ConfiguracionPorEmpresasData();

            List<MensajeDto> listaMensajes = smsData.GetMensajesData(logTransaccionId);

            foreach (var mensaje in listaMensajes)
            {
                List<ConfiguracionPorEmpresa> listConfiguracionEmpresa = configuracionEmpresa.GetConfiguracionEmpresaData(logTransaccionId, mensaje.EmpreaId, out CounterEmpresa counter, true);
                if (listConfiguracionEmpresa.Count <= 0)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"No se puedo Obtener la configuracion de la empresa");
                    continue;
                }
                int LimiteDiario = Convert.ToInt32(listConfiguracionEmpresa.Where(x => x.Clave == "LimiteDiario").First().valor);
                int LimiteMensual = Convert.ToInt32(listConfiguracionEmpresa.Where(x => x.Clave == "LimiteMensual").First().valor);
                bool EnviarDespuesDelLimite = Convert.ToBoolean(listConfiguracionEmpresa.Where(x => x.Clave == "EnviarDespuesDelLimite").First().valor);
                bool CobrarDespuesDelLimite = Convert.ToBoolean(listConfiguracionEmpresa.Where(x => x.Clave == "CobrarDespuesDelLimite").First().valor);
                if (counter.Mes > LimiteMensual && !EnviarDespuesDelLimite)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Se ha alcanzado el limite mensual de mensajes");
                    continue;
                }
                if (counter.dia > LimiteDiario && !EnviarDespuesDelLimite)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin SendText", $"Se ha alcanzado el limite diario de mensajes");
                    continue;
                }

                bool cobrar = false;
                if (counter.Mes >= LimiteMensual && CobrarDespuesDelLimite)
                {
                    cobrar = true;
                }
                List<ParamsBodyRequest> parametros = await ConstruirParametrosAsync(mensaje, saveDocumentoFisico, logTransaccionId);
                
                if (parametros.Count == 0)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error al enviar mensaje", $"No se encontraron parámetros para el mensaje con ID: {mensaje.SmsId}");
                    continue;
                }

                string rawResponse = await apiService.HttpUltraMsg(
                    ruta: mensaje.Ruta,
                    http: Method.Post,
                    instanceId: mensaje.InstanceIdUltraMsg,
                    token: mensaje.Token,
                    objBody: parametros
                );

                var response = !string.IsNullOrEmpty(rawResponse)
                    ? JsonConvert.DeserializeObject<SendTextResponseUltraMsg>(rawResponse)
                    : null;

                string message = response?.message ?? "Sin respuesta";

                await smsData.UpdateDestinatarioData(logTransaccionId, mensaje.DestinatarioId, message, rawResponse, cobrar);
            }
            return true;
        }
        private async Task<List<ParamsBodyRequest>> ConstruirParametrosAsync(MensajeDto mensaje, SaveDocumentoFisico saveDocumentoFisico, string logTransaccionId)
        {
            var parametros = new List<ParamsBodyRequest>();

            switch (mensaje.Descripcion)
            {
                case "CHAT" when mensaje.TipoSmsId == 1:
                    parametros.AddRange(new[]
                    {
                        new ParamsBodyRequest { clave = "to", valor = mensaje.NroTelefono },
                        new ParamsBodyRequest { clave = "body", valor = mensaje.Mensaje }
                    });
                    break;

                case "IMAGE" when mensaje.TipoSmsId == 2:
                case "DOCUMENT" when mensaje.TipoSmsId == 3:
                    var (data, status) = await saveDocumentoFisico.ConvertirArchivoABase64(mensaje.Documento);
                    if (!status)
                    {
                        LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error al convertir archivo a Base64", data);
                        break;
                    }

                    parametros.Add(new ParamsBodyRequest { clave = "to", valor = mensaje.NroTelefono });

                    if (mensaje.Descripcion == "IMAGE")
                    {
                        parametros.Add(new ParamsBodyRequest { clave = "image", valor = data });
                        parametros.Add(new ParamsBodyRequest { clave = "caption", valor = mensaje.Mensaje });
                    }
                    else if (mensaje.Descripcion == "DOCUMENT")
                    {
                        parametros.Add(new ParamsBodyRequest { clave = "filename", valor = mensaje.Caption });
                        parametros.Add(new ParamsBodyRequest { clave = "document", valor = data });
                        parametros.Add(new ParamsBodyRequest { clave = "caption", valor = mensaje.Mensaje });
                    }

                    break;
            }

            return parametros;
        }

    }
}
