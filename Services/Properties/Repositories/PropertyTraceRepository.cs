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
    public class PropertyTraceRepository : IPropertyTraceRepository
    {
        private readonly PropertiesContext _context;

        public PropertyTraceRepository(PropertiesContext context)
        {
            _context = context;
        }

        // Basic CRUD for PropertyTrace
        
        public async Task<List<PropertyTrace>> GetPropertyTraces() => await _context.PropertyTraces.ToListAsync();

        public async Task<PropertyTrace> GetPropertyTrace(Guid id) => await _context.PropertyTraces.FindAsync(id);

        public async Task<PropertyTrace> CreatePropertyTrace(PropertyTrace propertyTrace)
        {
            _context.PropertyTraces.Add(propertyTrace);
            await _context.SaveChangesAsync();
            return propertyTrace;
        }

        public async Task<PropertyTrace> UpdatePropertyTrace(PropertyTrace propertyTrace)
        {
            _context.Entry(propertyTrace).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return propertyTrace;
        }

        public async Task<bool> DeletePropertyTrace(PropertyTrace propertyTrace)
        {
            _context.PropertyTraces.Remove(propertyTrace);
            return await _context.SaveChangesAsync() > 0;
        }

        public bool PropertyTraceExists(Guid id)
        {
            return _context.PropertyTraces.Any(e => e.IdPropertyTrace == id);
        }
    }
}
