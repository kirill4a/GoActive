using FluentValidation;

namespace GoActive.WebApi.Infrastructure.Filters;

public class ValidationFilter<TRequest> : IEndpointFilter
{
    private readonly IValidator<TRequest> _validator;

    public ValidationFilter(IValidator<TRequest> validator)
    {
        ArgumentNullException.ThrowIfNull(validator);
        _validator = validator;
    }


    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().First();
        var validationResult = await _validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        return !validationResult.IsValid
            ? TypedResults.ValidationProblem(validationResult.ToDictionary())
            : await next.Invoke(context);
    }
}