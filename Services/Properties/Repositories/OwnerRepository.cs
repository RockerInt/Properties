using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Properties.Data;
using Properties.Models;
using Properties.Services.PropertyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Properties.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly PropertiesContext _context;

        public OwnerRepository(PropertiesContext context)
        {
            _context = context;
        }

        // Basic CRUD for Owner
        
        public async Task<List<Owner>> GetOwners() => await _context.Owners.ToListAsync();

        public async Task<Owner> GetOwner(Guid id) => await _context.Owners.FindAsync(id);

        public async Task<Owner> CreateOwner(Owner owner)
        {
            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();
            return owner;
        }

        public async Task<Owner> UpdateOwner(Owner owner)
        {
            _context.Entry(owner).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return owner;
        }

        public async Task<bool> DeleteOwner(Owner owner)
        {
            _context.Owners.Remove(owner);
            return await _context.SaveChangesAsync() > 0;
        }

        public bool OwnerExists(Guid id)
        {
            return _context.Owners.Any(e => e.IdOwner == id);
        }
    }
}
