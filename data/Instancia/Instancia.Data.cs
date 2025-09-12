using System.Data.SqlClient;
using Dapper;

namespace ApiMensajeria
{
    public class InstanciaData
    {
        string cnx;
        string nombreArchivo = "Instancia.Data.cs";
        public InstanciaData()
        {
            cnx = AppSettings.GetSetting("ConnectionStrings:cnx");
        }
        public bool SaveUsuarioData(string logTransaccionId, int empresaId, int instanciaId, string nroTelefono, int codigoUsuario)
        {
            bool response = true;
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: SaveUsuarioData()", $"empresaId: {empresaId}, instanciaId:{instanciaId}");
            try
            {
                string query = $@"INSERT INTO USUARIO
                                            (EmpresaId
                                            ,InstanciaId
                                            ,NroTelefono
                                            ,Codigousuario)
                                        VALUES
                                            ({empresaId}, {instanciaId}, '{nroTelefono}', {codigoUsuario})";
                using (var context = new SqlConnection(cnx))
                {
                    context.Query(query);
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: SaveUsuarioData()", $"error: {ex.Message}");
                response = false;
            }
            return response;
        }
        public int SaveUsuarioLoginData(string logTransaccionId, int loginUserId, int codigoUsuario, int usuarioId, string codeQr, string statusLogin, string subStatus, string statusjson)
        {
            int response = 0;
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: SaveUsuarioLoginData()", $"usuarioId: {usuarioId}, codeQr:{codeQr}");
            try
            {
                string query = loginUserId > 0 ? $@"
                    UPDATE LOGIN_USER SET
                        [StatusLogin] = '{statusLogin}'
                        ,[SubStatus] = '{subStatus}'
                        ,[StatusJson] = '{statusjson}'
                    WHERE LoginUserid = '{loginUserId}';
                    select {loginUserId}
                "
                : $@" 
                    UPDATE LOGIN_USER SET Estado = 0 WHERE UsuarioId = {usuarioId} and Estado = 1 and ISNULL(StatusLogin, '') = '';
                    INSERT INTO [dbo].[LOGIN_USER]
                        ([UsuarioId]
                        ,[CodeQr]
                        ,[Codigousuario])
                    VALUES
                        ({usuarioId}
                        ,'{codeQr}'
                        ,{codigoUsuario});
                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                                            ";
                using var context = new SqlConnection(cnx);
                response = context.QuerySingle<int>(query);
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: SaveUsuarioLoginData()", $"error: {ex.Message}");
                response = 0;
            }
            return response;
        }
        public List<Instancia> GetInstanciaData(string logTransaccionId, int instanciaId)
        {
            List<Instancia> instancia = new();
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetInstanciaData()", $" instanciaId:{instanciaId}");
            try
            {
                string query = instanciaId > 0 ? $@"SELECT * FROM INSTANCIA WHERE InstanciaId = {instanciaId}" :
                                                    $@"SELECT * FROM INSTANCIA WHERE Estado = 1";
                using (var context = new SqlConnection(cnx))
                {
                    instancia = context.Query<Instancia>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetInstanciaData()", $"error: {ex.Message}");
                instancia = new();
            }
            return instancia;
        }
        public InstanciaxEmpresa GetInstanciaDataXEmpresaCC(string logTransaccionId, string codigoEmpresaCC)
        {
            InstanciaxEmpresa instancia = new();
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetInstanciaDataXEmpresaCC()", $" codigoEmpresaCC:{codigoEmpresaCC}");
            try
            {
                string query = $@"SELECT TOP 1 I.*, E.EmpresaId  FROM INSTANCIA I
                                INNER JOIN USUARIO U ON U.InstanciaId = I.InstanciaId
                                INNER JOIN EMPRESA E ON E.EmpresaId = U.EmpresaId
                                WHERE E.CodigoCC = '{codigoEmpresaCC}' AND U.Estado = 1 AND I.Estado = 1 AND E.Estado = 1";
                using (var context = new SqlConnection(cnx))
                {
                    instancia = context.QueryFirstOrDefault<InstanciaxEmpresa>(query) ?? new InstanciaxEmpresa(); ;
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetInstanciaDataXEmpresaCC()", $"error: {ex.Message}");
                instancia = new();
            }
            return instancia;
        }

        public UsuarioEmpresaInstancia GetUsuarioData(string logTransaccionId, int empresaId, int instanciaId)
        {
            UsuarioEmpresaInstancia Usuario = new();
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetUsuarioData()", $"empresaId: {empresaId}, instanciaId:{instanciaId}");
            try
            {
                string query = instanciaId > 0 ?
                                $@"SELECT U.* FROM USUARIO U  WHERE U.Estado = 1 AND U.EmpresaId = '{empresaId}' AND U.InstanciaId = {instanciaId}" :
                                $@"SELECT U.* FROM USUARIO U WHERE U.Estado = 1 AND U.EmpresaId = '{empresaId}'";
                using (var context = new SqlConnection(cnx))
                {
                    Usuario = context.QueryFirstOrDefault<UsuarioEmpresaInstancia>(query) ?? new UsuarioEmpresaInstancia();
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetUsuarioData()", $"error: {ex.Message}");
                Usuario = new();
            }
            return Usuario;
        }
        public Empresa GetEmpresaData(string logTransaccionId, string codigoEmpresaCC)
        {
            Empresa Empresa = new();
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetEmrpesaData()", $"codigoEmpresaCC: {codigoEmpresaCC}");
            try
            {
                string query = $@"SELECT E.* FROM EMPRESA E 
                                WHERE E.Estado = 1 AND E.CodigoCC = '{codigoEmpresaCC}'";
                using (var context = new SqlConnection(cnx))
                {
                    Empresa = context.QueryFirstOrDefault<Empresa>(query) ?? new Empresa();
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetUsuarioData()", $"error: {ex.Message}");
                Empresa = new();
            }
            return Empresa;
        }

        public LoginUser GetLoginUserData(string logTransaccionId, int usurioId)
        {
            LoginUser objLoginUser = new();
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetUsuarioData()", $"usurioId: {usurioId}");
            try
            {
                string query = $@"select top 1 * from LOGIN_USER where UsuarioId = {usurioId} and Estado = 1 Order by fecharegistro desc";
                using (var context = new SqlConnection(cnx))
                {
                    objLoginUser = context.QueryFirstOrDefault<LoginUser>(query) ?? new LoginUser();
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetUsuarioData()", $"error: {ex.Message}");
                objLoginUser = new();
            }
            return objLoginUser;
        }

        public bool SaveInstanciaData(string logTransaccionId, Instancia instancia)
        {
            bool response = true;
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: SaveInstanciaData()", $"instanciaId: {instancia.InstanciaId}");
            try
            {
                string query = "";
                if (instancia.InstanciaId <= 0)
                {

                    query = $@"INSERT INTO INSTANCIA
                                    (InstanceIdUltraMsg, Token, Descripcion, NumeroTelefono, MultiEmpresa, FechaRegistro, Estado, Codigousuario)
                                  VALUES
                                    ('{instancia.InstanceIdUltraMsg}', '{instancia.Token}', '{instancia.Descripcion}', '{instancia.NumeroTelefono}', 
                                     {(instancia.MultiEmpresa ? 1 : 0)}, GETDATE(), {(instancia.Estado ? 1 : 0)}, {instancia.Codigousuario})";
                }
                else
                {
                    query = $@"UPDATE INSTANCIA SET 
                                    InstanceIdUltraMsg = '{instancia.InstanceIdUltraMsg}',
                                    Token = '{instancia.Token}',
                                    Descripcion = '{instancia.Descripcion}',
                                    NumeroTelefono = '{instancia.NumeroTelefono}',
                                    MultiEmpresa = {(instancia.MultiEmpresa ? 1 : 0)},
                                    Codigousuario = {instancia.Codigousuario}
                                  WHERE InstanciaId = {instancia.InstanciaId}";
                }
                using (var context = new SqlConnection(cnx))
                {
                    context.Execute(query);
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: SaveInstanciaData()", $"error: {ex.Message}");
                response = false;
            }
            return response;
        }
        public bool DeleteInstanciaData(string logTransaccionId, int instanciaId)
        {
            bool response = true;
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: DeleteInstanciaData()", $"instanciaId: {instanciaId}");
            try
            {
                string query = $@"UPDATE INSTANCIA SET Estado = 0 WHERE InstanciaId = {instanciaId}";
                using (var context = new SqlConnection(cnx))
                {
                    context.Execute(query);
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: DeleteInstanciaData()", $"error: {ex.Message}");
                response = false;
            }
            return response;
        }
        public (string instanceU, string token) GetInstanciaTokenData(string logTransaccionId, string codigoEmpresaCC)
        {
            (string instanceU, string token) response = ("", "");
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetInstanciaTokenData()", $"empresaId: {codigoEmpresaCC}");
            try
            {
                string query = $@"SELECT i.InstanceIdUltraMsg, i.Token  FROM USUARIO U
                        INNER JOIN INSTANCIA I ON I.InstanciaId= U.InstanciaId
                        INNER JOIN EMPRESA E ON E.EmpresaId = U.EmpresaId
                        WHERE E.CodigoCC = '{codigoEmpresaCC}' AND U.Estado = 1 AND I.Estado = 1 AND E.Estado = 1";
                using (var context = new SqlConnection(cnx))
                {
                    response = context.QueryFirstOrDefault<(string InstanceIdUltraMsg, string Token)>(query);
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetInstanciaTokenData()", $"error: {ex.Message}");
                response = ("", "");
            }
            return response;
        }
    }
}