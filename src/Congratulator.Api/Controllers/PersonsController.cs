using Congratulator.Core.Services;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Congratulator.Api.Controllers;

/// <summary>
/// Controller for managing persons.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PersonsController : Controller
{
    /// <summary>
    /// Create person
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePersonAsync(
        [FromServices] CreatePersonService service, 
        [FromBody] CreatePersonRequest request)
    {
        var result = await service.RunAsync(request);
        return Ok(result);
    }
    
    /// <summary>
    /// Update person
    /// </summary>
    [HttpPut]
    [Route("{personId}")]
    public async Task<IActionResult> UpdatePersonAsync(
        [FromRoute] Guid personId, 
        [FromServices] UpdatePersonService service, 
        [FromBody] UpdatePersonRequest request)
    {
        var result = await service.RunAsync(personId, request);
        return Ok(result);
    }
    
    /// <summary>
    /// Delete person
    /// </summary>
    [HttpDelete]
    [Route("{personId}")]
    public async Task<IActionResult> DeletePersonAsync(
        [FromRoute] Guid personId, 
        [FromServices] DeletePersonService service)
    {
        await service.RunAsync(personId);
        return NoContent();
    }
    
    /// <summary>
    /// Get paginated list of persons
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPersonsAsync( 
        [FromServices] GetPersonsService service,
        [FromQuery] GetPersonsRequest request)
    {
        var result = await service.RunAsync(request);
        return Ok(result);
    }
}