using Microsoft.EntityFrameworkCore;
using Quartz;
using QuartzTesting.Context;
using QuartzTesting.Models;

namespace QuartzTesting.Jobs
{
    public class ImportToDoJob : IJob
    {
        private readonly ToDoContext _todoContext;
        private readonly ImportedToDoContext _importedToDoContext;

        public ImportToDoJob(ToDoContext todoContext, ImportedToDoContext importedTodoContext)
        {
            _todoContext = todoContext;
            _importedToDoContext = importedTodoContext;
        }
        async Task IJob.Execute(IJobExecutionContext context)
        {
            Console.WriteLine("importar To do");
            var todosToImport = await _todoContext.Todos.AsNoTracking().Select(t => new TodoDto
            {
                Name = t.Name
            }).ToListAsync();
    
            var alreadyImported = await _importedToDoContext.ImportedTodos.AsNoTracking().Select(t=> new TodoDto
            {
                Name = t.Name
            }).ToListAsync();
        
            var union = todosToImport.ExceptBy(alreadyImported.Select(t=>t.Name),t=>t.Name);
            
            var notImported = union.Select(t => new ImportedTodo
            {
                Name = t.Name
            });
            await _importedToDoContext.AddRangeAsync(notImported);
            await _importedToDoContext.SaveChangesAsync();
        }
    }
}
