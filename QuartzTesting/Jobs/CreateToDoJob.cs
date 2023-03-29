using Microsoft.EntityFrameworkCore;
using Quartz;
using QuartzTesting.Context;
using QuartzTesting.Models;

namespace QuartzTesting.Jobs
{
    [DisallowConcurrentExecution]
    public class CreateToDoJob : IJob
    {
        private readonly ToDoContext _todoContext;
        public CreateToDoJob(ToDoContext toDoContext) { 
            _todoContext = toDoContext;
        }
        async Task IJob.Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string? jobString = dataMap.GetString("PassingData");

            Random random = new Random();
            Console.WriteLine("crear To do");
            List<string> args = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };
            await _todoContext.Todos.AddAsync(new Todo()
            {
                Name = args[random.Next(args.Count)]
            });
            await _todoContext.SaveChangesAsync();

        }


    }
}
