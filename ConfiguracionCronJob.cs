using Quartz;

public static class ConfiguracionCronJob
{
    public static void RegistrarTrabajo<TJob>(
         IServiceCollectionQuartzConfigurator quartzConfigurator,
         string jobKey,
         string triggerKey,
         string cronExpression
     ) where TJob : IJob
    {
        var job = new JobKey(jobKey);
        quartzConfigurator.AddJob<TJob>(opts => opts.WithIdentity(job));
        quartzConfigurator.AddTrigger(opts => opts
            .ForJob(job)
            .WithIdentity(triggerKey)
            .WithCronSchedule(cronExpression));
    }
}
