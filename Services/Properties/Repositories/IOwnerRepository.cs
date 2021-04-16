using Properties.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Properties.Repositories
{
    public interface IOwnerRepository
    {
        Task<List<Owner>> GetOwners();

        Task<Owner> GetOwner(Guid id);

        Task<Owner> CreateOwner(Owner owner);

        Task<Owner> UpdateOwner(Owner owner);

        Task<bool> DeleteOwner(Owner owner);

        bool OwnerExists(Guid id);
    }
}