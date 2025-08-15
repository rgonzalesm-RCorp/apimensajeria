using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json;

namespace ApiMensajeria
{
    public class EmpresaData
    {
        string cnx;
        string nombreArchivo = "Empresa.Data.cs";
        public EmpresaData()
        {
            cnx = AppSettings.GetSetting("ConnectionStrings:cnx");
        }
        public (int empresaId, bool success) SaveEmpresa(string logTransaccionId, SaveEmpresaRequest request)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: SaveEmpresa()", $"Empresa: {JsonConvert.SerializeObject(request.Empresa)}");

            try
            {
                using var connection = new SqlConnection(cnx);
                connection.Open();

                using var transaction = connection.BeginTransaction();

                try
                {
                    var insertEmpresaQuery = @"
                        INSERT INTO EMPRESA (Nombre, Srv, DB, Usuario, Pasword, CodigoCC, Codigousuario)
                        VALUES (@Nombre, @Srv, @DB, @Usuario, @Pasword, @CodigoCC, @Codigousuario);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                    int empresaId = connection.ExecuteScalar<int>(insertEmpresaQuery, request.Empresa, transaction);

                    if (empresaId <= 0)
                    {
                        LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Error", "No se pudo insertar la empresa.");
                        transaction.Rollback();
                        return (0, false);
                    }

                    if (request.Configuraciones != null && request.Configuraciones.Any())
                    {
                        var insertConfigQuery = @"
                            INSERT INTO EMPRESA_CONFIGURACION (EmpresaId, Clave, Valor)
                            VALUES (@EmpresaId, @Clave, @Valor);";

                        foreach (var config in request.Configuraciones)
                        {
                            config.EmpresaId = empresaId;
                            connection.Execute(insertConfigQuery, config, transaction);
                        }

                        LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Configuraciones guardadas", JsonConvert.SerializeObject(request.Configuraciones));
                    }

                    if (request.Usuario != null)
                    {
                        request.Usuario.EmpresaId = empresaId;

                        var insertUsuarioQuery = @"
                            INSERT INTO USUARIO (EmpresaId, InstanciaId, NroTelefono, CodigoUsuario)
                            VALUES (@EmpresaId, @InstanciaId, @NroTelefono, @CodigoUsuario);";

                        connection.Execute(insertUsuarioQuery, request.Usuario, transaction);
                    }
                    if (request.EmpresaId > 0)
                    {
                        var updateEmpresaQuery = $@"
                            UPDATE EMPRESA SET Estado = 0
                            WHERE EmpresaId = {request.EmpresaId};";
                        connection.Execute(updateEmpresaQuery, null, transaction);
                    }


                    transaction.Commit();

                    LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: SaveEmpresa()", "Empresa guardada correctamente.");
                    return (empresaId, true);
                }
                catch (Exception exInner)
                {
                    transaction.Rollback();
                    LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Rollback SaveEmpresa()", $"Error interno: {exInner.Message}");
                    return (0, false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Error de conexión SaveEmpresa()", $"Error externo: {ex.Message}");
                return (0, false);
            }
        }

        public (List<EmpresaDetalle> listado, bool success) GetEmpresas(string logTransaccionId)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetEmpresas()", $"");

            try
            {
                using var connection = new SqlConnection(cnx);
                connection.Open();

                var query = @"SELECT 
                            E.EmpresaId EmpresaId_
                            , E.Nombre
                            , E.CodigoCC
                            , E.FechaRegistro
                            , EC.*
                            , I.InstanciaId
                            , I.InstanceIdUltraMsg
                            , I.Token
                            , I.Descripcion
                            , I.MultiEmpresa
                            , I.NumeroTelefono
                            FROM EMPRESA E
                            INNER JOIN ConfiguracionEmpresa EC ON EC.EmpresaId = E.EmpresaId
                            INNER JOIN USUARIO U ON U.EmpresaId = E.EmpresaId
                            INNER JOIN INSTANCIA I ON I.InstanciaId = U.InstanciaId
                            WHERE E.Estado = 1
                            ORDER BY E.Nombre";

                var empresas = connection.Query<EmpresaDetalle>(query).ToList();

                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetEmpresas()", $"Total empresas encontradas: {empresas.Count}");
                return (empresas, true);
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Error GetEmpresas()", $"Error: {ex.Message}");
                return (null, false);
            }
        }
        public (int empresaId, bool success) DeleteEmpresa(string logTransaccionId, int empresaId)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: DeleteEmpresa()", $"Empresa: {empresaId}");

            try
            {
                using var connection = new SqlConnection(cnx);
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var updateEmpresaQuery = $@"
                        UPDATE EMPRESA SET Estado = 0
                        WHERE EmpresaId = {empresaId};";
                    connection.Execute(updateEmpresaQuery,null, transaction);

                    transaction.Commit();

                    LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: DeleteEmpresa()", "Empresa eliminada correctamente.");
                    return (0, true);
                }
                catch (Exception exInner)
                {
                    transaction.Rollback();
                    LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Rollback DeleteEmpresa()", $"Error interno: {exInner.Message}");
                    return (0, false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Error de conexión DeleteEmpresa()", $"Error externo: {ex.Message}");
                return (0, false);
            }
        }

    }
}
