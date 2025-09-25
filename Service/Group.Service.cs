using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ApiMensajeria
{
    public class GroupsService
    {
        private const string NOMBREARCHIVO = "Groups.Service.cs";
        public async Task<(List<WhatsAppGroup> lista, string mensaje, bool status)> GetGroupsService(string logTransaccionId, string codigoEmpresaCC)
        {
            try
            {
                var apiService = new ApiService();
                InstanciaData  instanciaData = new InstanciaData();
                (string instanceId, string token) = instanciaData.GetInstanciaTokenData(logTransaccionId, codigoEmpresaCC);
                Console.WriteLine($"InstanceId: {instanceId}, Token: {token}");
                if (string.IsNullOrEmpty(instanceId) || string.IsNullOrEmpty(token))
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error al obtener instancia o token", $"InstanceId or Token is null or empty for company code: {codigoEmpresaCC}");
                    return (new List<WhatsAppGroup>(), "Error al obtener instancia o token, la empresa no tiene una instancia activa", false);
                }
                string rawResponse = await apiService.HttpUltraMsg(
                    ruta: "groups",
                    http: Method.Get,
                    instanceId: instanceId,
                    token: token,
                    objBody: new List<ParamsBodyRequest>()
                );
                if (string.IsNullOrEmpty(rawResponse) || rawResponse.Length == 0)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Respuesta vacía de la API", $"Empty response from API for company code: {codigoEmpresaCC}");
                    return (new List<WhatsAppGroup>(), "Respuesta vacía de la API", false);
                }
                List<WhatsAppGroup> groups = JsonConvert.DeserializeObject<List<WhatsAppGroup>>(rawResponse);
                if (groups == null)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error al deserializar la respuesta", $"Response could not be deserialized for company code: {codigoEmpresaCC}");
                    return (new List<WhatsAppGroup>(), "Error al deserializar la respuesta de la API", false);
                }
                groups = groups.Where(x => x?.groupMetadata?.participants != null).ToList();
                groups = groups.Where(x => x?.groupMetadata?.participants.Count > 0).ToList();

                return (groups, "Success", true);
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Excepción en GetGroupsService", ex.Message);
                return (new List<WhatsAppGroup>(), "Error inesperado en el servicio de grupos", false);
            }

        }
    }
}
