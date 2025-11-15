using Microsoft.AspNetCore.Mvc;
using StringFiltering.API.Controllers.Upload.DTOs;
using StringFiltering.Application.Features.Upload.Models;
using StringFiltering.Application.Features.Upload.Services;

namespace StringFiltering.API.Controllers;

// Just for demonstration, not real controller
// Should be deleted
[ApiController]
[Route("api")]
public class FastTestController(IUploadService uploadService) : ControllerBase
{
    [HttpGet("just-test")]
    public IActionResult Test()
    {
        const string uploadId = "5ae6cc07-f881-469e-a0f4-637644077edb";
        for (int i = 0; i < 10; i++)
        {
            var text =
                @"What is Clean Architecture?
Clean Architecture is a design philosophy that emphasizes separation of concerns in a software application. It promotes a modular, maintainable codebase where business logic and application models are independent of the UI, database, and external interfaces.

This approach results in a system that’s flexible, testable, and easy to understand and maintain.

Key Components of Clean Architecture:
Domain Layer: Contains the business logic and entities. It’s the innermost layer with no dependencies on other layers.
Application Layer: Contains application logic and defines interfaces for the outer layers. It depends on the Domain layer but not directly on Presentation or Infrastructure layers.
Infrastructure Layer: Implements interfaces from the Application layer, dealing with data access, file systems, network, etc.
Presentation Layer (UI): The outermost layer, interacting with the Application layer and displaying information to the user."
                    .AsMemory();
            
            var chunk = new UploadChunkInput
            {
                UploadId = uploadId,
                ChunkIndex = i,
                Data = text,
                IsLastChunk = false
            };
            
            _ = uploadService.UploadChunk(chunk);
        }
        
        var lastChunk = new UploadChunkInput
        {
            UploadId = uploadId,
            ChunkIndex = 100000,
            Data = "Last chunk".AsMemory(),
            IsLastChunk = true
        };
        
        _ = uploadService.UploadChunk(lastChunk);
        
        return Ok(new UploadChunkResponse
        {
            Status = "Accepted",
            ResultId = uploadId
        });
    } 
}