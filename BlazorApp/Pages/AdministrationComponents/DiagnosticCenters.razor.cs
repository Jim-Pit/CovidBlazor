using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoViDAccountant.Components;
using DbDesign.Entities;

namespace CoViDAccountant.Pages.AdministrationComponents
{
    public partial class DiagnosticCenters
    {
        [CascadingParameter] Task<AuthenticationState> _authState { get; set; }

        [Parameter] public Guid UserId { get; set; }
        [Inject] Services.DiagnosticCenterManagementService ManagementService { get; set; }
        //[Inject] CoViDAccountantDbContext DbContext { get; set; }

        private List<DiagnosticCenter> _items;
        private DiagnosticCenterModel _model = new DiagnosticCenterModel();
        private DiagnosticCenterUsersAssignedModel _assignedUsersModel = new DiagnosticCenterUsersAssignedModel();

        private List<City> _cities;
        private List<District> _citiesDistricts => _cities?.SelectMany(x => x.Districts).ToList();

        private ModalDialog<DiagnosticCenterModel> _modal;
        private ModalDialog<DiagnosticCenterUsersAssignedModel> _assignToCenterModal;
        private string _errorMsg;

        private PageState _state;

        //private bool _loggedInAsStaff;
        private Func<DbDesign.CoViDAccountantDbContext, IQueryable<DiagnosticCenter>> _loggedInAsStaffQuery;
        protected override async Task OnInitializedAsync()
        {
            _state = new PageState();
            //_cities = await DbContext.Cities
            //    .Include(x => x.Districts)
            //    .Select(x => new City
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        Districts = x.Districts.Select(y => new District
            //        {
            //            Id = y.Id,
            //            Name = y.Name
            //        }).ToList()
            //    }).ToListAsync();
            await ManagementService.DoWork(async dbContext =>
            {
                _cities = await dbContext.Cities
                    //.Include(x => x.Districts)
                    .Select(x => new City
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Districts = x.Districts.Select(y => new District
                        {
                            Id = y.Id,
                            Name = y.Name,
                            City = new City
                            {
                                Id = x.Id, // in attach of district its city id must not be set to default value coz its state will be set to 'Added'
                                Name = x.Name
                            }
                        }).ToList()
                    }).ToListAsync();

                var loggedInUser = (await _authState).User;
                if (loggedInUser.IsInRole(Role.DoctorRoleKey) || loggedInUser.IsInRole(Role.StaffRoleKey))
                {
                    var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == loggedInUser.Identity.Name ||
                                                                               x.UserName == loggedInUser.Identity.Name);
                    //_loggedInAsStaffQuery = dbContext =>
                    //    dbContext.DiagnosticCenters.Where(x => dbContext.DiagnosticCenterUsers
                    //                                                    .Where(y => y.User.Id == user.Id)
                    //                                                    .Select(y => y.DiagnosticCenter.Id)
                    //                                                    .Any(id => id == x.Id));
                    _loggedInAsStaffQuery = dbContext =>
                        dbContext.DiagnosticCenterUsers.Where(x => x.User.Id == user.Id)
                                                       .Select(x => x.DiagnosticCenter);
                }
            });
            await Refresh();
        }

        private void OnModalShow(DiagnosticCenter selected = null)
        {
            _model = Mappings.Mapper.Map(selected);
            if(selected != null)
                _model.City = _cities.SingleOrDefault(x => x.Id == selected.Burg.City.Id);
            _modal.Show(_model);
        }

        private void OnModelCityChanged(ChangeEventArgs args)
        {
            _model.City = _cities.SingleOrDefault(x => x.Id == int.Parse(args.Value.ToString()));
            _modal.ReValidate(new FieldIdentifier(_model, nameof(_model.City)));
            StateHasChanged();
        }
        private void OnModelDistrictChanged(ChangeEventArgs args)
        {
            _model.Burg = _model.City.Districts.SingleOrDefault(x => x.Id == int.Parse(args.Value.ToString()));
            _modal.ReValidate(new FieldIdentifier(_model, nameof(_model.Burg)));
            StateHasChanged();
        }

        private async Task Submit(DiagnosticCenterModel model)
        {
            var isNew = model.Id == 0;

            var entity = await Save(model, isNew);
            if (entity != null)
            {
                if (isNew)
                {
                    await Refresh();
                    //StateHasChanged();
                }
                else
                {
                    var previous = _items.FirstOrDefault(x => x.Id == model.Id);
                    var idx = _items.IndexOf(previous);
                    _items.Remove(previous);
                    _items.Insert(idx, entity);
                }
            }
        }
        private async Task<DiagnosticCenter> Save(DiagnosticCenterModel model, bool isNew)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return null;
            }
            return await ManagementService.DoWork(async DbContext =>
            {
                var entity = isNew ? null : _items.SingleOrDefault(x => x.Id == model.Id);
                entity = Mappings.Mapper.Map(model, entity);

                if (isNew)
                {
                    DbContext.Attach(entity.Burg);
                    //var name = model.Name.Replace(" ", "_");
                    //var idx = 0;
                    //do
                    //{
                    //    entity.Code += name.Skip(idx).Take(1).FirstOrDefault();
                    //    idx += 3;
                    //} while (idx < name.Length - 1);
                    //DbContext.DiagnosticCenters.Add(entity);
                    DbContext.Attach(entity);
                }
                else
                {
                    DbContext.Update(entity);
                }
                await DbContext.SaveChangesAsync();
                return entity;
            });
        }

        //private async Task<DiagnosticCenter> Save(DiagnosticCenterModel model, bool isNew)
        //{
        //    if (string.IsNullOrEmpty(model.Name))
        //    {
        //        return null;
        //    }

        //    var entity = isNew ? null : _items.SingleOrDefault(x => x.Id == model.Id);
        //    entity = Mappings.Mapper.Map(model, entity);

        //    if (isNew)
        //    {
        //        //var name = model.Name.Replace(" ", "_");
        //        //var idx = 0;
        //        //do
        //        //{
        //        //    entity.Code += name.Skip(idx).Take(1).FirstOrDefault();
        //        //    idx += 3;
        //        //} while (idx < name.Length - 1);
        //        //DbContext.DiagnosticCenters.Add(entity);
        //        DbContext.Attach(entity);
        //    }
        //    else
        //    {
        //        DbContext.Update(entity);
        //    }
        //    await DbContext.SaveChangesAsync();
        //    return entity;
        //}

        private void OnDelete(DiagnosticCenter selected)
        {
        }

        //private async Task Refresh()
        //{
        //    var query = DbContext.DiagnosticCenters
        //        //.Include(x => x.Burg).ThenInclude(y => y.City)
        //        .AsNoTracking();//.AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(_state.Filter))
        //    {
        //        query = query.Where(x => x.Name.Contains(_state.Filter));
        //    }

        //    var records = await query.CountAsync();
        //    _state.TotalPages = (int)Math.Ceiling((double)records / _state.PageSize);

        //    //TODO: cannot
        //    _items = await query
        //        .Skip(_state.PageSize * (_state.CurrentPage - 1))
        //        .Take(_state.PageSize)
        //        .Select(x => new DiagnosticCenter
        //        {
        //            Id = x.Id,
        //            Name = x.Name,
        //            Code = x.Code,
        //            Address = new DbDesign.Entities.ValueObjects.Address
        //            {
        //                Street = x.Address.Street,
        //                Number = x.Address.Number,
        //                Zip = x.Address.Zip
        //            },
        //            Phone = x.Phone,
        //            Email = x.Email,
        //            Burg = new District
        //            {
        //                Id = x.Burg.Id,
        //                Name = x.Burg.Name,
        //                City = new City
        //                {
        //                    Id = x.Burg.City.Id,
        //                    Name = x.Burg.City.Name,
        //                    Districts = x.Burg.City.Districts
        //                    .Select(y => new District
        //                    {
        //                        Id = y.Id,
        //                        Name = y.Name
        //                    }).ToList()
        //                }
        //            }
        //        })
        //        .ToListAsync();
        //}

        private async Task Refresh()
        {
            await ManagementService.DoWork(async dbContext =>
            {
                var query = _loggedInAsStaffQuery == null 
                    ? dbContext.DiagnosticCenters.AsNoTracking()
                    : _loggedInAsStaffQuery(dbContext);

                if (!string.IsNullOrWhiteSpace(_state.Filter))
                {
                    query = query.Where(x => x.Name.Contains(_state.Filter));
                }

                if(_loggedInAsStaffQuery == null)
                {
                    var recordsCount = await query.CountAsync();
                    _state.TotalPages = (int)Math.Ceiling((double)recordsCount / _state.PageSize);
                }
                else
                {
                    _state.TotalPages = 1;
                }

                query = _loggedInAsStaffQuery == null ? query.Skip(_state.PageSize * (_state.CurrentPage - 1)).Take(_state.PageSize)
                                                      : query;

                _items = await query.Select(x => new DiagnosticCenter
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Address = new DbDesign.Entities.ValueObjects.Address
                            {
                                Street = x.Address.Street,
                                Number = x.Address.Number,
                                Zip = x.Address.Zip
                            },
                            Phone = x.Phone,
                            Email = x.Email,
                            Burg = new District
                            {
                                Id = x.Burg.Id,
                                Name = x.Burg.Name,
                                City = new City
                                {
                                    Id = x.Burg.City.Id,
                                    Name = x.Burg.City.Name,
                                    //Districts = x.Burg.City.Districts
                                    //.Select(y => new District
                                    //{
                                    //    Id = y.Id,
                                    //    Name = y.Name
                                    //}).ToList()
                                }
                            }
                        }).ToListAsync();
            });
        }

        private async Task OnAssignUser(DiagnosticCenter selected)
        {
            await ManagementService.DoWork(async dbContext =>
            {
                var availableUsers = await dbContext.Users
                    .Include(u => u.UserRole.Role)
                    .Where(u => u.UserRole != null)
                    .Where(u => u.UserRole.Role.Name == Role.StaffRoleKey ||
                                u.UserRole.Role.Name == Role.DoctorRoleKey)
                    .Where(u => dbContext.DiagnosticCenterUsers
                                         .Where(x => x.DiagnosticCenter.Id == selected.Id)
                                         .All(x => x.User.Id != u.Id))
                    .ToListAsync();
                _assignedUsersModel = new DiagnosticCenterUsersAssignedModel
                {
                    Center = new DiagnosticCenter
                    {
                        Id = selected.Id,
                        Name = selected.Name
                    },
                    AvailableUsers = availableUsers
                };
            });
            _assignToCenterModal.Show(_assignedUsersModel);
        }

        public void OnAssignUser(ChangeEventArgs args)
        {
            var userId = Guid.Parse(args.Value.ToString());
            var assignedUser = _assignedUsersModel.AvailableUsers.SingleOrDefault(x => x.Id == userId);
            if (assignedUser != null)
                _assignedUsersModel.AssignedUsers.Add(assignedUser);
        }

        private async Task Assign()
        {
            if (_assignedUsersModel.AssignedUsers.Any())
            {
                await ManagementService.DoWork(async dbContext =>
                {
                    var diagnosticCenter = _assignedUsersModel.Center;
                    dbContext.Attach(diagnosticCenter);
                    dbContext.AttachRange(_assignedUsersModel.AssignedUsers);
                    var assignedUsers = _assignedUsersModel.AssignedUsers.Select(u => new DiagnosticCenterUser
                    {
                        User = u,
                        DiagnosticCenter = diagnosticCenter
                    });
                    dbContext.DiagnosticCenterUsers.AddRange(assignedUsers);
                    await dbContext.SaveChangesAsync();
                    _assignToCenterModal.Hide();
                });
            }
        }

        private class DiagnosticCenterUsersAssignedModel
        {
            public DiagnosticCenter Center { get; set; }
            public List<User> AvailableUsers { get; set; }
            public List<User> AssignedUsers { get; set; } = new List<User>();

            public List<User> RestUsers => AvailableUsers.Except(AssignedUsers).ToList();
        }
    }
}
