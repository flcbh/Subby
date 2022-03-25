using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;

namespace Subby.Infrastructure.UserStores
{
    public class UserRoleStore : IRoleStore<Role>
    {
        private readonly IRepository _repository;

        public UserRoleStore(IRepository repository)
        {
            _repository = repository;
        }

        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            try
            {
                _repository.Add(role);
                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Description = ex.Message
                }));
            }
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            var match = _repository.Linq<Role>().FirstOrDefault(r => r.Id == role.Id);
            if (match != null)
            {
                match.Name = role.Name;

                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed());
            }
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            var match = _repository.Linq<Role>().FirstOrDefault(r => r.Id == role.Id);
            if (match != null)
            {
                _repository.Delete(match);

                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed());
            }
        }

        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var role = _repository.Linq<Role>().FirstOrDefault(r => r.Id.ToString() == roleId);

            return Task.FromResult(role);
        }

        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var role = _repository.Linq<Role>().FirstOrDefault(r => r.NormalizedName == normalizedRoleName);

            return Task.FromResult(role);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;

            return Task.FromResult(true);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            // Do nothing. In this simple example, the normalized name is generated from the role name.
            
            return Task.FromResult(true);
        }

        public void Dispose() { }
    }
}