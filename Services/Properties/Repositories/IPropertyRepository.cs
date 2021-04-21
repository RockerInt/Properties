using Properties.Models;
using Properties.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Properties.Repositories
{
    public interface IPropertyRepository
    {
        Task<List<Property>> GetProperties(PropertiesParameters parameters = null);

        Task<Property> GetProperty(Guid id);

        Task<Property> CreateProperty(Property @property);

        Task<Property> UpdateProperty(Property @property);

        Task<bool> DeleteProperty(Property @property);

        bool PropertyExists(Guid id);
    }
}