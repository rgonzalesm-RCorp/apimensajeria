using ApiMensajeria;
using Quartz;
using System;
using System.Threading.Tasks;

public class WorkerSendSms : IJob
{
    SendMensajesCronJob SendMensajesCronJob = new SendMensajesCronJob();
    public Task Execute(IJobExecutionContext context)
    {
        long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        SendMensajesCronJob.SendMsgCronJob(logTransaccionId.ToString());

        return Task.CompletedTask;
    }
}
