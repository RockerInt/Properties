using Properties.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Properties.Repositories
{
    public interface IPropertyTraceRepository
    {
        Task<List<PropertyTrace>> GetPropertyTraces();

        Task<PropertyTrace> GetPropertyTrace(Guid id);

        Task<PropertyTrace> CreatePropertyTrace(PropertyTrace propertyTrace);

        Task<PropertyTrace> UpdatePropertyTrace(PropertyTrace propertyTrace);

        Task<bool> DeletePropertyTrace(PropertyTrace propertyTrace);

        bool PropertyTraceExists(Guid id);
    }
}