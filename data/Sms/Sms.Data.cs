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
        public async Task<int> SaveSmsData(string logTransaccionId, SmsSave objSms)
        {
            int smsId = 0;
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: SaveSmsData()", $" ");
            try
            {
                using (IDbConnection db = new SqlConnection(cnx))
                {
                    db.Open();
                    using (var transaccion = db.BeginTransaction())
                    {
                        try
                        {
                            smsId = await db.ExecuteScalarAsync<int>($@"INSERT INTO SMS (EmpreaId, InstanciaId, Mensaje, CodigoUsuario, Documento, TipoSmsId, Caption)VALUES (@EmpreaId, @InstanciaId, @Mensaje, @CodigoUsuario, @Documento, @TipoSmsId, @Caption); SELECT IDENT_CURRENT('SMS')", objSms, transaccion);
                            transaccion.Commit();
                        }
                        catch (Exception)
                        {
                            transaccion.Rollback();
                            smsId = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: SaveUsuarioData()", $"error: {ex.Message}");

                smsId = 0;
            }
            return smsId;
        }
        public async Task<int> SaveDestinatarioData(string logTransaccionId, int smsId, int CodigoUsuario, RequestChatSmsDestinatario listaNumero)
        {
            int destinatarioId = 0;
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: SaveDestinatarioData()", $" ");
            try
            {
                using (IDbConnection db = new SqlConnection(cnx))
                {
                    db.Open();
                    using (var transaccion = db.BeginTransaction())
                    {
                        try
                        {
                            destinatarioId = await db.ExecuteScalarAsync<int>($@"insert into DESTINATARIO (SmsId, NroTelefono, CodigoUsuario, Nombre) values ({smsId}, @NroTelefono, {CodigoUsuario}, @Nombre); SELECT IDENT_CURRENT('DESTINATARIO')", listaNumero, transaccion);
                            transaccion.Commit();
                        }
                        catch (Exception)
                        {
                            destinatarioId = 0;
                            transaccion.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: SaveUsuarioData()", $"error: {ex.Message}");
                destinatarioId = 0;
            }
            return destinatarioId;
        }
        public async Task<bool> UpdateDestinatarioData(string logTransaccionId, int destinatarioId, string statusSms, string statusJson, bool cobrar = false)
        {
            bool response = false;
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: UpdateDestinatarioData()", $" ");
            try
            {
                using (IDbConnection db = new SqlConnection(cnx))
                {
                    db.Open();
                    using (var transaccion = db.BeginTransaction())
                    {
                        try
                        {
                            await db.ExecuteScalarAsync<int>($@" UPDATE DESTINATARIO SET StatusSms = '{statusSms}', JsonStatus = '{statusJson}', FechaEnvio = GETDATE(), CounterIntent = ISNULL(CounterIntent, 0)+1, 
                            Cobrar = {(cobrar ? 1 : 0)} WHERE DestinatarioId = {destinatarioId}", null, transaccion);
                            transaccion.Commit();
                            response = true;
                        }
                        catch (Exception ex )
                        {
                            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: SaveUsuarioData()", $"error: {ex.Message}");

                            transaccion.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: SaveUsuarioData()", $"error: {ex.Message}");
            }
            return response;
        }

        public List<TipoSms> GetTipoSmsData(string logTransaccionId, int tipoSmsId)
        {
            List<TipoSms> objTipoSms = [];
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetTipoSmsData()", $"tipoSmsId: {tipoSmsId}");
            try
            {
                string query = (tipoSmsId <= 0) ? $@"SELECT * FROM  TIPO_SMS WHERE Estado = 1" : $@"SELECT * FROM  TIPO_SMS WHERE TipoSmsId = {tipoSmsId}";
                using (var context = new SqlConnection(cnx))
                {
                    objTipoSms = context.Query<TipoSms>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetTipoSmsData()", $"error: {ex.Message}");
                objTipoSms = [];
            }
            return objTipoSms;
        }

        public List<MensajeDto> GetMensajesData(string logTransaccionId)
        {
            List<MensajeDto> mensajes = [];
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetMensajesData()", $"");
            try
            {
                string query = $@"WITH MensajesOrdenados AS (
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
                using (var context = new SqlConnection(cnx))
                {
                    mensajes = context.Query<MensajeDto>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetMensajesData()", $"error: {ex.Message}");
                mensajes = [];
            }
            return mensajes;
        }
    }
}