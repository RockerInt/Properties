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
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly PropertiesContext _context;

        public PropertyImageRepository(PropertiesContext context)
        {
            _context = context;
        }

        // Basic CRUD for PropertyImage
        
        public async Task<List<PropertyImage>> GetPropertyImages() => await _context.PropertyImages.ToListAsync();

        public async Task<PropertyImage> GetPropertyImage(Guid id) => await _context.PropertyImages.FindAsync(id);

        public async Task<PropertyImage> CreatePropertyImage(PropertyImage propertyImage)
        {
            _context.PropertyImages.Add(propertyImage);
            await _context.SaveChangesAsync();
            return propertyImage;
        }

        public async Task<PropertyImage> UpdatePropertyImage(PropertyImage propertyImage)
        {
            _context.Entry(propertyImage).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return propertyImage;
        }

        public async Task<bool> DeletePropertyImage(PropertyImage propertyImage)
        {
            _context.PropertyImages.Remove(propertyImage);
            return await _context.SaveChangesAsync() > 0;
        }

        public bool PropertyImageExists(Guid id)
        {
            return _context.PropertyImages.Any(e => e.IdPropertyImage == id);
        }
    }
}
