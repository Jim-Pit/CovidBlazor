using CoViDAccountant.Components;
using DbDesign;
using DbDesign.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoViDAccountant.Services;

namespace CoViDAccountant.Pages.AdministrationComponents
{
    public partial class Cities
    {
        [Inject] CoViDAccountantDbContext DbContext { get; set; }
        [Inject] private GenericOperationsService GenOpsService { get; set; }
        [Inject] private UIService UI { get; set; }

        private List<CityModel> _items;
        private City _cityModel = new City();
        private District _districtModel = new District();

        private ModalDialog<City> _cityModal;
        private ModalDialog<District> _districtModal;

        private string _errorMsg;

        private PageState _state;

        protected override async Task OnInitializedAsync()
        {
            _state = new PageState();
            await Refresh();
        }

        private void OnCityModalShow(City selected = null)
        {
            _cityModel = selected == null
                ? new City()
                : new City
                {
                    Id = selected.Id,
                    Name = selected.Name,
                    Districts = selected.Districts
                };
            _cityModal.Show(_cityModel);
        }

        private async Task Submit(City model)
        {
            var isNew = model.Id == 0;
            var success = await Save(model);
            if (success.HasValue)
            {
                if (isNew)
                {
                    await Refresh();
                    StateHasChanged();
                }
                else
                {
                    var previous = _items.FirstOrDefault(x => x.City.Id == model.Id);
                    var idx = _items.IndexOf(previous);
                    _items.Remove(previous);
                    _items.Insert(idx, new CityModel { City = model, ShowDistricts = previous.ShowDistricts });
                }
            }
        }

        private async Task<bool?> Save(City model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return null;
            }
            City entity = null;
            if (model.Id == 0)
            {
                entity = model;
                //DbContext.Cities.Add(model);
                DbContext.Attach(entity);
            }
            else
            {
                //entity = await DbContext.Cities.Include(x => x.Districts).SingleOrDefaultAsync(x => x.Id == model.Id);
                entity = _items.SingleOrDefault(x => x.City.Id == model.Id)?.City;
                if(entity != null)
                {
                    DbContext.Attach(entity);
                    entity.Name = model.Name;
                }
            }
            await DbContext.SaveChangesAsync();
            if(entity != null)
            {
                if(entity.Districts != null)
                {
                    entity.Districts.ForEach(x => DbContext.Entry(x).State = EntityState.Detached);
                }
                DbContext.Entry(entity).State = EntityState.Detached;
            }
            return true;
        }

        private void OnDelete(City selected)
        {
            UI.ShowConfirmDialog($"Are you sure you want to delete {selected.Name} from Cities?<br />All its districts will be deleted too.",
            async () =>
            {
                Func<CoViDAccountantDbContext, Task<string>> determineIfDeletableFunc = async dbContext =>
                {
                    if (await dbContext.DiagnosticCenters
                                       .AnyAsync(x => selected.Districts.Select(burg => burg.Id).Contains(x.Burg.Id)))
                        return $"There are diagnostic centers that are linked to {selected.Name} {nameof(City)}'s districts";
                    return string.Empty;
                };
                var result = await GenOpsService.DeleteEntity(selected, determineIfDeletableFunc);
                if (string.IsNullOrEmpty(result))
                {
                    await Refresh();
                    StateHasChanged();
                }
                else
                {
                    UI.ShowError($"{selected.Name} cannot be deleted.", result);
                }
            });
        }

        private void OnDistrictModalShow(City city, District selected = null)
        {
            _districtModel = selected == null
                ? new District()
                : new District
                {
                    Id = selected.Id,
                    Name = selected.Name
                };
            _districtModel.City = city;
            _districtModal.Show(_districtModel);
        }

        private async Task Submit(District model)
        {
            var isNew = model.Id == 0;
            _errorMsg = await Save(model);
            if (string.IsNullOrEmpty(_errorMsg))
            {
                var city = _items.SingleOrDefault(x => x.City.Id == model.City.Id).City;

                if (isNew)
                {
                    //city.Districts.Add(model); // AUTO
                    city.Districts = city.Districts.OrderBy(x => x.Name).ToList();
                    StateHasChanged();
                }
                else
                {
                    var previous = city.Districts.SingleOrDefault(x => x.Id == model.Id);
                    var idx = city.Districts.IndexOf(previous);
                    city.Districts.Remove(previous);
                    city.Districts.Insert(idx, model);
                }
            }
        }

        private async Task<string> Save(District model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return "Name Is Required";
            if (model.City == null)
                return "A city is required";
            DbContext.Attach(model.City);
            District entity = null;
            if (model.Id == 0)
            {
                entity = model;
                DbContext.Attach(entity);
                //DbContext.Districts.Add(model);
            }
            else
            {
                entity = await DbContext.Districts.Include(x => x.City).SingleOrDefaultAsync(x => x.Id == model.Id);
                entity.Name = model.Name;
            }
            await DbContext.SaveChangesAsync();
            if (entity != null)
            {
                DbContext.Entry(entity.City).State = EntityState.Detached;
                DbContext.Entry(entity).State = EntityState.Detached;
            }
            return string.Empty;
        }

        private void OnDelete(District selected, City city)
        {
            UI.ShowConfirmDialog("Are you sure you want to delete the selected Burg?",
            async () =>
            {
                Func<CoViDAccountantDbContext, Task<string>> determineIfDeletableFunc = async dbContext =>
                {
                    if (await dbContext.DiagnosticCenters
                                       .AnyAsync(x => x.Burg.Id == selected.Id))
                        return $"There are diagnostic centers that are linked to {selected.Name} {nameof(District)}";
                    return string.Empty;
                };
                var result = await GenOpsService.DeleteEntity(selected, determineIfDeletableFunc);
                if (string.IsNullOrEmpty(result))
                {
                    city.Districts.Remove(selected);
                    StateHasChanged();
                }
                else
                {
                    UI.ShowError($"{selected.Name} cannot be deleted.", result);
                }
            });
        }

        private async Task Refresh()
        {
            var query = DbContext.Cities.Include(x => x.Districts).AsQueryable();

            if (!string.IsNullOrWhiteSpace(_state.Filter))
            {
                query = query.Where(x => x.Name.Contains(_state.Filter) ||
                                         x.Districts.Any(x => x.Name.Contains(_state.Filter)));
            }

            var records = await query.CountAsync();
            _state.TotalPages = (int)Math.Ceiling((double)records / _state.PageSize);

            _items = await query.Skip(_state.PageSize * (_state.CurrentPage - 1))
                                .Take(_state.PageSize)
                                .Select(x => new CityModel 
                                {
                                    City = new City
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Districts = x.Districts.Select(y => new District
                                        {
                                            Id = y.Id,
                                            Name = y.Name
                                        }).OrderBy(x => x.Name).ToList()
                                    }
                                }).ToListAsync();
        }

        private class CityModel
        {
            public City City { get; set; }
            public bool ShowDistricts { get; set; }
            //public District District { get; set; }
        }
    }
}
