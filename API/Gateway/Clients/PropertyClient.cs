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
    public class PropertyClient : IPropertyClient
    {
        private readonly UrlsConfig _urls;
        private readonly ILogger<PropertyClient> _logger;
        private readonly bool _Grpc;

        public PropertyClient(IOptions<UrlsConfig> urls, ILogger<PropertyClient> logger)
        {
            _urls = urls.Value;
            _logger = logger;
            _Grpc = Convert.ToBoolean(Environment.GetEnvironmentVariable("GRPC")?.ToLower() ?? "false");
        }

        public async Task<IEnumerable<Models.Complete.Property>> Get(PropertiesParameters parameters = null) => 
            _Grpc ? await GrpcCallerService.CallService(_urls.PropertiesGrpc, async channel =>
            {
                var client = new Property.PropertyClient(channel);
                _logger.LogDebug("grpc client created, request");
                var response = await client.GetPropertiesAsync(new Properties.Services.PropertyTypes.GetPropertiesRequest());
                _logger.LogDebug("grpc response {@response}", response);

                return response.Properties.Select(p => MapToPropertyComplete(p));
            }) :
            WebUtilities.MapListResponse<Models.Complete.Property>(
                await WebUtilities.ConectAsync(WebUtilities.Method.Get, _urls.PropertiesService, UrlsConfig.PropertyOperations.Get
                    (parameters is null ? string.Empty : $"?{parameters.GetQueryString()}"), 
                    null
                )
            );
                

        public async Task<Models.Complete.Property> GetById(Guid id) =>
            _Grpc ? await GrpcCallerService.CallService(_urls.PropertiesGrpc, async channel =>
            {
                var client = new Property.PropertyClient(channel);
                _logger.LogDebug("grpc client created, request");
                var response = await client.GetPropertyAsync(new Properties.Services.PropertyTypes.GetPropertyRequest() { Id = id.ToString() });
                _logger.LogDebug("grpc response {@response}", response);

                return MapToPropertyComplete(response.Property);
            }) :
            WebUtilities.MapResponse<Models.Complete.Property>(
                await WebUtilities.ConectAsync(WebUtilities.Method.Get, _urls.PropertiesService, UrlsConfig.PropertyOperations.GetById(id), null)
            );

        public async Task<Models.Lite.PropertyLite> Create(Models.Lite.PropertyLite @property) =>
            _Grpc ? await GrpcCallerService.CallService(_urls.PropertiesGrpc, async channel =>
            {
                var client = new Property.PropertyClient(channel);
                _logger.LogDebug("grpc client created, request");
                var response = await client.CreatePropertyAsync(new Properties.Services.PropertyTypes.PropertyRequest() { Property = MapToProperty(@property) });
                _logger.LogDebug("grpc response {@response}", response);

                return MapToPropertyLite(response.Property);
            }) :
            WebUtilities.MapResponse<Models.Lite.PropertyLite>(
                await WebUtilities.ConectAsync(WebUtilities.Method.Post, _urls.PropertiesService, UrlsConfig.PropertyOperations.Create(), @property)
            );

        public async Task<Models.Lite.PropertyLite> Update(Models.Lite.PropertyLite @property) =>
            _Grpc ? await GrpcCallerService.CallService(_urls.PropertiesGrpc, async channel =>
            {
                var client = new Property.PropertyClient(channel);
                _logger.LogDebug("grpc client created, request");
                var response = await client.UpdatePropertyAsync(new Properties.Services.PropertyTypes.PropertyRequest() { Property = MapToProperty(@property) });
                _logger.LogDebug("grpc response {@response}", response);

                return MapToPropertyLite(response.Property);
            }) :
            WebUtilities.MapResponse<Models.Lite.PropertyLite>(
                await WebUtilities.ConectAsync(WebUtilities.Method.Post, _urls.PropertiesService, UrlsConfig.PropertyOperations.Update(), @property)
            );

        public async Task<bool> Delete(Guid id) =>
            _Grpc ? await GrpcCallerService.CallService(_urls.PropertiesGrpc, async channel =>
            {
                var client = new Property.PropertyClient(channel);
                _logger.LogDebug("grpc client created, request");
                var response = await client.DeletePropertyAsync(new Properties.Services.PropertyTypes.DeletePropertyRequest() { Id = id.ToString() });
                _logger.LogDebug("grpc response {@response}", response);

                return response.Success;
            }) :
            (await WebUtilities.ConectAsync(WebUtilities.Method.Post, _urls.PropertiesService, UrlsConfig.PropertyOperations.Delete(id), null))
                .StatusCode == System.Net.HttpStatusCode.NoContent;

        #region Mappers
        private static Properties.Services.Entities.Property MapToProperty(Models.Lite.PropertyLite @property)
        {
            var _property = new Properties.Services.Entities.Property()
            {
                IdProperty = @property.IdProperty.ToString(),
                Name = @property.Name,
                Address = @property.Address,
                Price = @property.Price,
                CodeInternal = @property.CodeInternal,
                Year = @property.Year,
                IdOwner = @property.IdOwner.ToString()
            };

            return _property;
        }
        private static Properties.Services.Entities.Property MapToProperty(Models.Complete.Property @property)
        {
            var _property = new Properties.Services.Entities.Property()
            {
                IdProperty = @property.IdProperty.ToString(),
                Name = @property.Name,
                Address = @property.Address,
                Price = @property.Price,
                CodeInternal = @property.CodeInternal,
                Year = @property.Year
            };

            if (@property.Owner != null)
            {
                _property.Owner = new Properties.Services.Entities.Owner()
                {
                    IdOwner = @property.Owner.IdOwner.ToString(),
                    Name = @property.Owner.Name,
                    Address = @property.Owner.Address,
                    Birthday = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(@property.Owner.Birthday.SetKind(DateTimeKind.Utc))
                };
            }

            if (@property.PropertyImages != null && @property.PropertyImages.Any())
            {
                foreach (var item in @property.PropertyImages)
                {
                    _property.PropertyImages.Add(new Properties.Services.Entities.PropertyImage()
                    {
                        IdPropertyImage = item.IdPropertyImage.ToString(),
                        Enabled = item.Enabled,
                        IdProperty = item.IdProperty.ToString()
                    });
                }
            }

            if (@property.PropertyTraces != null && @property.PropertyTraces.Any())
            {
                foreach (var item in @property.PropertyTraces)
                {
                    _property.PropertyTraces.Add(new Properties.Services.Entities.PropertyTrace()
                    {
                        IdPropertyTrace = item.IdProperty.ToString(),
                        Name = item.Name,
                        DateSale = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(item.DateSale.SetKind(DateTimeKind.Utc)),
                        Value = item.Value,
                        Tax = item.Tax,
                        IdProperty = item.IdProperty.ToString()
                    });
                }
            }

            return _property;
        }

        private static Models.Lite.PropertyLite MapToPropertyLite(Properties.Services.Entities.Property @property, Models.Lite.PropertyLite newProperty = null)
        {
            if (newProperty is null)
            {
                newProperty = new Models.Lite.PropertyLite()
                {
                    IdProperty = @property.IdProperty?.ToGuid().Value ?? Guid.NewGuid(),
                    Name = @property.Name,
                    Address = @property.Address,
                    Price = @property.Price,
                    CodeInternal = @property.CodeInternal,
                    Year = Convert.ToInt16(@property.Year),
                    IdOwner = @property.IdOwner?.ToGuid().Value ?? Guid.NewGuid()
                };
            }
            else
            {
                newProperty.IdProperty = @property.IdProperty?.ToGuid().Value ?? newProperty.IdProperty;
                newProperty.Name = @property.Name;
                newProperty.Address = @property.Address;
                newProperty.Price = @property.Price;
                newProperty.CodeInternal = @property.CodeInternal;
                newProperty.Year = Convert.ToInt16(@property.Year);
                newProperty.IdOwner = @property.IdOwner?.ToGuid().Value ?? newProperty.IdOwner;
            }

            return newProperty;
        }
        private static Models.Complete.Property MapToPropertyComplete(Properties.Services.Entities.Property @property, Models.Complete.Property newProperty = null)
        {
            if (newProperty is null)
            {
                newProperty = new Models.Complete.Property()
                {
                    IdProperty = @property.IdProperty?.ToGuid().Value ?? Guid.NewGuid(),
                    Name = @property.Name,
                    Address = @property.Address,
                    Price = @property.Price,
                    CodeInternal = @property.CodeInternal,
                    Year = Convert.ToInt16(@property.Year),
                    Owner = @property.Owner is null ? null : new Models.Lite.OwnerLite()
                    {
                        IdOwner = @property.Owner.IdOwner?.ToGuid() ?? Guid.NewGuid(),
                        Name = @property.Owner.Name,
                        Address = @property.Owner.Address,
                        Birthday = @property.Owner.Birthday.ToDateTime()
                    }
                };
            }
            else
            {
                newProperty.IdProperty = @property.IdProperty?.ToGuid().Value ?? newProperty.IdProperty;
                newProperty.Name = @property.Name;
                newProperty.Address = @property.Address;
                newProperty.Price = @property.Price;
                newProperty.CodeInternal = @property.CodeInternal;
                newProperty.Year = Convert.ToInt16(@property.Year);
                newProperty.Owner = @property.Owner?.IdOwner is null ? null : new Models.Lite.OwnerLite()
                {
                    IdOwner = @property.Owner.IdOwner?.ToGuid() ?? Guid.NewGuid(),
                    Name = @property.Owner.Name,
                    Address = @property.Owner.Address,
                    Birthday = @property.Owner.Birthday.ToDateTime()
                };
            }

            if (@property.PropertyImages != null && @property.PropertyImages.Any())
            {
                foreach (var item in @property.PropertyImages)
                {
                    newProperty.PropertyImages.Add(new Models.Lite.PropertyImageLite()
                    {
                        IdPropertyImage = item.IdPropertyImage?.ToGuid() ?? Guid.NewGuid(),
                        Enabled = item.Enabled,
                        IdProperty = item.IdProperty.ToGuid() ?? newProperty.IdProperty
                    });
                }
            }

            if (@property.PropertyTraces != null && @property.PropertyTraces.Any())
            {
                foreach (var item in @property.PropertyTraces)
                {
                    newProperty.PropertyTraces.Add(new Models.Lite.PropertyTraceLite()
                    {
                        IdPropertyTrace = item.IdProperty?.ToGuid() ?? Guid.NewGuid(),
                        Name = item.Name,
                        DateSale = item.DateSale.ToDateTime(),
                        Value = item.Value,
                        Tax = item.Tax,
                        IdProperty = item.IdProperty?.ToGuid() ?? newProperty.IdProperty
                    });
                }
            }

            return newProperty;
        }
        #endregion
    }
}
