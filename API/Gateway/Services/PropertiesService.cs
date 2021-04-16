using Properties.Gateway.Clients;
using Microsoft.Extensions.Logging;
using Properties.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Properties.Gateway.Services
{
    public class PropertiesService
    {
        private readonly ILogger<PropertiesService> _logger;
        private readonly PropertyClient _properties;

        public PropertiesService(ILogger<PropertiesService> logger, PropertyClient properties)
        {
            _logger = logger;
            _properties = properties;
        }

        public async Task<IEnumerable<Properties.Models.Complete.Property>> GetProperties()
        {
            _logger.LogDebug("Property client created, request = Get");
            var response = await _properties.Get();
            _logger.LogDebug("Property response {@response}", response);

            return response;
        }

        public async Task<Properties.Models.Complete.Property> GetPropertyById(Guid id)
        {
            _logger.LogDebug("Property client created, request = GetPropertyById{@id}", id);
            var response = await _properties.GetById(id);
            _logger.LogDebug("Property response {@response}", response);

            return response;
        }

        public async Task<Properties.Models.Lite.PropertyLite> CreateProperty(Properties.Models.Lite.PropertyLite @property)
        {
            _logger.LogDebug("Property client created, request = CreateProperty{@property}", @property);
            var response = await _properties.Create(@property);
            _logger.LogDebug("Property response {@response}", response);

            return response;
        }

    }
}
