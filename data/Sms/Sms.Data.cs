using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace ApiMensajeria
{
    public class SmsData
    {
        string cnx;
        string nombreArchivo = "Sms.Data.cs";
        public SmsData()
        {
            cnx = AppSettings.GetSetting("ConnectionStrings:cnx");
        }
        public async Task<int> X1(string A1, SmsSave A2)
        {
            int A3 = 0;
            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, "INI X1", $" ");
            try
            {
                using (IDbConnection A4 = new SqlConnection(cnx))
                {
                    A4.Open();
                    using (var A5 = A4.BeginTransaction())
                    {
                        try
                        {
                            A3 = await A4.ExecuteScalarAsync<int>(
                                $@"INSERT INTO SMS (EmpreaId, InstanciaId, Mensaje, CodigoUsuario, Documento, TipoSmsId, Caption)
                                VALUES (@EmpreaId, @InstanciaId, @Mensaje, @CodigoUsuario, @Documento, @TipoSmsId, @Caption);
                                SELECT IDENT_CURRENT('SMS')", A2, A5);
                            A5.Commit();
                        }
                        catch (Exception)
                        {
                            A5.Rollback();
                            A3 = 0;
                        }
                    }
                }
            }
            catch (Exception A6)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, "FIN", $"ERR:{A6.Message}");
                A3 = 0;
            }
            return A3;
        }
        public async Task<int> X2(string A1, int A2, int A3, RequestChatSmsDestinatario A4)
        {
            int A5 = 0;
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDoiratanitseDevaS :odotem oicinI");
            string L2 = R(")(ataDoirausUevaS :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $" ");
            try
            {
                using (IDbConnection A6 = new SqlConnection(cnx))
                {
                    A6.Open();
                    using (var A7 = A6.BeginTransaction())
                    {
                        try
                        {
                            A5 = await A6.ExecuteScalarAsync<int>(
                                $@"insert into DESTINATARIO (SmsId, NroTelefono, CodigoUsuario, Nombre) values ({A2}, @NroTelefono, {A3}, @Nombre); SELECT IDENT_CURRENT('DESTINATARIO')",
                                A4, A7
                            );
                            A7.Commit();
                        }
                        catch (Exception)
                        {
                            A5 = 0;
                            A7.Rollback();
                        }
                    }
                }
            }
            catch (Exception A8)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A8.Message}");
                A5 = 0;
            }
            return A5;
        }
        public async Task<bool> X3(string A1, int A2, string A3, string A4, bool A5 = false)
        {
            bool A6 = false;
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDoiratanitseDetadpU :odotem oicinI");
            string L2 = R(")(ataDoirausUevaS :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $" ");
            try
            {
                using (IDbConnection A7 = new SqlConnection(cnx))
                {
                    A7.Open();
                    using (var A8 = A7.BeginTransaction())
                    {
                        try
                        {
                            await A7.ExecuteScalarAsync<int>(
                                $@" UPDATE DESTINATARIO 
                                    SET StatusSms = '{A3}', 
                                        JsonStatus = '{A4}', 
                                        FechaEnvio = GETDATE(), 
                                        CounterIntent = ISNULL(CounterIntent, 0)+1, 
                                        Cobrar = {(A5 ? 1 : 0)} 
                                    WHERE DestinatarioId = {A2}", 
                                null, A8
                            );
                            A8.Commit();
                            A6 = true;
                        }
                        catch (Exception A9)
                        {
                            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A9.Message}");
                            A8.Rollback();
                        }
                    }
                }
            }
            catch (Exception B1)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{B1.Message}");
            }
            return A6;
        }
        public List<TipoSms> X4(string A1, int A2)
        {
            List<TipoSms> A3 = [];
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataSmsopiTteG :odotem oicinI");
            string L2 = R(")(ataSmsopiTteG :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"tipoSmsId: {A2}");
            try
            {
                string A4 = (A2 <= 0)
                    ? $@"SELECT * FROM  TIPO_SMS WHERE Estado = 1"
                    : $@"SELECT * FROM  TIPO_SMS WHERE TipoSmsId = {A2}";

                using (var A5 = new SqlConnection(cnx))
                {
                    A3 = A5.Query<TipoSms>(A4).ToList();
                }
            }
            catch (Exception A6)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A6.Message}");
                A3 = [];
            }
            return A3;
        }
        public List<MensajeDto> X5(string A1)
        {
            List<MensajeDto> A2 = [];
            Func<string, string> R = s => new string(s.Reverse().ToArray());

            string L1 = R(")(ataDsajesneMteG :odotem oicinI");
            string L2 = R(")(ataDsajesneMteG :odotem niF");
            string L3 = R(" :rorre");

            LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L1, $"");
            try
            {
                string A3 = $@"WITH MensajesOrdenados AS (
                                        SELECT top 100 percent
                                            E.Nombre
                                            , E.CodigoCC
                                            , I.InstanceIdUltraMsg
                                            , I.Token
                                            , TP.Archivo
                                            , TP.Ruta
                                            , TP.Descripcion
                                            , S.Mensaje
                                            , S.Documento
                                            , S.Caption
                                            , D.DestinatarioId
                                            , D.NroTelefono
                                            , S.SmsId
                                            , S.InstanciaId
                                            , S.TipoSmsId
                                            , S.EmpreaId
                                            , ROW_NUMBER() OVER (PARTITION BY S.InstanciaId ORDER BY D.DestinatarioId ASC) AS  rn
                                        FROM SMS S
                                            INNER JOIN DESTINATARIO D ON D.SmsId = S.SmsId
                                            INNER JOIN TIPO_SMS TP ON TP.TipoSmsId = S.TipoSmsId
                                            INNER JOIN INSTANCIA I ON I.InstanciaId = S.InstanciaId
                                            INNER JOIN EMPRESA E ON E.EmpresaId = S.EmpreaId
                                        WHERE ISNULL(D.StatusSms, '') <> 'ok' AND ISNULL(D.CounterIntent,0) <= 3
                                    )
                                    SELECT *
                                    FROM MensajesOrdenados
                                    WHERE rn <= 1
                                    ORDER BY InstanciaId, rn;";
                using (var A4 = new SqlConnection(cnx))
                {
                    A2 = A4.Query<MensajeDto>(A3).ToList();
                }
            }
            catch (Exception A5)
            {
                LogHelper.GuardarLogTransaccion(A1, nombreArchivo, L2, $"{L3}{A5.Message}");
                A2 = [];
            }
            return A2;
        }
    }
}