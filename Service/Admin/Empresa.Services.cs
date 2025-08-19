using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ApiMensajeria
{
    public class EmpresaService
    {
        private const string NOMBREARCHIVO = "Empresa.Service.cs";
        public async Task<ResponseListaMensajes> GuardarEmpresa(string logTransaccionId, SaveEmpresaRequest request)
        {
            var empresaDat = new EmpresaData();
            ResponseListaMensajes responseListaMensajes = new ResponseListaMensajes
            {
                status = "999",
                message = "Hay errores",
                data = new List<dynamic>()
            };


            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Inicio del Metodo GuardarEmpresa", $"codigoEmpresaCC: {JsonConvert.SerializeObject(request.Empresa)}");

            var (empresaId, success) = empresaDat.X1(logTransaccionId, request);
            if (!success)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo GuardarEmpresa", $"Error al guardar la empresa");
                return responseListaMensajes;
            }

            responseListaMensajes.status = "000";
            responseListaMensajes.message = "Mensajes encontrados correctamente";
            responseListaMensajes.data = null;
            return responseListaMensajes;
        }

        public async Task<List<EmpresaDetalle>> GetListaEmpresas(string logTransaccionId)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Inicio del Metodo GetListaEmpresas", $"");

            var empresaDat = new EmpresaData();
            var empresas = empresaDat.X2(logTransaccionId);

            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo GetListaEmpresas", $"");

            return empresas.R1;
        }
        public async Task<ResponseListaMensajes> DeleteEmpresa(string logTransaccionId, int empresaId)
        {
            var empresaDat = new EmpresaData();
            ResponseListaMensajes responseListaMensajes = new ResponseListaMensajes
            {
                status = "999",
                message = "Hay errores",
                data = new List<dynamic>()
            };

            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Inicio del Metodo DeleteEmpresa", $"empresaId: {empresaId}");

            var (deletedEmpresaId, success) = empresaDat.X3(logTransaccionId, empresaId);
            if (!success)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo DeleteEmpresa", $"Error al eliminar la empresa");
                return responseListaMensajes;
            }

            responseListaMensajes.status = "000";
            responseListaMensajes.message = "Empresa eliminada correctamente";
            responseListaMensajes.data = null;
            return responseListaMensajes;
        }
    }
}
