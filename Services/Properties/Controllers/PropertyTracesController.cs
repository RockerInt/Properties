using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Properties.Data;
using Properties.Models;
using Properties.Repositories;
using Utilities;

namespace Properties.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PropertyTracesController : ControllerBase
    {
        private readonly IPropertyTraceRepository _repository;
        private readonly ILogger<PropertyTracesController> _logger;

        public PropertyTracesController(ILogger<PropertyTracesController> logger, IPropertyTraceRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Consulta la totalidad de las PropertyTraces
        /// </summary>
        /// <returns>Retorna una colección de PropertyTrace</returns>
        [HttpGet]
        [Route("Get")]
        [ProducesResponseType(typeof(IEnumerable<PropertyTrace>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPropertyTraces() =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation("Begin HttpGet call GetPropertyTraces");

                    var response = await _repository.GetPropertyTraces();

                    if (response != null && response.Any())
                        return Ok(response.JsonSerialize());

                    return NotFound("No results found");
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Consulta una PropertyTrace por su Id
        /// </summary>
        /// <param name="id">Identificador de la PropertyTrace</param>
        /// <returns>Retorna una PropertyTrace</returns>
        [HttpGet]
        [Route("Get/{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(PropertyTrace), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPropertyTrace(Guid id) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation($"Begin HttpGet call GetPropertiy for id {id}");

                    var propertyTrace = await _repository.GetPropertyTrace(id);

                    if (propertyTrace is null) return NotFound($"The property with id {id} do not exist");

                    return Ok(propertyTrace.JsonSerialize());
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Agrega una nueva PropertyTrace
        /// </summary>
        /// <param name="property">PropertyTrace a crear</param>
        /// <returns>Retorna la PropertyTrace creada</returns>
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreatePropertyTrace([FromBody] PropertyTrace propertyTrace) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation($"Begin HttpPost call CreatePropertyTrace for: {propertyTrace}");

                        propertyTrace = await _repository.CreatePropertyTrace(propertyTrace);

                        return StatusCode((int)HttpStatusCode.Created, propertyTrace.JsonSerialize());
                    }
                    else return BadRequest();
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateException))
                        if (PropertyTraceExists(propertyTrace.IdPropertyTrace))
                            return Conflict($"The property with id {propertyTrace.IdPropertyTrace} already exist");
                    return await HttpErrorHandlerAsync(error);
                }
            );

        /// <summary>
        /// Actualiza una PropertyTrace
        /// </summary>
        /// <param name="property">PropertyTrace a actualizar</param>
        /// <returns>Retorna la PropertyTrace actualizada</returns>
        [HttpPost]
        [Route("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdatePropertyTrace([FromBody] PropertyTrace propertyTrace) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation($"Begin HttpPost call UpdatePropertyTrace for: {propertyTrace}");

                        propertyTrace = await _repository.UpdatePropertyTrace(propertyTrace);

                        return Ok(propertyTrace.JsonSerialize());
                    }
                    else return BadRequest();
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateConcurrencyException))
                        if (!PropertyTraceExists(propertyTrace.IdPropertyTrace))
                            return NotFound($"The property with id {propertyTrace.IdPropertyTrace} do not exist");
                    return await HttpErrorHandlerAsync(error);
                }
            );

        /// <summary>
        /// Elimina una PropertyTrace por su Id
        /// </summary>
        /// <param name="id">Identificador de la PropertyTrace</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete/{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeletePropertyTrace(Guid id) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation($"Begin HttpPost call DeletePropertyTrace for id {id}");

                    var propertyTrace = await _repository.GetPropertyTrace(id);

                    if (propertyTrace is null) return NotFound($"The property with id {id} do not exist");

                    var response = await _repository.DeletePropertyTrace(propertyTrace);

                    return response ? NoContent() : StatusCode((int)HttpStatusCode.NotModified);
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Valida si existe una PropertyTrace por su Id
        /// </summary>
        /// <param name="id">Identificador de la PropertyTrace</param>
        /// <returns>True si existe</returns>
        private bool PropertyTraceExists(Guid id) => _repository.PropertyTraceExists(id);

        private IActionResult HttpErrorHandler(Exception error)
        {
            _logger.LogError("An error has occurred: @error", error);
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error has occurred, contact the administrator!");
        }
        private async Task<IActionResult> HttpErrorHandlerAsync(Exception error) => await Task.FromResult(HttpErrorHandler(error));
    }
}
