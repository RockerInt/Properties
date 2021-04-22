using Properties.Gateway.Config;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Properties.Services;
using Microsoft.Extensions.Logging;
using Properties.Service;
using Gateway.Models.Parameters;

namespace Properties.Gateway.Clients
{
    public class PropertyImageClient : IPropertyImageClient
    {
        private readonly UrlsConfig _urls;
        private readonly ILogger<PropertyImageClient> _logger;

        public PropertyImageClient(IOptions<UrlsConfig> urls, ILogger<PropertyImageClient> logger)
        {
            _urls = urls.Value;
            _logger = logger;
        }

        public async Task<IEnumerable<Models.Complete.PropertyImage>> Get() => 
            WebUtilities.MapListResponse<Models.Complete.PropertyImage>(
                await WebUtilities.ConectAsync(WebUtilities.Method.Get, _urls.PropertiesService, UrlsConfig.PropertyImagesOperations.Get(), null)
            );
                
        public async Task<Models.Complete.PropertyImage> GetById(Guid id) =>
            WebUtilities.MapResponse<Models.Complete.PropertyImage>(
                await WebUtilities.ConectAsync(WebUtilities.Method.Get, _urls.PropertiesService, UrlsConfig.PropertyImagesOperations.GetById(id), null)
            );

        public async Task<Models.Lite.PropertyImageLite> Create(Models.Lite.PropertyImageLite propertyImage) =>
            WebUtilities.MapResponse<Models.Lite.PropertyImageLite>(
                await WebUtilities.ConectAsync(WebUtilities.Method.Post, _urls.PropertiesService, UrlsConfig.PropertyImagesOperations.Create(), propertyImage)
            );

        public async Task<Models.Lite.PropertyImageLite> Update(Models.Lite.PropertyImageLite propertyImage) =>
            WebUtilities.MapResponse<Models.Lite.PropertyImageLite>(
                await WebUtilities.ConectAsync(WebUtilities.Method.Post, _urls.PropertiesService, UrlsConfig.PropertyImagesOperations.Update(), propertyImage)
            );

        public async Task<bool> Delete(Guid id) =>
            (await WebUtilities.ConectAsync(WebUtilities.Method.Post, _urls.PropertiesService, UrlsConfig.PropertyImagesOperations.Delete(id), null))
                .StatusCode == System.Net.HttpStatusCode.NoContent;
    }
}
