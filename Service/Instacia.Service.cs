using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ApiMensajeria
{
    public class InstaciaService
    {
        private const string NOMBREARCHIVO = "Instacia.Service.cs";
        public async Task<ResponseRegistroUsuario> RegistroUsuarioService(string logTransaccionId, int codigoUsuario, string codigoEmpresaCC, int instanciaId)
        {
            var instanciaData = new InstanciaData();
            var apiService = new ApiService();
            ResponseRegistroUsuario responseRegistrousuario = new ResponseRegistroUsuario
            {
                status = "999",
                message = "Hay errores",
                data = new ResponseQrCode()
            };

            LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Inicio del Metodo RegistroUsuarioService", $"codigoEmpresaCC: {codigoEmpresaCC}, instanciaId: {instanciaId}");

            // 1. Obtener empresa
            Empresa empresa = instanciaData.GetEmpresaData(logTransaccionId, codigoEmpresaCC);
            if (empresa == null)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Empresa no encontrada", codigoEmpresaCC);
                responseRegistrousuario.message = "Empresa no encontrada";

                return responseRegistrousuario;
            }

            // 2. Obtener usuario
            UsuarioEmpresaInstancia usuario = instanciaData.GetUsuarioData(logTransaccionId, empresa.EmpresaId, instanciaId);

            // 3. Si no existe el usuario, crear uno
            if (usuario == null || usuario.UsuarioId <= 0)
            {
                bool usuarioCreado = instanciaData.SaveUsuarioData(logTransaccionId, empresa.EmpresaId, instanciaId, "", codigoUsuario);
                if (!usuarioCreado)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error al crear usuario", $"EmpresaId: {empresa.EmpresaId}, InstanciaId: {instanciaId}");
                    responseRegistrousuario.message = "Error al crear usuario";

                    return responseRegistrousuario;
                }

                usuario = instanciaData.GetUsuarioData(logTransaccionId, empresa.EmpresaId, instanciaId);
                if (usuario == null || usuario.UsuarioId <= 0)
                {
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Usuario no encontrado después de crearlo", "");
                    responseRegistrousuario.message = "Usuario no encontrado después de crearlo";
                    return responseRegistrousuario;
                }
            }
            // 4. Obtener Instancia
            List<Instancia> instancia = instanciaData.GetInstanciaData(logTransaccionId, instanciaId);
            if (instancia.Count <= 0)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Instancia no encontrada", instanciaId.ToString());
                responseRegistrousuario.message = "Instancia no encontrada";
                return responseRegistrousuario;
            }

            // 5. Obtener código QR desde UltraMsg
            string? responseQr = await apiService.HttpUltraMsg("instance/qrCode", Method.Get, instancia[0].InstanceIdUltraMsg, instancia[0].Token, new List<ParamsBodyRequest>());
            if (string.IsNullOrWhiteSpace(responseQr))
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Respuesta vacía de UltraMsg", "");
                responseRegistrousuario.message = "Respuesta vacía de UltraMsg";
                return responseRegistrousuario;
            }

            ResponseQrCode? qrResponse;
            try
            {
                qrResponse = JsonConvert.DeserializeObject<ResponseQrCode>(responseQr);
                qrResponse.usuarioId = usuario.UsuarioId;
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error deserializando respuesta de UltraMsg", ex.ToString());
                responseRegistrousuario.message = "Error deserializando respuesta de UltraMsg";
                return responseRegistrousuario;
            }

            // 6. Guardar login del usuario
            int loginGuardado = instanciaData.SaveUsuarioLoginData(
                logTransaccionId,
                loginUserId: 0,
                codigoUsuario: codigoUsuario,
                usuarioId: usuario.UsuarioId,
                codeQr: qrResponse?.qrCode ?? string.Empty,
                statusLogin: "",
                subStatus: "",
                statusjson: ""
            );

            if (loginGuardado <= 0)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error al guardar login del usuario", "");
                responseRegistrousuario.message = "Error al guardar login del usuario";
                return responseRegistrousuario;
            }

            Console.WriteLine(responseQr);
            qrResponse.loginUserId = loginGuardado;
            responseRegistrousuario.status = qrResponse != null ? "000" : "999";
            responseRegistrousuario.message = qrResponse != null ? "No hay errores" : "Hay errores";
            responseRegistrousuario.data = qrResponse ?? new ResponseQrCode();

            return responseRegistrousuario;
        }
        public List<Instancia> GetInstanciaService(string logTransaccionId, int instanceId)
        {
            InstanciaData INSTANCIADATA = new InstanciaData();
            List<Instancia> Lista = [];
            try
            {
                Lista = INSTANCIADATA.GetInstanciaData(logTransaccionId, instanceId);
            }
            catch (Exception)
            {
                Lista = [];
            }
            return Lista;
        }
        public Empresa GetEmpresaService(string logTransaccionId, string codigoEmpresaCC)
        {
            InstanciaData INSTANCIADATA = new();
            _ = new Empresa();
            Empresa objEmpresa;
            try
            {
                objEmpresa = INSTANCIADATA.GetEmpresaData(logTransaccionId, codigoEmpresaCC);
            }
            catch (Exception)
            {
                objEmpresa = new();
            }
            return objEmpresa;
        }
        public UsuarioEmpresaInstancia GetUsuarioEmrpesaInstanciaService(string logTransaccionId, int empresaId)
        {
            InstanciaData INSTANCIADATA = new();
            _ = new UsuarioEmpresaInstancia();
            UsuarioEmpresaInstancia objUsuario;
            try
            {
                objUsuario = INSTANCIADATA.GetUsuarioData(logTransaccionId, empresaId, 0);
            }
            catch (Exception)
            {
                objUsuario = new();
            }
            return objUsuario;
        }
        public async Task<(ApiResponseStatusQr apiResponseStatusQr, string message, bool status)> GetStatusService(string logTransaccionId, int instanciaId, int usurioId)
        {
            InstanciaData INSTANCIADATA = new InstanciaData();
            ApiService APISERVICE = new ApiService();
            ApiResponseStatusQr responseStatus = new();

            List<Instancia> instancia = INSTANCIADATA.GetInstanciaData(logTransaccionId, instanciaId);
            if (instancia.Count <= 0)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Instancia no encontrada", instanciaId.ToString());
                //responseRegistrousuario.message = "Instancia no encontrada";
                return (responseStatus, "Instancia no encontrada", false);
            }
            LoginUser loginUser = INSTANCIADATA.GetLoginUserData(logTransaccionId, usurioId);
            if (loginUser.LoginUserId <= 0)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "LoginUser no encontrada", usurioId.ToString());
                //responseRegistrousuario.message = "Instancia no encontrada";
                return (responseStatus, "LoginUser no encontrada", false);
            }

            string? responseQr = await APISERVICE.HttpUltraMsg(
                "instance/status"
                , Method.Get
                , instancia[0].InstanceIdUltraMsg
                , instancia[0].Token
                , new List<ParamsBodyRequest>());


            if (string.IsNullOrWhiteSpace(responseQr))
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Respuesta vacía de UltraMsg", "");
                return (responseStatus, "Respuesta vacía de UltraMsg", false);
            }

            ApiResponseStatusQr? qrSatusResponse;
            try
            {
                qrSatusResponse = JsonConvert.DeserializeObject<ApiResponseStatusQr>(responseQr);
                if (qrSatusResponse?.status == null)
                {
                    ApiResponseStatusError errorResponse = new ApiResponseStatusError();
                    errorResponse = JsonConvert.DeserializeObject<ApiResponseStatusError>(responseQr);
                    LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error deserializando respuesta de UltraMsg", responseQr);
                    return (responseStatus, errorResponse?.error ?? "Error desconocido al procesar la respuesta de UltraMsg", false);
                }


 
                responseStatus = qrSatusResponse;
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, NOMBREARCHIVO, "Error deserializando respuesta de UltraMsg", ex.ToString());

                return (responseStatus, "Error desconocido al procesar la respuesta de UltraMsg", false);
            }

            if (qrSatusResponse.status.accountStatus.status != "authenticated")
            {
                return (responseStatus, "Usuario no autenticado", true);
            }


            int loginGuardado = INSTANCIADATA.SaveUsuarioLoginData(
                logTransaccionId,
                loginUserId: loginUser.LoginUserId,
                codigoUsuario: 0,
                usuarioId: 0,
                codeQr: "",
                statusLogin: qrSatusResponse.status.accountStatus.status,
                subStatus: qrSatusResponse.status.accountStatus.substatus,
                statusjson: JsonConvert.SerializeObject(qrSatusResponse)
            );

            return (responseStatus, "No hay errores", true);
        }
        public bool SaveInstanciaData(string logTransaccionId, Instancia instancia)
        {
            InstanciaData INSTANCIADATA = new();
            bool response = false;
            try
            {
                response = INSTANCIADATA.SaveInstanciaData(logTransaccionId, instancia);
            }
            catch (Exception)
            {
                response = false;
            }
            return response;
        }
        public bool DeleteInstanciaData(string logTransaccionId, int instanciaId)
        {
            InstanciaData INSTANCIADATA = new();
            bool response = false;
            try
            {
                response = INSTANCIADATA.DeleteInstanciaData(logTransaccionId, instanciaId);
            }
            catch (Exception)
            {
                response = false;
            }
            return response;
        }
    }
}
