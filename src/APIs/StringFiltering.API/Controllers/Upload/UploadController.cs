using Microsoft.AspNetCore.Mvc;
using StringFiltering.API.Controllers.Upload.DTOs;
using StringFiltering.Application.Features.Upload.Services;

namespace StringFiltering.API.Controllers.Upload;

[ApiController]
[Route("api")]
public class UploadController(IUploadService uploadService) : ControllerBase
{
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadChunkResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UploadChunk([FromBody] UploadChunkRequest chunkRequest)
    {
        var model = chunkRequest.MapToInput(); 
        var result = uploadService.UploadChunk(model);
        return Ok(result.MapToResponse());
    } 
}