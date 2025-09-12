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
            List<WhatsAppGroup> groups = JsonConvert.DeserializeObject<List<WhatsAppGroup>>(rawResponse);
            return (groups, "Success", true);
        }
    }
}
