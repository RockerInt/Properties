using Gateway.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Properties.Gateway.Clients
{
    public interface IPropertyClient
    {
        Task<IEnumerable<Models.Complete.Property>> Get(PropertiesParameters parameters);

        Task<Models.Complete.Property> GetById(Guid id);

        Task<Models.Lite.PropertyLite> Create(Models.Lite.PropertyLite @property);

        Task<Models.Lite.PropertyLite> Update(Models.Lite.PropertyLite @property);

        Task<bool> Delete(Guid id);
    }
}