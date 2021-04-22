using Properties.Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Properties.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Utilities;
using Gateway.Models.Parameters;

namespace Properties.Gateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly PropertiesService _service;

        public PropertiesController(PropertiesService service)
        {
            _service = service;
        }

        /// <summary>
        /// Consulta la totalidad de las Properties
        /// </summary>
        /// <returns>Retorna una colección de Property</returns>
        [HttpGet]
        [Route("Get")]
        [ProducesResponseType(typeof(IEnumerable<Models.Complete.Property>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProperties([FromQuery] PropertiesParameters parameters) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    if (!parameters.ValidYearRange) return BadRequest("Max year cannot be less than min year");
                    if (!parameters.ValidPriceRange) return BadRequest("Max price cannot be less than min price");
                    return Ok(await _service.GetProperties(parameters));
                },
                HttpErrorHandler
            );

        /// <summary>
        /// Consulta una Property por su Id
        /// </summary>
        /// <param name="id">Identificador de la Property</param>
        /// <returns>Retorna una Property</returns>
        [HttpGet]
        [Route("Get/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Models.Complete.Property), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProperty(Guid id) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    var @property = await _service.GetPropertyById(id);

                    if (@property is null)
                    {
                        return NotFound();
                    }

                    return Ok(@property);
                },
                HttpErrorHandler
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
        public async Task<IActionResult> CreateProperty([FromBody] Models.Lite.PropertyLite @property) =>
            await Utilities.Utilities.TryCatchAsync(
               async () => ModelState.IsValid ?
                    StatusCode((int)HttpStatusCode.Created, await _service.CreateProperty(@property))
                    : BadRequest()
               ,
               HttpErrorHandler
            );

        /// <summary>
        /// Agrega una nueva Property
        /// </summary>
        /// <param name="property">Property a crear</param>
        /// <returns>Retorna la Property creada</returns>
        [HttpPost]
        [Route("Update")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> UpdateProperty([FromBody] Models.Lite.PropertyLite @property) =>
            await Utilities.Utilities.TryCatchAsync(
               async () => ModelState.IsValid ?
                    Ok(await _service.UpdateProperty(@property))
                    : BadRequest()
               ,
                async error =>
                {
                    if (!_service.PropertyExists(@property.IdProperty))
                        return NotFound($"The property with id {@property.IdProperty} do not exist");
                    return await HttpErrorHandler(error);
                }
            );

        /// <summary>
        /// Agrega una nueva PropertyImage
        /// </summary>
        /// <param name="propertyImage">Property a crear</param>
        /// <returns>Retorna la PropertyImage creada</returns>
        [HttpPost]
        [Route("CreateImage")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreatePropertyImage([FromBody] Models.Lite.PropertyImageLite propertyImage) =>
            await Utilities.Utilities.TryCatchAsync(
               async () => ModelState.IsValid ?
                    StatusCode((int)HttpStatusCode.Created, await _service.CreatePropertyImage(propertyImage))
                    : BadRequest()
               ,
               HttpErrorHandler
            );



        private async Task<IActionResult> HttpErrorHandler(Exception e) => await Task.FromResult(StatusCode((int)HttpStatusCode.InternalServerError, $"An error has occurred, contact the administrator! {e}"));
    }
}
