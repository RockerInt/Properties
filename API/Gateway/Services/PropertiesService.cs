using Properties.Gateway.Clients;
using Microsoft.Extensions.Logging;
using Properties.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gateway.Models.Parameters;

namespace Properties.Gateway.Services
{
    public class PropertiesService
    {
        private readonly ILogger<PropertiesService> _logger;
        private readonly IPropertyClient _propertyClient;
        private readonly IPropertyImageClient _propertyImageClient;

        public PropertiesService(ILogger<PropertiesService> logger, IPropertyClient propertyClient, IPropertyImageClient propertyImageClient)
        {
            _logger = logger;
            _propertyClient = propertyClient;
            _propertyImageClient = propertyImageClient;
        }

        public async Task<IEnumerable<Models.Complete.Property>> GetProperties(PropertiesParameters parameters)
        {
            _logger.LogDebug("Property client created, request = Get");
            var response = await _propertyClient.Get(parameters);
            _logger.LogDebug("Property response {@response}", response);

            return response;
        }

        public async Task<Models.Complete.Property> GetPropertyById(Guid id)
        {
            _logger.LogDebug("Property client created, request = GetPropertyById{@id}", id);
            var response = await _propertyClient.GetById(id);
            _logger.LogDebug("Property response {@response}", response);

            return response;
        }

        public async Task<Models.Lite.PropertyLite> CreateProperty(Models.Lite.PropertyLite @property)
        {
            _logger.LogDebug("Property client created, request = CreateProperty{@property}", @property);
            var response = await _propertyClient.Create(@property);
            _logger.LogDebug("Property response {@response}", response);

            return response;
        }

        public async Task<Models.Lite.PropertyLite> UpdateProperty(Models.Lite.PropertyLite @property)
        {
            _logger.LogDebug("Property client created, request = UpdateProperty{@property}", @property);
            var response = await _propertyClient.Update(@property);
            _logger.LogDebug("Property response {@response}", response);

            return response;
        }

        public async Task<Models.Lite.PropertyImageLite> CreatePropertyImage(Models.Lite.PropertyImageLite propertyImage)
        {
            _logger.LogDebug("Property client created, request = CreateProperty{@property}", propertyImage);
            var response = await _propertyImageClient.Create(propertyImage);
            _logger.LogDebug("Property response {@response}", response);

            return response;
        }

        public bool PropertyExists(Guid idProperty) =>
            Utilities.Utilities.TryCatch(
                () => GetPropertyById(idProperty).Result != null
                , error => false
            );
        
    }
}
