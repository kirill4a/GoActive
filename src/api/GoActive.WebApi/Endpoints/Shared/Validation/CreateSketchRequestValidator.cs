using FluentValidation;

using GoActive.WebApi.Endpoints.Shared.Requests;

namespace GoActive.WebApi.Endpoints.Shared.Validation;

public class CreateSketchRequestValidator : AbstractValidator<CreateSketchRequest>
{
    public CreateSketchRequestValidator()
    {
        RuleFor(r => r.ActivityTypes).NotEmpty().ForEach(a => a.IsInEnum());
        RuleFor(r => r.Title)
            .NotEmpty()
            .Length(1, 99)
            .Must(x => !x.StartsWith(" ") && !x.EndsWith(" "))
            .WithMessage(" Title value shouldn't contains heading and trailing white spaces.");
        RuleFor(r => r.Location).NotNull();
    }
}
