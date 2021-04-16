using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Properties.Data;
using Properties.Repositories;
using Properties.Services.PropertyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Properties.Services
{
    public class PropertyService : Property.PropertyBase
    {
        private readonly IPropertyRepository _repository;
        private readonly ILogger<PropertyService> _logger;

        public PropertyService(ILogger<PropertyService> logger, PropertyRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }

        #region EndPoints
        /// <summary>
        /// Consulta la totalidad de las Properties
        /// </summary>
        /// <param name="request">Parametro de entrada que viene vacio pero está preparado para futuras implementaciones</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Retorna una colección de Property</returns>
        public override async Task<PropertiesResponse> GetProperties(GetPropertiesRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation("Begin grpc call PropertyService.GetProperties");

                    var response = await _repository.GetProperties();

                    if (response != null && response.Any())
                    {
                        return MapToPropertiesResponse(response);
                    }

                    context.Status = new Status(StatusCode.NotFound, "No results found");

                    return null;
                },
                async error =>
                {
                    context.Status = new Status(StatusCode.Internal, "An error has occurred, contact the administrator!");
                    await DbErrorLog(error);
                    return new PropertiesResponse();
                }
            );

        /// <summary>
        /// Consulta una Property por su Id
        /// </summary>
        /// <param name="request">Contiene un string con formato Guid que representa el Id</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Retorna una Property</returns>
        public override async Task<PropertyResponse> GetProperty(GetPropertyRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    var id = request.Id?.ToGuid();

                    if (id is null)
                    {
                        context.Status = new Status(StatusCode.InvalidArgument, $"The id is required");

                        return null;
                    }

                    _logger.LogInformation($"Begin grpc call PropertyService.GetPropertiy for id {id}");

                    var @property = await _repository.GetProperty(id.Value);

                    if (@property is null)
                    {
                        context.Status = new Status(StatusCode.NotFound, $"The property with id {id} do not exist");

                        return null;
                    }

                    return MapToPropertyResponse(@property);
                },
                async error =>
                {
                    context.Status = new Status(StatusCode.Internal, "An error has occurred, contact the administrator!");
                    await DbErrorLog(error);
                    return new PropertyResponse();
                }
            );

        /// <summary>
        /// Agrega una nueva Property
        /// </summary>
        /// <param name="request">Contiene una entidad de tipo Property</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Retorna la Property creada</returns>
        public override async Task<PropertyResponse> CreateProperty(PropertyRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    var @property = MapToProperty(request.Property);

                    _logger.LogInformation($"Begin grpc call PropertyService.CreateProperty for: {@property}");

                    @property = await _repository.CreateProperty(@property);

                    return MapToPropertyResponse(@property);
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateException))
                    {
                        var @property = MapToProperty(request.Property);
                        if (PropertyExists(@property.IdProperty))
                            context.Status = new Status(StatusCode.AlreadyExists, $"The property with id {@property.IdProperty} already exist");
                    }
                    await DbErrorLog(error);
                    return new PropertyResponse();
                }
            );

        /// <summary>
        /// Actualiza una Property
        /// </summary>
        /// <param name="request">Contiene una entidad de tipo Property</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Retorna la Property actualizada</returns>
        public override async Task<PropertyResponse> UpdateProperty(PropertyRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    var @property = request.Property;

                    _logger.LogInformation($"Begin grpc call PropertyService.UpdateProperty for: {@property}");

                    var id = @property?.IdProperty?.ToGuid();

                    if (id is null)
                    {
                        context.Status = new Status(StatusCode.InvalidArgument, $"The id with value {id} is invalid");

                        return null;
                    }

                    if (@property?.IdOwner?.ToGuid() is null)
                    {
                        context.Status = new Status(StatusCode.InvalidArgument, $"The owner's id with value {@property?.IdOwner} is invalid");

                        return null;
                    }

                    var _property = await _repository.GetProperty(id.Value);

                    _property = MapToProperty(@property, _property);

                    _property = await _repository.UpdateProperty(_property);

                    return MapToPropertyResponse(_property);
                },
                async error =>
                { 
                    if (error.GetType() == typeof(DbUpdateConcurrencyException)) 
                    { 
                        var id = request.Property?.IdProperty?.ToGuid(); 
                        if (!PropertyExists(id.Value)) context.Status = new Status(StatusCode.NotFound, $"The property with id {id} do not exist"); 
                    } 
                    await DbErrorLog(error);
                    return new PropertyResponse();
                }
            );

        /// <summary>
        /// Elimina una Property por su Id
        /// </summary>
        /// <param name="request">Contiene un string con formato Guid que representa el Id</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Un bool que representa si logró eliminar la Property</returns>
        public override async Task<DeletePropertyResponse> DeleteProperty(DeletePropertyRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    var id = request.Id?.ToGuid();

                    _logger.LogInformation($"Begin grpc call PropertyService.DeleteProperty for id {id}");

                    if (id is null)
                    {
                        context.Status = new Status(StatusCode.InvalidArgument, $"The id is required");

                        return null;
                    }

                    var @property = await _repository.GetProperty(id.Value);

                    if (@property is null)
                    {
                        context.Status = new Status(StatusCode.NotFound, $"The property with id {id} do not exist");

                        return null;
                    }

                    var response = await _repository.DeleteProperty(@property);

                    return new DeletePropertyResponse() { Success = response };
                },
                async error =>
                {
                    context.Status = new Status(StatusCode.Internal, "An error has occurred, contact the administrator!");
                    await DbErrorLog(error);
                    return new DeletePropertyResponse() { Success = false };
                }
            );

        /// <summary>
        /// Valida si existe una Property por su Id
        /// </summary>
        /// <param name="id">Identificador de la Property</param>
        /// <returns>True si existe</returns>
        private bool PropertyExists(Guid id) => _repository.PropertyExists(id);
        /// <summary>
        /// Funcion que guarda un log de error
        /// </summary>
        /// <param name="error"></param>
        private async Task DbErrorLog(Exception error) => await Task.Run(() => _logger.LogError("An error has occurred: @error", error));

        #endregion

        #region Mappers
        private static PropertiesResponse MapToPropertiesResponse(List<Models.Property> properties)
        {
            var response = new PropertiesResponse();

            properties.ForEach(@property => response.Properties.Add(MapToProperty(@property)));

            return response;
        }
        private static PropertyResponse MapToPropertyResponse(Models.Property @property)
        {
            return new PropertyResponse()
            {
                Property = MapToProperty(@property)
            };
        }

        private static Entities.Property MapToProperty(Models.Property @property)
        {
            var _property = new Entities.Property()
            {
                IdProperty = @property.IdProperty.ToString(),
                Name = @property.Name,
                Address = @property.Address,
                Price = @property.Price,
                CodeInternal = @property.CodeInternal,
                Year = @property.Year,
                IdOwner = @property.IdOwner.ToString()
            };

            if (@property.Owner != null)
            {
                _property.Owner = new Entities.Owner()
                {
                    IdOwner = @property.Owner.IdOwner.ToString(),
                    Name = @property.Owner.Name,
                    Address = @property.Owner.Address,
                    Birthday = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(@property.Owner.Birthday.SetKind(DateTimeKind.Utc))
                    //Photo = @property.IdOwnerNavigation.Photo
                };
            }

            if (@property.PropertyImages != null && @property.PropertyImages.Any())
            {
                foreach (var item in @property.PropertyImages)
                {
                    _property.PropertyImages.Add(new Entities.PropertyImage()
                    {
                        IdPropertyImage = item.IdPropertyImage.ToString(),
                        Enabled = item.Enabled,
                        IdProperty = item.IdProperty.ToString()
                        //File = item.File
                    });
                }
            }

            if (@property.PropertyTraces != null && @property.PropertyTraces.Any())
            {
                foreach (var item in @property.PropertyTraces)
                {
                    _property.PropertyTraces.Add(new Entities.PropertyTrace()
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
        private static Models.Property MapToProperty(Entities.Property @property, Models.Property newProperty = null)
        {
            if (newProperty is null)
            {
                newProperty = new Models.Property()
                {
                    IdProperty = @property.IdProperty?.ToGuid().Value ?? Guid.NewGuid(),
                    Name = @property.Name,
                    Address = @property.Address,
                    Price = @property.Price,
                    CodeInternal = @property.CodeInternal,
                    Year = Convert.ToInt16(@property.Year),
                    IdOwner = @property.IdOwner?.ToGuid().Value ?? Guid.NewGuid(),
                    Owner = @property.Owner is null ? null : new Models.Owner()
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
                newProperty.IdOwner = @property.IdOwner?.ToGuid().Value ?? newProperty.IdOwner;
                newProperty.Owner = @property.Owner?.IdOwner is null ? null : new Models.Owner()
                {
                    IdOwner = @property.Owner.IdOwner?.ToGuid() ?? Guid.NewGuid(),
                    Name = @property.Owner.Name,
                    Address = @property.Owner.Address,
                    Birthday = @property.Owner.Birthday.ToDateTime()
                };
            }

            return newProperty;
        }
        #endregion
    }
}
