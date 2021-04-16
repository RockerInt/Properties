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
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerRepository _repository;
        private readonly ILogger<OwnersController> _logger;

        public OwnersController(ILogger<OwnersController> logger, IOwnerRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Consulta la totalidad de las Owners
        /// </summary>
        /// <returns>Retorna una colección de Owner</returns>
        [HttpGet]
        [Route("Get")]
        [ProducesResponseType(typeof(IEnumerable<Owner>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOwners() =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation("Begin HttpGet call GetOwners");

                    var response = await _repository.GetOwners();

                    if (response != null && response.Any())
                        return Ok(response.JsonSerialize());

                    return NotFound("No results found");
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Consulta un Owner por su Id
        /// </summary>
        /// <param name="id">Identificador del Owner</param>
        /// <returns>Retorna un Owner</returns>
        [HttpGet]
        [Route("Get/{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Owner), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOwner(Guid id) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation($"Begin HttpGet call GetPropertiy for id {id}");

                    var owner = await _repository.GetOwner(id);

                    if (owner is null) return NotFound($"The property with id {id} do not exist");

                    return Ok(owner.JsonSerialize());
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Agrega una nueva Owner
        /// </summary>
        /// <param name="property">Owner a crear</param>
        /// <returns>Retorna la Owner creada</returns>
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateOwner([FromBody] Owner owner) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation($"Begin HttpPost call CreateOwner for: {owner}");

                        owner = await _repository.CreateOwner(owner);

                        return StatusCode((int)HttpStatusCode.Created, owner.JsonSerialize());
                    }
                    else return BadRequest();
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateException))
                        if (OwnerExists(owner.IdOwner))
                            return Conflict($"The property with id {owner.IdOwner} already exist");
                    return await HttpErrorHandlerAsync(error);
                }
            );

        /// <summary>
        /// Actualiza un Owner
        /// </summary>
        /// <param name="property">Owner a actualizar</param>
        /// <returns>Retorna la Owner actualizada</returns>
        [HttpPost]
        [Route("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateOwner([FromBody] Owner owner) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation($"Begin HttpPost call UpdateOwner for: {owner}");

                        owner = await _repository.UpdateOwner(owner);

                        return Ok(owner.JsonSerialize());
                    }
                    else return BadRequest();
                },
                async error =>
                {
                    if (error.GetType() == typeof(DbUpdateConcurrencyException))
                        if (!OwnerExists(owner.IdOwner))
                            return NotFound($"The property with id {owner.IdOwner} do not exist");
                    return await HttpErrorHandlerAsync(error);
                }
            );

        /// <summary>
        /// Elimina un Owner por su Id
        /// </summary>
        /// <param name="id">Identificador del Owner</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete/{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteOwner(Guid id) =>
            await Utilities.Utilities.TryCatchAsync(
                async () =>
                {
                    _logger.LogInformation($"Begin HttpPost call DeleteOwner for id {id}");

                    var owner = await _repository.GetOwner(id);

                    if (owner is null) return NotFound($"The property with id {id} do not exist");

                    var response = await _repository.DeleteOwner(owner);

                    return response ? NoContent() : StatusCode((int)HttpStatusCode.NotModified);
                },
                HttpErrorHandlerAsync
            );

        /// <summary>
        /// Valida si existe un Owner por su Id
        /// </summary>
        /// <param name="id">Identificador del Owner</param>
        /// <returns>True si existe</returns>
        private bool OwnerExists(Guid id) => _repository.OwnerExists(id);

        private IActionResult HttpErrorHandler(Exception error)
        {
            _logger.LogError("An error has occurred: @error", error);
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error has occurred, contact the administrator!");
        }
        private async Task<IActionResult> HttpErrorHandlerAsync(Exception error) => await Task.FromResult(HttpErrorHandler(error));
    }
}
