using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Properties.Data;
using Properties.Repositories;
using Properties.Services.PropertyTraceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Properties.Services
{
    public class PropertyTraceService : PropertyTrace.PropertyTraceBase
    {
        private readonly IPropertyTraceRepository _repository;
        private readonly ILogger<PropertyTraceService> _logger;

        public PropertyTraceService(ILogger<PropertyTraceService> logger, PropertyTraceRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }

        #region EndPoints
        /// <summary>
        /// Consulta la totalidad de las PropertyTraces
        /// </summary>
        /// <param name="request">Parametro de entrada que viene vacio pero está preparado para futuras implementaciones</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Retorna una colección de PropertyTrace</returns>
        public override async Task<PropertyTracesResponse> GetPropertyTraces(GetPropertyTracesRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation("Begin grpc call PropertyTraceService.GetPropertyTraces");

                    var response = await _repository.GetPropertyTraces();

                    if (response != null && response.Any())
                    {
                        return MapToPropertyTracesResponse(response);
                    }

                    context.Status = new Status(StatusCode.NotFound, "No results found");

                    return null;
                },
                async error =>
                {
                    context.Status = new Status(StatusCode.Internal, "An error has occurred, contact the administrator!");
                    await DbErrorLog(error);
                    return new PropertyTracesResponse();
                }
            );

        /// <summary>
        /// Consulta una PropertyTrace por su Id
        /// </summary>
        /// <param name="request">Contiene un string con formato Guid que representa el Id</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Retorna una PropertyTrace</returns>
        public override async Task<PropertyTraceResponse> GetPropertyTrace(GetPropertyTraceRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    var id = request.Id?.ToGuid();

                    if (id is null)
                    {
                        context.Status = new Status(StatusCode.InvalidArgument, $"The id is required");

                        return null;
                    }

                    _logger.LogInformation($"Begin grpc call PropertyTraceService.GetPropertiy for id {id}");

                    var @property = await _repository.GetPropertyTrace(id.Value);

                    if (@property is null)
                    {
                        context.Status = new Status(StatusCode.NotFound, $"The property with id {id} do not exist");

                        return null;
                    }

                    return MapToPropertyTraceResponse(@property);
                },
                async error =>
                {
                    context.Status = new Status(StatusCode.Internal, "An error has occurred, contact the administrator!");
                    await DbErrorLog(error);
                    return new PropertyTraceResponse();
                }
            );

        /// <summary>
        /// Agrega una nueva PropertyTrace
        /// </summary>
        /// <param name="request">Contiene una entidad de tipo PropertyTrace</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Retorna la PropertyTrace creada</returns>
        public override async Task<PropertyTraceResponse> CreatePropertyTrace(PropertyTraceRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    var @property = MapToPropertyTrace(request.PropertyTrace);

                    _logger.LogInformation($"Begin grpc call PropertyTraceService.CreatePropertyTrace for: {@property}");

                    @property = await _repository.CreatePropertyTrace(@property);

                    return MapToPropertyTraceResponse(@property);
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateException))
                    {
                        var @property = MapToPropertyTrace(request.PropertyTrace);
                        if (PropertyTraceExists(@property.IdPropertyTrace))
                            context.Status = new Status(StatusCode.AlreadyExists, $"The property with id {@property.IdPropertyTrace} already exist");
                    }
                    await DbErrorLog(error);
                    return new PropertyTraceResponse();
                }
            );

        /// <summary>
        /// Actualiza una PropertyTrace
        /// </summary>
        /// <param name="request">Contiene una entidad de tipo PropertyTrace</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Retorna la PropertyTrace actualizada</returns>
        public override async Task<PropertyTraceResponse> UpdatePropertyTrace(PropertyTraceRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    var @property = request.PropertyTrace;

                    _logger.LogInformation($"Begin grpc call PropertyTraceService.UpdatePropertyTrace for: {@property}");

                    var id = @property?.IdPropertyTrace?.ToGuid();

                    if (id is null)
                    {
                        context.Status = new Status(StatusCode.InvalidArgument, $"The id with value {id} is invalid");

                        return null;
                    }

                    if (@property?.IdProperty?.ToGuid() is null)
                    {
                        context.Status = new Status(StatusCode.InvalidArgument, $"The owner's id with value {@property?.IdProperty} is invalid");

                        return null;
                    }

                    var _property = await _repository.GetPropertyTrace(id.Value);

                    _property = MapToPropertyTrace(@property, _property);

                    _property = await _repository.UpdatePropertyTrace(_property);

                    return MapToPropertyTraceResponse(_property);
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateConcurrencyException))
                    {
                        var id = request.PropertyTrace?.IdPropertyTrace?.ToGuid();
                        if (!PropertyTraceExists(id.Value)) context.Status = new Status(StatusCode.NotFound, $"The property with id {id} do not exist");
                    }
                    await DbErrorLog(error);
                    return new PropertyTraceResponse();
                }
            );

        /// <summary>
        /// Elimina una PropertyTrace por su Id
        /// </summary>
        /// <param name="request">Contiene un string con formato Guid que representa el Id</param>
        /// <param name="context">Contexto del llamado</param>
        /// <returns>Un bool que representa si logró eliminar la PropertyTrace</returns>
        public override async Task<DeletePropertyTraceResponse> DeletePropertyTrace(DeletePropertyTraceRequest request, ServerCallContext context) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    var id = request.Id?.ToGuid();

                    _logger.LogInformation($"Begin grpc call PropertyTraceService.DeletePropertyTrace for id {id}");

                    if (id is null)
                    {
                        context.Status = new Status(StatusCode.InvalidArgument, $"The id is required");

                        return null;
                    }

                    var @property = await _repository.GetPropertyTrace(id.Value);

                    if (@property is null)
                    {
                        context.Status = new Status(StatusCode.NotFound, $"The property with id {id} do not exist");

                        return null;
                    }

                    var response = await _repository.DeletePropertyTrace(@property);

                    return new DeletePropertyTraceResponse() { Success = response };
                },
                async error =>
                {
                    context.Status = new Status(StatusCode.Internal, "An error has occurred, contact the administrator!");
                    await DbErrorLog(error);
                    return new DeletePropertyTraceResponse() { Success = false };
                }
            );

        /// <summary>
        /// Valida si existe una PropertyTrace por su Id
        /// </summary>
        /// <param name="id">Identificador de la PropertyTrace</param>
        /// <returns>True si existe</returns>
        private bool PropertyTraceExists(Guid id) => _repository.PropertyTraceExists(id);
        /// <summary>
        /// Funcion que guarda un log de error
        /// </summary>
        /// <param name="error"></param>
        private async Task DbErrorLog(Exception error) => await Task.Run(() => _logger.LogError("An error has occurred: @error", error));

        #endregion

        #region Mappers
        private static PropertyTracesResponse MapToPropertyTracesResponse(List<Models.PropertyTrace> properties)
        {
            var response = new PropertyTracesResponse();

            properties.ForEach(propertyTrace => response.PropertyTraces.Add(MapToPropertyTrace(propertyTrace)));

            return response;
        }

        private static PropertyTraceResponse MapToPropertyTraceResponse(Models.PropertyTrace propertyTrace)
        {
            return new PropertyTraceResponse()
            {
                PropertyTrace = MapToPropertyTrace(propertyTrace)
            };
        }

        private static Entities.PropertyTrace MapToPropertyTrace(Models.PropertyTrace propertyTrace)
        {
            return new Entities.PropertyTrace()
            {
                IdPropertyTrace = propertyTrace.IdPropertyTrace.ToString(),
                Name = propertyTrace.Name,
                DateSale = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(propertyTrace.DateSale.SetKind(DateTimeKind.Utc)),
                Value = propertyTrace.Value,
                Tax = propertyTrace.Tax,
                IdProperty = propertyTrace.IdProperty.ToString()
            };
        }

        private static Models.PropertyTrace MapToPropertyTrace(Entities.PropertyTrace propertyTrace, Models.PropertyTrace newPropertyTrace = null)
        {
            if (newPropertyTrace is null)
            {
                newPropertyTrace = new Models.PropertyTrace()
                {
                    IdPropertyTrace = propertyTrace.IdPropertyTrace?.ToGuid().Value ?? Guid.NewGuid(),
                    Name = propertyTrace.Name,
                    DateSale = propertyTrace.DateSale?.ToDateTime() ?? DateTime.Now,
                    Value = propertyTrace.Value,
                    Tax = propertyTrace.Tax,
                    IdProperty = propertyTrace.IdProperty?.ToGuid().Value ?? Guid.NewGuid()
                };
            }
            else
            {
                newPropertyTrace.IdPropertyTrace = propertyTrace.IdPropertyTrace?.ToGuid().Value ?? newPropertyTrace.IdPropertyTrace;
                newPropertyTrace.Name = propertyTrace.Name;
                newPropertyTrace.DateSale = propertyTrace.DateSale?.ToDateTime() ?? newPropertyTrace.DateSale;
                newPropertyTrace.Value = propertyTrace.Value;
                newPropertyTrace.Tax = propertyTrace.Tax;
                newPropertyTrace.IdProperty = propertyTrace.IdProperty?.ToGuid().Value ?? newPropertyTrace.IdProperty;
            }

            return newPropertyTrace;
        }
        #endregion
    }
}
