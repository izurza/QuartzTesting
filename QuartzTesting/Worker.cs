using Quartz;
using QuartzTesting.Jobs;
using System.Drawing.Text;

namespace QuartzTesting;

public class Worker : BackgroundService
{
    private readonly ISchedulerFactory _schedulerFactory;

    public Worker(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(stoppingToken);
        JobKey key1 = new JobKey($"new1-{Guid.NewGuid()}");
        JobKey key2 = new JobKey($"new2-{Guid.NewGuid()}");


        var createToDoJob = JobBuilder.Create<CreateToDoJob>()
            .WithIdentity(key1)
            .RequestRecovery()
            .Build();

        var importToDoJob = JobBuilder.Create<ImportToDoJob>()
            .WithIdentity(key2)
            .RequestRecovery()
            .Build();

        var createToDoTrigger = TriggerBuilder.Create()
            .WithIdentity($"CreateToDo-trigger-{Guid.NewGuid()}")
            .WithCronSchedule("0/30 * * ? * * *")
            .ForJob(key1)
            .Build();


        var importToDoTrigger = TriggerBuilder.Create()
            .WithIdentity($"ImportToDo-trigger-{Guid.NewGuid()}")
            .WithCronSchedule("0 0/1 * ? * * *")
            .ForJob(key2)
            .Build();


        await scheduler.ScheduleJob(createToDoJob, createToDoTrigger, stoppingToken);
        await scheduler.ScheduleJob(importToDoJob, importToDoTrigger, stoppingToken);

        while (!stoppingToken.IsCancellationRequested) await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

    }
}
