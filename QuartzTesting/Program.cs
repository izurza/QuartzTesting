using QuartzTesting.Jobs;
using Quartz;
using QuartzTesting.Context;
using Microsoft.EntityFrameworkCore;
using QuartzTesting;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ToDoContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Principal")));
builder.Services.AddDbContext<ImportedToDoContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Secundario")));
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<QuartzOptions>(options =>
{
    options.SchedulerName = "My-scheluder";
    options.Scheduling.IgnoreDuplicates = true; // default: false
    options.Scheduling.OverWriteExistingData = true; // default: true
});

builder.Services.AddQuartz(options =>
{
    options.SchedulerId = "Scheluder-Core";
    options.UseMicrosoftDependencyInjectionJobFactory();
    options.UsePersistentStore(x =>
    {
        x.UseSqlServer(builder.Configuration.GetConnectionString("Principal"));
        x.UseJsonSerializer();
        x.PerformSchemaValidation = false;
    });    
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
