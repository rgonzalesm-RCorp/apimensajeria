using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json;

namespace ApiMensajeria
{
    public class ConfiguracionPorEmpresasData
    {
        string cnx;
        string nombreArchivo = "Pagos.Data.cs";
        public ConfiguracionPorEmpresasData()
        {
            cnx = AppSettings.GetSetting("ConnectionStrings:cnx");
        }
        public List<ConfiguracionPorEmpresa> GetConfiguracionEmpresaData(string logTransaccionId, int empresaId, out CounterEmpresa counter, bool isCount = false)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetConfiguracionEmpresaData()", $"empresaid :{empresaId}");
            List<ConfiguracionPorEmpresa> configuracionPorEmpresa = new();
            counter = new CounterEmpresa();
            try
            {
                string query = $@"SELECT * FROM EMPRESA_CONFIGURACION WHERE EmpresaId = {empresaId}";
                string queryCounter = $@"SELECT 
                                EmpreaId, SUM(counterMes)Mes, SUM(counteDia) dia
                                FROM (
                                    SELECT S.EmpreaId,
                                    1 counterMes,
                                    CASE WHEN DATEPART(DAY, D.FechaEnvio) = DATEPART(DAY, GETDATE()) THEN 1 ELSE 0 END counteDia
                                    FROM SMS S
                                    INNER JOIN DESTINATARIO D ON D.SmsId= S.SmsId
                                    WHERE D.StatusSms = 'OK' AND DATEPART(YEAR, D.FechaEnvio) = DATEPART(YEAR, GETDATE())
                                    AND DATEPART(MONTH, D.FechaEnvio) = DATEPART(MONTH, GETDATE()) AND S.EmpreaId = {empresaId}
                                ) DAT
                                GROUP BY EmpreaId";
                using (var context = new SqlConnection(cnx))
                {
                    configuracionPorEmpresa = context.Query<ConfiguracionPorEmpresa>(query).ToList() ?? new List<ConfiguracionPorEmpresa>();
                    if (isCount)
                    {
                        counter = context.Query<CounterEmpresa>(queryCounter).FirstOrDefault() ?? new CounterEmpresa();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetConfiguracionEmpresaData()", $"error: {ex.Message}");
                configuracionPorEmpresa = new();
            }

            return configuracionPorEmpresa;
        }
    }
}
