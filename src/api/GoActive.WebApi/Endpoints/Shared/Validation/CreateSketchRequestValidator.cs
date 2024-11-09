using FluentValidation;

using GoActive.WebApi.Endpoints.Shared.Requests;

namespace GoActive.WebApi.Endpoints.Shared.Validation;

public class CreateSketchRequestValidator : AbstractValidator<CreateSketchRequest>
{
    public CreateSketchRequestValidator()
    {
        RuleFor(r => r.ActivityType).NotEmpty().IsInEnum();
        RuleFor(r => r.Title).NotEmpty().MaximumLength(50);
        RuleFor(r => r.Location).NotNull();
    }
}
