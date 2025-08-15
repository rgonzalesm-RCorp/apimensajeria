using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

public class SaveDocumentoFisico
{
    public static string GuardarArchivoBase64(int empresaId, string base64Archivo, string extension)
    {
        try
        {
            // Obtener ruta base del proyecto
            string rutaBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Archivos");

            // Formatear subcarpetas: EmpresaId / Fecha
            string fecha = DateTime.Now.ToString("yyyy-MM-dd");
            string rutaFinal = Path.Combine(rutaBase, empresaId.ToString(), fecha);

            // Crear la carpeta si no existe
            if (!Directory.Exists(rutaFinal))
            {
                Directory.CreateDirectory(rutaFinal);
            }

            // Generar nombre de archivo único
            string nombreArchivo = Guid.NewGuid().ToString() + "." + extension.TrimStart('.');

            // Ruta completa al archivo
            string rutaArchivo = Path.Combine(rutaFinal, nombreArchivo);

            // Convertir base64 a byte[]
            byte[] archivoBytes = Convert.FromBase64String(base64Archivo);

            // Guardar archivo físicamente
            File.WriteAllBytes(rutaArchivo, archivoBytes);

            // Retornar la ruta relativa o absoluta del archivo guardado
            return rutaArchivo;
        }
        catch (Exception ex)
        {
            // Log o manejo de error
            throw new Exception("Error al guardar el archivo: " + ex.Message);
        }
    }

    public async Task<(string data, bool success)> ConvertirArchivoABase64(string rutaArchivo)
    {
        if (!File.Exists(rutaArchivo))
            return ($"No existe el archivo {rutaArchivo}", false);

        byte[] bytesArchivo = File.ReadAllBytes(rutaArchivo);
        return (Convert.ToBase64String(bytesArchivo), true);
    }

}
