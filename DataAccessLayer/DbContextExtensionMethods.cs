using DbDesign.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDesign
{
    public static class DbContextExtensionMethods
    {
        public static Func<CoViDAccountantDbContext, Task> SetDiagnosticCenter(
            this DiagnosticCenter diagnosticCenter, int diagnosticCenterId)
        {
            return async dbContext =>
            {
                diagnosticCenter = await dbContext.DiagnosticCenters.SingleOrDefaultAsync(x => x.Id == diagnosticCenterId);
            };
        }

        public static Func<CoViDAccountantDbContext, Task> SetDiagnosticCenter2(
            this DiagnosticCenter @this, int diagnosticCenterId)
        {
            return async dbContext =>
            {
                @this = await dbContext.DiagnosticCenters.SingleOrDefaultAsync(x => x.Id == diagnosticCenterId);
            };
        }

        public static Func<CoViDAccountantDbContext, Task> SetVaccines(
            this List<Vaccine> vaccines)
        {
            return async dbContext =>
            {
                vaccines = await dbContext.Vaccines.ToListAsync();
            };
        }

        public static Func<CoViDAccountantDbContext, Task> SetDiagnostiCenterVaccinations(
            this List<Vaccination> vaccinations, int diagnosticCenterId)
        {
            return async dbContext =>
            {
                vaccinations = await dbContext.Vaccinations
                                              .Include(x => x.Person)
                                              .Where(x => x.DiagnosticCenter.Id == diagnosticCenterId)
                                              .ToListAsync();
            };
        }
        public static Func<CoViDAccountantDbContext, Task> SetDiagnostiCenterCovidTests(
            this List<CovidTest> covidTests, int diagnosticCenterId)
        {
            return async dbContext =>
            {
                covidTests = await dbContext.CovidTests
                                            .Include(x => x.Person)
                                            .Where(x => x.DiagnosticCenter.Id == diagnosticCenterId)
                                            .ToListAsync();
            };
        }
    }
}
