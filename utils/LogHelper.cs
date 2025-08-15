using System;
using System.IO;

public class LogHelper
{
    private static readonly string logDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

    public static void GuardarLogTransaccion(string idTransaccion, string archivo, string descripcion, string detalles)
    {
        try
        {
            if (!Directory.Exists(logDirectoryPath))
            {
                Directory.CreateDirectory(logDirectoryPath);
            }

            string fileName = $"{DateTime.Now.ToString("dd-MM-yyyy")}.log";
            string logFilePath = Path.Combine(logDirectoryPath, fileName);
            string DetalleAux = detalles.Length > 0 ? $" Detalles: {detalles}" : "";
            string logMessage = $"{DateTime.Now}: Transacción ID: {idTransaccion}, Archivo: {archivo}, Info: {descripcion} {DetalleAux}";
            Console.WriteLine(logMessage);

            using (StreamWriter writer = new StreamWriter(logFilePath, true)) // true para añadir al final del archivo
            {
                writer.WriteLine(logMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al escribir el log: {ex.Message}");
        }
    }
}
