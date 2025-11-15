using Microsoft.AspNetCore.Mvc;
using StringFiltering.API.Controllers.Result.DTOs;
using StringFiltering.Application.Features.Result.Services;

namespace StringFiltering.API.Controllers.Result;

[ApiController]
[Route("api")]
public class ResultController(IResultService resultService) : ControllerBase
{
    [HttpGet("status/{uploadId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetStatusResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Status([FromRoute] string uploadId)
    {
        var result = resultService.GetStatus(uploadId);
        return Ok(result.MapToResponse());
    }

    [HttpGet("result/{uploadId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResultResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Result([FromRoute] string uploadId)
    {
        var result = resultService.GetResult(uploadId);
        return Ok(result.MapToResponse());
    }
}