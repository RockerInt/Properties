using Gateway.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Properties.Gateway.Clients
{
    public interface IPropertyImageClient
    {
        Task<IEnumerable<Models.Complete.PropertyImage>> Get();

        Task<Models.Complete.PropertyImage> GetById(Guid id);

        Task<Models.Lite.PropertyImageLite> Create(Models.Lite.PropertyImageLite propertyImage);

        Task<Models.Lite.PropertyImageLite> Update(Models.Lite.PropertyImageLite propertyImage);

        Task<bool> Delete(Guid id);
    }
}