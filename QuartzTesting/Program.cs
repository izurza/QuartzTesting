using QuartzTesting.Jobs;
using Quartz;
using QuartzTesting.Context;
using Microsoft.EntityFrameworkCore;
using QuartzTesting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ToDoContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Principal")));
builder.Services.AddDbContext<ImportedToDoContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Secundario")));
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var jobKey1 = new JobKey("CreateToDoJob");
    var jobKey2 = new JobKey("ImportToDoJob");
    q.AddJob<CreateToDoJob>(opts => {
        opts.WithIdentity(jobKey1);
        opts.UsingJobData("Passing data", "data passed");
        opts.UsingJobData("Passing data2", 1);
        opts.UsingJobData("Passing data3", true);
        
        });
    q.AddJob<ImportToDoJob>(opts => opts.WithIdentity(jobKey2));

    q.AddTrigger(opts =>
    opts.ForJob(jobKey1)
    .WithIdentity("CreateToDoJob-trigger")
    .WithCronSchedule("0/30 * * ? * * *"));
    q.AddTrigger(opts =>
    opts.ForJob(jobKey2)
    .WithIdentity("ImportToDoJob-trigger")
    .WithCronSchedule("0 0/1 * ? * * *"));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
