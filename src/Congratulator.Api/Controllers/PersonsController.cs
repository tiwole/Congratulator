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
    public async Task<IActionResult> CreatePersonAsync([FromServices] CreatePersonService service, [FromBody] CreatePersonRequest request)
    {
        var result = await service.RunAsync(request);
        return Ok(result);
    }
}