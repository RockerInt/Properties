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
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyRepository _repository;
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(ILogger<PropertiesController> logger, IPropertyRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Consulta la totalidad de las Properties
        /// </summary>
        /// <returns>Retorna una colección de Property</returns>
        [HttpGet]
        [Route("Get")]
        [ProducesResponseType(typeof(IEnumerable<Property>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProperties() =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation("Begin HttpGet call GetProperties");

                    var response = await _repository.GetProperties();

                    if (response != null && response.Any())
                        return Ok(response.JsonSerialize());

                    return NotFound("No results found");
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Consulta una Property por su Id
        /// </summary>
        /// <param name="id">Identificador de la Property</param>
        /// <returns>Retorna una Property</returns>
        [HttpGet]
        [Route("Get/{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Property), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProperty(Guid id) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation($"Begin HttpGet call GetPropertiy for id {id}");

                    var @property = await _repository.GetProperty(id);

                    if (@property is null) return NotFound($"The property with id {id} do not exist");

                    return Ok(@property.JsonSerialize());
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Agrega una nueva Property
        /// </summary>
        /// <param name="property">Property a crear</param>
        /// <returns>Retorna la Property creada</returns>
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateProperty([FromBody] Property @property) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation($"Begin HttpPost call CreateProperty for: {@property}");

                        @property = await _repository.CreateProperty(@property);

                        return StatusCode((int)HttpStatusCode.Created, @property.JsonSerialize());
                    }
                    else return BadRequest();
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateException))
                        if (PropertyExists(@property.IdProperty))
                            return Conflict($"The property with id {@property.IdProperty} already exist");
                    return await HttpErrorHandlerAsync(error);
                }
            );

        /// <summary>
        /// Actualiza una Property
        /// </summary>
        /// <param name="property">Property a actualizar</param>
        /// <returns>Retorna la Property actualizada</returns>
        [HttpPost]
        [Route("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProperty([FromBody] Property @property) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation($"Begin HttpPost call UpdateProperty for: {@property}");

                        @property = await _repository.UpdateProperty(@property);

                        return Ok(@property.JsonSerialize());
                    }
                    else return BadRequest();
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateConcurrencyException))
                        if (!PropertyExists(@property.IdProperty)) 
                            return NotFound($"The property with id {@property.IdProperty} do not exist");
                    return await HttpErrorHandlerAsync(error);
                }
            );

        /// <summary>
        /// Elimina una Property por su Id
        /// </summary>
        /// <param name="id">Identificador de la Property</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete/{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteProperty(Guid id) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation($"Begin HttpPost call DeleteProperty for id {id}");

                    var @property = await _repository.GetProperty(id);

                    if (@property is null) return NotFound($"The property with id {id} do not exist");

                    var response = await _repository.DeleteProperty(@property);

                    return response ? NoContent() : StatusCode((int)HttpStatusCode.NotModified);
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Valida si existe una Property por su Id
        /// </summary>
        /// <param name="id">Identificador de la Property</param>
        /// <returns>True si existe</returns>
        private bool PropertyExists(Guid id) => _repository.PropertyExists(id);

        private IActionResult HttpErrorHandler(Exception error)
        {
            _logger.LogError("An error has occurred: @error", error);
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error has occurred, contact the administrator!");
        }
        private async Task<IActionResult> HttpErrorHandlerAsync(Exception error) => await Task.FromResult(HttpErrorHandler(error));
    }
}
