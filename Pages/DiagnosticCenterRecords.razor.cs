using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoViDAccountant.Pages
{
    using DbDesign;
    using DbDesign.Entities;
    using Components;    
    using Services;
    using Microsoft.AspNetCore.Components;

    public partial class DiagnosticCenterRecords : ITabs
    {
        // ITabs
        //public bool Refreshing { get; set; } = true;

        // property that is set by TabContaier
        public string ActiveTabSign { get; set; }

        // this property is used to retain the active tab after an add or an update
        public bool Modified { get; set; }

        public async Task LoadData(string selectedTab)
        {
            await ManagementService.Execute(async dbContext =>
            {
                switch (selectedTab)
                {
                    case VACCINATIONS_TAB:
                        _covidTests = null;
                        _vaccinations = await LoadVaccinations(dbContext);//.Invoke(dbContext)
                        break;
                    case COVIDTESTS_TAB:
                        _vaccinations = null;
                        _covidTests = await LoadCovidTests(dbContext);
                        break;
                }
            });
            StateHasChanged();
            //Refreshing = false;
        }

        private const string VACCINATIONS_TAB = "Vaccinations";
        private const string COVIDTESTS_TAB = "Covid Tests";

        private TabContainer _tabContainer;
        //private List<string> _tabsSigns => _tabContainer.Tabs.Select(x => x.Sign).ToList();

        [Parameter]
        public int DiagnosticCenterId { get; set; }

        [Inject] private DiagnosticCenterManagementService ManagementService { get; set; }
        [Inject] private GenericOperationsService GenOpsService { get; set; }
        [Inject] private UIService UI { get; set; }

        private DiagnosticCenter _diagnosticCenter;
        private List<Person> _clients = new List<Person>();
        private List<Vaccine> _vaccines = new List<Vaccine>();

        private List<Vaccination> _vaccinations;// = new List<Vaccination>();
        private List<CovidTest> _covidTests;// = new List<CovidTest>();
        private PageState _vaccinationsState = new PageState();
        private PageState _covidTestsState = new PageState();

        private Record _model = new Vaccination { Person = new Person() };
        //private Person _personModel = new Person();

        private ModalDialog<Record> _recordModal;

        private string _errorMsg;

        protected override async Task OnInitializedAsync()
        {
            //_diagnosticCenter = await ManagementService.GetDiagnosticCenter(DiagnosticCenterId);
            //_covidTests = await ManagementService.GetCovidTests(DiagnosticCenterId);
            //_vaccinations = await ManagementService.GetVaccinations(DiagnosticCenterId);
            //_vaccines = await ManagementService.GetVaccines();

            //await ManagementService.InitializeDiagnosticCenterRecords(
            //    _diagnosticCenter.SetDiagnosticCenter(DiagnosticCenterId),
            //    _vaccines.SetVaccines(),
            //    _covidTests.SetDiagnostiCenterCovidTests(DiagnosticCenterId),
            //    _vaccinations.SetDiagnostiCenterVaccinations(DiagnosticCenterId));
            //await ManagementService.InitializeDiagnosticCenterRecords(async dbContext =>
            //{
            //    await _diagnosticCenter.SetDiagnosticCenter(DiagnosticCenterId).Invoke(dbContext);
            //    await _vaccines.SetVaccines().Invoke(dbContext);
            //    await _covidTests.SetDiagnostiCenterCovidTests(DiagnosticCenterId).Invoke(dbContext);
            //    await _vaccinations.SetDiagnostiCenterVaccinations(DiagnosticCenterId).Invoke(dbContext);
            //});
            await ManagementService.Execute(async dbContext =>
            {
                _diagnosticCenter = await dbContext.DiagnosticCenters.SingleOrDefaultAsync(x => x.Id == DiagnosticCenterId);
                _vaccines = await dbContext.Vaccines.ToListAsync();

                //_covidTests = await dbContext.CovidTests.Include(x => x.Person)
                //    .Where(x => x.DiagnosticCenter.Id == DiagnosticCenterId).ToListAsync();
                //_vaccinations = await dbContext.Vaccinations.Include(x => x.Person)
                //    .Where(x => x.DiagnosticCenter.Id == DiagnosticCenterId).ToListAsync();
            });

            //TODO: use whole Persons table to check for existing AMKA
            //SetDiagnosticCenterClients();
            //_clients = await ManagementService.GetPersons(DiagnosticCenterId);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_covidTests == null && _vaccinations == null)
            {
                await LoadData(_tabContainer.ActiveTab.Sign);
            }
        }

        private Func<CoViDAccountantDbContext, Task<List<Vaccination>>> LoadVaccinations => dbContext =>
        {
            var query = dbContext.Vaccinations
                     .Include(x => x.Person).Include(x => x.Vaccine)
                     .Where(x => x.DiagnosticCenter.Id == DiagnosticCenterId);
            if (!string.IsNullOrEmpty(_vaccinationsState.Filter))
            {
                if (DateTime.TryParseExact(_vaccinationsState.Filter.Trim(), PageState.DateFormats, null, System.Globalization.DateTimeStyles.None, out var date))
                    query = query.Where(x => x.EventDate == date);
                else
                {
                    var normalizedFilter = _vaccinationsState.Filter.ToLower();
                    query = query.Where(x => x.Person.FirstName.ToLower().Contains(normalizedFilter) ||
                                             x.Person.LastName.ToLower().Contains(normalizedFilter) ||
                                             x.Vaccine.Code.ToLower().Contains(normalizedFilter) ||
                                             x.Vaccine.Description.ToLower().Contains(normalizedFilter));
                }
            }
            return _vaccinationsState.ExecutePagedQuery(query);

        };

        private Func<CoViDAccountantDbContext, Task<List<CovidTest>>> LoadCovidTests => dbContext =>
        {
            var query = dbContext.CovidTests
                     .Include(x => x.Person)
                     .Where(x => x.DiagnosticCenter.Id == DiagnosticCenterId);
            if (!string.IsNullOrEmpty(_covidTestsState.Filter))
            {
                if (DateTime.TryParseExact(_covidTestsState.Filter.Trim(), PageState.DateFormats, null, System.Globalization.DateTimeStyles.None, out var date))
                    query = query.Where(x => x.EventDate == date);
                else
                {
                    var normalizedFilter = _covidTestsState.Filter.ToLower();
                    query = query.Where(x => x.Person.FirstName.ToLower().Contains(normalizedFilter) ||
                                             x.Person.LastName.ToLower().Contains(normalizedFilter) ||
                                             x.TestType.ToLower().Contains(normalizedFilter));
                }
            }
            return _covidTestsState.ExecutePagedQuery(query);
        };

        private Task<IEnumerable<Person>> SearchPersons(string amka, int maxResults)
        {
            return ManagementService.Execute(async dbContext =>
            {
                amka = amka.Trim();
                // var normalizedText = _personModel.AMKA.ToLowerInvariant();
                var results = await dbContext.Persons
                    .Where(x => x.AMKA.StartsWith(amka))
                    .Take(maxResults)
                    .ToListAsync();
                if (results.Count == 0)
                {
                    _model.Person = new Person { AMKA = amka };
                    await InvokeAsync(StateHasChanged);
                }
                return results.AsEnumerable();
            });
        }

        private void PersonFound(Person selected)
        {
            _model.Person = selected;
            //_personModel.FirstName = selected.FirstName;
            //_personModel.LastName = selected.LastName;
            //_personModel.Tel = selected.Tel;
            //_personModel.AMKA = selected.AMKA;
        }

        private void OnAddRecord(RecordType recordType)
        {
            _model = recordType switch
            {
                RecordType.CovidTest => new CovidTest { Person = new Person() },
                RecordType.Vaccination => new Vaccination { Person = new Person() },
                _ => null
            };
            _model.DiagnosticCenter = _diagnosticCenter;
            _model.EventDate = DateTime.Now.Date;
            if (_model as CovidTest != null)
            {

            }
            else if (_model as Vaccination != null)
            {

            }
            //_personModel = new Person();
            _recordModal.Show(_model);
        }

        private void OnEditRecord(Record record)
        {
            _errorMsg = null;
            switch (record)
            {
                case CovidTest covidTest:
                    _model = covidTest;
                    break;
                case Vaccination vaccination:
                    _model = vaccination;
                    break;
                default:
                    _model = null;
                    break;
            };
            if (_model as CovidTest != null)
            {

            }
            else if (_model as Vaccination != null)
            {

            }
            //_personModel = new Person();
            _recordModal.Show(_model);
        }

        //private void OnPersonSelected(Guid selectedPersonId)
        //{
        //    _model.Person = _clients.SingleOrDefault(x => x.Id == selectedPersonId);
        //}
        private void OnVaccineSelected(short selectedVaccineId)
        {
            (_model as Vaccination).Vaccine = _vaccines.SingleOrDefault(x => x.Id == selectedVaccineId);
        }
        private void OnResultChanged(CovidTestResult result)
        {
            (_model as CovidTest).Result =
                result == CovidTestResult.Negative
                    ? false
                    : result == CovidTestResult.Positive
                        ? true
                        : (bool?)null;
        }

        private async Task Submit()
        {
            var isNew = _model.Id == 0;
            try
            {
                if (string.IsNullOrWhiteSpace(_model.Person?.AMKA))
                {
                    _errorMsg = "AMKA is required";
                    StateHasChanged();
                    return;
                }
                if (_model as Vaccination != null)
                {
                    var vaccination = _model as Vaccination;
                    if (isNew)
                    {
                        var createdRecord = await ManagementService.CreateNewVaccinationRecord(vaccination); //, _model.Person == null ? _personModel : null);
                        _vaccinations.Add(createdRecord);
                    }
                    else
                    {
                        await ManagementService.UpdateRecord(vaccination);
                    }
                }
                else if (_model as CovidTest != null)
                {
                    var covidTest = _model as CovidTest;
                    if (isNew)
                    {
                        var createdRecord = await ManagementService.CreateNewCovidTestRecord(covidTest); //, _model.Person == null ? _personModel : null);
                        _covidTests.Add(createdRecord);
                    }
                    else
                    {
                        await ManagementService.UpdateRecord(covidTest);
                    }
                }
                //SetDiagnosticCenterClients();
                _recordModal.Hide();
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;
            }
        }
        //private void SetDiagnosticCenterClients()
        //{
        //    _clients = _covidTests.Select(x => x.Person).Distinct().Concat(
        //                   _vaccinations.Select(x => x.Person).Distinct()).ToList();
        //}

        private void OnDelete(CovidTest selected)
        {
            UI.ShowConfirmDialog($"Are you sure you want to delete {selected.Person?.FullName}'s CoViD test on {selected.EventDate.ToString("dd MMM yyyy")}?",
                //async () =>
                //{
                //    if (await GenOpsService.DeleteEntity<long>(selected))
                //    {
                //        _covidTests.Remove(selected);
                //        await InvokeAsync(StateHasChanged);
                //    }
                //    else
                //    {
                //        UI.ShowError("Delete failed");
                //    }
                //});
                EventCallback.Factory.Create(this, DeleteCallback));

            async Task DeleteCallback()
            {
                if (await GenOpsService.DeleteEntity<long>(selected))
                {
                    _covidTests.Remove(selected);
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    UI.ShowError("Delete failed");
                }
            }
        }

        private void OnDelete(Vaccination selected)
        {
            UI.ShowConfirmDialog($"Are you sure you want to delete {selected.Person?.FullName}'s Vaccination on {selected.EventDate.ToString("dd MMM yyyy")}?",
                async () =>
                {
                    if (await GenOpsService.DeleteEntity<long>(selected))
                    {
                        _vaccinations.Remove(selected);
                        await InvokeAsync(StateHasChanged);
                    }
                    else
                    {
                        UI.ShowError("Delete failed");
                    }
                });
        }

        private void OnDelete2(Vaccination selected)
        {
            UI.ShowConfirmDialog($"Are you sure you want to delete {selected.Person?.FullName}'s Vaccination on {selected.EventDate.ToString("dd MMM yyyy")}?",
                () => Task.Run(async () =>
                {
                    if (await GenOpsService.DeleteEntity<long>(selected))
                    {
                        _vaccinations.Remove(selected);
                        await InvokeAsync(StateHasChanged);
                    }
                    else
                    {
                        UI.ShowError("Delete failed");
                    }
                }));
        }

        private async Task RefreshVaccinations(PageState state)
        {
            //_vaccinationsState = state;
            await ManagementService.Execute(async dbContext =>
            {
                _vaccinations = await LoadVaccinations(dbContext);//.Invoke(dbContext)
            });
        }

        private async Task RefreshCovidTests()
        {
            await ManagementService.Execute(async dbContext =>
            {
                _covidTests = await LoadCovidTests(dbContext);
            });
        }
    }
}
