using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Properties.Data;
using Properties.Models;
using Properties.Models.Parameters;
using Properties.Services.PropertyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Properties.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly PropertiesContext _context;

        public PropertyRepository(PropertiesContext context)
        {
            _context = context;
        }

        // Basic CRUD for Property

        public async Task<List<Property>> GetProperties(PropertiesParameters parameters = null)
        {
            IQueryable<Property> properties = _context.Properties.Include(l => l.Owner)
                                                                 .Include(a => a.PropertyImages)
                                                                 .Include(l => l.PropertyTraces);

            if (parameters != null)
                properties = properties.Where(
                    x => parameters.MinPrice <= x.Price && x.Price <= parameters.MaxPrice &&
                         parameters.MinYear <= x.Year && x.Year <= parameters.MaxYear
                );

            properties = properties.OrderBy(x => x.Price);

            if (parameters?.Paging ?? false)
                return await properties.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                                       .Take(parameters.PageSize)
                                       .ToListAsync();

            return await properties.ToListAsync();
        }

        public async Task<Property> GetProperty(Guid id) =>
            await _context.Properties.Include(l => l.Owner)
                                     .Include(a => a.PropertyImages)
                                     .Include(l => l.PropertyTraces)
                                     .Where(x => x.IdProperty == id).FirstOrDefaultAsync();

        public async Task<Property> CreateProperty(Property @property)
        {
            _context.Properties.Add(@property);
            await _context.SaveChangesAsync();
            return @property;
        }

        public async Task<Property> UpdateProperty(Property @property)
        {
            _context.Entry(@property).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return @property;
        }

        public async Task<bool> DeleteProperty(Property @property)
        {
            _context.Properties.Remove(@property);
            return await _context.SaveChangesAsync() > 0;
        }

        public bool PropertyExists(Guid id)
        {
            return _context.Properties.Any(e => e.IdProperty == id);
        }
    }
}
