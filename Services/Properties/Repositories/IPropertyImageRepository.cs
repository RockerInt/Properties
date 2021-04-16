using Properties.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Properties.Repositories
{
    public interface IPropertyImageRepository
    {
        Task<List<PropertyImage>> GetPropertyImages();

        Task<PropertyImage> GetPropertyImage(Guid id);

        Task<PropertyImage> CreatePropertyImage(PropertyImage propertyImage);

        Task<PropertyImage> UpdatePropertyImage(PropertyImage propertyImage);

        Task<bool> DeletePropertyImage(PropertyImage propertyImage);

        bool PropertyImageExists(Guid id);
    }
}