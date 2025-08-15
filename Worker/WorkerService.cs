using ApiMensajeria;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class WorkerService : BackgroundService
{
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1); // 🔒 Evita solapamientos
    SendMensajesCronJob SendMensajesCronJob = new SendMensajesCronJob();
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Servicio en segundo plano iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (await _lock.WaitAsync(0)) // Solo entra si no está ocupado
            {
                try
                {
                    Console.WriteLine($"Tarea iniciada: {DateTime.Now}");
                    long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    await SendMensajesCronJob.SendMsgCronJob(logTransaccionId.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    _lock.Release(); // Libera el semáforo
                }
            }
            else
            {
                Console.WriteLine("Tarea ya en ejecución, esperando...");
            }

            await Task.Delay(15000, stoppingToken); 
        }

        Console.WriteLine("Servicio en segundo plano detenido.");
    }
}
