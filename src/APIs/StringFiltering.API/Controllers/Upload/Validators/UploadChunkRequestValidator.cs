using FluentValidation;
using StringFiltering.API.Controllers.Upload.DTOs;

namespace StringFiltering.API.Controllers.Upload.Validators;

public class UploadChunkRequestValidator : AbstractValidator<UploadChunkRequest>
{
    public UploadChunkRequestValidator()
    {
        RuleFor(x => x.UploadId)
            .NotEmpty()
            .WithMessage("Dəyər tələb olunur.");

        RuleFor(x => x.ChunkIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Dəyər 0-dan kiçik ola bilməz.");
        
        RuleFor(x => x.Data)
            .NotEmpty()
            .WithMessage("Dəyər tələb olunur.");
    }
}