using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbDesign;
using DbDesign.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoViDAccountant.Services
{
    public class DiagnosticCenterManagementService : ServiceBase
    {
        public DiagnosticCenterManagementService(IServiceScopeFactory serviceScopeFactory)
            : base(serviceScopeFactory)
        {}

        public async Task Execute(params Func<CoViDAccountantDbContext, Task>[] tasks)
        {
            await WorkTasks(tasks);
        }

        public async Task Execute(Func<CoViDAccountantDbContext, Task> task)
        {
            await DoWork(task);
        }

        // i.e. TypeAhead use
        public async Task<T> Execute<T>(Func<CoViDAccountantDbContext, Task<T>> task)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                return await DoWork(task);
            }
        }

        #region Record
        public async Task UpdateRecord(Record record)
        {
            await UseDbContext(async dbContext =>
            {
                dbContext.Update(record);
                dbContext.Entry(record).Reference(x => x.DiagnosticCenter).IsModified = false;
                await dbContext.SaveChangesAsync();
            });
        }
        #endregion

        public async Task<DiagnosticCenter> GetDiagnosticCenter(int diagnosticCenterId)
        {
            return await UseDbContext(dbContext =>
            {
                return dbContext.DiagnosticCenters.SingleOrDefaultAsync(x => x.Id == diagnosticCenterId);
            });
        }

        public async Task<List<Vaccine>> GetVaccines()
        {
            return await UseDbContext(dbContext =>
            {
                return dbContext.Vaccines.ToListAsync();
            });
        }
        

        private IQueryable<Person> GetDiagnosticCenterPersonsQuery(
            CoViDAccountantDbContext dbContext, int diagnosticCenterId)
        {
            return dbContext.Records.Where(x => x.DiagnosticCenter.Id == diagnosticCenterId)
                            .Select(x => x.Person);
        }

        public async Task<List<Person>> GetPersons(int diagnosticCenterId)
        {
            return await UseDbContext(dbContext =>
            {
                return GetDiagnosticCenterPersonsQuery(dbContext, diagnosticCenterId).ToListAsync();
            });
        }

        private IQueryable<Vaccination> GetPersonsVaccinationsQuery(
            IQueryable<Vaccination> query, List<Guid> personIds)
        {
            return query.Where(x => personIds.Any(id => id == x.Person.Id));
        }

        public async Task<List<Vaccination>> GetVaccinations(int diagnosticCenterId)
        {
            return await UseDbContext(dbContext =>
            {
                return dbContext.Vaccinations
                                .Include(x => x.Person)
                                .Where(x => x.DiagnosticCenter.Id == diagnosticCenterId)
                                .ToListAsync();
            });
        }

        public async Task<List<CovidTest>> GetCovidTests(int diagnosticCenterId)
        {
            return await UseDbContext(dbContext =>
            {
                return dbContext.CovidTests
                                .Include(x => x.Person)
                                .Where(x => x.DiagnosticCenter.Id == diagnosticCenterId)
                                .ToListAsync();
            });
        }

        public async Task<Vaccination> CreateNewVaccinationRecord(Vaccination newVaccination) //, Person newPerson)
        {
            return await UseDbContext(async dbContext =>
            {
                if(newVaccination.Vaccine == null)
                    throw new InvalidOperationException("Select Vaccine first");
                if (newVaccination.DiagnosticCenter == null)
                    throw new InvalidOperationException("Select Vaccine first");
                dbContext.Attach(newVaccination.DiagnosticCenter);
                dbContext.Attach(newVaccination.Vaccine);

                if (newVaccination.Person != null)
                    dbContext.Attach(newVaccination.Person);
                //else if (newPerson != null)
                //{
                //    //dbContext.Persons.Add(newPerson);
                //    newVaccination.Person = newPerson;
                //}
                else
                    throw new InvalidOperationException("Person?");

                dbContext.Vaccinations.Add(newVaccination);
                await dbContext.SaveChangesAsync();
                return newVaccination;
            });
        }

        public async Task<CovidTest> CreateNewCovidTestRecord(CovidTest newCovidTest) //, Person newPerson)
        {
            return await UseDbContext(async dbContext =>
            {
                if (newCovidTest.DiagnosticCenter == null)
                    throw new InvalidOperationException();
                dbContext.Attach(newCovidTest.DiagnosticCenter);

                if (newCovidTest.Person != null)
                    dbContext.Attach(newCovidTest.Person);
                //else if (newPerson != null)
                //{
                //    //dbContext.Persons.Add(newPerson);
                //    newCovidTest.Person = newPerson;
                //}
                else
                    throw new InvalidOperationException();

                dbContext.CovidTests.Add(newCovidTest);
                await dbContext.SaveChangesAsync();
                return newCovidTest;
            });
        }
    }
}
