using FluentValidation;

using GoActive.WebApi.Endpoints.Sketch.Requests;

namespace GoActive.WebApi.Endpoints.Sketch.Validation;

public class CreateSketchRequestValidator : AbstractValidator<CreateSketchRequest>
{
    private const char Whitespace = ' ';

    public CreateSketchRequestValidator()
    {
        RuleFor(r => r.ActivityTypes).NotEmpty().ForEach(a => a.IsInEnum());

        RuleFor(r => r.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(1, 100)
            .Must(x => !x.StartsWith(Whitespace) && !x.EndsWith(Whitespace))
            .WithMessage(" Title value shouldn't contains heading and trailing white spaces.");

        RuleFor(r => r.Location).NotNull();
    }
}
