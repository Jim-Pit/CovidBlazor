using DbDesign;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoViDAccountant.Services
{
    public class GenericOperationsService : ServiceBase
    {
        public GenericOperationsService(IServiceScopeFactory serviceScopeFactory)
            : base(serviceScopeFactory)
        { }

        public async Task<bool> DeleteEntity<TId>(SharedClasses.Abstractions.Entity<TId> entity)
        {
            try
            {
                var result = await UseDbContext(async dbContext =>
                {
                    dbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    return await dbContext.SaveChangesAsync();
                });
                return result == 1;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> DeleteEntity<TId>(SharedClasses.Abstractions.Entity<TId> entity,
            Func<CoViDAccountantDbContext, Task<string>> determineIfDeletable)
        {
            try
            {
                return await UseDbContext(async dbContext =>
                {
                    var error = await determineIfDeletable(dbContext);
                    if(string.IsNullOrEmpty(error))
                    {
                        dbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                        await dbContext.SaveChangesAsync();
                    }
                    return error;
                });
            }
            catch(Exception x)
            {
                return x.Message;
            }
        }
    }
}
