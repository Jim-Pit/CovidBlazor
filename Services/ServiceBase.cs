using DbDesign;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoViDAccountant.Services
{
    public abstract class ServiceBase
    {
        protected readonly IServiceScopeFactory _serviceScopeFactory;

        protected ServiceBase(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected async Task UseDbContext(Func<CoViDAccountantDbContext, Task> work)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                await work(scope.ServiceProvider.GetService<CoViDAccountantDbContext>());
            }
        }
        protected async Task<T> UseDbContext<T>(Func<CoViDAccountantDbContext, Task<T>> work)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                return await work(scope.ServiceProvider.GetService<CoViDAccountantDbContext>());
            }
        }
        public async Task DoWork(Func<CoViDAccountantDbContext, Task> work)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                await work(scope.ServiceProvider.GetService<CoViDAccountantDbContext>());
            }
        }

        public async Task<T> DoWork<T>(Func<CoViDAccountantDbContext, Task<T>> work)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                return await work(scope.ServiceProvider.GetService<CoViDAccountantDbContext>());
            }
        }

        protected async Task WorkTasks(IEnumerable<Func<CoViDAccountantDbContext, Task>> tasks)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<CoViDAccountantDbContext>();
                // InvalidOperationException: A second operation started on this context before a previous operation completed.
                // This is usually caused by different threads using the same instance of DbContext
                // await Task.WhenAll(tasks.Select(t => t(dbContext)));
                foreach (var task in tasks)
                {
                    await task(dbContext);
                }
            }
        }

        public void TestSyncExecution(Action<CoViDAccountantDbContext> work)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                work(scope.ServiceProvider.GetService<CoViDAccountantDbContext>());
            }
        }

        protected void UseDbContext(Action<CoViDAccountantDbContext> work)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                work(scope.ServiceProvider.GetService<CoViDAccountantDbContext>());
            }
        }
        
        protected T UseDbContext<T>(Func<CoViDAccountantDbContext, T> work)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                return work(scope.ServiceProvider.GetService<CoViDAccountantDbContext>());
            }
        }

        private TService Get<TService>(IServiceScope @this)
        {
            return @this.ServiceProvider.GetService<TService>();
        }
    }
}
