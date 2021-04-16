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
    public class PropertyImagesController : ControllerBase
    {
        private readonly IPropertyImageRepository _repository;
        private readonly ILogger<PropertyImagesController> _logger;

        public PropertyImagesController(ILogger<PropertyImagesController> logger, IPropertyImageRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Consulta la totalidad de las PropertyImages
        /// </summary>
        /// <returns>Retorna una colección de PropertyImage</returns>
        [HttpGet]
        [Route("Get")]
        [ProducesResponseType(typeof(IEnumerable<PropertyImage>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPropertyImages() =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation("Begin HttpGet call GetPropertyImages");

                    var response = await _repository.GetPropertyImages();

                    if (response != null && response.Any())
                        return Ok(response.JsonSerialize());

                    return NotFound("No results found");
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Consulta una PropertyImage por su Id
        /// </summary>
        /// <param name="id">Identificador de la PropertyImage</param>
        /// <returns>Retorna una PropertyImage</returns>
        [HttpGet]
        [Route("Get/{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(PropertyImage), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPropertyImage(Guid id) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation($"Begin HttpGet call GetPropertiy for id {id}");

                    var propertyImage = await _repository.GetPropertyImage(id);

                    if (propertyImage is null) return NotFound($"The property with id {id} do not exist");

                    return Ok(propertyImage.JsonSerialize());
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Agrega una nueva PropertyImage
        /// </summary>
        /// <param name="property">PropertyImage a crear</param>
        /// <returns>Retorna la PropertyImage creada</returns>
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreatePropertyImage([FromBody] PropertyImage propertyImage) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation($"Begin HttpPost call CreatePropertyImage for: {propertyImage}");

                        propertyImage = await _repository.CreatePropertyImage(propertyImage);

                        return StatusCode((int)HttpStatusCode.Created, propertyImage.JsonSerialize());
                    }
                    else return BadRequest();
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateException))
                        if (PropertyImageExists(propertyImage.IdPropertyImage))
                            return Conflict($"The property with id {propertyImage.IdPropertyImage} already exist");
                    return await HttpErrorHandlerAsync(error);
                }
            );

        /// <summary>
        /// Actualiza una PropertyImage
        /// </summary>
        /// <param name="property">PropertyImage a actualizar</param>
        /// <returns>Retorna la PropertyImage actualizada</returns>
        [HttpPost]
        [Route("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdatePropertyImage([FromBody] PropertyImage propertyImage) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation($"Begin HttpPost call UpdatePropertyImage for: {propertyImage}");

                        propertyImage = await _repository.UpdatePropertyImage(propertyImage);

                        return Ok(propertyImage.JsonSerialize());
                    }
                    else return BadRequest();
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateConcurrencyException))
                        if (!PropertyImageExists(propertyImage.IdPropertyImage))
                            return NotFound($"The property with id {propertyImage.IdPropertyImage} do not exist");
                    return await HttpErrorHandlerAsync(error);
                }
            );

        /// <summary>
        /// Elimina una PropertyImage por su Id
        /// </summary>
        /// <param name="id">Identificador de la PropertyImage</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete/{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeletePropertyImage(Guid id) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation($"Begin HttpPost call DeletePropertyImage for id {id}");

                    var propertyImage = await _repository.GetPropertyImage(id);

                    if (propertyImage is null) return NotFound($"The property with id {id} do not exist");

                    var response = await _repository.DeletePropertyImage(propertyImage);

                    return response ? NoContent() : StatusCode((int)HttpStatusCode.NotModified);
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Valida si existe una PropertyImage por su Id
        /// </summary>
        /// <param name="id">Identificador de la PropertyImage</param>
        /// <returns>True si existe</returns>
        private bool PropertyImageExists(Guid id) => _repository.PropertyImageExists(id);

        private IActionResult HttpErrorHandler(Exception error)
        {
            _logger.LogError("An error has occurred: @error", error);
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error has occurred, contact the administrator!");
        }
        private async Task<IActionResult> HttpErrorHandlerAsync(Exception error) => await Task.FromResult(HttpErrorHandler(error));
    }
}
