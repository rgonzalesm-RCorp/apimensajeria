using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ApiMensajeria
{
    public class TextoPredeterminadoService
    {
        private const string NOMBREARCHIVO = "TextoPredeterminado.Service.cs";
        public ResponseTextoPredeterminado GuardarTextoPredeterminadoService(string logTransaccionId, Request_TextoPredeterminado request)
        {
            var textoPredeterminadoDat = new TextoPredeterminadoData();
            ResponseTextoPredeterminado responseTextoPredeterminado = new ResponseTextoPredeterminado
            {
                status = "999",
                message = "Hay errores",
                data = null
            };
            InstanciaData instanciaData = new InstanciaData();

            InstanciaxEmpresa instancia = instanciaData.GetInstanciaDataXEmpresaCC(logTransaccionId, request.CodigoEmpresaCC);
            if (instancia == null)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo GuardarTextoPredeterminadoService", $"Error al obtener la instancia");
                responseTextoPredeterminado.message = "Error al obtener la instancia";
                return responseTextoPredeterminado;
            }
            else
            {
                if (instancia.EmpresaId == null || instancia.EmpresaId == 0)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo GuardarTextoPredeterminadoService", $"Error al obtener la empresaId");
                    responseTextoPredeterminado.message = "Error al obtener la empresaId";
                    return responseTextoPredeterminado;
                }
            }

            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Inicio del Metodo GuardarTextoPredeterminadoService", $"request: {JsonConvert.SerializeObject(request)}");

            bool responseAbm = textoPredeterminadoDat.GuardarTextPredeterminadoData(logTransaccionId, request, instancia.EmpresaId);
            if (!responseAbm)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo GuardarTextoPredeterminadoService", $"Error al guardar el texto predeterminado");
                return responseTextoPredeterminado;
            }

            responseTextoPredeterminado.status = "000";
            responseTextoPredeterminado.message = "Se guardó correctamente el texto predeterminado.";
            responseTextoPredeterminado.data = null;
            return responseTextoPredeterminado;
        }

       public (List<Lista_TextoPredeterminado>, bool) GetListaTextoPredeterminadoService(string logTransaccionId, string codigoEmpresaCC)
       {
           var textoPredeterminadoDat = new TextoPredeterminadoData();
            InstanciaData instanciaData = new InstanciaData();

            InstanciaxEmpresa instancia = instanciaData.GetInstanciaDataXEmpresaCC(logTransaccionId, codigoEmpresaCC);
            if (instancia == null)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo GuardarTextoPredeterminadoService", $"Error al obtener la instancia");
                return (new List<Lista_TextoPredeterminado>(), false);
            }else
            {
                if (instancia.EmpresaId == null || instancia.EmpresaId == 0)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Fin del Metodo GuardarTextoPredeterminadoService", $"Error al obtener la empresaId");
                    return (new List<Lista_TextoPredeterminado>(), false);
                }
            }
            return (textoPredeterminadoDat.GetListaTextoPredeterminado(logTransaccionId, instancia.EmpresaId), true);
       }
    }
}
