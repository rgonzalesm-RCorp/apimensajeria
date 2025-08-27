using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json;

namespace ApiMensajeria
{
    public class TextoPredeterminadoData
    {
        string cnx;
        string nombreArchivo = "TextoPredeterminado.Data.cs";
        public TextoPredeterminadoData()
        {
            cnx = AppSettings.GetSetting("ConnectionStrings:cnx");
        }
        public bool GuardarTextPredeterminadoData(string logTransaccionId, Request_TextoPredeterminado_Save request_TextoPredeterminado, int? empresaId)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GuardarTextPredeterminadoData()", $"CodigoCC :{JsonConvert.SerializeObject(request_TextoPredeterminado)}");
            bool resultado = false;
            try
            {
                string query = $@"INSERT INTO TEXTO_PREDETERMINADO
                            (EmpresaId, Texto, CodigoUsuario)
                            VALUES
                            ({empresaId}, '{request_TextoPredeterminado.Texto}', {request_TextoPredeterminado.CodigoUsuario})";

                using var context = new SqlConnection(cnx);
                context.Execute(query);
                resultado = true;
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GuardarTextPredeterminadoData()", $"error: {ex.Message}");
                resultado = false;
            }
            return resultado;
        }
        public bool UpdateTextPredeterminadoData(string logTransaccionId, Request_TextoPredeterminado_Update request_TextoPredeterminado)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: UpdateTextPredeterminadoData()", $"CodigoCC :{JsonConvert.SerializeObject(request_TextoPredeterminado)}");
            bool resultado = false;
            try
            {
                string query = $@"UPDATE TEXTO_PREDETERMINADO
                            SET Texto = '{request_TextoPredeterminado.Texto}', CodigoUsuario = {request_TextoPredeterminado.CodigoUsuario}
                            WHERE TextoPredeterminadoId = {request_TextoPredeterminado.TextoPredeterminadoId}";

                using var context = new SqlConnection(cnx);
                context.Execute(query);
                resultado = true;
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GuardarTextPredeterminadoData()", $"error: {ex.Message}");
                resultado = false;
            }
            return resultado;
        }

        public List<Lista_TextoPredeterminado> GetListaTextoPredeterminado(string logTransaccionId, int empresaId)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: GetListaTextoPredeterminado()", $"empresaId :{empresaId}");
            List<Lista_TextoPredeterminado> listaTextoPredeterminado = new List<Lista_TextoPredeterminado>();
            try
            {
                string query = $@"SELECT TextoPredeterminadoId, EmpresaId, Texto, FechaCreacion, Estado, CodigoUsuario
                                  FROM TEXTO_PREDETERMINADO
                                  WHERE Estado = 1 and EmpresaId = {empresaId}";

                using var context = new SqlConnection(cnx);
                listaTextoPredeterminado = context.Query<Lista_TextoPredeterminado>(query).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: GetListaTextoPredeterminado()", $"error: {ex.Message}");
                listaTextoPredeterminado = new List<Lista_TextoPredeterminado>();
            }
            return listaTextoPredeterminado;
        }
        public bool DeleteTextoPredeterminado(string logTransaccionId, int textoPredeterminadoId)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Inicio metodo: DeleteTextoPredeterminado()", $"textoPredeterminadoId :{textoPredeterminadoId}");
            bool resultado = false;
            try
            {
                string query = $@"UPDATE TEXTO_PREDETERMINADO
                                  SET Estado = 0
                                  WHERE TextoPredeterminadoId = {textoPredeterminadoId}";

                using var context = new SqlConnection(cnx);
                context.Execute(query);
                resultado = true;
            }
            catch (Exception ex)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId, nombreArchivo, "Fin metodo: DeleteTextoPredeterminado()", $"error: {ex.Message}");
                resultado = false;
            }
            return resultado;
        }
    }
}
