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
        public (int, bool) X1(string A1, SaveEmpresaRequest A2)
        {
            Func<string, string> R = s => new string(s.Reverse().ToArray());
            string L1 = R(")(aesrepmEv aS :odotem oicinI");
            string L2 = R(")(aesrepmEv aS :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"Empresa: {JsonConvert.SerializeObject(A2.Empresa)}");

            try
            {
                using var A3 = new SqlConnection(cnx);
                A3.Open();

                using var A4 = A3.BeginTransaction();

                try
                {
                    var A5 = @"
                        INSERT INTO EMPRESA (Nombre, Srv, DB, Usuario, Pasword, CodigoCC, Codigousuario)
                        VALUES (@Nombre, @Srv, @DB, @Usuario, @Pasword, @CodigoCC, @Codigousuario);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                    int A6 = A3.ExecuteScalar<int>(A5, A2.Empresa, A4);

                    if (A6 <= 0)
                    {
                        LogHelper.GuardarLogTransaccion(A1, nombreArchivo, "Error", "No se pudo insertar la empresa.");
                        A4.Rollback();
                        return (0, false);
                    }

                    if (A2.Configuraciones != null && A2.Configuraciones.Any())
                    {
                        var A7 = @"
                            INSERT INTO EMPRESA_CONFIGURACION (EmpresaId, Clave, Valor)
                            VALUES (@EmpresaId, @Clave, @Valor);";

                        foreach (var A8 in A2.Configuraciones)
                        {
                            A8.EmpresaId = A6;
                            A3.Execute(A7, A8, A4);
                        }

                        LogHelper.GuardarLogTransaccion(A1, nombreArchivo, "Configuraciones guardadas", JsonConvert.SerializeObject(A2.Configuraciones));
                    }

                    if (A2.Usuario != null)
                    {
                        A2.Usuario.EmpresaId = A6;
                        var A9 = @"
                            INSERT INTO USUARIO (EmpresaId, InstanciaId, NroTelefono, CodigoUsuario)
                            VALUES (@EmpresaId, @InstanciaId, @NroTelefono, @CodigoUsuario);";
                        A3.Execute(A9, A2.Usuario, A4);
                    }

                    if (A2.EmpresaId > 0)
                    {
                        var A10 = $@"
                            UPDATE EMPRESA SET Estado = 0
                            WHERE EmpresaId = {A2.EmpresaId};";
                        A3.Execute(A10, null, A4);
                    }

                    A4.Commit();
                    LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, "Empresa guardada correctamente.");
                    return (A6, true);
                }
                catch (Exception A11)
                {
                    A4.Rollback();
                    LogHelper.GuardarLogTransaccion(A1, nombreArchivo, "Rollback X1()", $"Error interno: {A11.Message}");
                    return (0, false);
                }
            }
            catch (Exception A12)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, "Error de conexión X1()", $"Error externo: {A12.Message}");
                return (0, false);
            }
        }
        public (List<EmpresaDetalle> R1, bool R2) X2(string A1)
        {
            Func<string, string> R = s => new string(s.Reverse().ToArray());
            string L1 = R(")(saserpme t eG :odotem oicinI");
            string L2 = R(")(saserpme t eG :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"");

            try
            {
                using var A2 = new SqlConnection(cnx);
                A2.Open();

                var A3 = @"
                    SELECT 
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

                var A4 = A2.Query<EmpresaDetalle>(A3).ToList();

                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"Total empresas encontradas: {A4.Count}");
                return (A4, true);
            }
            catch (Exception A5)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, "Error X2()", $"{L3}{A5.Message}");
                return (null, false);
            }
        }
        public (int R1, bool R2) X3(string A1, int A2)
        {
            Func<string, string> R = s => new string(s.Reverse().ToArray());
            string L1 = R(")(aesrepmEv ete leD :odotem oicinI");
            string L2 = R(")(aesrepmEv ete leD :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"Empresa: {A2}");

            try
            {
                using var A3 = new SqlConnection(cnx);
                A3.Open();

                using var A4 = A3.BeginTransaction();

                try
                {
                    var A5 = $@"
                        UPDATE EMPRESA SET Estado = 0
                        WHERE EmpresaId = {A2};";

                    A3.Execute(A5, null, A4);

                    A4.Commit();

                    LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, "Empresa eliminada correctamente.");
                    return (0, true);
                }
                catch (Exception A6)
                {
                    A4.Rollback();
                    LogHelper.GuardarLogTransaccion(A1, nombreArchivo, "Rollback X3()", $"Error interno: {A6.Message}");
                    return (0, false);
                }
            }
            catch (Exception A7)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, "Error de conexión X3()", $"Error externo: {A7.Message}");
                return (0, false);
            }
        }
    }
}
