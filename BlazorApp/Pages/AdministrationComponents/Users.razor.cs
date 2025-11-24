using CoViDAccountant.Components;
using DbDesign;
using DbDesign.Entities;
using DbDesign.Entities.ValueObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoViDAccountant.Pages.AdministrationComponents
{
    public partial class Users
    {
        [Inject] CoViDAccountantDbContext DbContext { get; set; }
        [Inject] RoleManager<Role> RoleManager { get; set; }
        [Inject] UserManager<User> UserManager { get; set; }

        private List<User> _users;
        private User _model = new User();
        private List<Role> _roles = new List<Role>();
        private List<string> _roleNames;
        private ModalDialog<User> _modal;
        private string _errorMsg;

        private PageState _state;

        protected override async Task OnInitializedAsync()
        {
            _state = new PageState();
            _roles = await RoleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            _roleNames = _roles.Select(r => r.Name).ToList();
            await Refresh();
        }

        private void OnModalShow(User selected = null)
        {
            _model = selected == null
                ? new User()
                : new User(selected);
            _modal.Show(_model);
        }

        private async Task Submit(User model)
        {
            var isNew = model.Id == Guid.Empty;
            var success = await Save(model);
            if (success.HasValue)
            {
                if (isNew)
                {
                    _users.Add(model);
                    StateHasChanged();
                }
                else
                {
                    var previous = _users.FirstOrDefault(x => x.Id == model.Id);
                    var idx = _users.IndexOf(previous);
                    _users.Remove(previous);
                    _users.Insert(idx, model);
                }
            }
        }

        private async Task<bool?> Save(User model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return null;
            }
            if (model.Id == Guid.Empty)
            {
                var createResult = await UserManager.CreateAsync(model);
                if(model.UserRole != null && model.UserRole.RoleId != Guid.Empty)
                {
                    var addRoleResult = await UserManager.AddToRoleAsync(model, model.UserRole.Role.Name);
                }
            }
            else
            {
                var entity = await DbContext.Users
                    .Include(x => x.UserRole).ThenInclude(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Id == model.Id);
                if(entity.UserRole?.RoleId != model.UserRole?.RoleId)
                {
                    if (entity.UserRole != null)
                    {
                        DbContext.UserRoles.Remove(entity.UserRole);
                    }
                    if (model.UserRole != null)
                    {
                        DbContext.UserRoles.Add(new UserRole
                        {
                            User = entity,
                            Role = model.UserRole.Role
                        });
                    }
                }
                Copy(model, entity);
                await DbContext.SaveChangesAsync();
            }
            return true;
        }

        private void Copy(User from, User to)
        {
            to.FirstName = from.FirstName;
            to.LastName = from.LastName;
            to.Email = from.Email;
            Address.CopyValues(from.Address, to.Address);
        }

        private void OnDelete(User selected)
        {
        }
        private void RoleChanged(ChangeEventArgs args)
        {
            var roleStr = args.Value?.ToString();
            //var role = roleStr != null 
            //    ? _roles.SingleOrDefault(r => r.Name.Equals(roleStr, StringComparison.OrdinalIgnoreCase))
            //    : new Role();
            var role = _roles.SingleOrDefault(r => r.Name.Equals(roleStr, StringComparison.OrdinalIgnoreCase));
            _model.UserRole = role != null 
                ? new UserRole
                      {
                        Role = role,
                        RoleId = role.Id
                      }
                : null;
        }

        private void EmailChanged(string email)
        {
            _model.Email = email;
            _model.UserName = email;
        }

        private async Task Refresh()
        {
            var query = DbContext.Users
                .Include(u => u.UserRole).ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(_state.Filter))
            {
                query = query.Where(x => (x.FirstName + ' ' + x.LastName).Contains(_state.Filter) ||
                                         (x.LastName + ' ' + x.FirstName).Contains(_state.Filter) ||
                                         x.Address.Zip.Contains(_state.Filter));
            }

            _users = await _state.ExecutePagedQuery<User>(query);
        }
    }
}
