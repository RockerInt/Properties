using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Properties.Gateway.Clients
{
    public interface IPropertyClient
    {
        Task<IEnumerable<Properties.Models.Complete.Property>> Get();

        Task<Properties.Models.Complete.Property> GetById(Guid id);

        Task<Properties.Models.Lite.PropertyLite> Create(Properties.Models.Lite.PropertyLite @property);

        Task<Properties.Models.Lite.PropertyLite> Update(Properties.Models.Lite.PropertyLite @property);

        Task<bool> Delete(Guid id);
    }
}