using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json;

namespace ApiMensajeria
{
    public class AdminData
    {
        string cnx;
        string nombreArchivo = "Admin.Data.cs";
        public AdminData()
        {
            cnx = AppSettings.GetSetting("ConnectionStrings:cnx");
        }
        public List<ListaMensajes> GetListaMensajes(string logTransaccionId, string CodigoCC, int tipoMensajeId, string fechaInicio, string fechaFin)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetListaMensajes()", $"CodigoCC :{CodigoCC}");
            List<ListaMensajes> listaMensajes = new();
            try
            {
                string query = $@"SELECT 
                                        I.Descripcion + ' (' + I.NumeroTelefono + ')' Instancia
                                        , TS.Descripcion DescripcionTipo
                                        , S.Mensaje MensajeTexto
                                        , S.Caption
                                        , S.Documento
                                        , S.FechaCreacion
                                        , COUNT(*) Total
                                        , SUM(CASE WHEN D.StatusSms = 'ok' THEN 1 ELSE 0 END) Enviado
                                        , SUM(CASE WHEN D.StatusSms <> 'ok' THEN 1 ELSE 0 END) NoEnviado
                                        , I.InstanciaId
                                        , S.EmpreaId
                                        , S.SmsId
                                        , TS.TipoSmsId
                                    FROM SMS S
                                    INNER JOIN INSTANCIA I ON I.InstanciaId = S.InstanciaId
                                    INNER JOIN TIPO_SMS TS ON TS.TipoSmsId = S.TipoSmsId
                                    INNER JOIN DESTINATARIO D ON D.SmsId = S.SmsId
                                    INNER JOIN EMPRESA E ON E.EmpresaId = S.EmpreaId
                                    WHERE E.CodigoCC = '{CodigoCC}'
                                    AND TS.TipoSmsId = {tipoMensajeId}
                                    AND S.FechaCreacion BETWEEN '{fechaInicio}' AND '{fechaFin}'
                                    GROUP BY
                                        I.Descripcion
                                        , TS.Descripcion
                                        , S.Mensaje
                                        , S.Caption
                                        , S.Documento
                                        , I.InstanciaId
                                        , S.EmpreaId
                                        , S.SmsId
                                        , TS.TipoSmsId
                                        , S.FechaCreacion
                                        , I.NumeroTelefono";
                using var context = new SqlConnection(cnx);
                listaMensajes = context.Query<ListaMensajes>(query).ToList() ?? new List<ListaMensajes>();
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetListaMensajes()", $"error: {ex.Message}");
                listaMensajes = new();
            }
            return listaMensajes;
        }
        public List<ListaDestinatario> GetDetalleMensaje(string logTransaccionId, int smsId)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetDetalleMensaje()", $"SmsId :{smsId}");
            List<ListaDestinatario> listaDestinatarios = new();
            try
            {
                string query = $@"SELECT 
                                    DestinatarioId
                                    ,SmsId
                                    ,NroTelefono
                                    ,ISNULL(Nombre, '')Nombre
                                    ,ISNULL(StatusSms, '')StatusSms
                                    ,ISNULL(JsonStatus, '')JsonStatus
                                    ,FechaEnvio
                                    ,FechaCreacion
                                    ,Estado
                                    ,CodigoUsuario
                                    ,CounterIntent
                                    ,ISNULL(Cobrar, 0) Cobrar
                                FROM DESTINATARIO WHERE SmsId = {smsId}";
                using var context = new SqlConnection(cnx);
                listaDestinatarios = context.Query<ListaDestinatario>(query).ToList() ?? new List<ListaDestinatario>();
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetDetalleMensaje()", $"error: {ex.Message}");
                listaDestinatarios = new();
            }
            return listaDestinatarios;
        }
        public async Task<SmsDetails?> GetMensajeData(string logTransaccionId, int smsId)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetMensaje()", $"SmsId :{smsId}");
            SmsDetails? sms = null;
            try
            {
                string query = $@"SELECT * FROM SMS WHERE SmsId = {smsId}";
                using var context = new SqlConnection(cnx);
                sms = await context.QuerySingleOrDefaultAsync<SmsDetails>(query);
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetMensaje()", $"error: {ex.Message}");
                sms = null;
            }
            return sms;
        }
    }
}
