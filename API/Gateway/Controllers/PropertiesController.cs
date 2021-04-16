using Properties.Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Properties.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Utilities;

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
        [ProducesResponseType(typeof(IEnumerable<Properties.Models.Complete.Property>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProperties() =>
            await Utilities.Utilities.TryCatchAsync(
                async () => (IActionResult)Ok(await _service.GetProperties()),
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
        [ProducesResponseType(typeof(Properties.Models.Complete.Property), (int)HttpStatusCode.OK)]
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
        public async Task<IActionResult> CreateProperty([FromBody] Properties.Models.Lite.PropertyLite @property) =>
            await Utilities.Utilities.TryCatchAsync(
               async () => ModelState.IsValid ?
                    StatusCode((int)HttpStatusCode.Created, await _service.CreateProperty(@property))
                    : BadRequest()
               ,
               HttpErrorHandler
            );

        

        private async Task<IActionResult> HttpErrorHandler(Exception e) => await Task.FromResult(StatusCode((int)HttpStatusCode.InternalServerError, $"An error has occurred, contact the administrator! {e}"));
    }
}
