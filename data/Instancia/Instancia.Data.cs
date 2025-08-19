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
        public bool X1(string A1, int A2, int A3, string A4, int A5)
        {
            bool A6 = true;
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDousuaveS :odotem oicinI"); // "Inicio metodo: SaveUsuarioData()"
            string L2 = R(")(ataDousuaveS :odotem niF");   // "Fin metodo: SaveUsuarioData()"
            string L3 = R(" :rorre");                      // "error: "

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"empresaId: {A2}, instanciaId:{A3}");
            try
            {
                string A7 = $@"INSERT INTO USUARIO
                                            (EmpresaId
                                            ,InstanciaId
                                            ,NroTelefono
                                            ,Codigousuario)
                                        VALUES
                                            ({A2}, {A3}, '{A4}', {A5})";
                using (var A8 = new SqlConnection(cnx))
                {
                    A8.Query(A7);
                }
            }
            catch (Exception A9)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A9.Message}");
                A6 = false;
            }
            return A6;
        }
        public int X2(string A1, int A2, int A3, int A4, string A5, string A6, string A7, string A8)
        {
            int A9 = 0;
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDgolnigUsuaveS :odotem oicinI"); // "Inicio metodo: SaveUsuarioLoginData()"
            string L2 = R(")(ataDgolnigUsuaveS :odotem niF");   // "Fin metodo: SaveUsuarioLoginData()"
            string L3 = R(" :rorre");                            // "error: "

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"usuarioId: {A4}, codeQr:{A5}");
            try
            {
                string A10 = A2 > 0
                    ? $@"
                        UPDATE LOGIN_USER SET
                            [StatusLogin] = '{A6}'
                            ,[SubStatus] = '{A7}'
                            ,[StatusJson] = '{A8}'
                        WHERE LoginUserid = '{A2}';
                        select {A2}
                    "
                    : $@"
                        UPDATE LOGIN_USER SET Estado = 0 WHERE UsuarioId = {A4} and Estado = 1 and ISNULL(StatusLogin, '') = '';
                        INSERT INTO [dbo].[LOGIN_USER]
                            ([UsuarioId]
                            ,[CodeQr]
                            ,[Codigousuario])
                        VALUES
                            ({A4}
                            ,'{A5}'
                            ,{A3});
                        SELECT CAST(SCOPE_IDENTITY() AS INT);
                    ";
                using var A11 = new SqlConnection(cnx);
                A9 = A11.QuerySingle<int>(A10);
            }
            catch (Exception A12)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A12.Message}");
                A9 = 0;
            }
            return A9;
        }
        public List<Instancia> X3(string A1, int A2)
        {
            List<Instancia> A3 = new();
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDcainatsnIt eG :odotem oicinI");
            string L2 = R(")(ataDcainatsnIt eG :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $" instanciaId:{A2}");
            try
            {
                string A4 = A2 > 0
                    ? $@"SELECT * FROM INSTANCIA WHERE InstanciaId = {A2}"
                    : $@"SELECT * FROM INSTANCIA WHERE Estado = 1";

                using (var A5 = new SqlConnection(cnx))
                {
                    A3 = A5.Query<Instancia>(A4).ToList();
                }
            }
            catch (Exception A6)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A6.Message}");
                A3 = new();
            }
            return A3;
        }
        public InstanciaxEmpresa X4(string A1, string A2)
        {
            InstanciaxEmpresa A3 = new();
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(CCasapmExAtadAcnatsnIt eG :odotem oicinI");
            string L2 = R(")(CCasapmExAtadAcnatsnIt eG :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $" codigoEmpresaCC:{A2}");
            try
            {
                string A4 = $@"SELECT TOP 1 I.*, E.EmpresaId  FROM INSTANCIA I
                                INNER JOIN USUARIO U ON U.InstanciaId = I.InstanciaId
                                INNER JOIN EMPRESA E ON E.EmpresaId = U.EmpresaId
                                WHERE E.CodigoCC = '{A2}' AND U.Estado = 1 AND I.Estado = 1 AND E.Estado = 1";

                using (var A5 = new SqlConnection(cnx))
                {
                    A3 = A5.QueryFirstOrDefault<InstanciaxEmpresa>(A4) ?? new InstanciaxEmpresa();
                }
            }
            catch (Exception A6)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A6.Message}");
                A3 = new();
            }
            return A3;
        }
        public UsuarioEmpresaInstancia X5(string A1, int A2, int A3)
        {
            UsuarioEmpresaInstancia A4 = new();
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDosuav eG :odotem oicinI");
            string L2 = R(")(ataDosuav e G :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"empresaId: {A2}, instanciaId:{A3}");
            try
            {
                string A5 = A3 > 0
                    ? $@"SELECT U.* FROM USUARIO U  WHERE U.Estado = 1 AND U.EmpresaId = '{A2}' AND U.InstanciaId = {A3}"
                    : $@"SELECT U.* FROM USUARIO U WHERE U.Estado = 1 AND U.EmpresaId = '{A2}'";

                using (var A6 = new SqlConnection(cnx))
                {
                    A4 = A6.QueryFirstOrDefault<UsuarioEmpresaInstancia>(A5) ?? new UsuarioEmpresaInstancia();
                }
            }
            catch (Exception A7)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A7.Message}");
                A4 = new();
            }
            return A4;
        }
        public Empresa X6(string A1, string A2)
        {
            Empresa A3 = new();
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDsarep mEteG :odotem oicinI");
            string L2 = R(")(ataDosuav e G :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"codigoEmpresaCC: {A2}");
            try
            {
                string A4 = $@"SELECT E.* FROM EMPRESA E 
                                WHERE E.Estado = 1 AND E.CodigoCC = '{A2}'";

                using (var A5 = new SqlConnection(cnx))
                {
                    A3 = A5.QueryFirstOrDefault<Empresa>(A4) ?? new Empresa();
                }
            }
            catch (Exception A6)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A6.Message}");
                A3 = new();
            }
            return A3;
        }
        public LoginUser X7(string A1, int A2)
        {
            LoginUser A3 = new();
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDosuav e G :odotem oicinI");
            string L2 = R(")(ataDosuav e G :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"usurioId: {A2}");
            try
            {
                string A4 = $@"select top 1 * from LOGIN_USER where UsuarioId = {A2} and Estado = 1 Order by fecharegistro desc";

                using (var A5 = new SqlConnection(cnx))
                {
                    A3 = A5.QueryFirstOrDefault<LoginUser>(A4) ?? new LoginUser();
                }
            }
            catch (Exception A6)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A6.Message}");
                A3 = new();
            }
            return A3;
        }
        public bool X8(string A1, Instancia A2)
        {
            bool A3 = true;
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDcainatsnIevaS :odotem oicinI");
            string L2 = R(")(ataDcainatsnIevaS :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"instanciaId: {A2.InstanciaId}");
            try
            {
                string A4 = "";

                if (A2.InstanciaId <= 0)
                {
                    A4 = $@"INSERT INTO INSTANCIA
                                (InstanceIdUltraMsg, Token, Descripcion, NumeroTelefono, MultiEmpresa, FechaRegistro, Estado, Codigousuario)
                            VALUES
                                ('{A2.InstanceIdUltraMsg}', '{A2.Token}', '{A2.Descripcion}', '{A2.NumeroTelefono}', 
                                {(A2.MultiEmpresa ? 1 : 0)}, GETDATE(), {(A2.Estado ? 1 : 0)}, {A2.Codigousuario})";
                }
                else
                {
                    A4 = $@"UPDATE INSTANCIA SET 
                                InstanceIdUltraMsg = '{A2.InstanceIdUltraMsg}',
                                Token = '{A2.Token}',
                                Descripcion = '{A2.Descripcion}',
                                NumeroTelefono = '{A2.NumeroTelefono}',
                                MultiEmpresa = {(A2.MultiEmpresa ? 1 : 0)},
                                Codigousuario = {A2.Codigousuario}
                            WHERE InstanciaId = {A2.InstanciaId}";
                }

                using (var A5 = new SqlConnection(cnx))
                {
                    A5.Execute(A4);
                }
            }
            catch (Exception A6)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A6.Message}");
                A3 = false;
            }

            return A3;
        }
        public bool X9(string A1, int A2)
        {
            bool A3 = true;
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDcainatsnIete leD :odotem oicinI");
            string L2 = R(")(ataDcainatsnIete leD :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"instanciaId: {A2}");
            try
            {
                string A4 = $@"UPDATE INSTANCIA SET Estado = 0 WHERE InstanciaId = {A2}";

                using (var A5 = new SqlConnection(cnx))
                {
                    A5.Execute(A4);
                }
            }
            catch (Exception A6)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A6.Message}");
                A3 = false;
            }

            return A3;
        }
    }
}